using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace Skywolf.Utility.Proxy
{
    public class ConnectionProxy
    {
        public static T CreateMaxItemsChannel<T>(Binding binding, EndpointAddress endpoint)
        {
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, endpoint);

            foreach (OperationDescription op in factory.Endpoint.Contract.Operations)
            {
                var dataContractBehavior = op.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dataContractBehavior != null)
                {
                    dataContractBehavior.MaxItemsInObjectGraph = int.MaxValue;
                }
            }

            return factory.CreateChannel();
        }

        public static BasicHttpBinding BuildBasicHttpBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                CloseTimeout = new TimeSpan(0, 10, 0),
                OpenTimeout = new TimeSpan(0, 10, 0),
                ReceiveTimeout = new TimeSpan(0, 10, 0),
                SendTimeout = new TimeSpan(0, 10, 0),
                MaxBufferSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
                MessageEncoding = WSMessageEncoding.Mtom,
                UseDefaultWebProxy = false,
                Security = { Mode = BasicHttpSecurityMode.None },
                ReaderQuotas =
                    new System.Xml.XmlDictionaryReaderQuotas
                    {
                        MaxArrayLength = int.MaxValue,
                        MaxBytesPerRead = int.MaxValue,
                        MaxDepth = int.MaxValue,
                        MaxStringContentLength = int.MaxValue,
                        MaxNameTableCharCount = int.MaxValue
                    }
            };

            return binding;
        }

        public static BasicHttpBinding BuildBasicHttpBindingLongTime()
        {
            BasicHttpBinding binding = new BasicHttpBinding
            {
                CloseTimeout = new TimeSpan(1, 50, 0),
                OpenTimeout = new TimeSpan(1, 50, 0),
                ReceiveTimeout = new TimeSpan(1, 50, 0),
                SendTimeout = new TimeSpan(1, 50, 0),
                MaxBufferSize = int.MaxValue,
                MaxBufferPoolSize = int.MaxValue,
                MaxReceivedMessageSize = int.MaxValue,
                MessageEncoding = WSMessageEncoding.Mtom,
                UseDefaultWebProxy = false,
                Security = { Mode = BasicHttpSecurityMode.None },
                ReaderQuotas =
                    new System.Xml.XmlDictionaryReaderQuotas
                    {
                        MaxArrayLength = int.MaxValue,
                        MaxBytesPerRead = int.MaxValue,
                        MaxDepth = int.MaxValue,
                        MaxStringContentLength = int.MaxValue,
                        MaxNameTableCharCount = int.MaxValue
                    }
            };

            return binding;
        }

        public static NetTcpBinding BuildNetTcpBinding()
        {
            NetTcpBinding binding = new NetTcpBinding
            {
                CloseTimeout = new TimeSpan(1, 50, 0),
                OpenTimeout = new TimeSpan(1, 50, 0),
                ReceiveTimeout = new TimeSpan(1, 50, 0),
                SendTimeout = new TimeSpan(1, 50, 0),
                TransactionFlow = false,
                TransferMode = TransferMode.Buffered,
                TransactionProtocol = TransactionProtocol.OleTransactions,
                HostNameComparisonMode = HostNameComparisonMode.StrongWildcard,
                ListenBacklog = 10,
                MaxBufferPoolSize = int.MaxValue,
                MaxBufferSize = int.MaxValue,
                MaxConnections = 10,
                MaxReceivedMessageSize = int.MaxValue,
                ReliableSession = { Ordered = true, InactivityTimeout = new TimeSpan(1, 50, 0), Enabled = false }
            };
            binding.Security.Mode = SecurityMode.None;
            binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas
            {
                MaxArrayLength = int.MaxValue,
                MaxBytesPerRead = int.MaxValue,
                MaxDepth = int.MaxValue,
                MaxStringContentLength = int.MaxValue,
                MaxNameTableCharCount = int.MaxValue
            };

            return binding;
        }

        public static EndpointAddress BuildEndpointAddress(string key)
        {
            var endpointAddr = ConfigurationManager.AppSettings[key];
            return new EndpointAddress(endpointAddr);
        }
    }
}
