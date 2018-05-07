using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skywolf.Contracts.DataContracts.Instrument;
using Skywolf.Contracts.DataContracts.MarketData;
using Skywolf.Contracts.Services;
using Skywolf.Client;
using System.ServiceModel;
using BitFactory.Logging;
using System.Collections.Concurrent;

namespace AlphaVantageMarketDataAuto
{
    public class MarketDataHandler
    {
        public static readonly CompositeLogger _logger = Program._logger;

        const string DATASOURCE = "av";
        BarFrequency _period = BarFrequency.None;
        bool _isAdjustedValue = false;
        int _priorityStart = 0;
        int _priorityEnd = 0;

        public MarketDataHandler(string period, bool isAdjustedValue, int priorityStart, int priorityEnd)
        {
            _period = ConvertStringToBarFrequency(period);
            _isAdjustedValue = isAdjustedValue;
            _priorityStart = priorityStart;
            _priorityEnd = priorityEnd;
        }

        public void Update(int iStartFrom = 0)
        {
            //var o = RetrieveStockTimeSeriesPrices("HAE",  BarFrequency.Day1, true, -1);
            //StoreHistoryPrices(200004253, BarFrequency.Day1, true, o.Data);
            _logger.LogInfo(string.Format("Update historical prices for {0} from Alpha Vantage.", _period));

            PricingRule[] pricingRules = GetPricingRules();

            _logger.LogInfo(string.Format("Total {0} symbols.", pricingRules.Count()));

            string[] keys = GetAPIKeys();

            _logger.LogInfo(string.Format("Total {0} active keys.", keys.Count()));

            long totalSymbols = pricingRules.Count();
            int keyCount = keys.Count();

            _logger.LogInfo("Get latest prices.");
            IDictionary<string, StockBar> stockBars = GetLatestStockHistoryPrices(pricingRules.Select(p => p.Ticker).ToArray(), _period, _isAdjustedValue);

            object countObj = new object();

            int handleCount = 0;
            _logger.LogInfo("Start retriving prices from Alpha Vantage.");
            foreach (var pricingRule in pricingRules)
            {
                int idx;
                lock (countObj)
                {
                    idx = handleCount + 1;
                    handleCount++;
                }

                if (idx < iStartFrom)
                {
                    continue;
                }

                try
                {
                    StockBar bar = null;

                    if (stockBars != null && stockBars.Count > 0)
                    {
                        if (stockBars.ContainsKey(pricingRule.Ticker))
                        {
                            bar = stockBars[pricingRule.Ticker];
                        }
                    }

                    TimeSeriesDataOutput output = null;
                    if (bar != null)
                    {
                        output = RetrieveStockTimeSeriesPrices(pricingRule.Ticker, _period, _isAdjustedValue, 0);
                        if (output != null && output.Data != null && output.Data.Count() > 0)
                        {
                            if (output.Data.Select(p => p.AsOfDate).Min() <= bar.AsOfDate)
                            {
                                if (_isAdjustedValue)
                                {
                                    StockBar sameBar = (from p in output.Data
                                                        where p.AsOfDate == bar.AsOfDate
                                                        select p).FirstOrDefault() as StockBar;
                                    if (sameBar != null && sameBar.AdjClose.Value != bar.AdjClose.Value)
                                    {
                                        output = null;
                                    }
                                }
                            }
                            else
                            {
                                output = null;
                            }
                        }
                        else
                        {
                            output = null;
                        }
                    }

                    if (output == null)
                    {
                        output = RetrieveStockTimeSeriesPrices(pricingRule.Ticker, _period, _isAdjustedValue, -1);
                    }

                    if (output != null && output.Data != null && output.Data.Count() > 0)
                    {
                        if (StoreHistoryPrices(pricingRule.SID, _period, _isAdjustedValue, output.Data))
                        {
                            _logger.LogInfo(string.Format("({3}/{4}) Symbol:{0}, SID:{1}, P:{5}, Stored {2} bars", pricingRule.Ticker, pricingRule.SID, output.Data.Count(), idx, totalSymbols, pricingRule.Priority));
                        }
                        else
                        {
                            _logger.LogInfo(string.Format("({3}/{4}) Symbol:{0}, SID:{1}, P:{5}, Failed stored {2} bars", pricingRule.Ticker, pricingRule.SID, output.Data.Count(), idx, totalSymbols, pricingRule.Priority));
                        }
                    }
                    else
                    {
                        _logger.LogInfo(string.Format("({2}/{3}) Symbol:{0}, SID:{1}, P:{4}, Can't retrive data", pricingRule.Ticker, pricingRule.SID, idx, totalSymbols, pricingRule.Priority));
                    }
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(string.Format("({3}/{4}) Symbol:{0}, SID:{1}, Error:{2}", pricingRule.Ticker, pricingRule.SID, ex.Message, idx, totalSymbols));
                }
            };
        }

        public EndpointAddress BuildMarketDataSkywolfHttpEndpointAddress()
        {
            string key = "MarketDataDSkywolfHttp";

            switch (_period)
            {
                case BarFrequency.Day1:
                    key = "MarketDataDSkywolfHttp";
                    break;
                case BarFrequency.Week1:
                    key = "MarketDataWSkywolfHttp";
                    break;
                case BarFrequency.Month1:
                    key = "MarketDataMNSkywolfHttp";
                    break;
                default:
                    key = "MarketDataMSkywolfHttp";
                    break;
            }

            return Utility.BuildEndpointAddress(key);
        }

        public IDictionary<string, long> GetNameToSIDMap(string[] symbols)
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = BuildMarketDataSkywolfHttpEndpointAddress();

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                return skywolf.Instance.GetSIDFromName(symbols);
            }
        }

        public PricingRule[] GetPricingRules()
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = BuildMarketDataSkywolfHttpEndpointAddress();

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                PricingRule[] pricingRules = skywolf.Instance.GetPricingRule(true, "yahoo");
                if (pricingRules != null && pricingRules.Count() > 0)
                {
                    return (from p in pricingRules
                            where p.Priority >= _priorityStart && p.Priority <= _priorityEnd
                            orderby p.Priority descending, p.SID ascending
                            select p).ToArray();
                }
            }

            return null;
        }

        public string[] GetAPIKeys()
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = BuildMarketDataSkywolfHttpEndpointAddress();

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                return skywolf.Instance.VA_GetAvailableAPIKey();
            }
        }

        public TimeSeriesDataOutput RetrieveStockTimeSeriesPrices(string symbol, BarFrequency period, bool isAjustedValue, long outputCount)
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = BuildMarketDataSkywolfHttpEndpointAddress();

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                TimeSeriesDataInput input = new TimeSeriesDataInput();
                input.Frequency = period;
                input.IsAdjustedValue = isAjustedValue;
                input.Symbol = symbol;
                input.OutputCount = outputCount;

                return skywolf.Instance.GetTimeSeriesData(input, DATASOURCE);
            }
        }

        public CryptoTimeSeriesDataOutput RetrieveCryptoTimeSeriesPrices(string symbol, string market, BarFrequency period, bool isAjustedValue, long outputCount)
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = BuildMarketDataSkywolfHttpEndpointAddress();

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                CryptoTimeSeriesDataInput input = new CryptoTimeSeriesDataInput();
                input.Frequency = period;
                input.IsAdjustedValue = isAjustedValue;
                input.Symbol = symbol;
                input.Market = market;
                input.OutputCount = outputCount;

                return skywolf.Instance.GetTimeSeriesData(input, DATASOURCE) as CryptoTimeSeriesDataOutput;
            }
        }

        public bool StoreHistoryPrices(long SID, BarFrequency period, bool isAdjustedValue, Bar[] bars)
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = BuildMarketDataSkywolfHttpEndpointAddress();

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                return skywolf.Instance.VA_StorePrices(SID, period, isAdjustedValue, bars);
            }
        }

        public IDictionary<string, StockBar> GetLatestStockHistoryPrices(string[] symbols, BarFrequency period, bool isAdjustedValue)
        {
            int iCount = 0;

            while (iCount < 3)
            {
                try
                {
                    BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
                    EndpointAddress endpoint = BuildMarketDataSkywolfHttpEndpointAddress();

                    using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
                    {
                        IDictionary<string, StockBar> stockBars = skywolf.Instance.GetLatestStockHistoryPrices(symbols, period, isAdjustedValue, DATASOURCE);

                        ConcurrentDictionary<string, StockBar> dictStockBars = new ConcurrentDictionary<string, StockBar>();

                        if (stockBars != null && stockBars.Count > 0)
                        {
                            foreach (var pair in stockBars)
                            {
                                dictStockBars[pair.Key] = pair.Value;
                            }
                        }

                        return dictStockBars;
                    }
                }
                catch (Exception ex)
                {
                    if (iCount == 2)
                    {
                        throw ex;
                    }
                    iCount++;
                }
            }

            return null;
        }

        public IDictionary<string, CryptoBar> GetLatestCryptoHistoryPrices(string[] symbols, string market, BarFrequency period)
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = BuildMarketDataSkywolfHttpEndpointAddress();

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                return skywolf.Instance.GetLatestCryptoHistoryPrices(symbols, market, period, DATASOURCE);
            }
        }

        public static BarFrequency ConvertStringToBarFrequency(string freq)
        {
            BarFrequency result = BarFrequency.None;
            switch (freq.Trim().ToUpper())
            {
                case "MN":
                    result = BarFrequency.Month1;
                    break;
                case "W1":
                    result = BarFrequency.Week1;
                    break;
                case "D1":
                    result = BarFrequency.Day1;
                    break;
                case "H4":
                    result = BarFrequency.Hour4;
                    break;
                case "H1":
                    result = BarFrequency.Hour1;
                    break;
                case "M30":
                    result = BarFrequency.Minute30;
                    break;
                case "M15":
                    result = BarFrequency.Minute15;
                    break;
                case "M5":
                    result = BarFrequency.Minute5;
                    break;
                case "M1":
                    result = BarFrequency.Minute1;
                    break;
                case "T":
                    result = BarFrequency.Tick;
                    break;
            }

            return result;
        }
    }
}
