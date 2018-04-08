using Castle.DynamicProxy.Ref;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Threading;

namespace Skywolf.Client
{
    public class SkywolfChannelManager<TChannel>
        where TChannel : class
    {
        private static readonly List<ChannelFactory<TChannel>> clientFactories = new List<ChannelFactory<TChannel>>();
        private static readonly Dictionary<Uri, int> addressIndex = new Dictionary<Uri, int>();
        private static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();
        private static readonly string configName;
        private readonly IInterceptor[] wcfInterceptor;

        private readonly object syncRoot = new object();
        private readonly EndpointState[] epStates = new EndpointState[clientFactories.Count];

        static SkywolfChannelManager()
        {
            var sca = typeof(TChannel).GetCustomAttributes(typeof(ServiceContractAttribute), false);
            if (sca.Length == 0)
                throw new ArgumentException("TChannel should be a kind of ServiceContract.", "TChannel");
            configName = ((ServiceContractAttribute)sca[0]).ConfigurationName;
            if (string.IsNullOrEmpty(configName)) configName = typeof(TChannel).FullName;

            InitEndpoints();
        }

        public int EndpointCount
        {
            get { return epStates.Length; }
        }


        private static void InitEndpoints()
        {
            var clientEndpoints = ((ClientSection)ConfigurationManager.GetSection("system.serviceModel/client")).Endpoints;
            for (int i = 0, j = 0; i < clientEndpoints.Count; i++)
            {
                if (clientEndpoints[i].Contract == configName)
                {
                    clientFactories.Add(new ChannelFactory<TChannel>(clientEndpoints[i].Name));
                    addressIndex.Add(clientEndpoints[i].Address, j++);
                }
            }
        }

        public SkywolfChannelManager()
        {
            var ret = new List<IInterceptor>();

            if (clientFactories.Count > 1)
                ret.Add(new WcfLoadBalancingInterceptor(this));

            ret.Add(new WcfDisposingInterceptor(this));
            SetupAccessGate();

            wcfInterceptor = ret.ToArray();
        }

        private void SetupAccessGate()
        {
            int i = 0;
            foreach (var address in addressIndex.Keys)
            {
                if (address.Scheme == "http" || address.Scheme == "https")
                {
                    var connectionLimit = ServicePointManager.FindServicePoint(address).ConnectionLimit;
                    epStates[addressIndex[address]].AccessGate = new SemaphoreSlim(connectionLimit);
                }
                i++;
            }
        }

        public TChannel GetChannel()
        {
            var ch = GetOriginalChannel();
            return wcfInterceptor.Length == 0 ? ch : proxyGenerator.CreateInterfaceProxyWithTargetInterface(ch, wcfInterceptor);
        }

        private TChannel GetOriginalChannel()
        {
            if (clientFactories.Count == 0)
                throw new ConfigurationErrorsException(string.Format("No endpoint of {0} found.", typeof(TChannel).FullName));

            int index = 0;
            lock (syncRoot)
            {
                for (int i = 0; i < epStates.Length; i++)
                {
                    if (epStates[i].CallCount < epStates[index].CallCount)
                        index = i;
                }
                epStates[index].CallCount++;
            }

            if (epStates[index].AccessGate != null) epStates[index].AccessGate.Wait();
            return clientFactories[index].CreateChannel(); ;
        }

        private void ReleaseAccessGate(EndpointAddress remoteAddress)
        {
            var index = addressIndex[remoteAddress.Uri];
            if (epStates[index].AccessGate != null) epStates[index].AccessGate.Release();
        }

        private void MarkFaultChannel(EndpointAddress remoteAddress)
        {
            var index = addressIndex[remoteAddress.Uri];
            lock (syncRoot) epStates[index].CallCount += epStates[index].FaultIncreasement;
        }

        private class WcfLoadBalancingInterceptor : IInterceptor
        {
            SkywolfChannelManager<TChannel> channelManager;
            public WcfLoadBalancingInterceptor(SkywolfChannelManager<TChannel> channelManager)
            {
                this.channelManager = channelManager;
            }

            public void Intercept(IInvocation invocation)
            {
                bool done = true;
                int retryCountDown = channelManager.epStates.Length;
                var exceptions = new Queue<Exception>();

                do
                {
                    var channel = (IClientChannel)invocation.InvocationTarget;
                    done = true;
                    try
                    {
                        invocation.Proceed();
                    }
                    catch (CommunicationException cex)
                    {
                        if ((cex is EndpointNotFoundException || cex is ServerTooBusyException) && channelManager.epStates.Length > 1)
                        {
                            channelManager.MarkFaultChannel(channel.RemoteAddress);
                            exceptions.Enqueue(cex);
                            if (--retryCountDown > 0)
                            {
                                var proxyTarget = (IChangeProxyTarget)invocation;
                                var ch = channelManager.GetOriginalChannel();
                                proxyTarget.ChangeInvocationTarget(ch);
                                proxyTarget.ChangeProxyTarget(ch);
                                done = false;
                            }
                            else
                                throw new AggregateException(
                                    string.Format("All endpoints of {0} are down.", typeof(TChannel).FullName), exceptions);
                        }
                        else
                            throw;
                    }
                } while (!done);
            }
        }

        private class WcfDisposingInterceptor : IInterceptor
        {
            private static readonly ConcurrentDictionary<MethodBase, bool> methodRepo = new ConcurrentDictionary<MethodBase, bool>();

            private SkywolfChannelManager<TChannel> channelManager;
            public WcfDisposingInterceptor(SkywolfChannelManager<TChannel> channelManager)
            {
                this.channelManager = channelManager;
            }

            public void Intercept(IInvocation invocation)
            {
                bool isAsync = IsAsyncMethod(invocation.Method);
                var channel = (IClientChannel)invocation.InvocationTarget;
                try
                {
                    invocation.Proceed();
                    if (!isAsync) channel.Close();
                }
                catch (Exception)
                {
                    channel.Abort();
                    throw;
                }
                finally
                {
                    if (!isAsync) channelManager.ReleaseAccessGate(channel.RemoteAddress);
                }
            }

            private bool IsAsyncMethod(MethodBase method)
            {
                bool isAsync = false;
                if (methodRepo.TryGetValue(method, out isAsync))
                    return isAsync;
                else
                {
                    var attrs = method.GetCustomAttributes(typeof(OperationContractAttribute), false);
                    if (attrs.Length > 0)
                        isAsync = ((OperationContractAttribute)attrs[0]).AsyncPattern;

                    methodRepo.TryAdd(method, isAsync);
                    return isAsync;
                }
            }
        }

        private struct EndpointState
        {
            private int _increasement;

            public int FaultIncreasement
            {
                get
                {
                    if (_increasement <= 2000) _increasement += 100;
                    return _increasement;
                }
            }

            public long CallCount;

            public SemaphoreSlim AccessGate;
        }
    }
}
