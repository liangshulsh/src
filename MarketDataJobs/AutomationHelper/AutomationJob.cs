using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;

namespace AutomationHelper
{
    public class AutomationJob
    {
        private static IDictionary<string, string> _dictParams = null;
        private static int _statusId = 0;
        private static DateTime _startTime;
        private static DateTime _endTime;

        public static void Start(string[] args)
        {
            try
            {
                _startTime = DateTime.UtcNow;
                _dictParams = ArgumentParser.ParseArgs(args);
                InitStatus();
            }
            catch (Exception)
            {
            }
        }

        public static string GetArgument(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            string value = null;

            try
            {
                value = ConfigurationManager.AppSettings[key];
            }
            catch (Exception)
            {
            }

            string valueParam = null;

            if (_dictParams != null)
            {
                _dictParams.TryGetValue(key.ToLower(), out valueParam);
            }

            if (string.IsNullOrEmpty(valueParam))
            {
                return value;
            }
            else
            {
                return valueParam;
            }
        }

        public static void AddDataAttachment(string name, AttachmentType type, string data)
        {
            if (_statusId <= 0)
            {
                return;
            }


        }

        public static void AddFileAttachment(string name, AttachmentType type, string filePath)
        {
            if (_statusId <= 0)
            {
                return;
            }
        }

        public static void AddObjectAttachment(string name, AttachmentType type, object obj)
        {
            if (_statusId <= 0)
            {
                return;
            }
        }

        public static bool ShareIt(string fullFileName)
        {

            return false;
        }

        public static void End()
        {
            End(null);
        }

        public static void End(string logFileName)
        {
            _endTime = DateTime.UtcNow;
            Finilized(logFileName);
        }

        public static void Finilized(string logFileName)
        {
        }

        private static void InitStatus()
        {
        }
    }
}
