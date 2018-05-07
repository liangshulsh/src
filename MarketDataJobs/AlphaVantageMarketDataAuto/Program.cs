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
            string marketDataDUrl = AutomationHelper.AutomationJob.GetArgument("MarketDataDSkywolfHttp");
            string marketDataMUrl = AutomationHelper.AutomationJob.GetArgument("MarketDataMSkywolfHttp");
            string marketDataWUrl = AutomationHelper.AutomationJob.GetArgument("MarketDataWSkywolfHttp");
            string marketDataMNUrl = AutomationHelper.AutomationJob.GetArgument("MarketDataMNSkywolfHttp");
            string priorityStart = AutomationHelper.AutomationJob.GetArgument("PriorityStart");
            string priorityEnd = AutomationHelper.AutomationJob.GetArgument("PriorityEnd");
            
            bool isAdjustedValue = false;

            bool.TryParse(adjustedvalue, out isAdjustedValue);
            int iStartFrom = 0;
            int.TryParse(startFrom, out iStartFrom);

            int iPriorityStart = 0;
            int iPriorityEnd = 0;

            int.TryParse(priorityStart, out iPriorityStart);
            int.TryParse(priorityEnd, out iPriorityEnd);

            try
            {
                _logger.LogInfo("LogFileName:" + logfilename);
                _logger.LogInfo("Period:" + period);
                _logger.LogInfo("AdjustedValue:" + isAdjustedValue.ToString());
                _logger.LogInfo("StartFrom:" + iStartFrom);
                _logger.LogInfo("PriorityStart:" + iPriorityStart);
                _logger.LogInfo("PriorityEnd:" + iPriorityEnd);
                _logger.LogInfo("MarketDataDUrl:" + marketDataDUrl);
                _logger.LogInfo("MarketDataMUrl:" + marketDataMUrl);
                _logger.LogInfo("MarketDataWUrl:" + marketDataWUrl);
                _logger.LogInfo("MarketDataMNUrl:" + marketDataMNUrl);

                MarketDataHandler marketData = new MarketDataHandler(period, isAdjustedValue, iPriorityStart, iPriorityEnd);

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
