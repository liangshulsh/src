using BitFactory.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlphaVantageMarketDataAuto
{
    class Program
    {
        public static readonly CompositeLogger _logger = new CompositeLogger();

        static void Main(string[] args)
        {
            string defaultLogFilename = ConfigurationManager.AppSettings["LogFileName"];
            string logfilename = string.Format(@"{0}\{1}_{2}_{4}{3}",
                ConfigurationManager.AppSettings["LogFilePath"],
                Path.GetFileNameWithoutExtension(defaultLogFilename),
                Environment.MachineName,
                Path.GetExtension(defaultLogFilename),
                DateTime.Today.ToString("MMMddyyyy", CultureInfo.InvariantCulture));
            _logger.AddLogger("file", new FileLogger(logfilename));
            _logger.AddLogger("debug", new DebugLogger());
            _logger.AddLogger("console", TextWriterLogger.NewConsoleLogger());

            try
            {
                Skywolf.Client.Utility.SendReportMail(null, "liangshulsh@hotmail.com", "liangshulsh@126.com", "test smtp", "hello world");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
            }
        }
    }
}
