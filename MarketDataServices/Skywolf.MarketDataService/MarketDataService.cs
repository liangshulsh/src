using System;
using System.Collections.Concurrent;
using System.Data;
using System.ServiceModel;
using System.Text;
using log4net;
using Skywolf.Contracts.Services;
using Skywolf.Utility;
using System.Globalization;
using Skywolf.Contracts.DataContracts.MarketData;
using Skywolf.MarketDataGrabber;
using Skywolf.DatabaseRepository;
using System.Collections.Generic;
using Skywolf.Contracts.DataContracts.Instrument;
using System.Configuration;

namespace Skywolf.MarketDataService
{
    [ServiceBehavior(Namespace = Constants.NAMESPACE, InstanceContextMode = InstanceContextMode.PerCall)]
    public class MarketDataService : EchoService, IMarketDataService
    {
        public const string DATASOURCE_ALPHAVANTAGE = "av";
        public const string DATASOURCE_DEFAULT = "av";
        private static int _AVKeyBatchId = 1;
        private static ILog _Logger;
        protected static ConcurrentDictionary<string, IMarketDataGrabber> _dataGrabber = new ConcurrentDictionary<string, IMarketDataGrabber>();
        static MarketDataService()
        {
            string AVKeyBatchId = ConfigurationManager.AppSettings["AVKeyBatchId"];
            if (!string.IsNullOrWhiteSpace(AVKeyBatchId))
            {
                _AVKeyBatchId = Convert.ToInt32(AVKeyBatchId);
            }

            _Logger = LogManager.GetLogger(typeof(MarketDataService));
            InitDataSource();
        }

        protected static void InitDataSource()
        {
            try
            {
                _dataGrabber[DATASOURCE_ALPHAVANTAGE] = new AVMarketDataGrabber();
                new AVMarketDataGrabber().UpdateAPIKeys(new MarketDataDatabase().VA_GetAvailableAPIKey(_AVKeyBatchId));
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }


        public IDictionary<string, long> GetSIDFromName(string[] names)
        {
            try
            {
                return new MarketDataDatabase().GetSIDFromName(names);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public IDictionary<long, string> GetNameFromSID(long[] SIDs)
        {
            try
            {
                return new MarketDataDatabase().GetNameFromSID(SIDs);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public PricingRule[] GetPricingRule(bool active, string datasource)
        {
            try
            {
                return new MarketDataDatabase().GetPricingRules(datasource, active);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public TimeSeriesDataOutput GetTimeSeriesData(TimeSeriesDataInput input, string datasource)
        {
            if (input == null || string.IsNullOrEmpty(input.Symbol))
            {
                return null;
            }

            TimeSeriesDataOutput output = null;

            try
            {
                if (string.IsNullOrEmpty(datasource))
                {
                    datasource = DATASOURCE_DEFAULT;
                }

                output = _dataGrabber[datasource.Trim().ToLower()].GetTimeSeriesData(input);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }

            return output;
        }

        public Quote[] GetStockBatchQuote(string[] symbols, string datasource)
        {
            if (symbols == null || symbols.Length <= 0)
            {
                return null;
            }

            Quote[] quotes = null;

            try
            {
                if (string.IsNullOrEmpty(datasource))
                {
                    datasource = DATASOURCE_DEFAULT;
                }

                quotes = _dataGrabber[datasource.Trim().ToLower()].StockBatchQuote(symbols);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }

            return quotes;
        }

        public IDictionary<string, StockBar[]> GetStockHistoryPrices(string[] symbols, BarFrequency frequency, DateTime? startDate, DateTime? endDate, int outputCount, bool isAdjustedValue, string datasource)
        {
            if (symbols == null || symbols.Length == 0)
            {
                return null;
            }

            IDictionary<string, StockBar[]> output = null;

            try
            {
                if (string.IsNullOrEmpty(datasource))
                {
                    datasource = DATASOURCE_DEFAULT;
                }

                datasource = datasource.Trim().ToLower();

                if (datasource == DATASOURCE_ALPHAVANTAGE)
                {
                    output = new MarketDataDatabase().VA_GetStockPrices(symbols, frequency, startDate, endDate, outputCount, isAdjustedValue);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }

            return output;
        }

        public IDictionary<string, CryptoBar[]> GetCryptoHistoryPrices(string[] symbols, string market, BarFrequency frequency, DateTime? startDate, DateTime? endDate, int outputCount, string datasource)
        {
            if (symbols == null || symbols.Length == 0 || string.IsNullOrEmpty(market))
            {
                return null;
            }

            IDictionary<string, CryptoBar[]> output = null;

            try
            {
                if (string.IsNullOrEmpty(datasource))
                {
                    datasource = DATASOURCE_DEFAULT;
                }

                datasource = datasource.Trim().ToLower();

                if (datasource == DATASOURCE_ALPHAVANTAGE)
                {
                    output = new MarketDataDatabase().VA_GetCryptoPrices(symbols, market, frequency, startDate, endDate, outputCount);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }

            return output;
        }

        public IDictionary<string, StockBar> GetLatestStockHistoryPrices(string[] symbols, BarFrequency frequency, bool isAdjustedValue, string datasource)
        {
            if (symbols == null || symbols.Length == 0)
            {
                return null;
            }

            IDictionary<string, StockBar> output = null;

            try
            {
                if (string.IsNullOrEmpty(datasource))
                {
                    datasource = DATASOURCE_DEFAULT;
                }

                datasource = datasource.Trim().ToLower();

                if (datasource == DATASOURCE_ALPHAVANTAGE)
                {
                    output = new MarketDataDatabase().VA_GetLatestStockPrices(symbols, frequency, isAdjustedValue);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }

            return output;
        }

        public IDictionary<string, CryptoBar> GetLatestCryptoHistoryPrices(string[] symbols, string market, BarFrequency frequency, string datasource)
        {
            if (symbols == null || symbols.Length == 0 || string.IsNullOrEmpty(market))
            {
                return null;
            }

            IDictionary<string, CryptoBar> output = null;

            try
            {
                if (string.IsNullOrEmpty(datasource))
                {
                    datasource = DATASOURCE_DEFAULT;
                }

                datasource = datasource.Trim().ToLower();

                if (datasource == DATASOURCE_ALPHAVANTAGE)
                {
                    output = new MarketDataDatabase().VA_GetLatestCryptoPrices(symbols, market, frequency);
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }

            return output;
        }

        #region Alpha Vantage Functions
        public void AV_AddAPIKeys(string[] apiKeys)
        {
            try
            {
                new AVMarketDataGrabber().AddAPIKeys(apiKeys);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public void AV_RemoveAPIKeys(string[] apiKeys)
        {
            try
            {
                new AVMarketDataGrabber().RemoveAPIKeys(apiKeys);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public void AV_UpdateAPIKeys(string[] apiKeys)
        {
            try
            {
                if (apiKeys != null && apiKeys.Length > 0)
                    new AVMarketDataGrabber().UpdateAPIKeys(apiKeys);
                else
                    new AVMarketDataGrabber().UpdateAPIKeys(new MarketDataDatabase().VA_GetAvailableAPIKey(_AVKeyBatchId));
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string[] VA_GetAvailableAPIKey()
        {
            try
            {
                return new MarketDataDatabase().VA_GetAvailableAPIKey(_AVKeyBatchId);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public bool VA_StorePrices(long SID, BarFrequency frequency, bool isAdjustedValue, Bar[] bars)
        {
            try
            {
                return new MarketDataDatabase().VA_StorePrices(SID, frequency, isAdjustedValue, bars);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        #endregion
    }
}
