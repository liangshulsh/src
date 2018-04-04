using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.MarketDataGrabber
{
    public class APIKey
    {
        public APIKey(string key)
        {
            Key = key;
            LockObj = new object();
            LastCallingTime = new DateTime(2000, 1, 1);
        }

        public string Key { get; set; }
        public object LockObj { get; set; }
        public DateTime LastCallingTime { get; set; }
        
        public double TimeEclipsed()
        {
            return (DateTime.Now - LastCallingTime).TotalMilliseconds;
        }
    }
}
