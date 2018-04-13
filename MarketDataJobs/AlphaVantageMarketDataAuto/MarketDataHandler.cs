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
        
        public MarketDataHandler(string period, bool isAdjustedValue)
        {
            _period = ConvertStringToBarFrequency(period);
            _isAdjustedValue = isAdjustedValue;
        }

        public void Update()
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
            IDictionary<string, StockBar[]> stockBars = GetStockHistoryPrices(pricingRules.Select(p => p.Ticker).ToArray(), _period, _isAdjustedValue);

            object countObj = new object();

            int handleCount = 0;
            _logger.LogInfo("Start retriving prices from Alpha Vantage.");
            Parallel.ForEach(pricingRules, new ParallelOptions() { MaxDegreeOfParallelism = (keyCount > 1 ? keyCount : 1) }, pricingRule =>
            {
                int idx;
                lock (countObj)
                {
                    idx = handleCount + 1;
                    handleCount++;
                }

                try
                {
                    StockBar bar = null;

                    if (stockBars != null && stockBars.Count > 0)
                    {
                        StockBar[] bars = null;
                        if (stockBars.TryGetValue(pricingRule.Ticker, out bars))
                        {
                            if (bars.Count() > 0)
                            {
                                bar = bars.OrderByDescending(p => p.AsOfDate).FirstOrDefault();
                            }
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
                            _logger.LogInfo(string.Format("({3}/{4}) Symbol:{0}, SID:{1}, Stored {2} bars", pricingRule.Ticker, pricingRule.SID, output.Data.Count(), idx, totalSymbols));
                        }
                        else
                        {
                            _logger.LogInfo(string.Format("({3}/{4}) Symbol:{0}, SID:{1}, Failed stored {2} bars", pricingRule.Ticker, pricingRule.SID, output.Data.Count(), idx, totalSymbols));
                        }
                    }
                    else
                    {
                        _logger.LogInfo(string.Format("({2}/{3}) Symbol:{0}, SID:{1}, Can't retrive data", pricingRule.Ticker, pricingRule.SID, idx, totalSymbols));
                    }
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError(string.Format("({3}/{4}) Symbol:{0}, SID:{1}, Error:{2}", pricingRule.Ticker, pricingRule.SID, ex.Message, idx, totalSymbols));
                }
            });
        }

        public IDictionary<string, long> GetNameToSIDMap(string[] symbols)
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = Utility.BuildEndpointAddress("MarketDataSkywolfHttp");

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                return skywolf.Instance.GetSIDFromName(symbols);
            }
        }

        public PricingRule[] GetPricingRules()
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = Utility.BuildEndpointAddress("MarketDataSkywolfHttp");

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                return skywolf.Instance.GetPricingRule(true, "yahoo");
            }
        }

        public string[] GetAPIKeys()
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = Utility.BuildEndpointAddress("MarketDataSkywolfHttp");

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                return skywolf.Instance.VA_GetAvailableAPIKey();
            }
        }

        public TimeSeriesDataOutput RetrieveStockTimeSeriesPrices(string symbol, BarFrequency period, bool isAjustedValue, long outputCount)
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = Utility.BuildEndpointAddress("MarketDataSkywolfHttp");

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
            EndpointAddress endpoint = Utility.BuildEndpointAddress("MarketDataSkywolfHttp");

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
            EndpointAddress endpoint = Utility.BuildEndpointAddress("MarketDataSkywolfHttp");

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                return skywolf.Instance.VA_StorePrices(SID, period, isAdjustedValue, bars);
            }
        }

        public IDictionary<string, StockBar[]> GetStockHistoryPrices(string[] symbols, BarFrequency period, bool isAdjustedValue)
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = Utility.BuildEndpointAddress("MarketDataSkywolfHttp");

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                IDictionary<string, StockBar[]> stockBars = skywolf.Instance.GetStockHistoryPrices(symbols, period, null, null, 1, isAdjustedValue, DATASOURCE);
                if (stockBars != null && stockBars.Count > 0)
                {
                    ConcurrentDictionary<string, StockBar[]> dictStockBars = new ConcurrentDictionary<string, StockBar[]>();
                    foreach (var pair in stockBars)
                    {
                        dictStockBars[pair.Key] = pair.Value;
                    }

                    return dictStockBars;
                }
            }

            return null;
        }

        public IDictionary<string, CryptoBar[]> GetCryptoHistoryPrices(string[] symbols, string market, BarFrequency period)
        {
            BasicHttpBinding binding = Utility.BuildBasicHttpBinding();
            EndpointAddress endpoint = Utility.BuildEndpointAddress("MarketDataSkywolfHttp");

            using (SkywolfClient<IMarketDataService> skywolf = new SkywolfClient<IMarketDataService>(binding, endpoint))
            {
                return skywolf.Instance.GetCryptoHistoryPrices(symbols, market, period, null, null, 1, DATASOURCE);
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
