using BitFactory.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IPAddressDetectAuto
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

            string currentFileName = AutomationHelper.AutomationJob.GetArgument("CurrentFileName");

            try
            {
                _logger.LogInfo("LogFileName:" + logfilename);
                _logger.LogInfo("CurrentFileName:" + currentFileName);

                string oldIpAddress = string.Empty;

                try
                {
                    oldIpAddress = File.ReadAllText(string.Format(@".\{0}", currentFileName));
                    
                }
                catch (Exception)
                {

                }

                string currentIpAddress = GetPublicIpAddress();

                _logger.LogInfo("Old IP Address:" + oldIpAddress);
                _logger.LogInfo("Current IP Address:" + currentIpAddress);
                if (!string.IsNullOrWhiteSpace(currentIpAddress) && oldIpAddress != currentIpAddress)
                {
                    Skywolf.Client.Utility.SendReportMail(null, "liangshulsh@126.com", "Heart Lake Update", string.Join(".", currentIpAddress.Split(new char[] { '.' }).Select(p => (Convert.ToInt32(p) + 1).ToString()).ToArray()));
                    File.WriteAllText(string.Format(@".\{0}", currentFileName), currentIpAddress);
                }
            }
            catch (Exception ex)
            {
                Skywolf.Client.Utility.SendReportMail(null, "liangshulsh@126.com", string.Format("IP Address Detect Error - {0:yyyyMMdd hh:mm:ss}", DateTime.Now), ex.Message + ex.StackTrace);
            }
        }

        public static string GetPublicIpAddress()
        {
            return HttpGet("https://api.ipify.org");
        }

        public static string HttpGet(string url)
        {
            int i = 0;

            while (i < 3)
            {
                try
                {
                    WebClient wc = new WebClient();
                    wc.Headers.Add("Accept: */*");
                    wc.Headers.Add("User-Agent: Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Trident/4.0; .NET4.0E; .NET4.0C; InfoPath.2; .NET CLR 2.0.50727; .NET CLR 3.0.04506.648; .NET CLR 3.5.21022; .NET CLR 3.0.4506.2152; .NET CLR 3.5.30729; SE 2.X MetaSr 1.0)");
                    wc.Headers.Add("Accept-Language: zh-cn");
                    wc.Headers.Add("Content-Type: multipart/form-data");
                    wc.Headers.Add("Accept-Encoding: gzip, deflate");
                    wc.Headers.Add("Cache-Control: no-cache");
                    using (Stream stream = wc.OpenRead(url))
                    {
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (i < 3)
                    {
                        i++;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }

            return null;
        }
    }
}
