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
using Skywolf.Contracts.DataContracts.MarketData.TVC;
using System.Threading.Tasks;
using System.Linq;

namespace Skywolf.MarketDataService
{
    [ServiceBehavior(Namespace = Constants.NAMESPACE, InstanceContextMode = InstanceContextMode.PerCall)]
    public class MarketDataService : EchoService, IMarketDataService
    {
        public const string DATASOURCE_ALPHAVANTAGE = "av";
        public const string DATASOURCE_TVC = "tvc";
        public const string DATASOURCE_SINA = "sina";
        public const string DATASOURCE_DEFAULT = "av";
        private static int _AVKeyBatchId = 1;
        private static ILog _Logger;
        protected static ConcurrentDictionary<string, IMarketDataGrabber> _dataGrabber = new ConcurrentDictionary<string, IMarketDataGrabber>();
        static MarketDataService()
        {
            try
            {
                string AVKeyBatchId = ConfigurationManager.AppSettings["AVKeyBatchId"];
                if (!string.IsNullOrWhiteSpace(AVKeyBatchId))
                {
                    _AVKeyBatchId = Convert.ToInt32(AVKeyBatchId);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex);
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
                TVCMarketDataGrabber tvc = new TVCMarketDataGrabber();
                tvc._getTVCSymbolsHandler = new GetTVCSymbols(GetTVCSymbolsFromDB);
                tvc._updateTVCSymbolesHandler = new UpdateTVCSymbols(StoreTVCSymbols);
                tvc._updateTVCQuotesHandler = new UpdateTVCQuotes(StoreTVCQuotes);
                _dataGrabber[DATASOURCE_TVC] = tvc;
                _dataGrabber[DATASOURCE_SINA] = new SinaMarketDataGrabber();
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        static void StoreTVCQuotes(IEnumerable<TVCQuoteResponse> quotes)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    MarketDataDatabase marketData = new MarketDataDatabase();
                    marketData.TVC_StoreQuotes(quotes);
                }
                catch (Exception ex)
                {
                    _Logger.Error(ex);
                }
            });
        }

        static void StoreTVCSymbols(IEnumerable<TVCSymbolResponse> symbols)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    MarketDataDatabase marketData = new MarketDataDatabase();
                    marketData.TVC_StoreSymbolList(symbols);
                }
                catch (Exception ex)
                {
                    _Logger.Error(ex);
                }
            });
        }

        static Dictionary<string, TVCSymbolResponse> GetTVCSymbolsFromDB(IEnumerable<string> symbols)
        {
            MarketDataDatabase marketData = new MarketDataDatabase();
            TVCSymbolResponse[] responses = marketData.TVC_GetSymbolList(symbols);
            if (responses != null)
            {
                return responses.ToDictionary(k => k.name, v => v);
            }
            else
            {
                return new Dictionary<string, TVCSymbolResponse>();
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

        #region TVC functions

        public TVCHistoryResponse TVC_GetHistoricalPrices(string symbol, BarFrequency frequency, DateTime from, DateTime to)
        {
            try
            {
                return (_dataGrabber[DATASOURCE_TVC] as TVCMarketDataGrabber).GetHistoricalPrices(symbol, frequency, from, to);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public TVCQuotesResponse TVC_GetQuotes(IEnumerable<string> symbols)
        {
            try
            {
                return (_dataGrabber[DATASOURCE_TVC] as TVCMarketDataGrabber).GetQuotes(symbols);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public TVCSymbolResponse TVC_GetSymbolInfo(string symbol)
        {
            try
            {
                return (_dataGrabber[DATASOURCE_TVC] as TVCMarketDataGrabber).GetSymbolInfo(symbol);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public TVCSearchResponse[] TVC_GetSymbolSearch(string query, string type = "", string exchange = "", int limit = 30)
        {
            try
            {
                return (_dataGrabber[DATASOURCE_TVC] as TVCMarketDataGrabber).GetSymbolSearch(query, type, exchange, limit);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public TVCCalendar[] TVC_GetCalendars(DateTime fromDate, DateTime toDate, string country = "", string currentTab = "custom")
        {
            try
            {
                return (_dataGrabber[DATASOURCE_TVC] as TVCMarketDataGrabber).GetCalendars(fromDate, toDate, country, currentTab);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public void TVC_StoreHolidays(IEnumerable<TVCCalendar> holidays)
        {
            try
            {
                new MarketDataDatabase().TVC_StoreHolidays(holidays);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public void TVC_StoreQuotes(IEnumerable<TVCQuoteResponse> quotes)
        {
            StoreTVCQuotes(quotes);
        }

        public void TVC_StoreSymbols(IEnumerable<TVCSymbolResponse> symbols)
        {
            StoreTVCSymbols(symbols);
        }

        #endregion

        #region History Function
        
        public DataTable GetHistory(string now, string country, IEnumerable<string> symbols, string field, BarFrequency freq, int count)
        {
            throw new NotImplementedException();
        }



        #endregion
    }
}
