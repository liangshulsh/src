using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceProcess;
using System.Xml.XPath;
using log4net;
using Skywolf.Utility;

namespace Skywolf.ServiceDynHost
{
    class ServiceDynHost : ServiceBase
    {
        public CustomServiceHost[] svcHosts = null;
        private static readonly string ECHO_INTERFACE = "Skywolf.Utility.IEcho";
        private static ILog _log;

        public static void Main()
        {
            ServiceBase.Run(new ServiceDynHost());
        }
        
        // Start the Windows service.
        protected override void OnStart(string[] args)
        {
            try
            {
                EchoService.StartLogger();
                _log = EchoService.Logger;

                List<Type> serviceTypes = new List<Type>();

                string path = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]);
                var files = Directory.GetFiles(path, "Skywolf.*Service.dll");

                List<string> serviceBinaries = new List<string>();
                files.ToList().ForEach(serviceBinaries.Add);

                TypeFilter myFilter = new TypeFilter((typeObj, criteriaObj) => typeObj.ToString() == criteriaObj.ToString());

                foreach (string filePath in serviceBinaries)
                {
                    _log.InfoFormat("Module: {0}", filePath);
                    Assembly asm = Assembly.LoadFile(filePath);
                    Type[] exportedTypes = asm.GetExportedTypes();

                    foreach (Type t in exportedTypes)
                    {
                        if (t.IsAbstract)
                        {
                            continue;
                        }

                        Type[] myInterfaces = t.FindInterfaces(myFilter, ECHO_INTERFACE);
                        if (myInterfaces.Length > 0 && CustomServiceHost.Contains(t))
                        {
                            serviceTypes.Add(t);
                        }
                    }
                }

                string hostMode = ConfigurationManager.AppSettings["HostMode"];
                if (string.IsNullOrEmpty(hostMode))
                    hostMode = "Multiple";
                if (string.Equals(hostMode, "Single", StringComparison.OrdinalIgnoreCase) && serviceTypes.Count > 1)
                {
                    throw new ArgumentException("current design of service hose supports only one model.");
                }

                if (serviceTypes.Count() == 0)
                {
                    throw new ArgumentException("no service is found.");
                }
                svcHosts = new CustomServiceHost[serviceTypes.Count];

                int i = 0;
                var config = new XPathDocument(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
                var servicesElement = config.CreateNavigator().SelectSingleNode("//system.serviceModel/services");
                var logOperationBehavior = new LogOperationBehavior();

                foreach (Type serviceType in serviceTypes)
                {
                    var host = new CustomServiceHost(serviceType);
                    var firstEp = host.Description.Endpoints[0];
                    var binding = firstEp.Binding;
                    foreach (var op in firstEp.Contract.Operations)
                        if (op.Name != "UpdateStatus") //filter out UpdateStatus.
                            op.Behaviors.Add(logOperationBehavior);

                    host.AddServiceEndpoint(ECHO_INTERFACE, binding, "Echo");
                    Binding newBinding;
                    var bindingConfig = servicesElement.SelectSingleNode(
                        string.Format("service[@name='{0}']/endpoint[1]/@bindingConfiguration", host.Description.ConfigurationName));

                    if (binding is NetTcpBinding)
                    {
                        var newTcpBinding = new NetTcpBinding(bindingConfig.Value);
                        newTcpBinding.Security.Mode = SecurityMode.Transport;
                        newTcpBinding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.Sign;
                        newBinding = newTcpBinding;
                    }
                    else if (binding is WebHttpBinding)
                    {
                        var newHttpBinding = new WebHttpBinding(bindingConfig.Value);
                        newHttpBinding.Security.Mode = WebHttpSecurityMode.TransportCredentialOnly;
                        newHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                        newBinding = newHttpBinding;
                    }
                    else
                    {
                        var newHttpBinding = new BasicHttpBinding(bindingConfig.Value);
                        newHttpBinding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                        newHttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                        newBinding = newHttpBinding;
                    }

                    host.AddServiceEndpoint(host.Description.Endpoints[0].Contract.ContractType, newBinding, "wid");
                    host.Open();

                    _log.Info(String.Format("{0} service is ready.", serviceType.Name));
                    foreach (var endpoint in host.Description.Endpoints)
                        _log.Info(String.Format("service endpoint: {0}", endpoint.Address));
                    svcHosts[i] = host;
                    i++;
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("Exception thrown from {0}", "unknown"), ex);
                throw;
            }
        }

        protected override void OnStop()
        {
            if (svcHosts != null)
            {
                foreach (var host in svcHosts)
                {
                    if (host.State == CommunicationState.Faulted)
                        host.Abort();
                    else
                        host.Close();
                }
            }
        }

    }
}
