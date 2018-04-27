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
            AutomationHelper.AutomationJob.Start(args);
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

            string period = AutomationHelper.AutomationJob.GetArgument("period");
            string adjustedvalue = AutomationHelper.AutomationJob.GetArgument("adjustedvalue");
            string startFrom = AutomationHelper.AutomationJob.GetArgument("StartFrom");
            string marketDataUrl = AutomationHelper.AutomationJob.GetArgument("MarketDataSkywolfHttp");
            bool isAdjustedValue = false;

            bool.TryParse(adjustedvalue, out isAdjustedValue);
            int iStartFrom = 0;
            int.TryParse(startFrom, out iStartFrom);
            try
            {
                _logger.LogInfo("LogFileName:" + logfilename);
                _logger.LogInfo("Period:" + period);
                _logger.LogInfo("AdjustedValue:" + isAdjustedValue.ToString());
                _logger.LogInfo("StartFrom:" + iStartFrom);
                _logger.LogInfo("MarketDataUrl:" + marketDataUrl);

                MarketDataHandler marketData = new MarketDataHandler(period, isAdjustedValue);

                marketData.Update(iStartFrom);

                string title = string.Empty;

                if (isAdjustedValue)
                {
                    title = string.Format("Alpha Vantage Market Data ({0} Adjusted) Done", period);
                }
                else
                {
                    title = string.Format("Alpha Vantage Market Data ({0}) Done", period);
                }

                Skywolf.Client.Utility.SendReportMail(null, "liangshulsh@126.com", title, string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                
                Skywolf.Client.Utility.SendReportMail(null, "liangshulsh@126.com",
                    string.Format("Alpha Vantage Market Data ({0}) Error", period), ex.Message + ex.StackTrace);
            }
            finally
            {
                AutomationHelper.AutomationJob.End(logfilename);
            }
        }
    }
}
