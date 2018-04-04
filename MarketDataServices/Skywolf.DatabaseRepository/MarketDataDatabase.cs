using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skywolf.Contracts.DataContracts.MarketData;
using System.Collections.Concurrent;

namespace Skywolf.DatabaseRepository
{
    public class MarketDataDatabase
    {
        public string[] VA_GetAvailableAPIKey()
        {
            using (MarketDataDataContext marketData = new MarketDataDataContext())
            {
                return (from p in marketData.VA_APIKeyLists
                 where p.Active
                 select p.Key).ToArray();
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

            using (MarketDataDataContext marketData = new MarketDataDataContext())
            {
                string tableName = GetTableName(frequency, isAdjustedValue);
                DateTime maxDate = bars.Select(p => p.AsOfDate).Max();
                DateTime minDate = bars.Select(p => p.AsOfDate).Min();
                marketData.ExecuteCommand(string.Format("delete from {0} where SID = {1} and AsOfDate >= '{2}' and AsOfDate <= '{3}'", tableName, SID, minDate, maxDate));

                if (isAdjustedValue)
                {
                    switch (frequency)
                    {
                        case BarFrequency.Day1:
                            marketData.VA_AdjPrices_D1s.InsertAllOnSubmit(bars.Select(bar => new VA_AdjPrices_D1()
                            {
                                AsOfDate = bar.AsOfDate,
                                AdjClose = bar is StockBar ? (bar as StockBar).AdjClose : (double?)null,
                                Close = bar.Close,
                                High = bar.High,
                                Low = bar.Low,
                                Open = bar.Open,
                                SID = SID,
                                Volume = bar.Volume,
                                DividendAmount = bar is StockBar ? (bar as StockBar).DividendAmount : (double?)null,
                                SplitCoefficient = bar is StockBar ? (bar as StockBar).SplitCoefficient : (double?)null,
                                TS = DateTime.UtcNow
                            }).ToArray());
                            break;
                        case BarFrequency.Week1:
                            marketData.VA_AdjPrices_W1s.InsertAllOnSubmit(bars.Select(bar => new VA_AdjPrices_W1()
                            {
                                AsOfDate = bar.AsOfDate,
                                AdjClose = bar is StockBar ? (bar as StockBar).AdjClose : (double?)null,
                                Close = bar.Close,
                                High = bar.High,
                                Low = bar.Low,
                                Open = bar.Open,
                                SID = SID,
                                Volume = bar.Volume,
                                DividendAmount = bar is StockBar ? (bar as StockBar).DividendAmount : (double?)null,
                                TS = DateTime.UtcNow
                            }).ToArray());
                            break;
                        case BarFrequency.Month1:
                            marketData.VA_AdjPrices_MNs.InsertAllOnSubmit(bars.Select(bar => new VA_AdjPrices_MN()
                            {
                                AsOfDate = bar.AsOfDate,
                                AdjClose = bar is StockBar ? (bar as StockBar).AdjClose : (double?)null,
                                Close = bar.Close,
                                High = bar.High,
                                Low = bar.Low,
                                Open = bar.Open,
                                SID = SID,
                                Volume = bar.Volume,
                                DividendAmount = bar is StockBar ? (bar as StockBar).DividendAmount : (double?)null,
                                TS = DateTime.UtcNow
                            }).ToArray());
                            break;
                    }
                }
                else
                {
                    switch (frequency)
                    {
                        case BarFrequency.Minute1:
                            marketData.VA_Prices_M1s.InsertAllOnSubmit(bars.Select(bar => new VA_Prices_M1()
                            {
                                AsOfDate = bar.AsOfDate,
                                Close = bar.Close,
                                High = bar.High,
                                Low = bar.Low,
                                Open = bar.Open,
                                SID = SID,
                                Volume = bar.Volume,
                                MarketCap = bar is CryptoBar ? (bar as CryptoBar).MarketCap : (decimal?)null,
                                TS = DateTime.UtcNow
                            }).ToArray());
                            break;
                        case BarFrequency.Minute5:
                            marketData.VA_Prices_M5s.InsertAllOnSubmit(bars.Select(bar => new VA_Prices_M5()
                            {
                                AsOfDate = bar.AsOfDate,
                                Close = bar.Close,
                                High = bar.High,
                                Low = bar.Low,
                                Open = bar.Open,
                                SID = SID,
                                Volume = bar.Volume,
                                MarketCap = bar is CryptoBar ? (bar as CryptoBar).MarketCap : (decimal?)null,
                                TS = DateTime.UtcNow
                            }).ToArray());
                            break;
                        case BarFrequency.Minute15:
                            marketData.VA_Prices_M15s.InsertAllOnSubmit(bars.Select(bar => new VA_Prices_M15()
                            {
                                AsOfDate = bar.AsOfDate,
                                Close = bar.Close,
                                High = bar.High,
                                Low = bar.Low,
                                Open = bar.Open,
                                SID = SID,
                                Volume = bar.Volume,
                                MarketCap = bar is CryptoBar ? (bar as CryptoBar).MarketCap : (decimal?)null,
                                TS = DateTime.UtcNow
                            }).ToArray());
                            break;
                        case BarFrequency.Minute30:
                            marketData.VA_Prices_M30s.InsertAllOnSubmit(bars.Select(bar => new VA_Prices_M30()
                            {
                                AsOfDate = bar.AsOfDate,
                                Close = bar.Close,
                                High = bar.High,
                                Low = bar.Low,
                                Open = bar.Open,
                                SID = SID,
                                Volume = bar.Volume,
                                MarketCap = bar is CryptoBar ? (bar as CryptoBar).MarketCap : (decimal?)null,
                                TS = DateTime.UtcNow
                            }).ToArray());
                            break;
                        case BarFrequency.Hour1:
                            marketData.VA_Prices_H1s.InsertAllOnSubmit(bars.Select(bar => new VA_Prices_H1()
                            {
                                AsOfDate = bar.AsOfDate,
                                Close = bar.Close,
                                High = bar.High,
                                Low = bar.Low,
                                Open = bar.Open,
                                SID = SID,
                                Volume = bar.Volume,
                                MarketCap = bar is CryptoBar ? (bar as CryptoBar).MarketCap : (decimal?)null,
                                TS = DateTime.UtcNow
                            }).ToArray());
                            break;
                        case BarFrequency.Day1:
                            marketData.VA_Prices_D1s.InsertAllOnSubmit(bars.Select(bar => new VA_Prices_D1()
                            {
                                AsOfDate = bar.AsOfDate,
                                Close = bar.Close,
                                High = bar.High,
                                Low = bar.Low,
                                Open = bar.Open,
                                SID = SID,
                                Volume = bar.Volume,
                                MarketCap = bar is CryptoBar ? (bar as CryptoBar).MarketCap : (decimal?)null,
                                TS = DateTime.UtcNow
                            }).ToArray());
                            break;
                        case BarFrequency.Week1:
                            marketData.VA_Prices_W1s.InsertAllOnSubmit(bars.Select(bar => new VA_Prices_W1()
                            {
                                AsOfDate = bar.AsOfDate,
                                Close = bar.Close,
                                High = bar.High,
                                Low = bar.Low,
                                Open = bar.Open,
                                SID = SID,
                                Volume = bar.Volume,
                                MarketCap = bar is CryptoBar ? (bar as CryptoBar).MarketCap : (decimal?)null,
                                TS = DateTime.UtcNow
                            }).ToArray());
                            break;
                        case BarFrequency.Month1:
                            marketData.VA_Prices_MNs.InsertAllOnSubmit(bars.Select(bar => new VA_Prices_MN()
                            {
                                AsOfDate = bar.AsOfDate,
                                Close = bar.Close,
                                High = bar.High,
                                Low = bar.Low,
                                Open = bar.Open,
                                SID = SID,
                                Volume = bar.Volume,
                                MarketCap = bar is CryptoBar ? (bar as CryptoBar).MarketCap : (decimal?)null,
                                TS = DateTime.UtcNow
                            }).ToArray());
                            break;
                    }
                }

                marketData.SubmitChanges();
            }

            return true;
        }

        public string GetTableName(BarFrequency frequency, bool isAdjustedValue)
        {
            string tableName = null;
            if (isAdjustedValue)
            {
                switch (frequency)
                {
                    case BarFrequency.Day1:
                        tableName = "av.AdjPrices_D1";
                        break;
                    case BarFrequency.Week1:
                        tableName = "av.AdjPrices_W1";
                        break;
                    case BarFrequency.Month1:
                        tableName = "av.AdjPrices_MN";
                        break;
                }
            }
            else
            {
                switch (frequency)
                {
                    case BarFrequency.Minute1:
                        tableName = "av.Prices_M1";
                        break;
                    case BarFrequency.Minute5:
                        tableName = "av.Prices_M5";
                        break;
                    case BarFrequency.Minute15:
                        tableName = "av.Prices_M15";
                        break;
                    case BarFrequency.Minute30:
                        tableName = "av.Prices_M30";
                        break;
                    case BarFrequency.Hour1:
                        tableName = "av.Prices_H1";
                        break;
                    case BarFrequency.Day1:
                        tableName = "av.Prices_D1";
                        break;
                    case BarFrequency.Week1:
                        tableName = "av.Prices_W1";
                        break;
                    case BarFrequency.Month1:
                        tableName = "av.Prices_MN";
                        break;
                }
            }

            return tableName;
        }

        public IDictionary<string, StockBar[]> VA_GetStockPrices(string[] symbols, BarFrequency frequency, DateTime? startDate, DateTime? endDate, int outputCount, bool isAdjustedValue)
        {
            IDictionary<string, long> tickerToSIDMap = GetSIDFromName(symbols);
            long[] SIDs = tickerToSIDMap.Values.ToArray();

            DateTime begin = startDate.HasValue ? startDate.Value : new DateTime(1900,1,1);
            DateTime end = endDate.HasValue ? endDate.Value : DateTime.Now.AddDays(4);

            Dictionary<string, StockBar[]> stockBarResult = null;
            if (SIDs != null && SIDs.Count() > 0)
            {
                IDictionary<long, string> SIDToTickerMap = tickerToSIDMap.ToDictionary(k => k.Value, v => v.Key);

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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
                                                      Volume = p.Volume
                                                  }).ToArray());
                            }
                            else
                            {
                                stockBarResult = (from p in marketData.VA_Prices_M1s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
                                                      Volume = p.Volume
                                                  }).ToArray());
                            }
                            else
                            {
                                stockBarResult = (from p in marketData.VA_Prices_M5s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
                                                      Volume = p.Volume
                                                  }).ToArray());
                            }
                            else
                            {
                                stockBarResult = (from p in marketData.VA_Prices_M15s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
                                                      Volume = p.Volume
                                                  }).ToArray());
                            }
                            else
                            {
                                stockBarResult = (from p in marketData.VA_Prices_M30s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
                                                      Volume = p.Volume
                                                  }).ToArray());
                            }
                            else
                            {
                                stockBarResult = (from p in marketData.VA_Prices_H1s
                                                  where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                  orderby p.AsOfDate descending
                                                  group p by p.SID into g
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new StockBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
                                                          Volume = p.Volume
                                                      }).ToArray());
                                }
                                else
                                {
                                    stockBarResult = (from p in marketData.VA_Prices_D1s
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
                                                          Volume = p.Volume
                                                      }).ToArray());
                                }
                                else
                                {
                                    stockBarResult = (from p in marketData.VA_Prices_W1s
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
                                                          Volume = p.Volume
                                                      }).ToArray());
                                }
                                else
                                {
                                    stockBarResult = (from p in marketData.VA_Prices_MNs
                                                      where SIDs.Contains(p.SID) && p.AsOfDate >= begin && p.AsOfDate <= end
                                                      orderby p.AsOfDate descending
                                                      group p by p.SID into g
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new StockBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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

        public IDictionary<string, CryptoBar[]> VA_GetCryptoPrices(string[] symbols, string market, BarFrequency frequency, DateTime? startDate, DateTime? endDate, int outputCount)
        {
            string[] tickers = symbols.Select(p => p.ToUpper() + market).ToArray();
            IDictionary<string, long> tickerToSIDMap = GetSIDFromName(tickers);
            long[] SIDs = tickerToSIDMap.Values.ToArray();

            DateTime begin = startDate.HasValue ? startDate.Value : new DateTime(1900, 1, 1);
            DateTime end = endDate.HasValue ? endDate.Value : DateTime.Now.AddDays(4);

            Dictionary<string, CryptoBar[]> cryptoBarResult = null;
            if (SIDs != null && SIDs.Count() > 0)
            {
                IDictionary<long, string> SIDToTickerMap = tickerToSIDMap.ToDictionary(k => k.Value, v => v.Key);

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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(m => new CryptoBar
                                                  {
                                                      AsOfDate = m.AsOfDate,
                                                      Close = m.Close,
                                                      High = m.High,
                                                      Low = m.Low,
                                                      Open = m.Open,
                                                      TS = m.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(m =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = m.AsOfDate,
                                                      Close = m.Close,
                                                      High = m.High,
                                                      Low = m.Low,
                                                      Open = m.Open,
                                                      TS = m.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(m =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = m.AsOfDate,
                                                      Close = m.Close,
                                                      High = m.High,
                                                      Low = m.Low,
                                                      Open = m.Open,
                                                      TS = m.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                  select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                  new CryptoBar
                                                  {
                                                      AsOfDate = p.AsOfDate,
                                                      Close = p.Close,
                                                      High = p.High,
                                                      Low = p.Low,
                                                      Open = p.Open,
                                                      TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new CryptoBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new CryptoBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new CryptoBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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
                                                      select g).ToDictionary(k => SIDToTickerMap[k.Key], v => v.Select(p =>
                                                      new CryptoBar
                                                      {
                                                          AsOfDate = p.AsOfDate,
                                                          Close = p.Close,
                                                          High = p.High,
                                                          Low = p.Low,
                                                          Open = p.Open,
                                                          TS = p.TS.Value,
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
