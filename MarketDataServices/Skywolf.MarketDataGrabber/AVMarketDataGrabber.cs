using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Skywolf.Contracts.DataContracts.MarketData;
using Skywolf.Utility;
using System.Data;
using log4net;
using System.Threading;

namespace Skywolf.MarketDataGrabber
{
    public class AVMarketDataGrabber : BaseMarketDataGrabber
    {
        const string AV_STOCK_TIME_SERIES_INTERVAL_FORMAT = @"https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&symbol={1}&interval={2}&outputsize={3}&datatype=csv&apikey={0}";
        const string AV_STOCK_TIME_SERIES_DAILY_FORMAT = @"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={1}&outputsize={2}&datatype=csv&apikey={0}";
        const string AV_STOCK_TIME_SERIES_DAILY_ADJUSTED_FORMAT = @"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={1}&outputsize={2}&datatype=csv&apikey={0}";
        const string AV_STOCK_TIME_SERIES_WEEKLY_FORMAT = @"https://www.alphavantage.co/query?function=TIME_SERIES_WEEKLY&symbol={1}&datatype=csv&apikey={0}";
        const string AV_STOCK_TIME_SERIES_WEEKLY_ADJUSTED_FORMAT = @"https://www.alphavantage.co/query?function=TIME_SERIES_WEEKLY_ADJUSTED&symbol={1}&datatype=csv&apikey={0}";
        const string AV_STOCK_TIME_SERIES_MONTHLY_FORMAT = @"https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY&symbol={1}&datatype=csv&apikey={0}";
        const string AV_STOCK_TIME_SERIES_MONTHLY_ADJUSTED_FORMAT = @"https://www.alphavantage.co/query?function=TIME_SERIES_MONTHLY_ADJUSTED&symbol={1}&datatype=csv&apikey={0}";
        const string AV_STOCK_BATCH_QUOTES = @"https://www.alphavantage.co/query?function=BATCH_STOCK_QUOTES&symbols={1}&apikey={0}&datatype=csv";
        const string AV_CURRENCY_EXCHANGE_RATE_FORMAT = @"https://www.alphavantage.co/query?function=CURRENCY_EXCHANGE_RATE&from_currency={1}&to_currency={2}&apikey={0}";
        const string AV_CRYPTO_DIGITAL_CURRENCY_INTRADAY = @"https://www.alphavantage.co/query?function=DIGITAL_CURRENCY_INTRADAY&symbol={1}&market={2}&apikey={0}&datatype=csv";
        const string AV_CRYPTO_DIGITAL_CURRENCY_DAILY = @"https://www.alphavantage.co/query?function=DIGITAL_CURRENCY_DAILY&symbol={1}&market={2}&apikey={0}&datatype=csv";
        const string AV_CRYPTO_DIGITAL_CURRENCY_WEEKLY = @"https://www.alphavantage.co/query?function=DIGITAL_CURRENCY_WEEKLY&symbol={1}&market={2}&apikey={0}&datatype=csv";
        const string AV_CRYPTO_DIGITAL_CURRENCY_MONTHLY = @"https://www.alphavantage.co/query?function=DIGITAL_CURRENCY_MONTHLY&symbol={1}&market={2}&apikey={0}&datatype=csv";

        const string AV_FIELD_SYMBOL = "symbol";
        const string AV_FIELD_PRICE = "price";
        const string AV_FIELD_VOLUME = "volume";
        const string AV_FIELD_TIMESTAMP = "timestamp";
        const string AV_FIELD_OPEN = "open";
        const string AV_FIELD_HIGH = "high";
        const string AV_FIELD_LOW = "low";
        const string AV_FIELD_CLOSE = "close";
        const string AV_FIELD_DIVIDEND_AMOUNT = "dividend_amount";
        const string AV_FIELD_SPLIT_COEFFICIENT = "split_coefficient";
        const string AV_FIELD_ADJUSTED_CLOSE = "adjusted_close";
        const string AV_FIELD_MARKET_CAP = "market cap";

        static string[] AV_FIELD_LIST = new string[] {
            AV_FIELD_SYMBOL,
            AV_FIELD_PRICE,
            AV_FIELD_VOLUME,
            AV_FIELD_TIMESTAMP,
            AV_FIELD_OPEN,
            AV_FIELD_HIGH,
            AV_FIELD_LOW,
            AV_FIELD_CLOSE,
            AV_FIELD_DIVIDEND_AMOUNT,
            AV_FIELD_SPLIT_COEFFICIENT,
            AV_FIELD_ADJUSTED_CLOSE
        };

        protected static APIKeyManager _keyManager = new APIKeyManager();

        static AVMarketDataGrabber()
        {
            string defaultKey = ConfigurationManager.AppSettings["AlphaVentageAPIKey"];
            if (!string.IsNullOrEmpty(defaultKey))
                _keyManager.AddAPIKey(new string[] { defaultKey });

        }

        #region AV Calls

        public override TimeSeriesDataOutput GetTimeSeriesData(TimeSeriesDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.Symbol))
            {
                return null;
            }

            string timeSeriesData = GetTimeSeriesDataDirect(input);

            if (!string.IsNullOrEmpty(timeSeriesData))
            {
                DataTable dtTimeSeries = TextUtility.ConvertCSVToTable(timeSeriesData, input.Symbol);
                return ConvertDataToTimeSeriesDataOutput(input, dtTimeSeries);
            }

            return null;
        }

        protected static object _AVHttpGetObj = new object();
        protected static DateTime _lastCallingTime = DateTime.MinValue;

        public string AVHttpGet(string url)
        {
            lock (_AVHttpGetObj)
            {
                while ((DateTime.Now - _lastCallingTime).TotalSeconds < 1.0)
                {
                    Thread.Sleep(1000);
                }

                string result = HttpGet(url);
                _lastCallingTime = DateTime.Now;

                while (!string.IsNullOrEmpty(result) && result.StartsWith("{"))
                {
                    Thread.Sleep(5000);
                    result = HttpGet(url);
                    _lastCallingTime = DateTime.Now;
                }

                return result;
            }
        }

        public string GetTimeSeriesDataDirect(TimeSeriesDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.Symbol))
            {
                return null;
            }

            string outputsize = "compact";
            if (input.OutputCount < 0)
            {
                outputsize = "full";
            }

            string output = string.Empty;

            if (input is CryptoTimeSeriesDataInput)
            {
                CryptoTimeSeriesDataInput cryptoInput = input as CryptoTimeSeriesDataInput;
                string market = "CNY";
                if (cryptoInput.Market != "USD")
                {
                    market = cryptoInput.Market;
                }
                switch (input.Frequency)
                {
                    case BarFrequency.Minute5:
                        output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_CRYPTO_DIGITAL_CURRENCY_INTRADAY, key, cryptoInput.Symbol, market)));
                        break;
                    case BarFrequency.Day1:
                        output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_CRYPTO_DIGITAL_CURRENCY_DAILY, key, cryptoInput.Symbol, market)));
                        break;
                    case BarFrequency.Week1:
                        output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_CRYPTO_DIGITAL_CURRENCY_WEEKLY, key, cryptoInput.Symbol, market)));
                        break;
                    case BarFrequency.Month1:
                        output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_CRYPTO_DIGITAL_CURRENCY_MONTHLY, key, cryptoInput.Symbol, market)));
                        break;
                }
            }
            else
            {
                if (input.Frequency < BarFrequency.Day1)
                {
                    switch (input.Frequency)
                    {
                        case BarFrequency.Minute1:
                            output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_STOCK_TIME_SERIES_INTERVAL_FORMAT, key, input.Symbol, "1min", outputsize)));
                            break;
                        case BarFrequency.Minute5:
                            output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_STOCK_TIME_SERIES_INTERVAL_FORMAT, key, input.Symbol, "5min", outputsize)));
                            break;
                        case BarFrequency.Minute15:
                            output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_STOCK_TIME_SERIES_INTERVAL_FORMAT, key, input.Symbol, "15min", outputsize)));
                            break;
                        case BarFrequency.Minute30:
                            output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_STOCK_TIME_SERIES_INTERVAL_FORMAT, key, input.Symbol, "30min", outputsize)));
                            break;
                        case BarFrequency.Hour1:
                            output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_STOCK_TIME_SERIES_INTERVAL_FORMAT, key, input.Symbol, "60min", outputsize)));
                            break;
                    }
                }
                else 
                {
                    if (input.IsAdjustedValue)
                    {
                        switch (input.Frequency)
                        {
                            case BarFrequency.Day1:
                                output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_STOCK_TIME_SERIES_DAILY_ADJUSTED_FORMAT, key, input.Symbol, outputsize)));
                                break;
                            case BarFrequency.Week1:
                                output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_STOCK_TIME_SERIES_WEEKLY_ADJUSTED_FORMAT, key, input.Symbol)));
                                break;
                            case BarFrequency.Month1:
                                output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_STOCK_TIME_SERIES_MONTHLY_ADJUSTED_FORMAT, key, input.Symbol)));
                                break;
                        }
                    }
                    else
                    {
                        switch (input.Frequency)
                        {
                            case BarFrequency.Day1:
                                output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_STOCK_TIME_SERIES_DAILY_FORMAT, key, input.Symbol, outputsize)));
                                break;
                            case BarFrequency.Week1:
                                output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_STOCK_TIME_SERIES_WEEKLY_FORMAT, key, input.Symbol)));
                                break;
                            case BarFrequency.Month1:
                                output = _keyManager.Call<string>(key => AVHttpGet(string.Format(AV_STOCK_TIME_SERIES_MONTHLY_FORMAT, key, input.Symbol)));
                                break;
                        }
                    }
                }
            }

            return output;
        }

        public override Quote[] StockBatchQuote(IEnumerable<string> symbols)
        {
            List<string[]> batchList = new List<string[]>();

            string[] symbolNames = symbols.Distinct().ToArray();

            while (symbolNames.Length > 0)
            {
                string[] splitedSymbols = symbolNames.Take(100).ToArray();
                symbolNames = symbolNames.Skip(100).ToArray();
                batchList.Add(splitedSymbols);
            }

            ConcurrentBag<Quote> quotes = new ConcurrentBag<Quote>();
            Parallel.ForEach(batchList, s =>
            {
                try
                {
                    string symbolString = string.Join(",", s);
                    string quoteResult = _keyManager.Call<string>(key => HttpGet(string.Format(AV_STOCK_BATCH_QUOTES, key, symbolString)));

                    if (!string.IsNullOrEmpty(quoteResult))
                    {
                        DataTable dtQuotes = TextUtility.ConvertCSVToTable(quoteResult, "Quotes");
                        foreach (DataRow row in dtQuotes.AsEnumerable())
                        {
                            try
                            {
                                Quote quote = new Quote();
                                quote.Price = double.Parse(row.Field<string>(AV_FIELD_PRICE));
                                quote.Ask = quote.Price;
                                quote.Bid = quote.Price;
                                quote.Symbol = row.Field<string>(AV_FIELD_SYMBOL);
                                quote.TimeZone = "US/Eastern";
                                try
                                {
                                    quote.Volume = double.Parse(row.Field<string>(AV_FIELD_VOLUME));
                                }
                                catch (Exception)
                                {

                                }
                                try
                                {
                                    quote.TimeStamp = DateTime.Parse(row.Field<string>(AV_FIELD_TIMESTAMP));
                                }
                                catch (Exception)
                                {

                                }

                                quotes.Add(quote);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
            });

            return quotes.ToArray();
        }

        #endregion

        #region Helper Functions

        protected TimeSeriesDataOutput ConvertDataToTimeSeriesDataOutput(TimeSeriesDataInput input, DataTable dtTimeSeries)
        {
            if (input == null || string.IsNullOrEmpty(input.Symbol) || dtTimeSeries == null || dtTimeSeries.Rows.Count == 0)
            {
                return null;
            }

            TimeSeriesDataOutput result = null;

            if (input is CryptoTimeSeriesDataInput)
            {
                CryptoTimeSeriesDataInput cryptoInput = input as CryptoTimeSeriesDataInput;
                CryptoTimeSeriesDataOutput output = new CryptoTimeSeriesDataOutput();
                output.Symbol = cryptoInput.Symbol;
                output.Market = cryptoInput.Market;
                output.TimeZone = "UTC";
                List<CryptoBar> cryptoBars = new List<CryptoBar>();
                List<string> fieldList = new List<string>();

                foreach (DataColumn col in dtTimeSeries.Columns)
                {
                    if (col.ColumnName.Contains(output.Market))
                    {
                        fieldList.Add(col.ColumnName);
                    }
                    else if (AV_FIELD_LIST.Contains(col.ColumnName))
                    {
                        fieldList.Add(col.ColumnName);
                    }
                }

                foreach (DataRow row in dtTimeSeries.AsEnumerable())
                {
                    CryptoBar cryptoBar = new CryptoBar();
                    foreach (string field in fieldList)
                    {
                        if (field == AV_FIELD_TIMESTAMP)
                        {
                            cryptoBar.AsOfDate = Convert.ToDateTime(row[field]);
                        }
                        else if (field == AV_FIELD_VOLUME)
                        {
                            cryptoBar.Volume = row[field].ToDecimal();
                        }
                        else if (field.StartsWith(AV_FIELD_MARKET_CAP))
                        {
                            cryptoBar.MarketCap = row[field].ToDecimal();
                        }
                        else if (field.StartsWith(AV_FIELD_OPEN))
                        {
                            cryptoBar.Open = row[field].ToDouble();
                        }
                        else if (field.StartsWith(AV_FIELD_HIGH))
                        {
                            cryptoBar.High = row[field].ToDouble();
                        }
                        else if (field.StartsWith(AV_FIELD_LOW))
                        {
                            cryptoBar.Low = row[field].ToDouble();
                        }
                        else if (field.StartsWith(AV_FIELD_CLOSE) || field.StartsWith(AV_FIELD_PRICE))
                        {
                            cryptoBar.Close = row[field].ToDouble();
                        }
                    }

                    cryptoBars.Add(cryptoBar);
                }

                output.Data = cryptoBars.ToArray();
                result = output;
            }
            else
            {
                TimeSeriesDataOutput output = new TimeSeriesDataOutput();
                output.Symbol = input.Symbol;
                output.TimeZone = "US/Eastern";
                List<StockBar> stockBars = new List<StockBar>();
                List<string> fields = new List<string>();

                foreach (string field in AV_FIELD_LIST)
                {
                    if (dtTimeSeries.Columns.Contains(field))
                    {
                        fields.Add(field);
                    }
                }

                foreach (DataRow row in dtTimeSeries.AsEnumerable())
                {
                    StockBar bar = new StockBar();
                    foreach (string field in fields)
                    {
                        switch (field)
                        {
                            case AV_FIELD_TIMESTAMP:
                                bar.AsOfDate = DateTime.Parse(row.Field<string>(field));
                                break;
                            case AV_FIELD_OPEN:
                                bar.Open = row[AV_FIELD_OPEN].ToDouble();
                                break;
                            case AV_FIELD_HIGH:
                                bar.High = row[AV_FIELD_HIGH].ToDouble();
                                break;
                            case AV_FIELD_LOW:
                                bar.Low = row[AV_FIELD_LOW].ToDouble();
                                break;
                            case AV_FIELD_PRICE:
                            case AV_FIELD_CLOSE:
                                bar.Close = row[AV_FIELD_CLOSE].ToDouble();
                                break;
                            case AV_FIELD_VOLUME:
                                bar.Volume = row[AV_FIELD_VOLUME].ToDecimal();
                                break;
                            case AV_FIELD_SPLIT_COEFFICIENT:
                                bar.SplitCoefficient = row[AV_FIELD_SPLIT_COEFFICIENT].ToDouble();
                                break;
                            case AV_FIELD_DIVIDEND_AMOUNT:
                                bar.DividendAmount = row[AV_FIELD_DIVIDEND_AMOUNT].ToDouble();
                                break;
                            case AV_FIELD_ADJUSTED_CLOSE:
                                bar.AdjClose = row[AV_FIELD_ADJUSTED_CLOSE].ToDouble();
                                break;
                           
                        }
                    }
                    stockBars.Add(bar);
                }

                output.Data = stockBars.ToArray();
                result = output;
            }

            return result;
        }

        #endregion

        #region AV Keys

        public void AddAPIKeys(IEnumerable<string> apiKeys)
        {
            _keyManager.AddAPIKey(apiKeys);
        }

        public void RemoveAPIKeys(IEnumerable<string> apiKeys)
        {
            _keyManager.RemoveAPIKey(apiKeys);
        }

        public void UpdateAPIKeys(IEnumerable<string> apiKeys)
        {
            _keyManager.UpdateAPIKey(apiKeys);
        }

        public int GetAPIKeyCount()
        {
            return _keyManager.GetKeyCount();
        }
        #endregion
    }
}
