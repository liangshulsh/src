using Skywolf.Contracts.DataContracts.MarketData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skywolf.Utility;

namespace Skywolf.MarketDataService.Restful
{
    public class RestfulHelper
    {
        public static string ConvertBarFrequencyToString(BarFrequency freq)
        {
            string result = string.Empty;

            switch (freq)
            {
                case BarFrequency.Month1:
                    result = "MN";
                    break;
                case BarFrequency.Week1:
                    result = "W1";
                    break;
                case BarFrequency.Day1:
                    result = "D1";
                    break;
                case BarFrequency.Hour4:
                    result = "H4";
                    break;
                case BarFrequency.Hour1:
                    result = "H1";
                    break;
                case BarFrequency.Minute30:
                    result = "M30";
                    break;
                case BarFrequency.Minute15:
                    result = "M15";
                    break;
                case BarFrequency.Minute5:
                    result = "M5";
                    break;
                case BarFrequency.Minute1:
                    result = "M1";
                    break;
                case BarFrequency.Tick:
                    result = "T";
                    break;
            }

            return result;
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

        public static string ConvertBarToCSV(Bar[] bars)
        {
            if (bars is StockBar[])
            {
                StockBar[] stockBars = bars as StockBar[];
                return  stockBars.ToCSV();
            }
            else if (bars is CryptoBar[])
            {
                CryptoBar[] cryptoBars = bars as CryptoBar[];
                return cryptoBars.ToCSV();
            }

            return bars.ToCSV();
        }
    }
}
