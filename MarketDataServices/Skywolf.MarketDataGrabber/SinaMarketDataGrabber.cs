using System;
using System.Collections.Concurrent;
using Skywolf.Contracts.DataContracts.MarketData;
using Skywolf.Contracts.DataContracts.MarketData.TVC;
using HtmlAgilityPack;
using Skywolf.Utility;
using System.Data;
using log4net;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace Skywolf.MarketDataGrabber
{
    public class SinaMarketDataGrabber : BaseMarketDataGrabber
    {
        const string SINA_URL = @"https://hq.sinajs.cn/?list=";

        //var hq_str_gb_aapl = 
        //                   "苹果," +   name
        //                   "127.9000," + close price
        //                     "1.34," + price change percent
        //                     "2021-04-08 16:23:43," + beijing update time
        //                     "1.6900," + price change value
        //                     "125.8300," + open price
        //                     "127.9200," + high price
        //                    "125.1400," + low price
        //                     "515.1400," + 52w high price
        //                     "103.1000," + 52w low price
        //                     "83466716," + volume
        //                     "89363871," + 10 day average volume
        //                     "2147197478400," + market value
        //                     "5.70," +     earning per stock
        //                     "22.440000," + PE
        //                     "0.00," +
        //                    "0.00," +
        //                    "0.20," +     dividend
        //                     "0.00," +
        //                     "16788096000," + total shares
        //                     "63," +
        //                     "128.7900," + pre-market price
        //                     "0.70," + pre-market price change percent
        //                     "0.89," + pre-market price change value
        //                     "Apr 08 04:23AM EDT," + US East Time(After Market Close)
        //                     "Apr 07 04:00PM EDT," + US East Time(Market Close)
        //                     "126.2100," + last day close price
        //                     "34432," + pre-mareket volume
        //                     "1," + 
        //                     "2021," +
        //                     "10608796580.4435," +
        //                     "128.9000," +
        //                     "128.5000," +
        //                     "4432757.1700";
        public override TimeSeriesDataOutput GetTimeSeriesData(TimeSeriesDataInput input)
        {
            throw new NotImplementedException();
        }

        public override Quote[] StockBatchQuote(IEnumerable<string> symbols)
        {
            string symbollist = string.Join(",", symbols.Select(p => "gb_" + p.ToLower()).ToArray());
            string result = HttpGet(SINA_URL + symbollist);
            if (!string.IsNullOrEmpty(result) && result.Length > 100)
            {
                List<Quote> quotelist = new List<Quote>();
                string[] lines = result.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (lines != null && lines.Count() > 0)
                {
                    foreach (string line in lines)
                    {
                        string[] items = line.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                        if (items != null && items.Count() >= 2)
                        {
                            try
                            {
                                Quote quote = new Quote();
                                string[] symbolfields = items[0].Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                                string symbol = symbolfields[symbolfields.Length - 1].ToUpper();
                                string[] values = items[1].Split(new char[] { ',' });
                                double price = Convert.ToDouble(values[1].Trim());
                                quote.Symbol = symbol;
                                quote.Price = price;
                                quotelist.Add(quote);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }
                }

                return quotelist.ToArray();
            }

            return null;
        }
    }
}
