using System;
using System.IO;
using log4net;
using log4net.Config;

namespace Skywolf.Utility
{
    public abstract class EchoService : IEcho
    {
        public string Ping()
        {
            return "Pong response for Ping......";
        }

        public static ILog Logger
        {
            get
            {
                return _logger;
            }
        }

        public static void StartLogger()
        {
            var logCfg = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logCfg);
            //CheckLog();
            _logger = log4net.LogManager.GetLogger(typeof(EchoService));

        }

        public static void StartLoggerNoEmail()
        {
            StartLogger();
        }

        //public static void CheckLog()
        //{
        //    string logPath = @".\log-file.txt";
        //    if (File.Exists(logPath))
        //    {
        //        PostMan postMan = new PostMan();
        //        string dir = Directory.GetCurrentDirectory();
        //        postMan.Send(string.Format("{0} crashed", dir), "", logPath);
        //    }
        //}

        //public static void DeleteLog()
        //{
        //    LogManager.Shutdown();
        //    string logPath = "./log-file.txt";
        //    File.Delete(logPath);
        //}

        //public static IDisposable SetLogContext(string ndc, string mdc)
        //{
        //    SetLogMdc(mdc);
        //    return SetLogNdc(ndc);
        //}

        //public static void ResetLogContext(IDisposable ndc)
        //{
        //    ResetLogMdc();
        //    ndc.Dispose();
        //}

        //public static IDisposable SetLogNdc(string ndc)
        //{
        //    return log4net.NDC.Push(ndc);
        //}

        //public static void SetLogMdc(string mdc)
        //{
        //    ThreadContext.Properties["auth"] = mdc;
        //}

        //public static void ResetLogMdc()
        //{
        //    ThreadContext.Properties["auth"] = string.Empty;
        //}

        static ILog _logger;
    }
}
