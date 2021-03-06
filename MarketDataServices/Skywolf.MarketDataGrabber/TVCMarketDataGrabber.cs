﻿using System;
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
    public delegate Dictionary<string, TVCSymbolResponse> GetTVCSymbols(IEnumerable<string> symbols);
    public delegate void UpdateTVCSymbols(IEnumerable<TVCSymbolResponse> tvcSymbols);
    public delegate void UpdateTVCQuotes(IEnumerable<TVCQuoteResponse> tvcQuotes);

    public class TVCMarketDataGrabber : BaseMarketDataGrabber
    {
        const string TVC_URL = @"http://tvc4.forexpros.com/";
        const string TVC_URL_BASE = @"http://tvc4.forexpros.com/{0}/{1}/1/1/8/";
        const string TVC_URL_HISTORY = @"history?symbol={0}&resolution={1}&from={2}&to={3}";
        const string TVC_URL_QUOTES = @"quotes?symbols={0}";
        const string TVC_URL_SYMBOLS = @"symbols?symbol={0}";
        const string TVC_URL_SEARCH = @"search?limit={0}&query={1}&type={2}&exchange={3}";
        const string TVC_URL_CALENDAR = @"https://www.investing.com/holiday-calendar/Service/getCalendarFilteredData";

        public static ConcurrentDictionary<string, TVCSymbolResponse> _SymbolToSymbolInfo = new ConcurrentDictionary<string, TVCSymbolResponse>();
        public GetTVCSymbols _getTVCSymbolsHandler;
        public UpdateTVCSymbols _updateTVCSymbolesHandler;
        public UpdateTVCQuotes _updateTVCQuotesHandler;

        public override TimeSeriesDataOutput GetTimeSeriesData(TimeSeriesDataInput input)
        {
            if (input == null || string.IsNullOrEmpty(input.Symbol))
            {
                return null;
            }

            DateTime fromDate = new DateTime(1970, 1, 1);
            DateTime toDate = DateTime.Now;
            if (input.OutputCount >= 0)
            {
                switch (input.Frequency)
                {
                    case BarFrequency.Day1:
                        fromDate = toDate.AddDays(-input.OutputCount * 5);
                        break;
                    case BarFrequency.Week1:
                        fromDate = toDate.AddDays(-input.OutputCount * 8);
                        break;
                    case BarFrequency.Month1:
                        fromDate = toDate.AddDays(-input.OutputCount * 31);
                        break;
                    case BarFrequency.Hour1:
                        fromDate = toDate.AddDays(-(input.OutputCount / 6.5) * 5);
                        break;
                    case BarFrequency.Hour4:
                        fromDate = toDate.AddDays(-(input.OutputCount / (6.5 / 4) * 5));
                        break;
                    case BarFrequency.Minute1:
                        fromDate = toDate.AddDays(-(input.OutputCount / 390 >= 1.0 ? input.OutputCount / 390 : 1) * 5);
                        break;
                    case BarFrequency.Minute15:
                        fromDate = toDate.AddDays(-(input.OutputCount / (390 / 15) > 1.0 ? input.OutputCount / (390 / 15) : 1) * 5);
                        break;
                    case BarFrequency.Minute30:
                        fromDate = toDate.AddDays(-(input.OutputCount / (390 / 30) > 1.0 ? input.OutputCount / (390 / 30) : 1) * 5);
                        break;
                    case BarFrequency.Minute5:
                        fromDate = toDate.AddDays(-(input.OutputCount / (390 / 5) > 1.0 ? input.OutputCount / (390 / 5) : 1) * 5);
                        break;
                }
            }

            TVCHistoryResponse tvchistory = GetHistoricalPrices(input.Symbol, input.Frequency, fromDate, toDate);

            if (tvchistory != null && tvchistory.s == "ok")
            {
                TimeSeriesDataOutput output = new TimeSeriesDataOutput();
                output.Symbol = input.Symbol;
                output.TimeZone = "UTC";
                List<StockBar> stockBars = new List<StockBar>();
                if (tvchistory.t != null && tvchistory.t.Length > 0)
                {
                    for (int i = 0; i < tvchistory.t.Length; i++)
                    {
                        StockBar bar = new StockBar();
                        bar.AsOfDate = new DateTime(1970, 1, 1).AddSeconds(tvchistory.t[i]);
                        bar.Close = tvchistory.c[i];
                        bar.High = tvchistory.h[i];
                        bar.Low = tvchistory.l[i];
                        bar.Open = tvchistory.o[i];
                        bar.SplitCoefficient = 1.0;
                        bar.Volume = Convert.ToDecimal(tvchistory.v[i]);
                        bar.TS = DateTime.UtcNow;
                        stockBars.Add(bar);
                    }
                }

                if (stockBars.Count > 0)
                {
                    if (input.OutputCount >= 0 && stockBars.Count > input.OutputCount)
                    {
                        output.Data = stockBars.Skip(stockBars.Count - (int)input.OutputCount).ToArray();
                    }
                    else
                    {
                        output.Data = stockBars.ToArray();
                    }
                }

                return output;
            }

            return null;
        }

        public override Quote[] StockBatchQuote(IEnumerable<string> symbols)
        {
            TVCQuotesResponse response = GetQuotes(symbols);
            if (response != null && response.d != null && response.d.Count > 0)
            {
                List<Quote> quotes = new List<Quote>();
                foreach (var tvc in response.d)
                {
                    if (tvc.s == "ok")
                    {
                        Quote quote = ConvertTVCQuotesToQuotes(tvc);
                        if (quote != null)
                            quotes.Add(quote);
                    }
                }
                return quotes.ToArray();
            }

            return null;
        }

        private Quote ConvertTVCQuotesToQuotes(TVCQuoteResponse tvc)
        {
            if (tvc != null && tvc.v != null)
            {
                Quote quote = new Quote();
                if (!string.IsNullOrEmpty(tvc.n))
                {
                    if (tvc.n.Contains(":"))
                    {
                        quote.Symbol = tvc.n.Split(new char[] { ':' })[1];
                    }
                    else
                    {
                        quote.Symbol = tvc.n;
                    }
                }
                
                quote.TimeZone = "UTC";
                quote.TimeStamp = DateTime.UtcNow;
                
                try
                {
                    quote.Ask = Convert.ToDouble(tvc.v.ask);
                }
                catch (Exception)
                {

                }

                try
                {
                    quote.Bid = Convert.ToDouble(tvc.v.bid);
                }
                catch (Exception)
                {

                }

                try
                {
                    quote.Price = Convert.ToDouble(tvc.v.lp);
                }
                catch (Exception)
                {

                }

                try
                {
                    quote.Volume = Convert.ToDouble(tvc.v.volume);
                }
                catch (Exception)
                {

                }

                return quote;
            }

            return null;
        }

        public string ConvertBarFrequencyToTVCResolution(BarFrequency frequency)
        {
            string freq = "D";

            switch (frequency)
            {
                case BarFrequency.Day1:
                    freq = "D";
                    break;
                case BarFrequency.Hour1:
                    freq = "60";
                    break;
                case BarFrequency.Hour4:
                    freq = "240";
                    break;
                case BarFrequency.Minute1:
                    freq = "1";
                    break;
                case BarFrequency.Minute15:
                    freq = "15";
                    break;
                case BarFrequency.Minute30:
                    freq = "30";
                    break;
                case BarFrequency.Minute5:
                    freq = "5";
                    break;
                case BarFrequency.Month1:
                    freq = "M";
                    break;
                case BarFrequency.Week1:
                    freq = "W";
                    break;
            }

            return freq;
        }

        public TVCCalendar[] GetCalendars(DateTime fromDate, DateTime toDate, string country="", string currentTab="custom")
        {
            List<TVCCalendarResponse> responses = new List<TVCCalendarResponse>();

            TVCCalendarResponse response = RequestCalendar(fromDate, toDate, country, currentTab);

            if (response != null)
            {
                responses.Add(response);

                int limit_from = 1;
                while (response != null && response.bind_scroll_handler)
                {
                    response = RequestCalendar(fromDate, toDate, country, currentTab, limit_from, true, 0, response.last_time_scope, true);
                    if (response != null)
                    {
                        responses.Add(response);
                        limit_from++;
                    }
                }
            }

            if (responses.Count > 0)
            {
                List<TVCCalendar> calendarList = new List<TVCCalendar>();
                foreach (TVCCalendarResponse calendarResponse in responses)
                {
                    TVCCalendar[] calendars = ConvertTVCCalendarResponseToTVCCalendar(calendarResponse);
                    if (calendars != null)
                    {
                        calendarList.AddRange(calendars);
                    }
                }

                return calendarList.ToArray();
            }

            return null;
        }

        public static TVCCalendar[] ConvertTVCCalendarResponseToTVCCalendar(TVCCalendarResponse response)
        {
            if (response != null && !string.IsNullOrEmpty(response.data))
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(response.data);
                HtmlNodeCollection trs = doc.DocumentNode.SelectNodes("tr");
                List<TVCCalendar> calendars = new List<TVCCalendar>();
                string lastdate = string.Empty;
                foreach (HtmlNode tr in trs)
                {
                    HtmlNodeCollection tds = tr.SelectNodes("td");
                    string date;
                    string country;
                    string exchange;
                    string holiday;
                    TVCCalendar calendar = new TVCCalendar();
                    HtmlNodeCollection aNodes = tds[0].SelectNodes("a");
                    if (aNodes != null && aNodes.Count > 0)
                    {
                        date = aNodes[0].InnerText;
                    }
                    else
                    {
                        date = tds[0].InnerText;
                    }

                    aNodes = tds[1].SelectNodes("a");
                    if (aNodes != null && aNodes.Count > 0)
                    {
                        country = aNodes[0].InnerText;
                    }
                    else
                    {
                        country = tds[1].InnerText;
                    }

                    if (country != null && country.Contains(";"))
                    {
                        country = country.Split(new char[] { ';' })[1];
                    }

                    aNodes = tds[2].SelectNodes("a");
                    if (aNodes != null && aNodes.Count > 0)
                    {
                        exchange = aNodes[0].InnerText;
                    }
                    else
                    {
                        exchange = tds[2].InnerText;
                    }

                    aNodes = tds[3].SelectNodes("a");
                    if (aNodes != null && aNodes.Count > 0)
                    {
                        holiday = aNodes[0].InnerText;
                    }
                    else
                    {
                        holiday = tds[3].InnerText;
                    }

                    if (string.IsNullOrEmpty(date))
                    {
                        date = lastdate;
                    }
                    else
                    {
                        lastdate = date;
                    }

                    calendar.AsOfDate = Convert.ToDateTime(date);
                    calendar.Country = country;
                    calendar.Exchange = exchange;
                    calendar.Holiday = holiday;

                    if (!string.IsNullOrEmpty(holiday) && holiday.Contains("Early close at"))
                    {
                        string[] closetimes = holiday.Split(new string[] { "Early close at" }, StringSplitOptions.RemoveEmptyEntries);
                        if (closetimes != null && closetimes.Count() > 1)
                        {
                            calendar.EarlyClose = closetimes[1].Trim();
                        }
                    }
                    calendars.Add(calendar);
                }

                return calendars.ToArray();
            }

            return null;
        }

        public TVCCalendarResponse RequestCalendar(DateTime fromDate, DateTime toDate, string country="", string currentTab="custom", int limit_from=0, bool showMore = false, int submitFilters = 0, int last_time_scope = 0, bool byHandler = false)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Accept", "*/*");
            //headers.Add("Accept-Encoding", "gzip, deflate, br");
            headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            headers.Add("Host", "www.investing.com");
            headers.Add("Origin", "https://www.investing.com");
            headers.Add("Referer", "https://www.investing.com/holiday-calendar/");
            headers.Add("User-Agent", "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 68.0.3440.106 Safari / 537.36");
            headers.Add("X-Requested-With", "XMLHttpRequest");

            string parameters = string.Empty;

            if (byHandler)
            {
                parameters = string.Format("dateFrom={0}&dateTo={1}&country={2}&currentTab={3}&limit_from={4}&showMore={5}&submitFilters={6}&last_time_scope={7}&byHandler={8}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"), country, currentTab, limit_from, showMore, submitFilters, last_time_scope, byHandler);
            }
            else
            {
                parameters = string.Format("dateFrom={0}&dateTo={1}&country={2}&currentTab={3}&limit_from={4}", fromDate.ToString("yyyy-MM-dd"), toDate.ToString("yyyy-MM-dd"), country, currentTab, limit_from);
            }

            string result = HttpPost(TVC_URL_CALENDAR, parameters, "application/x-www-form-urlencoded", headers);
            TVCCalendarResponse calendar = JsonConvert.DeserializeObject<TVCCalendarResponse>(result);
            return calendar;
        }


        public TVCHistoryResponse GetHistoricalPrices(string symbol, BarFrequency frequency, DateTime from, DateTime to)
        {
            string freq = ConvertBarFrequencyToTVCResolution(frequency);
            string result = TVCHttpGet(string.Format(TVC_URL_HISTORY, symbol, freq, from.ToUnixDateTime(), to.ToUnixDateTime()));
            TVCHistoryResponse history = JsonConvert.DeserializeObject<TVCHistoryResponse>(result);
            return history;
        }

        public TVCQuotesResponse GetQuotes(IEnumerable<string> symbols)
        {
            if (symbols != null && symbols.Count() > 0)
            {
                List<List<string>> symbolBatchList = new List<List<string>>();

                List<string> symbolList = symbols.ToList();

                while (symbolList.Count > 0)
                {
                    symbolBatchList.Add(symbolList.Take(50).ToList());
                    symbolList = symbolList.Skip(50).ToList();
                }

                TVCQuotesResponse totalQuotes = null;

                foreach (List<string> current in symbolBatchList)
                {
                    string[] covertedSymbols = ConvertSymbolFormat(current);
                    string joinedsymbols = string.Join(",", covertedSymbols);
                    string result = TVCHttpGet(string.Format(TVC_URL_QUOTES, joinedsymbols));
                    TVCQuotesResponse quotes = JsonConvert.DeserializeObject<TVCQuotesResponse>(result);
                    if (quotes != null && quotes.d != null)
                    {
                        if (totalQuotes == null)
                        {
                            totalQuotes = quotes;
                        }
                        else
                        {
                            totalQuotes.d.AddRange(quotes.d);
                        }
                    }
                }

                _updateTVCQuotesHandler?.Invoke(totalQuotes.d);

                return totalQuotes;
            }

            return null;
        }

        public string[] ConvertSymbolFormat(IEnumerable<string> symbols)
        {
            List<string> results = new List<string>();
            List<string> symbolRemain = new List<string>();
            if (symbols != null && symbols.Count() > 0)
            {
                foreach (string symbol in symbols)
                {
                    if (symbol.Contains(":"))
                    {
                        results.Add(symbol);
                    }
                    else
                    {
                        symbolRemain.Add(symbol);
                    }
                }

                if (symbolRemain.Count > 0)
                {
                    List<string> symbolRemainTemp = new List<string>();
                    foreach (string symbol in symbolRemain)
                    {
                        TVCSymbolResponse response;
                        if (_SymbolToSymbolInfo.TryGetValue(symbol, out response))
                        {
                            if (!string.IsNullOrEmpty(response.exchange_traded))
                            {
                                results.Add(response.exchange_traded + "%3A" + symbol);
                            }
                            else
                            {
                                results.Add(symbol);
                            }
                        }
                        else
                        {
                            symbolRemainTemp.Add(symbol);
                        }
                    }

                    symbolRemain = symbolRemainTemp;
                }

                if (symbolRemain.Count > 0)
                {
                    List<string> symbolRemainTemp = new List<string>();
                    if (_getTVCSymbolsHandler != null)
                    {
                        Dictionary<string, TVCSymbolResponse> symbolToTVCSymbol =  _getTVCSymbolsHandler(symbolRemain);
                        if (symbolToTVCSymbol != null)
                        {
                            foreach (string symbol in symbolRemain)
                            {
                                TVCSymbolResponse response;
                                if (symbolToTVCSymbol.TryGetValue(symbol, out response))
                                {
                                    _SymbolToSymbolInfo[symbol] = response;
                                    if (!string.IsNullOrEmpty(response.exchange_traded))
                                    {
                                        results.Add(response.exchange_traded + "%3A" + symbol);
                                    }
                                    else
                                    {
                                        results.Add(symbol);
                                    }
                                }
                                else
                                {
                                    symbolRemainTemp.Add(symbol);
                                }
                            }

                            symbolRemain = symbolRemainTemp;
                        }
                    }
                }

                if (symbolRemain.Count > 0)
                {
                    List<TVCSymbolResponse> newResponses = new List<TVCSymbolResponse>();
                    foreach (string symbol in symbolRemain)
                    {
                        //TVCSymbolResponse response = GetSymbolInfo(symbol);
                        TVCSymbolResponse response = null;
                        if (response != null && !string.IsNullOrEmpty(response.name))
                        {
                            newResponses.Add(response);
                            _SymbolToSymbolInfo[symbol] = response;
                            if (!string.IsNullOrEmpty(response.exchange_traded))
                            {
                                results.Add(response.exchange_traded + "%3A" + symbol);
                            }
                            else
                            {
                                results.Add(symbol);
                            }
                        }
                    }

                    _updateTVCSymbolesHandler?.Invoke(newResponses);
                }
            }

            return results.ToArray();
        }

        public TVCSearchResponse[] GetSymbolSearch(string query, string type = "", string exchange = "", int limit = 30)
        {
            string result = TVCHttpGet(string.Format(TVC_URL_SEARCH, limit.ToString(), query, type, exchange));
            try
            {
                TVCSearchResponse[] searchResponse = JsonConvert.DeserializeObject<TVCSearchResponse[]>(result);
                return searchResponse;
            }
            catch (Exception)
            {

            }

            return null;
        }

        public TVCSymbolResponse GetSymbolInfo(string symbol)
        {
            string result = TVCHttpGet(string.Format(TVC_URL_SYMBOLS, symbol));

            try
            {
                TVCSymbolResponse symbolResponse = JsonConvert.DeserializeObject<TVCSymbolResponse>(result);
                return symbolResponse;
            }
            catch (Exception)
            {
            }

            return null;
        }

        protected static object _TVCHttpGetObj = new object();
        protected static DateTime _lastCallingTime = DateTime.MinValue;
        protected static Random _rand = new Random(DateTime.Now.Day);
        protected static string _TVCCarrier = null;
        protected static string _TVCTime = null;

        private bool UpdateTVCCarrierAndTime()
        {
            string tvcchart = HttpGet(TVC_URL);

            if (!string.IsNullOrEmpty(tvcchart))
            {
                string keyword_carrier = "carrier=";
                string keyword_time = "time=";

                int carrierIdx = tvcchart.IndexOf(keyword_carrier);
                int andIdx = tvcchart.IndexOf("&", carrierIdx);
                _TVCCarrier = tvcchart.Substring(carrierIdx + keyword_carrier.Length, andIdx - (carrierIdx + keyword_carrier.Length));

                int timeIdx = tvcchart.IndexOf(keyword_time, carrierIdx);
                andIdx = tvcchart.IndexOf("&", timeIdx);
                _TVCTime = tvcchart.Substring(timeIdx + keyword_time.Length, andIdx - (timeIdx + keyword_time.Length));

                _lastCallingTime = DateTime.Now;

                if (!string.IsNullOrEmpty(_TVCCarrier) && !string.IsNullOrEmpty(_TVCTime))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetUrlBase()
        {
            return string.Format(TVC_URL_BASE, _TVCCarrier, _TVCTime);
        }

        public string TVCHttpGet(string url)
        {
            lock (_TVCHttpGetObj)
            {
                if ((DateTime.Now - _lastCallingTime).TotalMinutes > 10)
                {
                    UpdateTVCCarrierAndTime();
                }

                string result = string.Empty;
                int i = 0;
                while (string.IsNullOrEmpty(result) || (result.StartsWith("[") && result.Length < 50 && result != "[]"))
                {
                    if (i > 0)
                    {
                        UpdateTVCCarrierAndTime();
                    }

                    result = HttpGet(GetUrlBase() + url);
                    i++;

                    if (i >= 3)
                    {
                        break;
                    }
                }
                
                return result;
            }
        }
    }
}
