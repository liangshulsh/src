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
using Skywolf.Contracts.DataContracts.MarketData;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
            try
            {
                StringBuilder nameToSIDBuilder = new StringBuilder();
                nameToSIDBuilder.AppendLine("Name,SID");
                if (!string.IsNullOrEmpty(names))
                {
                    string[] nameList = names.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    IDictionary<string, long> NameToSIDMap = new MarketDataService().GetSIDFromName(nameList);
                    if (NameToSIDMap != null && NameToSIDMap.Count > 0)
                    {
                        foreach (var pair in NameToSIDMap)
                        {
                            nameToSIDBuilder.AppendLine(string.Format("{0},{1}", pair.Key, pair.Value));
                        }
                    }
                }
                return nameToSIDBuilder.ToString();
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string GetNameFromSID(string sids)
        {
            try
            {
                StringBuilder SIDToNameBuilder = new StringBuilder();
                SIDToNameBuilder.AppendLine("SID,Name");
                if (!string.IsNullOrEmpty(sids))
                {
                    long[] sidList = sids.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(p => Convert.ToInt64(p)).ToArray();
                    IDictionary<long, string> SIDToNameMap = new MarketDataService().GetNameFromSID(sidList);
                    if (SIDToNameMap != null && SIDToNameMap.Count > 0)
                    {
                        foreach (var pair in SIDToNameMap)
                        {
                            SIDToNameBuilder.AppendLine(string.Format("{0},{1}", pair.Key, pair.Value));
                        }
                    }
                }
                return SIDToNameBuilder.ToString();
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string GetTimeSeriesData(string symbol, string market, string frequency, string outputcount, string isadjustedvalue, string datasource)
        {
            try
            {
                TimeSeriesDataInput input = new TimeSeriesDataInput();
                if (!string.IsNullOrEmpty(market))
                {
                    CryptoTimeSeriesDataInput inputCrypto = new CryptoTimeSeriesDataInput();
                    inputCrypto.Market = market;
                    input = inputCrypto;
                }

                input.Symbol = symbol;
                input.Frequency = RestfulHelper.ConvertStringToBarFrequency(frequency);
                input.IsAdjustedValue = Convert.ToBoolean(isadjustedvalue);
                input.OutputCount = Convert.ToInt64(outputcount);
                TimeSeriesDataOutput output = new MarketDataService().GetTimeSeriesData(input, datasource);
                return RestfulHelper.ConvertBarToCSV(output.Data);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        

        public string GetStockBatchQuote(string symbols, string datasource)
        {
            try
            {
                string[] symbolList = symbols.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                Quote[] quotes = new MarketDataService().GetStockBatchQuote(symbolList.ToArray(), datasource);
                return quotes.ToCSV();
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string GetStockHistoryPrices(string symbols, string frequency, string startdate, string enddate, string outputcount, string isadjustedvalue, string datasource)
        {
            try
            {
                string[] symbolList = symbols.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                BarFrequency freq = RestfulHelper.ConvertStringToBarFrequency(frequency);
                DateTime? start = null;

                if (!string.IsNullOrWhiteSpace(startdate))
                {
                    try
                    {
                        start = Convert.ToDateTime(startdate);
                    }
                    catch (Exception)
                    {
                        return "Error:startDate";
                    }
                }
                DateTime? end = null;
                if (!string.IsNullOrWhiteSpace(enddate))
                {
                    try
                    {
                        end = Convert.ToDateTime(enddate);
                    }
                    catch (Exception)
                    {
                        return "Error:endDate";
                    }
                }
                int count = 0;

                if (!string.IsNullOrWhiteSpace(outputcount))
                {
                    if(!int.TryParse(outputcount, out count))
                    {
                        return "Error:outputCount";
                    }
                }
                bool isAdjusted = false;
                if (!string.IsNullOrWhiteSpace(isadjustedvalue))
                {
                    if (!bool.TryParse(isadjustedvalue, out isAdjusted))
                    {
                        return "Error:isAdjustedValue";
                    }
                }

                IDictionary<string, StockBar[]> result = new MarketDataService().GetStockHistoryPrices(symbolList, freq, start, end, count, isAdjusted, datasource);

                StringBuilder resultBuilder = new StringBuilder();

                if (result != null && result.Count > 0)
                {
                    foreach (string s in symbolList)
                    {
                        resultBuilder.AppendLine("<symbol>");
                        resultBuilder.AppendLine(s);
                        resultBuilder.AppendLine("</symbol>");
                        StockBar[] stockBars = null;
                        resultBuilder.AppendLine("<data>");

                        if (result.TryGetValue(s, out stockBars) && stockBars != null)
                        {
                            resultBuilder.Append(stockBars.ToCSV());
                        }
                        resultBuilder.AppendLine("</data>");
                    }
                }

                return resultBuilder.ToString();
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string GetCryptoHistoryPrices(string symbols, string market, string frequency, string startdate, string enddate, string outputcount, string datasource)
        {
            try
            {
                string[] symbolList = symbols.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                BarFrequency freq = RestfulHelper.ConvertStringToBarFrequency(frequency);
                DateTime? start = null;

                if (!string.IsNullOrWhiteSpace(startdate))
                {
                    try
                    {
                        start = Convert.ToDateTime(startdate);
                    }
                    catch (Exception)
                    {
                        return "Error:startDate";
                    }
                }
                DateTime? end = null;
                if (!string.IsNullOrWhiteSpace(enddate))
                {
                    try
                    {
                        end = Convert.ToDateTime(enddate);
                    }
                    catch (Exception)
                    {
                        return "Error:endDate";
                    }
                }
                int count = 0;

                if (!string.IsNullOrWhiteSpace(outputcount))
                {
                    if (!int.TryParse(outputcount, out count))
                    {
                        return "Error:outputCount";
                    }
                }

                IDictionary<string, CryptoBar[]> result = new MarketDataService().GetCryptoHistoryPrices(symbolList, market, freq, start, end, count, datasource);

                StringBuilder resultBuilder = new StringBuilder();

                if (result != null && result.Count > 0)
                {
                    foreach (string s in symbolList)
                    {
                        resultBuilder.AppendLine("<symbol>");
                        resultBuilder.AppendLine(s);
                        resultBuilder.AppendLine("</symbol>");
                        CryptoBar[] cryptoBars = null;
                        resultBuilder.AppendLine("<data>");

                        if (result.TryGetValue(s, out cryptoBars) && cryptoBars != null)
                        {
                            resultBuilder.Append(cryptoBars.ToCSV());
                        }
                        resultBuilder.AppendLine("</data>");
                    }
                }

                return resultBuilder.ToString();
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string VA_GetAvailableAPIKey()
        {
            try
            {
                string[] keys = new MarketDataService().VA_GetAvailableAPIKey();

                StringBuilder keyBuilder = new StringBuilder();
                keyBuilder.AppendLine("APIKey");
                foreach (string key in keys)
                {
                    keyBuilder.AppendLine(key);
                }

                return keyBuilder.ToString();
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string AV_AddAPIKeys(string apiKeys)
        {
            try
            {
                string[] keys = apiKeys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                new MarketDataService().AV_AddAPIKeys(keys);
                return "OK";
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string AV_RemoveAPIKeys(string apiKeys)
        {
            try
            {
                string[] keys = apiKeys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                new MarketDataService().AV_RemoveAPIKeys(keys);
                return "OK";
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string AV_UpdateAPIKeys(string apiKeys)
        {
            try
            {
                string[] keys = apiKeys.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                new MarketDataService().AV_UpdateAPIKeys(keys);
                return "OK";
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }
    }
}
