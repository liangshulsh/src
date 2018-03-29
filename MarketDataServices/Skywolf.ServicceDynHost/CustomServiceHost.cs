using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Net;
using System.ServiceModel.Description;

namespace Skywolf.ServiceDynHost
{
    class CustomServiceHost : ServiceHost
    {

        private readonly static Dictionary<string, ServiceElement> elementDict;
        private readonly static string ThisComputerName = Dns.GetHostName();

        static CustomServiceHost()
        {
            elementDict = new Dictionary<string, ServiceElement>();
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceSection = config.GetSection("system.serviceModel/services") as ServicesSection;
            foreach (ServiceElement element in serviceSection.Services)
            {
                ReplaceBaseAddress(element.Host.BaseAddresses);
                elementDict.Add(element.Name, element);
            }
        }

        public static bool Contains(Type serviceType)
        {
            return elementDict.ContainsKey(serviceType.FullName);
        }

        private static void ReplaceBaseAddress(BaseAddressElementCollection baseAddressElementCollection)
        {
            for (int i = 0; i < baseAddressElementCollection.Count; i++)
            {
                var uriBuilder = new UriBuilder(baseAddressElementCollection[i].BaseAddress);
                uriBuilder.Host = ThisComputerName;
                baseAddressElementCollection[i].BaseAddress = uriBuilder.ToString();
            }
        }

        public CustomServiceHost(Type serviceType, params Uri[] baseAddresses)
            : base(serviceType, baseAddresses)
        { }

        protected override void ApplyConfiguration()
        {
            this.LoadConfigurationSection(elementDict[this.Description.ConfigurationName]);
            EnsureMetaPublish(this.Description);
        }

        private void EnsureMetaPublish(ServiceDescription description)
        {
            ServiceMetadataBehavior item = description.Behaviors.Find<ServiceMetadataBehavior>();
            if (item == null)
            {
                item = new ServiceMetadataBehavior();
                description.Behaviors.Add(item);
            }
            item.HttpGetEnabled = true;
        }
    }

}
