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
    public class CalendarService
    {
        public const string CLOSE_TIME = "Close Time";
        
        private static ILog _Logger;
        static CalendarService()
        {
            _Logger = LogManager.GetLogger(typeof(MarketDataService));
            _marketSessions["United States-STK"] = new MarketSession() { Country = "United States", Type = "STK", OpenTime = new DateTime(2000, 1, 1, 9, 30, 0), CloseTime = new DateTime(2000, 1, 1, 16, 0, 0) };
            _marketSessions["China-STK"] = new MarketSession() { Country = "China", Type = "STK", OpenTime = new DateTime(2000, 1, 1, 9, 30, 0), CloseTime = new DateTime(2000, 1, 1, 15, 0, 0) };
        }

        static ConcurrentDictionary<string, TVCCalendar[]> _holidayCache = new ConcurrentDictionary<string, TVCCalendar[]>();
        static ConcurrentDictionary<string, List<DateTime>> _marketOpenDayCache = new ConcurrentDictionary<string, List<DateTime>>();
        static ConcurrentDictionary<string, List<DateTime>> _marketOpenMinCache = new ConcurrentDictionary<string, List<DateTime>>();
        static ConcurrentDictionary<string, DateTime> _calendarStartDates = new ConcurrentDictionary<string, DateTime>();
        static ConcurrentDictionary<string, DateTime> _calendarFinalDates = new ConcurrentDictionary<string, DateTime>();
        static DateTime _calendarCacheLastUpdate = DateTime.MinValue;
        static ConcurrentDictionary<string, MarketSession> _marketSessions = new ConcurrentDictionary<string, MarketSession>();

        protected void UpdateMarketOpenDays()
        {
            foreach (var session in _marketSessions)
            {
                _marketOpenDayCache[session.Key] = CalculateMarketOpenDays(session.Value.Country, session.Value.Type, session.Value.OpenTime, session.Value.CloseTime);
            }
        }

        protected List<DateTime> CalculateMarketOpenDays(string country, string type, DateTime openTime, DateTime closeTime)
        {
            List<DateTime> marketOpenDates = new List<DateTime>();

            if (_holidayCache.Keys.Contains(country))
            {
                TVCCalendar[] holidays = _holidayCache[country];

                List<DateTime> holidayDates = (from p in holidays
                 where string.IsNullOrEmpty(p.EarlyClose)
                 orderby p.AsOfDate descending
                 select new DateTime(p.AsOfDate.Year, p.AsOfDate.Month, p.AsOfDate.Day)).ToList();

                Dictionary<DateTime, string> earlyCloseDates = (from p in holidays
                                                                where !string.IsNullOrWhiteSpace(p.EarlyClose)
                                                                select p).ToDictionary(k => k.AsOfDate.Date, v => v.EarlyClose);

                DateTime maxDate = holidayDates.Max();

                DateTime minDate = holidayDates.Min();

                DateTime finalDate = new DateTime(maxDate.Year, 12, 31);
                DateTime startDate = new DateTime(minDate.Year, 1, 1);

                _calendarStartDates[country + "-" + type] = startDate;
                _calendarFinalDates[country + "-" + type] = finalDate;

                for (DateTime curDate = finalDate; curDate >= startDate; curDate = curDate.AddDays(-1))
                {
                    if (curDate.DayOfWeek == DayOfWeek.Saturday || curDate.DayOfWeek == DayOfWeek.Sunday || holidayDates.Contains(curDate))
                    {
                        continue;
                    }

                    if (earlyCloseDates.ContainsKey(curDate))
                    {
                        string earlyCloseTime = earlyCloseDates[curDate];
                        string[] earlyTimes = earlyCloseTime.Split(new char[] { ':' });
                        int hour = Convert.ToInt32(earlyTimes[0]);
                        int minutes = Convert.ToInt32(earlyTimes[1]);
                        marketOpenDates.Add(new DateTime(curDate.Year, curDate.Month, curDate.Day, hour, minutes, 0));
                    }
                    else
                    {
                        marketOpenDates.Add(new DateTime(curDate.Year, curDate.Month, curDate.Day, closeTime.Hour, closeTime.Minute, closeTime.Second));
                    }
                }
            }

            return marketOpenDates;
        }

        protected void UpdateMarketOpenMins()
        {
            foreach (var session in _marketSessions)
            {
                _marketOpenMinCache[session.Key] = CalculateMarketOpenMins(session.Value.Country, session.Value.Type, session.Value.OpenTime, session.Value.CloseTime);
            }
        }

        protected List<DateTime> CalculateMarketOpenMins(string country, string type, DateTime openTime, DateTime closeTime)
        {
            List<DateTime> marketOpenDates = new List<DateTime>();

            if (_holidayCache.Keys.Contains(country))
            {
                TVCCalendar[] holidays = _holidayCache[country];

                List<DateTime> holidayDates = (from p in holidays
                                               where string.IsNullOrEmpty(p.EarlyClose)
                                               orderby p.AsOfDate descending
                                               select new DateTime(p.AsOfDate.Year, p.AsOfDate.Month, p.AsOfDate.Day)).ToList();

                Dictionary<DateTime, string> earlyCloseDates = (from p in holidays
                                                                where !string.IsNullOrWhiteSpace(p.EarlyClose)
                                                                select p).ToDictionary(k => k.AsOfDate.Date, v => v.EarlyClose);

                DateTime maxDate = holidayDates.Max();

                DateTime minDate = holidayDates.Min();

                DateTime finalDate = new DateTime(maxDate.Year, 12, 31);
                DateTime startDate = new DateTime(minDate.Year, 1, 1);

                _calendarStartDates[country + "-" + type] = startDate;
                _calendarFinalDates[country + "-" + type] = finalDate;

                for (DateTime curDate = finalDate; curDate >= startDate; curDate = curDate.AddDays(-1))
                {
                    if (curDate.DayOfWeek == DayOfWeek.Saturday || curDate.DayOfWeek == DayOfWeek.Sunday || holidayDates.Contains(curDate))
                    {
                        continue;
                    }

                    DateTime curOpenTime = new DateTime(curDate.Year, curDate.Month, curDate.Day, openTime.Hour, openTime.Minute, 0);
                    DateTime curCloseTime = new DateTime(curDate.Year, curDate.Month, curDate.Day, closeTime.Hour, closeTime.Minute, 0);

                    if (earlyCloseDates.ContainsKey(curDate))
                    {
                        string earlyCloseTime = earlyCloseDates[curDate];
                        string[] earlyTimes = earlyCloseTime.Split(new char[] { ':' });
                        int hour = Convert.ToInt32(earlyTimes[0]);
                        int minutes = Convert.ToInt32(earlyTimes[1]);
                        curCloseTime = new DateTime(curDate.Year, curDate.Month, curDate.Day, hour, minutes, 0);
                    }

                    for (DateTime curTime = curCloseTime; curTime >= curOpenTime; curTime = curTime.AddMinutes(-1))
                    {
                        marketOpenDates.Add(curTime);
                    }
                }
            }

            return marketOpenDates;
        }

        protected void RefreshCache()
        {
            if ((DateTime.Now - _calendarCacheLastUpdate).TotalDays > 1)
            {
                var newHolidayCache = new MarketDataDatabase().TVC_GetHolidays(null, null, null);
                if (newHolidayCache != null)
                {
                    _holidayCache = new ConcurrentDictionary<string, TVCCalendar[]>();
                    foreach (var holiday in newHolidayCache)
                    {
                        _holidayCache[holiday.Key] = holiday.Value;
                    }

                    UpdateMarketOpenDays();
                    UpdateMarketOpenMins();
                }
            }
        }

        public List<DateTime> GetDateTimes(string country, DateTime endDate, int count = 1, BarFrequency freq = BarFrequency.Day1, string type = "STK")
        {
            RefreshCache();

            List<DateTime> dateRange = new List<DateTime>();
            string key = country + "-" + type;
            if (_calendarFinalDates.ContainsKey(key))
            {
                DateTime startTime = _calendarStartDates[key];
                DateTime finalTime = _calendarFinalDates[key];

                if (endDate >= startTime && endDate <= finalTime)
                {
                    List<DateTime> allDateRange = null;

                    switch (freq)
                    {
                        case BarFrequency.Day1:
                            allDateRange = _marketOpenDayCache[key];
                            break;
                        case BarFrequency.Minute1:
                            allDateRange = _marketOpenMinCache[key];
                            break;
                    }

                    if (allDateRange != null)
                    {
                        DateTime endDateInRange = (from p in allDateRange
                         where p <= endDate
                         orderby p descending
                         select p).FirstOrDefault();

                        int endDateIdx = allDateRange.IndexOf(endDateInRange);

                        dateRange = allDateRange.GetRange(endDateIdx, count);
                    }
                }
            }

            return dateRange;
        }

        public Dictionary<string, TVCCalendar[]> GetHolidays(string[] countries, DateTime fromDate, DateTime toDate)
        {
            Dictionary<string, TVCCalendar[]> holidays = new Dictionary<string, TVCCalendar[]>();

            if (countries != null && countries.Count() > 0)
            {
                RefreshCache();

                if (_holidayCache != null && _holidayCache.Count > 0)
                {
                    foreach (string country in countries)
                    {
                        TVCCalendar[] countryCalendars = null;

                        if (_holidayCache.TryGetValue(country, out countryCalendars))
                        {
                            if (countryCalendars != null && countryCalendars.Count() > 0)
                            {
                                holidays[country] = (from p in countryCalendars
                                                      where p.AsOfDate >= fromDate && p.AsOfDate <= toDate
                                                      select p).ToArray();
                            }
                        }
                    }
                }
            }

            return holidays;
        }
    }
}
