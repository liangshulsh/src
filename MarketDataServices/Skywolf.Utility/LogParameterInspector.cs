using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;
using log4net;
using ServiceStack.Text;

namespace Skywolf.Utility
{
    /// <summary>
    ///     An inspector to log invocation infomations such as the client ip, user name, method called and parameter etc.
    /// </summary>
    public class LogParameterInspector : IParameterInspector
    {
        private readonly ILog logger;
        private static readonly Action<Task> ignoreException = task => task.Exception.Handle(e => true);

        public LogParameterInspector(string serviceName)
        {
            logger = LogManager.GetLogger(serviceName);
        }

        object IParameterInspector.BeforeCall(string operationName, object[] inputs)
        {
            if (logger.IsInfoEnabled)
            {
                Task.Factory.StartNew(c =>
                {
                    if (logger.IsDebugEnabled)
                    {
                        logger.DebugFormat("{0} called {1} with {2}.", c, operationName, inputs.Dump());
                    }
                    else
                    {
                        logger.InfoFormat("{0} called {1}.", c, operationName);
                    }
                }, GetContextInfo()).ContinueWith(ignoreException, TaskContinuationOptions.OnlyOnFaulted);
            }
            return null;
        }

        void IParameterInspector.AfterCall(string operationName, object[] outputs, object returnValue,
            object correlationState)
        {
            if (logger.IsDebugEnabled)
            {
                Task.Factory.StartNew(c =>
                    logger.DebugFormat("{0} exited {1}. Outputs: {2}, return: {3}", c, operationName,
                        outputs.Dump(), returnValue.Dump())
                    , GetContextInfo()).ContinueWith(ignoreException, TaskContinuationOptions.OnlyOnFaulted);
            }
        }

        private static string GetContextInfo()
        {
            var oc = OperationContext.Current;
            if (oc != null)
            {
                var remoteEndpointProperty =
                    oc.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                var clientAddress = remoteEndpointProperty == null ? null : remoteEndpointProperty.Address;

                string userName;
                if (oc.ServiceSecurityContext != null && !oc.ServiceSecurityContext.IsAnonymous)
                    userName = oc.ServiceSecurityContext.PrimaryIdentity.Name;
                else
                    userName = "<anonymous>";

                return string.Format("{1}@{0}", clientAddress, userName);
            }
            return "<null context>";
        }
    }
}
