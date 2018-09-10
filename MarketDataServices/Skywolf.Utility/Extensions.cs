using System;
using System.Data;
using System.Text;

namespace Skywolf.Utility
{
    public static class Extensions
    {
        public static int ToUnixDateTime(this DateTime dt)
        {
            return Convert.ToInt32((dt - new DateTime(1970, 1, 1)).TotalSeconds);
        }

        public static DateTime FromUnixDateTime(this DateTime dt, int unixDateTime)
        {
            return new DateTime(1970, 1, 1).AddSeconds(unixDateTime);
        }

        public static DateTime ToUnspecifiedDateTime(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, 15, 0, 0, DateTimeKind.Unspecified);
        }

        public static string ToTransferFormatString(this DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
            {
                return string.Empty;
            }

            string grid = (dt.Rows.Count).ToString() + "x" + dt.Columns.Count.ToString();

            StringBuilder output = new StringBuilder();
            output.AppendLine(grid);
            foreach (DataColumn col in dt.Columns)
            {
                output.AppendLine(col.ColumnName);
            }

            foreach (DataRow row in dt.AsEnumerable())
            {
                foreach(DataColumn col in dt.Columns)
                {
                    if (col.DataType == typeof(DateTime))
                    {
                        output.AppendLine(row.Field<DateTime>(col).ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        output.AppendLine(row[col].ToString());
                    }
                }
            }

            return output.ToString();
        }
    }
}
