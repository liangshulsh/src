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
