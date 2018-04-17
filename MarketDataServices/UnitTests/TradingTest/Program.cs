using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skywolf.TradingService;
using Skywolf.Utility;
using log4net;

namespace TradingTest
{
    class Program
    {
        static ILog _log;

        static void Main(string[] args)
        {
            EchoService.StartLogger();
            _log = EchoService.Logger;
            ConnectTest();
        }

        static void ConnectTest()
        {
            IBUser user = new IBUser("127.0.0.1", 4002, 2, _log);
            if (user.Connect())
            {
                var summary = user.RequestAccountSummary();
                foreach (var item in summary)
                {
                    Console.WriteLine(string.Format("{0} {1} {2} {3} {4}", item.RequestId, item.Account, item.Tag, item.Currency, item.Value));
                }
            }

            Console.Read();
        }
    }
}
