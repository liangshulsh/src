using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Utility
{
    public static class TextUtility
    {
        public static DataTable ConvertCSVToTable(string csvData, string tblName)
        {
            if (string.IsNullOrEmpty(csvData)) return null;

            int flag = 1;

            string[] lines = csvData.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            DataTable csvTable = new DataTable(tblName);

            for (int lineIdx = 0; lineIdx < lines.Length; lineIdx++)
            {
                string[] dataArray = lines[lineIdx].Split(',');
                //add cols
                if (lineIdx == 0)
                {
                    foreach (string colName in dataArray)
                    {
                        if (csvTable.Columns.Contains(colName.Trim()))
                        {
                            csvTable.Columns.Add(new DataColumn(colName.Trim() + flag++));
                        }
                        else
                        {
                            csvTable.Columns.Add(new DataColumn(colName.Trim()));
                        }
                    }
                    continue;
                }
                DataRow newrow = csvTable.NewRow();

                for (int dataIdx = 0; dataIdx < csvTable.Columns.Count; dataIdx++)
                {
                    newrow[dataIdx] = !string.IsNullOrEmpty(dataArray[dataIdx]) ? dataArray[dataIdx].Trim() : null;
                }
                csvTable.Rows.Add(newrow);
            }
            return csvTable;
        }

        private static DateTime DateUpLimit = new DateTime(2200, 1, 1);
        private static DateTime DateDownLimit = new DateTime(1800, 1, 1);

        public static bool ConvertStringToDateTime(string strDate, out DateTime date)
        {
            bool bResult;
            if (!string.IsNullOrEmpty(strDate))
            {
                strDate = strDate.Trim();
            }

            if (!string.IsNullOrEmpty(strDate))
            {
                if (!DateTime.TryParseExact(strDate, "yyyyMMdd", null, DateTimeStyles.None, out date))
                {
                    if (!DateTime.TryParse(strDate, System.Threading.Thread.CurrentThread.CurrentCulture, DateTimeStyles.None, out date))
                    {
                        double dateValue;
                        if (double.TryParse(strDate, out dateValue) && !strDate.ToLower().Contains('e'))
                        {
                            date = DateTime.FromOADate(dateValue);

                            if (date < new DateTime(1900, 1, 1))
                            {
                                date = date.AddDays(1);
                            }

                            if (date > DateDownLimit && date < DateUpLimit)
                            {
                                bResult = true;
                            }
                            else
                            {
                                bResult = false;
                            }
                        }
                        else
                        {
                            date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                            bResult = false;
                        }
                    }
                    else
                    {
                        bResult = true;
                    }
                }
                else
                {
                    bResult = true;
                }
            }
            else
            {
                date = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                bResult = false;
            }

            return bResult;
        }

        public static object GetFloatValue(float? value)
        {
            if (value.HasValue)
            {
                return (object)(float.Parse(value.ToString()));
            }

            return DBNull.Value;
        }

        public static object GetPtsFloatValue(float? value)
        {
            if (value.HasValue)
            {
                return (object)(float.Parse(value.ToString()) * 100);
            }

            return DBNull.Value;
        }

        public static object GetFloatValueInBP(float? value, int amount, bool useFace)
        {
            if (value.HasValue)
            {
                return value.Value / amount * (useFace ? 1 : 100);
            }
            return DBNull.Value;
        }

        public static Stream ToStream(this string str)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(str);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static decimal? ToDecimal(this object obj)
        {
            try
            {
                return Convert.ToDecimal(obj);
            }
            catch (Exception)
            {

            }

            return null;
        }

        public static double? ToDouble(this object obj)
        {
            try
            {
                return Convert.ToDouble(obj);
            }
            catch (Exception)
            {

            }

            return null;
        }

        public static DateTime? ToDateTime(this object obj)
        {
            try
            {
                return Convert.ToDateTime(obj);
            }
            catch (Exception)
            {

            }

            return null;
        }
    }
}
