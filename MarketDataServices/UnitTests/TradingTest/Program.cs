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
            ConnectServiceTest();
        }
        static void ConnectServiceTest()
        {
            IBTradingService service = new IBTradingService();

            string username = "ying0806";
            string account = "127.0.0.1";

            if (service.CreateUser(username, account, "127.0.0.1", 4002))
            {
                var summary = service.GetAccountSummary(username);

                var properties = summary[0].GetType().GetProperties();
                
                foreach (var property in properties)
                {
                    Console.WriteLine(string.Format("{0}:{1}", property.Name, property.GetValue(summary[0])));
                }

                var simpleOrder = new Skywolf.Contracts.DataContracts.Trading.SimpleOrder()
                {
                    UserName = "ying0806",
                    Action = Skywolf.Contracts.DataContracts.Trading.TradeAction.SELL,
                    Currency = "USD",
                    Folder = "Index",
                    Quantity = 20000,
                    SecurityType = Skywolf.Contracts.DataContracts.Trading.TradeSecurityType.FX,
                    Strategy = "SmartBeta",
                    Fund = "FX",
                    Symbol = "EUR"
                };

                try
                {
                    int result = service.PlaceSimpleOrder(simpleOrder);
                    Console.WriteLine(string.Format("OrderId is {0}", result));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error is {0}", ex.Message));
                }
            }

            Console.Read();
        }

        static void ConnectTest()
        {
            IBUser user = new IBUser("127.0.0.1", 4002, 2, _log);
            if (user.Connect())
            {
                var summary = user.GetAccountSummary();
                foreach (var item in summary)
                {
                    Console.WriteLine(string.Format("{0} {1} {2} {3} {4}", item.RequestId, item.Account, item.Tag, item.Currency, item.Value));
                }

                var simpleOrder = new Skywolf.Contracts.DataContracts.Trading.SimpleOrder() {
                    UserName = "ying0806",
                    Action = Skywolf.Contracts.DataContracts.Trading.TradeAction.BUY,
                    Currency = "USD",
                    Folder = "Index",
                    Quantity = 100,
                    SecurityType = Skywolf.Contracts.DataContracts.Trading.TradeSecurityType.Stock,
                    Strategy = "SmartBeta",
                    Symbol = "SPY" };

                try
                {
                    var contract = IBContractSamples.GetContract(simpleOrder);
                    var order = IBOrderSamples.GetOrder(simpleOrder);
                    int result = user.PlaceOrder(contract, order);
                    Console.WriteLine(string.Format("OrderId is {0}", result));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format("Error is {0}", ex.Message));
                }
            }

            Console.Read();
        }
    }
}
