﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace Skywolf.Client
{
    public class OptimusClient<TClient> : ClientBase<TClient>, IDisposable where TClient : class
    {
        public OptimusClient() { }

        public OptimusClient(string endpointConfigurationName) : base(endpointConfigurationName) { }

        public OptimusClient(Binding binding, EndpointAddress remoteAddress) : base(binding, remoteAddress) { }

        public OptimusClient(InstanceContext callbackInstance) : base(callbackInstance) { }

        public OptimusClient(InstanceContext callbackInstance, Binding binding, EndpointAddress remoteAddress) : base(callbackInstance, binding, remoteAddress) { }

        public TClient Instance
        {
            get { return base.Channel; }
        }

        public void Dispose()
        {
            AbortClose();
        }

        public void AbortClose()
        {
            //avoid the CommunicationObjectFaultedException 
            if (this.State == CommunicationState.Faulted)
                this.Abort();
            else
                try
                {
                    this.Close();
                }
                catch
                {
                    this.Abort();
                }
        }

        public static void Using(Action<TClient> action)
        {
            using (var c = new OptimusClient<TClient>())
            {
                try
                {
                    c.Open();
                    action(c.Instance);
                }
                catch (FaultException<ServiceFault> fsex)
                {
                    throw new ApplicationException(string.Format("Error occured when calling {0}: \r\n{1}"
                        , fsex.Action, fsex.Detail.Message));
                }
                catch (FaultException fex)
                {
                    throw new ApplicationException(string.Format("Error occured when calling {0}: \r\n{1}"
                        , fex.Action, fex.Message));
                }
            }
        }

        public static TResult Using<TResult>(Func<TClient, TResult> action)
        {
            using (var c = new OptimusClient<TClient>())
            {
                try
                {
                    c.Open();
                    return action(c.Instance);
                }
                catch (FaultException<ServiceFault> fsex)
                {
                    throw new ApplicationException(string.Format("Error occured when calling {0}: \r\n{1}"
                        , fsex.Action, fsex.Detail.Message));
                }
                catch (FaultException fex)
                {
                    throw new ApplicationException(string.Format("Error occured when calling {0}: \r\n{1}"
                        , fex.Action, fex.Message));
                }
            }
        }
    }
}

