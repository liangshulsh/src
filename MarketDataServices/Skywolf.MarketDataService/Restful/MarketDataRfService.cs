using System;
using System.Collections.Concurrent;
using System.Data;
using System.ServiceModel;
using System.Text;
using log4net;
using Skywolf.Contracts.Services;
using Skywolf.Utility;
using System.Globalization;
using Skywolf.Contracts.Services.Restful;

namespace Skywolf.MarketDataService.Restful
{
    [ServiceBehavior(Namespace = Constants.NAMESPACE, InstanceContextMode = InstanceContextMode.PerCall)]
    public class MarketDataRfService : EchoService, IMarketDataRfService
    {
        private static ILog _Logger;

        static MarketDataRfService()
        {
            _Logger = LogManager.GetLogger(typeof(MarketDataRfService));
        }

        public string GetSIDFromName(string names)
        {

        }

        public string GetNameFromSID(string sids)
        {

        }

        public string GetTimeSeriesData(string symbol, string frequency, string outputcount, string isadjustedvalue, string datasource)
        {

        }

        public string GetStockBatchQuote(string symbols, string datasource)
        {

        }

        public string GetStockHistoryPrices(string symbols, string frequency, string startDate, string endDate, string outputCount, string isAdjustedValue, string datasource)
        {

        }

        public string GetCryptoHistoryPrices(string symbols, string market, string frequency, string startdate, string enddate, string outputCount, string datasource)
        {

        }

        public string VA_GetAvailableAPIKey()
        {

        }

        public string AV_AddAPIKeys(string apiKeys)
        {

        }

        public string AV_RemoveAPIKeys(string apiKeys)
        {

        }

        public string AV_UpdateAPIKeys(string apiKeys)
        {

        }
    }
}
