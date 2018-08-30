using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skywolf.Contracts.DataContracts.MarketData;
using Skywolf.Contracts.DataContracts.Instrument;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Skywolf.DatabaseRepository
{
    public class MarketDataDatabase
    {
        public static readonly DateTime DB_Min_Date = new DateTime(1900, 1, 1);
        
        protected static object _storeLockObj = new object();

        public PricingRule[] GetPricingRules(string datasource, bool active)
        {
            using (MarketDataDataContext marketData = new MarketDataDataContext())
            {
                return (from p in marketData.vw_PricingRules
                        where p.Active == active && p.DataSource == datasource
                        orderby p.Priority descending
                        select new PricingRule()
                        {
                            Active = p.Active,
                            AsOfDate = p.AsOfDate,
                            DataSource = p.DataSource,
                            SID = p.SID,
                            Ticker = p.Ticker,
                            TimeZone = p.TimeZone,
                            Priority = p.Priority,
                            User = p.Usr,
                            TS = p.TS
                        }).ToArray();
            }
        }

        public string[] VA_GetAvailableAPIKey(int batchId)
        {
            using (MarketDataDataContext marketData = new MarketDataDataContext())
            {
                return (from p in marketData.VA_APIKeyLists
                        where p.Active && p.BatchId == batchId
                        select p.Key).ToArray();
            }
        }

        public IDictionary<long, string> GetSIDToNameMapping()
        {
            using (MarketDataDataContext marketData = new MarketDataDataContext())
            {
                Dictionary<long, string> map = marketData.vw_InstrumentNames.ToDictionary(k => k.SID, v => v.Name);
                return map;
            }
        }

        public IDictionary<string, long> GetSIDFromName(IEnumerable<string> names)
        {
            List<string[]> batches = new List<string[]>();
            string[] securityNames = names.Distinct().ToArray();
            while (securityNames.Length > 0)
            {
                string[] batch = securityNames.Take(1000).ToArray();
                securityNames = securityNames.Skip(1000).ToArray();
                batches.Add(batch);
            }

            ConcurrentDictionary<string, long> dictNameToSID = new ConcurrentDictionary<string, long>();

            Parallel.ForEach(batches, new ParallelOptions() { MaxDegreeOfParallelism = batches.Count() }, batch =>
            {
                if (batch != null && batch.Count() > 0)
                {
                    using (MarketDataDataContext marketData = new MarketDataDataContext())
                    {
                        Dictionary<string, long> map = (from p in marketData.vw_InstrumentNames
                                                        where batch.Contains(p.Name)
                                                        select p).ToDictionary(k => k.Name, v => v.SID);

                        if (map != null && map.Count > 0)
                        {
                            foreach (var pair in map.AsEnumerable())
                            {
                                dictNameToSID[pair.Key] = pair.Value;
                            }
                        }
                    }
                }
            });

            return dictNameToSID;
        }

        public IDictionary<long, string> GetNameFromSID(IEnumerable<long> SIDs)
        {
            List<long[]> batches = new List<long[]>();
            long[] securityIDs = SIDs.Distinct().ToArray();
            while (securityIDs.Length > 0)
            {
                long[] batch = securityIDs.Take(1000).ToArray();
                securityIDs = securityIDs.Skip(1000).ToArray();
                batches.Add(batch);
            }

            ConcurrentDictionary<long, string> dictSIDToName = new ConcurrentDictionary<long, string>();

            Parallel.ForEach(batches, new ParallelOptions() { MaxDegreeOfParallelism = batches.Count() }, batch =>
            {
                if (batch != null && batch.Count() > 0)
                {
                    using (MarketDataDataContext marketData = new MarketDataDataContext())
                    {
                        Dictionary<long, string> map = (from p in marketData.vw_InstrumentNames
                                                        where batch.Contains(p.SID)
                                                        select p).ToDictionary(k => k.SID, v => v.Name);

                        if (map != null && map.Count > 0)
                        {
                            foreach (var pair in map.AsEnumerable())
                            {
                                dictSIDToName[pair.Key] = pair.Value;
                            }
                        }
                    }
                }
            });

            return dictSIDToName;
        }

        public bool VA_StorePrices(long SID, BarFrequency frequency, bool isAdjustedValue, Bar[] bars)
        {
            if (bars == null || bars.Length == 0)
            {
                return false;
            }
            
            bars = (from p in bars
                    where p.AsOfDate >= DB_Min_Date
                    group p by p.AsOfDate into g
                    select g.First()).ToArray();

            using (MarketDataDataContext marketData = new MarketDataDataContext())
            {
                string storedprocedure = AV_GetInsertStoredProcedureName(frequency, isAdjustedValue);
                DataTable dtPrices = AV_ConvertBarToDataTable(SID, isAdjustedValue, bars);

                if (dtPrices != null && dtPrices.Rows.Count > 0)
                {
                    try
                    {

                        marketData.Connection.Open();
                        var cmd = marketData.Connection.CreateCommand();
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = storedprocedure;
                        cmd.CommandTimeout = 3600;
                        cmd.Parameters.Add(new SqlParameter("@TableSR", dtPrices));
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        marketData.Connection.Close();
                    }
                }
            }

            return true;
        }

        private DataTable AV_ConvertBarToDataTable(long SID, bool isAdjustedValue, Bar[] bars)
        {
            if (bars == null || bars.Count() == 0)
            {
                return null;
            }

            DataTable prices = AV_CreateTableType(isAdjustedValue, "Prices");

            foreach (Bar bar in bars)
            {
                DataRow row = prices.NewRow();
                row[Constants.FIELD_SID] = SID;
                row[Constants.FIELD_ASOFDATE] = bar.AsOfDate;
                if (bar.Open.HasValue)
                {
                    row[Constants.FIELD_OPEN] = bar.Open.Value;
                }

                if (bar.High.HasValue)
                {
                    row[Constants.FIELD_HIGH] = bar.High.Value;
                }

                if (bar.Low.HasValue)
                {
                    row[Constants.FIELD_LOW] = bar.Low.Value;
                }

                if (bar.Close.HasValue)
                {
                    row[Constants.FIELD_CLOSE] = bar.Close.Value;
                }

                if (bar.Volume.HasValue)
                {
                    row[Constants.FIELD_VOLUME] = bar.Volume.Value;
                }

                if (bar.TS.HasValue)
                {
                    row[Constants.FIELD_TS] = bar.TS.Value;
                }

                if (isAdjustedValue)
                {
                    StockBar stockBar = bar as StockBar;
                    if (stockBar != null)
                    {
                        if (stockBar.AdjClose.HasValue)
                        {
                            row[Constants.FIELD_ADJCLOSE] = stockBar.AdjClose.Value;
                        }

                        if (stockBar.DividendAmount.HasValue)
                        {
                            row[Constants.FIELD_DIVIDENDAMOUNT] = stockBar.DividendAmount.Value;
                        }

                        if (stockBar.SplitCoefficient.HasValue)
                        {
                            row[Constants.FIELD_SPLITCOEFFICIENT] = stockBar.SplitCoefficient;
                        }
                    }
                }
                else
                {
                    CryptoBar cryptoBar = bar as CryptoBar;
                    if (cryptoBar != null)
                    {
                        if (cryptoBar.MarketCap.HasValue)
                        {
                            row[Constants.FIELD_MARKETCAP] = cryptoBar.MarketCap.Value;
                        }
                    }
                }

                prices.Rows.Add(row);
            }

            return prices;
        }

        private DataTable AV_CreateTableType(bool isAdjustedValue, string tableName)
        {
            DataTable prices = new DataTable(tableName);
            if (isAdjustedValue)
            {
                prices.Columns.Add(Constants.FIELD_SID, typeof(long));
                prices.Columns.Add(Constants.FIELD_ASOFDATE, typeof(DateTime));
                prices.Columns.Add(Constants.FIELD_OPEN, typeof(double));
                prices.Columns.Add(Constants.FIELD_HIGH, typeof(double));
                prices.Columns.Add(Constants.FIELD_LOW, typeof(double));
                prices.Columns.Add(Constants.FIELD_CLOSE, typeof(double));
                prices.Columns.Add(Constants.FIELD_VOLUME, typeof(decimal));
                prices.Columns.Add(Constants.FIELD_ADJCLOSE, typeof(double));
                prices.Columns.Add(Constants.FIELD_DIVIDENDAMOUNT, typeof(double));
                prices.Columns.Add(Constants.FIELD_SPLITCOEFFICIENT, typeof(double));
                prices.Columns.Add(Constants.FIELD_TS, typeof(DateTime));
            }
            else
            {
                prices.Columns.Add(Constants.FIELD_SID, typeof(string));
                prices.Columns.Add(Constants.FIELD_ASOFDATE, typeof(DateTime));
                prices.Columns.Add(Constants.FIELD_OPEN, typeof(double));
                prices.Columns.Add(Constants.FIELD_HIGH, typeof(double));
                prices.Columns.Add(Constants.FIELD_LOW, typeof(double));
                prices.Columns.Add(Constants.FIELD_CLOSE, typeof(double));
                prices.Columns.Add(Constants.FIELD_VOLUME, typeof(decimal));
                prices.Columns.Add(Constants.FIELD_MARKETCAP, typeof(decimal));
                prices.Columns.Add(Constants.FIELD_TS, typeof(DateTime));

            }

            return prices;
        }

        private string AV_GetInsertStoredProcedureName(BarFrequency frequency, bool isAdjustedValue)
        {
            string storedProcedure = null;
            if (isAdjustedValue)
            {
                switch (frequency)
                {
                    case BarFrequency.Day1:
                        storedProcedure = "av.usp_AdjPrices_D1_InsertMany";
                        break;
                    case BarFrequency.Week1:
                        storedProcedure = "av.usp_AdjPrices_W1_InsertMany";
                        break;
                    case BarFrequency.Month1:
                        storedProcedure = "av.usp_AdjPrices_MN_InsertMany";
                        break;
                }
            }
            else
            {
                switch (frequency)
                {
                    case BarFrequency.Minute1:
                        storedProcedure = "av.usp_Prices_M1_InsertMany";
                        break;
                    case BarFrequency.Minute5:
                        storedProcedure = "av.usp_Prices_M5_InsertMany";
                        break;
                    case BarFrequency.Minute15:
                        storedProcedure = "av.usp_Prices_M15_InsertMany";
                        break;
                    case BarFrequency.Minute30:
                        storedProcedure = "av.usp_Prices_M30_InsertMany";
                        break;
                    case BarFrequency.Hour1:
                        storedProcedure = "av.usp_Prices_H1_InsertMany";
                        break;
                    case BarFrequency.Day1:
                        storedProcedure = "av.usp_Prices_D1_InsertMany";
                        break;
                    case BarFrequency.Week1:
                        storedProcedure = "av.usp_Prices_W1_InsertMany";
                        break;
                    case BarFrequency.Month1:
                        storedProcedure = "av.usp_Prices_MN_InsertMany";
                        break;
                }
            }

            return storedProcedure;
        }

        public IDictionary<string, StockBar[]> VA_GetStockPrices(string[] symbols, BarFrequency frequency, DateTime? startDate, DateTime? endDate, int outputCount, bool isAdjustedValue)
        {
            IDictionary<string, long> tickerToSIDMap = GetSIDFromName(symbols);
            
            if (tickerToSIDMap != null && tickerToSIDMap.Count > 0)
            {
                long[] SIDs = tickerToSIDMap.Values.ToArray();

                List<long[]> batches = new List<long[]>();
                long[] securityIDs = SIDs.Distinct().ToArray();
                while (securityIDs.Length > 0)
                {
                    long[] batch = securityIDs.Take(2000).ToArray();
                    securityIDs = securityIDs.Skip(2000).ToArray();
                    batches.Add(batch);
                }

                ConcurrentDictionary<long, string> SIDToTickerMap = new ConcurrentDictionary<long, string>();
                foreach (var pair in tickerToSIDMap)
                {
                    SIDToTickerMap[pair.Value] = pair.Key;
                }

                ConcurrentDictionary<string, StockBar[]> tickerToStockBarMap = new ConcurrentDictionary<string, StockBar[]>();
                Parallel.ForEach(batches, new ParallelOptions() { MaxDegreeOfParallelism = batches.Count() }, batch
                     =>
                {
                    IDictionary<long, StockBar[]> sidToStockBarMap = _VA_GetStockPrices(batch.ToArray(), frequency, startDate, endDate, outputCount, isAdjustedValue);

                    if (sidToStockBarMap != null && sidToStockBarMap.Count > 0)
                    {
                        foreach (var pair in sidToStockBarMap)
                        {
                            tickerToStockBarMap[SIDToTickerMap[pair.Key]] = pair.Value;
                        }
                    }
                });

                return tickerToStockBarMap;
            }

            return null;
        }

        public IDictionary<string, CryptoBar[]> VA_GetCryptoPrices(string[] symbols, string market, BarFrequency frequency, DateTime? startDate, DateTime? endDate, int outputCount)
        {
            string[] tickers = symbols.Select(p => p.ToUpper() + market).ToArray();
            IDictionary<string, long> tickerToSIDMap = GetSIDFromName(tickers);

            if (tickerToSIDMap != null && tickerToSIDMap.Count > 0)
            {
                long[] SIDs = tickerToSIDMap.Values.ToArray();

                List<long[]> batches = new List<long[]>();
                long[] securityIDs = SIDs.Distinct().ToArray();
                while (securityIDs.Length > 0)
                {
                    long[] batch = securityIDs.Take(2000).ToArray();
                    securityIDs = securityIDs.Skip(2000).ToArray();
                    batches.Add(batch);
                }

                ConcurrentDictionary<long, string> SIDToTickerMap = new ConcurrentDictionary<long, string>();
                foreach (var pair in tickerToSIDMap)
                {
                    SIDToTickerMap[pair.Value] = pair.Key.Replace(market, string.Empty);
                }

                ConcurrentDictionary<string, CryptoBar[]> tickerToCryptoBarMap = new ConcurrentDictionary<string, CryptoBar[]>();
                Parallel.ForEach(batches, new ParallelOptions() { MaxDegreeOfParallelism = batches.Count() }, batch
                     =>
                {
                    IDictionary<long, CryptoBar[]> sidToCryptoBarMap = _VA_GetCryptoPrices(batch.ToArray(), frequency, startDate, endDate, outputCount);

                    if (sidToCryptoBarMap != null && sidToCryptoBarMap.Count > 0)
                    {
                        foreach (var pair in sidToCryptoBarMap)
                        {
                            tickerToCryptoBarMap[SIDToTickerMap[pair.Key]] = pair.Value;
                        }
                    }
                });

                return tickerToCryptoBarMap;
            }

            return null;
        }

        public IDictionary<string, StockBar> VA_GetLatestStockPrices(string[] symbols, BarFrequency frequency, bool isAdjustedValue)
        {
            IDictionary<string, long> tickerToSIDMap = GetSIDFromName(symbols);

            if (tickerToSIDMap != null && tickerToSIDMap.Count > 0)
            {
                long[] SIDs = tickerToSIDMap.Values.ToArray();

                List<long[]> batches = new List<long[]>();
                long[] securityIDs = SIDs.Distinct().ToArray();
                while (securityIDs.Length > 0)
                {
                    long[] batch = securityIDs.Take(2000).ToArray();
                    securityIDs = securityIDs.Skip(2000).ToArray();
                    batches.Add(batch);
                }

                ConcurrentDictionary<long, string> SIDToTickerMap = new ConcurrentDictionary<long, string>();
                foreach (var pair in tickerToSIDMap)
                {
                    SIDToTickerMap[pair.Value] = pair.Key;
                }

                ConcurrentDictionary<string, StockBar> tickerToStockBarMap = new ConcurrentDictionary<string, StockBar>();
                Parallel.ForEach(batches, new ParallelOptions() { MaxDegreeOfParallelism = batches.Count() }, batch
                     =>
                {
                    IDictionary<long, StockBar> sidToStockBarMap = _VA_GetLatestStockPrices(batch.ToArray(), frequency, isAdjustedValue);

                    if (sidToStockBarMap != null && sidToStockBarMap.Count > 0)
                    {
                        foreach (var pair in sidToStockBarMap)
                        {
                            tickerToStockBarMap[SIDToTickerMap[pair.Key]] = pair.Value;
                        }
                    }
                });

                return tickerToStockBarMap;
            }

            return null;
        }

        public IDictionary<string, CryptoBar> VA_GetLatestCryptoPrices(string[] symbols, string market, BarFrequency frequency)
        {
            string[] tickers = symbols.Select(p => p.ToUpper() + market).ToArray();
            IDictionary<string, long> tickerToSIDMap = GetSIDFromName(tickers);

            if (tickerToSIDMap != null && tickerToSIDMap.Count > 0)
            {
                long[] SIDs = tickerToSIDMap.Values.ToArray();

                List<long[]> batches = new List<long[]>();
                long[] securityIDs = SIDs.Distinct().ToArray();
                while (securityIDs.Length > 0)
                {
                    long[] batch = securityIDs.Take(2000).ToArray();
                    securityIDs = securityIDs.Skip(2000).ToArray();
                    batches.Add(batch);
                }

                ConcurrentDictionary<long, string> SIDToTickerMap = new ConcurrentDictionary<long, string>();
                foreach (var pair in tickerToSIDMap)
                {
                    SIDToTickerMap[pair.Value] = pair.Key.Replace(market, string.Empty);
                }

                ConcurrentDictionary<string, CryptoBar> tickerToCryptoBarMap = new ConcurrentDictionary<string, CryptoBar>();
                Parallel.ForEach(batches, new ParallelOptions() { MaxDegreeOfParallelism = batches.Count() }, batch
                     =>
                {
                    IDictionary<long, CryptoBar> sidToCryptoBarMap = _VA_GetLatestCryptoPrices(batch.ToArray(), frequency);

                    if (sidToCryptoBarMap != null && sidToCryptoBarMap.Count > 0)
                    {
                        foreach (var pair in sidToCryptoBarMap)
                        {
                            tickerToCryptoBarMap[SIDToTickerMap[pair.Key]] = pair.Value;
                        }
                    }
                });

                return tickerToCryptoBarMap;
            }

            return null;
        }

        private IDictionary<long, StockBar> _VA_GetLatestStockPrices(long[] SIDs, BarFrequency frequency, bool isAdjustedValue)
        {
            Dictionary<long, StockBar> stockBarResult = null;
            if (SIDs != null && SIDs.Count() > 0)
            {
                switch (frequency)
                {
                    case BarFrequency.Minute1:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_M1_Latests
                                                where SIDs.Contains(p.SID)
                                                select p).ToDictionary(k => k.SID, v =>
                                                new StockBar
                                                {
                                                    AsOfDate = v.AsOfDate,
                                                    Close = v.Close,
                                                    High = v.High,
                                                    Low = v.Low,
                                                    Open = v.Open,
                                                    TS = v.TS,
                                                    Volume = v.Volume
                                                });
                        }
                        break;
                    case BarFrequency.Minute5:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_M5_Latests
                                                where SIDs.Contains(p.SID)
                                                select p).ToDictionary(k => k.SID, v => 
                                                new StockBar
                                                {
                                                    AsOfDate = v.AsOfDate,
                                                    Close = v.Close,
                                                    High = v.High,
                                                    Low = v.Low,
                                                    Open = v.Open,
                                                    TS = v.TS,
                                                    Volume = v.Volume
                                                });
                        }
                        break;
                    case BarFrequency.Minute15:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_M15_Latests
                                                where SIDs.Contains(p.SID)
                                                select p).ToDictionary(k => k.SID, v =>
                                                new StockBar
                                                {
                                                    AsOfDate = v.AsOfDate,
                                                    Close = v.Close,
                                                    High = v.High,
                                                    Low = v.Low,
                                                    Open = v.Open,
                                                    TS = v.TS,
                                                    Volume = v.Volume
                                                });
                        }
                        break;
                    case BarFrequency.Minute30:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_M30_Latests
                                                where SIDs.Contains(p.SID)
                                                select p).ToDictionary(k => k.SID, v => 
                                                new StockBar
                                                {
                                                    AsOfDate = v.AsOfDate,
                                                    Close = v.Close,
                                                    High = v.High,
                                                    Low = v.Low,
                                                    Open = v.Open,
                                                    TS = v.TS,
                                                    Volume = v.Volume
                                                });
                        }
                        break;
                    case BarFrequency.Hour1:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_H1_Latests
                                                where SIDs.Contains(p.SID)
                                                select p).ToDictionary(k => k.SID, v =>
                                                new StockBar
                                                {
                                                    AsOfDate = v.AsOfDate,
                                                    Close = v.Close,
                                                    High = v.High,
                                                    Low = v.Low,
                                                    Open = v.Open,
                                                    TS = v.TS,
                                                    Volume = v.Volume
                                                });
                        }
                        break;
                    case BarFrequency.Day1:
                        if (isAdjustedValue)
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                stockBarResult = (from p in marketData.VA_AdjPrices_D1_Latests
                                                    where SIDs.Contains(p.SID)
                                                    select p).ToDictionary(k => k.SID, v =>
                                                    new StockBar
                                                    {
                                                        AsOfDate = v.AsOfDate,
                                                        Close = v.Close,
                                                        High = v.High,
                                                        Low = v.Low,
                                                        Open = v.Open,
                                                        TS = v.TS,
                                                        Volume = v.Volume,
                                                        AdjClose = v.AdjClose,
                                                        DividendAmount = v.DividendAmount,
                                                        SplitCoefficient = v.SplitCoefficient
                                                    });
                            }
                        }
                        else
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                stockBarResult = (from p in marketData.VA_Prices_D1_Latests
                                                    where SIDs.Contains(p.SID)
                                                    select p).ToDictionary(k => k.SID, v =>
                                                    new StockBar
                                                    {
                                                        AsOfDate = v.AsOfDate,
                                                        Close = v.Close,
                                                        High = v.High,
                                                        Low = v.Low,
                                                        Open = v.Open,
                                                        TS = v.TS,
                                                        Volume = v.Volume
                                                    });
                            }
                        }
                        break;
                    case BarFrequency.Week1:
                        if (isAdjustedValue)
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                stockBarResult = (from p in marketData.VA_AdjPrices_W1_Latests
                                                    where SIDs.Contains(p.SID)
                                                    select p).ToDictionary(k => k.SID, v =>
                                                    new StockBar
                                                    {
                                                        AsOfDate = v.AsOfDate,
                                                        Close = v.Close,
                                                        High = v.High,
                                                        Low = v.Low,
                                                        Open = v.Open,
                                                        TS = v.TS,
                                                        Volume = v.Volume,
                                                        AdjClose = v.AdjClose,
                                                        DividendAmount = v.DividendAmount
                                                    });

                            }
                        }
                        else
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                stockBarResult = (from p in marketData.VA_Prices_W1_Latests
                                                    where SIDs.Contains(p.SID)
                                                    select p).ToDictionary(k => k.SID, v =>
                                                    new StockBar
                                                    {
                                                        AsOfDate = v.AsOfDate,
                                                        Close = v.Close,
                                                        High = v.High,
                                                        Low = v.Low,
                                                        Open = v.Open,
                                                        TS = v.TS,
                                                        Volume = v.Volume
                                                    });
                            }
                        }
                        break;
                    case BarFrequency.Month1:
                        if (isAdjustedValue)
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                stockBarResult = (from p in marketData.VA_AdjPrices_MN_Latests
                                                    where SIDs.Contains(p.SID)
                                                    select p).ToDictionary(k => k.SID, v =>
                                                    new StockBar
                                                    {
                                                        AsOfDate = v.AsOfDate,
                                                        Close = v.Close,
                                                        High = v.High,
                                                        Low = v.Low,
                                                        Open = v.Open,
                                                        TS = v.TS,
                                                        Volume = v.Volume,
                                                        AdjClose = v.AdjClose,
                                                        DividendAmount = v.DividendAmount
                                                    });

                            }
                        }
                        else
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                stockBarResult = (from p in marketData.VA_Prices_MN_Latests
                                                    where SIDs.Contains(p.SID)
                                                    select p).ToDictionary(k => k.SID, v =>
                                                    new StockBar
                                                    {
                                                        AsOfDate = v.AsOfDate,
                                                        Close = v.Close,
                                                        High = v.High,
                                                        Low = v.Low,
                                                        Open = v.Open,
                                                        TS = v.TS,
                                                        Volume = v.Volume
                                                    });
                            }
                        }
                        break;
                }
            }

            return stockBarResult;
        }

        private IDictionary<long, CryptoBar> _VA_GetLatestCryptoPrices(long[] SIDs, BarFrequency frequency)
        {
            Dictionary<long, CryptoBar> stockBarResult = null;
            if (SIDs != null && SIDs.Count() > 0)
            {
                switch (frequency)
                {
                    case BarFrequency.Minute1:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_M1_Latests
                                              where SIDs.Contains(p.SID)
                                              orderby p.AsOfDate descending
                                              group p by p.SID into g
                                              select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                              new CryptoBar
                                              {
                                                  AsOfDate = p.AsOfDate,
                                                  Close = p.Close,
                                                  High = p.High,
                                                  Low = p.Low,
                                                  Open = p.Open,
                                                  TS = p.TS,
                                                  Volume = p.Volume,
                                                  MarketCap = p.MarketCap
                                              }).First());
                        }
                        break;
                    case BarFrequency.Minute5:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_M5_Latests
                                              where SIDs.Contains(p.SID)
                                              orderby p.AsOfDate descending
                                              group p by p.SID into g
                                              select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                              new CryptoBar
                                              {
                                                  AsOfDate = p.AsOfDate,
                                                  Close = p.Close,
                                                  High = p.High,
                                                  Low = p.Low,
                                                  Open = p.Open,
                                                  TS = p.TS,
                                                  Volume = p.Volume,
                                                  MarketCap = p.MarketCap
                                              }).First());
                        }
                        break;
                    case BarFrequency.Minute15:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_M15_Latests
                                              where SIDs.Contains(p.SID)
                                              orderby p.AsOfDate descending
                                              group p by p.SID into g
                                              select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                              new CryptoBar
                                              {
                                                  AsOfDate = p.AsOfDate,
                                                  Close = p.Close,
                                                  High = p.High,
                                                  Low = p.Low,
                                                  Open = p.Open,
                                                  TS = p.TS,
                                                  Volume = p.Volume,
                                                  MarketCap = p.MarketCap
                                              }).First());
                        }
                        break;
                    case BarFrequency.Minute30:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_M30_Latests
                                              where SIDs.Contains(p.SID)
                                              orderby p.AsOfDate descending
                                              group p by p.SID into g
                                              select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                              new CryptoBar
                                              {
                                                  AsOfDate = p.AsOfDate,
                                                  Close = p.Close,
                                                  High = p.High,
                                                  Low = p.Low,
                                                  Open = p.Open,
                                                  TS = p.TS,
                                                  Volume = p.Volume,
                                                  MarketCap = p.MarketCap
                                              }).First());
                        }
                        break;
                    case BarFrequency.Hour1:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_H1_Latests
                                              where SIDs.Contains(p.SID)
                                              orderby p.AsOfDate descending
                                              group p by p.SID into g
                                              select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                              new CryptoBar
                                              {
                                                  AsOfDate = p.AsOfDate,
                                                  Close = p.Close,
                                                  High = p.High,
                                                  Low = p.Low,
                                                  Open = p.Open,
                                                  TS = p.TS,
                                                  Volume = p.Volume,
                                                  MarketCap = p.MarketCap
                                              }).First());
                        }
                        break;
                    case BarFrequency.Day1:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_D1_Latests
                                                where SIDs.Contains(p.SID)
                                                orderby p.AsOfDate descending
                                                group p by p.SID into g
                                                select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                new CryptoBar
                                                {
                                                    AsOfDate = p.AsOfDate,
                                                    Close = p.Close,
                                                    High = p.High,
                                                    Low = p.Low,
                                                    Open = p.Open,
                                                    TS = p.TS,
                                                    Volume = p.Volume,
                                                    MarketCap = p.MarketCap
                                                }).First());
                        }
                        break;
                    case BarFrequency.Week1:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_W1_Latests
                                                where SIDs.Contains(p.SID)
                                                orderby p.AsOfDate descending
                                                group p by p.SID into g
                                                select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                new CryptoBar
                                                {
                                                    AsOfDate = p.AsOfDate,
                                                    Close = p.Close,
                                                    High = p.High,
                                                    Low = p.Low,
                                                    Open = p.Open,
                                                    TS = p.TS,
                                                    Volume = p.Volume,
                                                    MarketCap = p.MarketCap
                                                }).First());
                        }

                        break;
                    case BarFrequency.Month1:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            stockBarResult = (from p in marketData.VA_Prices_MN_Latests
                                                where SIDs.Contains(p.SID)
                                                orderby p.AsOfDate descending
                                                group p by p.SID into g
                                                select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                new CryptoBar
                                                {
                                                    AsOfDate = p.AsOfDate,
                                                    Close = p.Close,
                                                    High = p.High,
                                                    Low = p.Low,
                                                    Open = p.Open,
                                                    TS = p.TS,
                                                    Volume = p.Volume,
                                                    MarketCap = p.MarketCap
                                                }).First());
                        }
                        break;
                }
            }

            return stockBarResult;
        }

        private IDictionary<long, StockBar[]> _VA_GetStockPrices(long[] SIDs, BarFrequency frequency, DateTime? startDate, DateTime? endDate, int outputCount, bool isAdjustedValue)
        {
            DateTime begin = startDate.HasValue ? startDate.Value : new DateTime(1900,1,1);
            DateTime end = endDate.HasValue ? endDate.Value : DateTime.Now.AddDays(4);

            Dictionary<long, StockBar[]> stockBarResult = null;
            if (SIDs != null && SIDs.Count() > 0)
            {
                switch (frequency)
                {
                    case BarFrequency.Minute1:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            if (outputCount <= 0)
                            {
                                stockBarResult = (from p in marketData.VA_Prices_M1s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume
                                                  }).ToArray());
                            }
                            else
                            {
                                stockBarResult = (from p in marketData.VA_Prices_M1s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume
                                                  }).Take(outputCount).ToArray());
                            }
                        }
                        break;
                    case BarFrequency.Minute5:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            if (outputCount <= 0)
                            {
                                stockBarResult = (from p in marketData.VA_Prices_M5s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume
                                                  }).ToArray());
                            }
                            else
                            {
                                stockBarResult = (from p in marketData.VA_Prices_M5s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume
                                                  }).Take(outputCount).ToArray());
                            }
                        }
                        break;
                    case BarFrequency.Minute15:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            if (outputCount <= 0)
                            {
                                stockBarResult = (from p in marketData.VA_Prices_M15s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume
                                                  }).ToArray());
                            }
                            else
                            {
                                stockBarResult = (from p in marketData.VA_Prices_M15s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume
                                                  }).Take(outputCount).ToArray());
                            }
                        }
                        break;
                    case BarFrequency.Minute30:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            if (outputCount <= 0)
                            {
                                stockBarResult = (from p in marketData.VA_Prices_M30s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume
                                                  }).ToArray());
                            }
                            else
                            {
                                stockBarResult = (from p in marketData.VA_Prices_M30s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume
                                                  }).Take(outputCount).ToArray());
                            }
                        }
                        break;
                    case BarFrequency.Hour1:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            if (outputCount <= 0)
                            {
                                stockBarResult = (from p in marketData.VA_Prices_H1s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume
                                                  }).ToArray());
                            }
                            else
                            {
                                stockBarResult = (from p in marketData.VA_Prices_H1s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume
                                                  }).Take(outputCount).ToArray());
                            }
                        }
                        break;
                    case BarFrequency.Day1:
                        if (isAdjustedValue)
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                if (outputCount <= 0)
                                {
                                    stockBarResult = (from p in marketData.VA_AdjPrices_D1s
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume,
                                                          AdjClose = p.AdjClose,
                                                          DividendAmount = p.DividendAmount,
                                                          SplitCoefficient = p.SplitCoefficient
                                                      }).ToArray());
                                }
                                else
                                {
                                    stockBarResult = (from p in marketData.VA_AdjPrices_D1s
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume,
                                                          AdjClose = p.AdjClose,
                                                          DividendAmount = p.DividendAmount,
                                                          SplitCoefficient = p.SplitCoefficient
                                                      }).Take(outputCount).ToArray());
                                }
                            }
                        }
                        else
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                if (outputCount <= 0)
                                {
                                    stockBarResult = (from p in marketData.VA_Prices_D1s
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume
                                                      }).ToArray());
                                }
                                else
                                {
                                    stockBarResult = (from p in marketData.VA_Prices_D1s
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume
                                                      }).Take(outputCount).ToArray());
                                }
                            }
                        }
                        break;
                    case BarFrequency.Week1:
                        if (isAdjustedValue)
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                if (outputCount <= 0)
                                {
                                    stockBarResult = (from p in marketData.VA_AdjPrices_W1s
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume,
                                                          AdjClose = p.AdjClose,
                                                          DividendAmount = p.DividendAmount
                                                      }).ToArray());
                                }
                                else
                                {
                                    stockBarResult = (from p in marketData.VA_AdjPrices_W1s
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume,
                                                          AdjClose = p.AdjClose,
                                                          DividendAmount = p.DividendAmount
                                                      }).Take(outputCount).ToArray());
                                }
                            }
                        }
                        else
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                if (outputCount <= 0)
                                {
                                    stockBarResult = (from p in marketData.VA_Prices_W1s
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume
                                                      }).ToArray());
                                }
                                else
                                {
                                    stockBarResult = (from p in marketData.VA_Prices_W1s
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume
                                                      }).Take(outputCount).ToArray());
                                }
                            }
                        }
                        break;
                    case BarFrequency.Month1:
                        if (isAdjustedValue)
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                if (outputCount <= 0)
                                {
                                    stockBarResult = (from p in marketData.VA_AdjPrices_MNs
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume,
                                                          AdjClose = p.AdjClose,
                                                          DividendAmount = p.DividendAmount
                                                      }).ToArray());
                                }
                                else
                                {
                                    stockBarResult = (from p in marketData.VA_AdjPrices_MNs
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume,
                                                          AdjClose = p.AdjClose,
                                                          DividendAmount = p.DividendAmount
                                                      }).Take(outputCount).ToArray());
                                }
                            }
                        }
                        else
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                if (outputCount <= 0)
                                {
                                    stockBarResult = (from p in marketData.VA_Prices_MNs
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume
                                                      }).ToArray());
                                }
                                else
                                {
                                    stockBarResult = (from p in marketData.VA_Prices_MNs
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume
                                                      }).Take(outputCount).ToArray());
                                }
                            }
                        }
                        break;
                }
            }

            return stockBarResult;
        }

        private IDictionary<long, CryptoBar[]> _VA_GetCryptoPrices(long[] SIDs, BarFrequency frequency, DateTime? startDate, DateTime? endDate, int outputCount)
        {
            DateTime begin = startDate.HasValue ? startDate.Value : new DateTime(1900, 1, 1);
            DateTime end = endDate.HasValue ? endDate.Value : DateTime.Now.AddDays(4);

            Dictionary<long, CryptoBar[]> cryptoBarResult = null;
            if (SIDs != null && SIDs.Count() > 0)
            {
                switch (frequency)
                {
                    case BarFrequency.Minute1:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            if (outputCount <= 0)
                            {
                                cryptoBarResult = (from p in marketData.VA_Prices_M1s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(m => new CryptoBar
                                                  {
                                                      AsOfDate = m.AsOfDate,
                                                      Close = m.Close,
                                                      High = m.High,
                                                      Low = m.Low,
                                                      Open = m.Open,
                                                      TS = m.TS,
                                                      Volume = m.Volume,
                                                      MarketCap = m.MarketCap
                                                  }).ToArray());
                            }
                            else
                            {
                                cryptoBarResult = (from p in marketData.VA_Prices_M1s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(m =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = m.AsOfDate,
                                                      Close = m.Close,
                                                      High = m.High,
                                                      Low = m.Low,
                                                      Open = m.Open,
                                                      TS = m.TS,
                                                      Volume = m.Volume,
                                                      MarketCap = m.MarketCap
                                                  }).Take(outputCount).ToArray());
                            }
                        }
                        break;
                    case BarFrequency.Minute5:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            if (outputCount <= 0)
                            {
                                cryptoBarResult = (from p in marketData.VA_Prices_M5s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(m =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = m.AsOfDate,
                                                      Close = m.Close,
                                                      High = m.High,
                                                      Low = m.Low,
                                                      Open = m.Open,
                                                      TS = m.TS,
                                                      Volume = m.Volume,
                                                      MarketCap = m.MarketCap
                                                  }).ToArray());
                            }
                            else
                            {
                                cryptoBarResult = (from p in marketData.VA_Prices_M5s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume,
                                                      MarketCap = p.MarketCap
                                                  }).Take(outputCount).ToArray());
                            }
                        }
                        break;
                    case BarFrequency.Minute15:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            if (outputCount <= 0)
                            {
                                cryptoBarResult = (from p in marketData.VA_Prices_M15s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume,
                                                      MarketCap = p.MarketCap
                                                  }).ToArray());
                            }
                            else
                            {
                                cryptoBarResult = (from p in marketData.VA_Prices_M15s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume,
                                                      MarketCap = p.MarketCap
                                                  }).Take(outputCount).ToArray());
                            }
                        }
                        break;
                    case BarFrequency.Minute30:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            if (outputCount <= 0)
                            {
                                cryptoBarResult = (from p in marketData.VA_Prices_M30s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume,
                                                      MarketCap = p.MarketCap
                                                  }).ToArray());
                            }
                            else
                            {
                                cryptoBarResult = (from p in marketData.VA_Prices_M30s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume,
                                                      MarketCap = p.MarketCap
                                                  }).Take(outputCount).ToArray());
                            }
                        }
                        break;
                    case BarFrequency.Hour1:
                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            if (outputCount <= 0)
                            {
                                cryptoBarResult = (from p in marketData.VA_Prices_H1s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume,
                                                      MarketCap = p.MarketCap
                                                  }).ToArray());
                            }
                            else
                            {
                                cryptoBarResult = (from p in marketData.VA_Prices_H1s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume,
                                                      MarketCap = p.MarketCap
                                                  }).Take(outputCount).ToArray());
                            }
                        }
                        break;
                    case BarFrequency.Day1:

                        using (MarketDataDataContext marketData = new MarketDataDataContext())
                        {
                            if (outputCount <= 0)
                            {
                                cryptoBarResult = (from p in marketData.VA_Prices_D1s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume,
                                                      MarketCap = p.MarketCap
                                                  }).ToArray());
                            }
                            else
                            {
                                cryptoBarResult = (from p in marketData.VA_Prices_D1s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS,
                                                      Volume = p.Volume,
                                                      MarketCap = p.MarketCap
                                                  }).Take(outputCount).ToArray());
                            }
                        }
                        break;
                    case BarFrequency.Week1:
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                if (outputCount <= 0)
                                {
                                    cryptoBarResult = (from p in marketData.VA_Prices_W1s
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new CryptoBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume,
                                                          MarketCap = p.MarketCap
                                                      }).ToArray());
                                }
                                else
                                {
                                    cryptoBarResult = (from p in marketData.VA_Prices_W1s
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new CryptoBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume,
                                                          MarketCap = p.MarketCap
                                                      }).Take(outputCount).ToArray());
                                }
                            }
                        }
                        break;
                    case BarFrequency.Month1:
                        {
                            using (MarketDataDataContext marketData = new MarketDataDataContext())
                            {
                                if (outputCount <= 0)
                                {
                                    cryptoBarResult = (from p in marketData.VA_Prices_MNs
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new CryptoBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume,
                                                          MarketCap = p.MarketCap
                                                      }).ToArray());
                                }
                                else
                                {
                                    cryptoBarResult = (from p in marketData.VA_Prices_MNs
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => k.Key, v => v.Select(p =>
                                                      new CryptoBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS,
                                                          Volume = p.Volume,
                                                          MarketCap = p.MarketCap
                                                      }).Take(outputCount).ToArray());
                                }
                            }
                        }
                        break;
                }
            }

            return cryptoBarResult;
        }
    }
}
