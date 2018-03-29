using System;
using System.Collections.Concurrent;
using System.Data;
using System.ServiceModel;
using System.Text;
using log4net;
using Skywolf.Contracts.Services;
using Skywolf.Utility;
using System.Globalization;

namespace Skywolf.MarketDataService
{
    [ServiceBehavior(Namespace = Constants.NAMESPACE, InstanceContextMode = InstanceContextMode.PerCall)]
    public class MarketDataService : EchoService, IMarketDataService
    {
        private static ILog _Logger;

        static MarketDataService()
        {
            _Logger = LogManager.GetLogger(typeof(MarketDataService).Name);
        }

        public string Test(string quoteDate, string betaName)
        {
            return "ok:" + quoteDate + betaName;
        }
    }
}
