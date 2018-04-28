using log4net;
using Skywolf.Contracts.DataContracts.Trading;
using Skywolf.Contracts.Services.Restful;
using Skywolf.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.TradingService.Restful
{
    [ServiceBehavior(Namespace = Constants.NAMESPACE, InstanceContextMode = InstanceContextMode.PerCall)]
    public class IBTradingRfService : EchoService, IIBTradingRfService
    {
        private static ILog _Logger;

        static IBTradingRfService()
        {
            _Logger = LogManager.GetLogger(typeof(IBTradingRfService));
        }

        public string CancelOrder(string username, string orderId)
        {
            try
            {
                int iOrderId = 0;

                int.TryParse(orderId, out iOrderId);
                if (new IBTradingService().CancelOrder(username, iOrderId))
                {
                    return "OK";
                }

                return "Failed";
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }

        }

        public string CreateUser(string username, string account, string host, string port)
        {
            try
            {
                int iPort = 0;
                int.TryParse(port, out iPort);
                if (new IBTradingService().CreateUser(username, account, host, iPort))
                {
                    return "OK";
                }
                else
                {
                    return "Failed";
                }
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string FilterTrades(string username, string clientid, string acctcode, string symbol, string sectype, string time, string exchange, string side)
        {
            try
            {
                TradeFilter filter = new TradeFilter();
                filter.AcctCode = acctcode;
                int iClientId = 0;
                if (int.TryParse(clientid, out iClientId))
                {
                    filter.ClientId = iClientId;
                }
                filter.Exchange = exchange;
                filter.SecType = sectype;
                filter.Side = side;
                filter.Symbol = symbol;
                filter.Time = time;
                Trade[] trades = new IBTradingService().FilterTrades(username, filter);
                if (trades != null)
                {
                    trades.ToCSV();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string GetAccountSummary(string username)
        {
            try
            {
                AccountSummary[] summary = new IBTradingService().GetAccountSummary(username);
                if (summary != null)
                {
                    return summary.ToCSV();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string GetAccountUpdateTime(string username)
        {
            try
            {
                return new IBTradingService().GetAccountUpdateTime(username);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string GetAllOpenOrders(string username)
        {
            try
            {
                Order[] orders = new IBTradingService().GetAllOpenOrders(username);
                if (orders != null)
                {
                    return orders.ToCSV();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string GetAllTrades(string username)
        {
            try
            {
                Trade[] trades = new IBTradingService().GetAllTrades(username);
                if (trades != null)
                {
                    return trades.ToCSV();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string GetOpenOrder(string username, string orderId)
        {
            try
            {
                int iOrderId = 0;
                int.TryParse(orderId, out iOrderId);
                Order order = new IBTradingService().GetOpenOrder(username, iOrderId);
                if (order != null)
                {
                    return new Order[] { order }.ToCSV();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string GetPortfolios(string username)
        {
            try
            {
                PositionPortfolio[] portfolios = new IBTradingService().GetPortfolios(username);
                if (portfolios != null)
                {
                    return portfolios.ToCSV();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string GetPositions(string username)
        {
            try
            {
                Position[] positions = new IBTradingService().GetPositions(username);
                if (positions != null)
                {
                    return positions.ToCSV();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string PlaceSimpleOrder(string username, string orderid, string securitytype, string symbol, string currency, string quantity, string ordertype, string action, string limitprice, string stopprice)
        {
            try
            {
                SimpleOrder order = null;

                int iOrderId = 0;
                int.TryParse(orderid, out iOrderId);

                double dQuantity = double.Parse(quantity);

                double dLmtPrice = 0;
                double.TryParse(limitprice, out dLmtPrice);

                double dStpPrice = 0;
                double.TryParse(stopprice, out dStpPrice);

                switch (ordertype)
                {
                    case "MKT":
                        order = new SimpleMarketOrder();
                        break;
                    case "LMT":
                        order = new SimpleLimitOrder();
                        (order as SimpleLimitOrder).LimitPrice = dLmtPrice;
                        break;
                    case "STP":
                        order = new SimpleStopOrder();
                        (order as SimpleStopOrder).StopPrice = dStpPrice;
                        break;
                }

                switch (action)
                {
                    case "BUY":
                        order.Action = TradeAction.BUY;
                        break;
                    case "SELL":
                        order.Action = TradeAction.SELL;
                        break;
                }

                order.Currency = currency;
                order.OrderId = iOrderId;
                order.Quantity = dQuantity;
                
                switch (securitytype)
                {
                    case "CASH":
                        order.SecurityType = TradeSecurityType.FX;
                        break;
                    case "STK":
                        order.SecurityType = TradeSecurityType.Stock;
                        break;
                }

                order.Symbol = symbol;
                order.UserName = username;

                int retOrderId = new IBTradingService().PlaceSimpleOrder(order);
                return retOrderId.ToString();
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string RefreshAllOpenOrders(string username)
        {
            try
            {
                Order[] openOrders = new IBTradingService().RefreshAllOpenOrders(username);
                if (openOrders != null)
                {
                    return openOrders.ToCSV();
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string RemoveUser(string username)
        {
            try
            {
                new IBTradingService().RemoveUser(username);
                return "OK";
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string SubscribeAccountUpdates(string username, string account)
        {
            try
            {
                if (new IBTradingService().SubscribeAccountUpdates(username, account))
                {
                    return "OK";
                }

                return "Failed";
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public string UnsubscribeAccountUpdates(string username)
        {
            try
            {
                if (new IBTradingService().UnsubscribeAccountUpdates(username))
                {
                    return "OK";
                }

                return "Failed";
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }
    }
}
