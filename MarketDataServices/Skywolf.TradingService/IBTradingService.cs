using System;
using System.Collections.Concurrent;
using System.Data;
using System.ServiceModel;
using System.Text;
using log4net;
using Skywolf.Contracts.Services;
using Skywolf.Utility;
using System.Globalization;
using System.Collections.Generic;
using Skywolf.Contracts.DataContracts.Instrument;
using Skywolf.Contracts.DataContracts.Trading;
using Trading = Skywolf.Contracts.DataContracts.Trading;
using Skywolf.IBApi;
using System.Linq;

namespace Skywolf.TradingService
{
    [ServiceBehavior(Namespace = Constants.NAMESPACE, InstanceContextMode = InstanceContextMode.PerCall)]
    public class IBTradingService : EchoService, IIBTradingService
    {
        private static ILog _Logger;
        private static IBUserManager _userManager;
        static IBTradingService()
        {
            _Logger = LogManager.GetLogger(typeof(IBTradingService));
            _userManager = new IBUserManager();
        }

        public bool CreateUser(string username, string account, string host, int port)
        {
            try
            {
                IBUser user = _userManager.GetUser(username);

                if (user == null)
                {
                    user = _userManager.AddUser(username, host, port);
                }

                if (user != null)
                {
                    user.SubscribeAccountUpdates(account);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public void RemoveUser(string username)
        {
            try
            {
                _userManager.RemoveUser(username);
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public int PlaceSimpleOrder(SimpleOrder simpleOrder)
        {
            try
            {
                IBUser user = _userManager.GetUser(simpleOrder.UserName);
                if (user != null)
                {
                    IBApi.Contract contract = IBContractSamples.GetContract(simpleOrder);
                    IBApi.Order order = IBOrderSamples.GetOrder(simpleOrder);
                    return user.PlaceOrder(contract, order);
                }

                return -1;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public bool CancelOrder(string userName, int orderId)
        {
            try
            {
                IBUser user = _userManager.GetUser(userName);
                if (user != null)
                {
                    return user.CancelOrder(orderId);
                }

                return false;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        private static Trading.Contract ConvertContract(IBApi.Contract contractIB)
        {
            if (contractIB != null)
            {
                Trading.Contract contract = new Trading.Contract();
                contract.ComboLegsDescription = contractIB.ComboLegsDescription;
                contract.ContractId = contractIB.ConId;
                contract.Symbol = contractIB.Symbol;
                contract.SecType = contractIB.SecType;
                contract.LastTradeDateOrContractMonth = contractIB.LastTradeDateOrContractMonth;
                contract.Strike = contractIB.Strike;
                contract.Right = contractIB.Right;
                contract.Multiplier = contractIB.Multiplier;
                contract.Exchange = contractIB.Exchange;
                contract.Currency = contractIB.Currency;
                contract.LocalSymbol = contractIB.LocalSymbol;
                contract.PrimaryExchange = contractIB.PrimaryExch;
                contract.TradingClass = contractIB.TradingClass;
                contract.SecIdType = contractIB.SecIdType;
                contract.SecId = contractIB.SecId;
                return contract;
            }

            return null;
        }

        private static Trading.Order ConvertOpenOrderMessageToOrder(Messages.OpenOrderMessage openOrder)
        {
            if (openOrder != null)
            {
                Trading.Order order = new Trading.Order();
                order.OrderId = openOrder.OrderId;
                order.Contract = ConvertContract(openOrder.Contract);
                if (openOrder.Order != null)
                {
                    order.Action = openOrder.Order.Action;
                    order.ActiveStartTime = openOrder.Order.ActiveStartTime;
                    order.ActiveStopTime = openOrder.Order.ActiveStopTime;
                    order.OrderId = openOrder.Order.OrderId;
                    order.ClientId = openOrder.Order.ClientId;
                    order.PermId = openOrder.Order.PermId;
                    order.TotalQuantity = openOrder.Order.TotalQuantity;
                    order.OrderType = openOrder.Order.OrderType;
                    order.LimitPrice = openOrder.Order.LmtPrice;
                    order.AuxPrice = openOrder.Order.AuxPrice;
                    order.Tif = openOrder.Order.Tif;
                    order.OcaGroup = openOrder.Order.OcaGroup;
                    order.OcaType = openOrder.Order.OcaType;
                    order.OrderRef = openOrder.Order.OrderRef;
                    order.Transmit = openOrder.Order.Transmit;
                    order.ParentId = openOrder.Order.ParentId;
                    order.BlockOrder = openOrder.Order.BlockOrder;
                    order.SweepToFill = openOrder.Order.SweepToFill;
                    order.DisplaySize = openOrder.Order.DisplaySize;
                    order.TriggerMethod = openOrder.Order.TriggerMethod;
                    order.OutsideRth = openOrder.Order.OutsideRth;
                    order.Hidden = openOrder.Order.Hidden;
                    order.GoodAfterTime = openOrder.Order.GoodAfterTime;
                    order.GoodTillDate = openOrder.Order.GoodTillDate;
                    order.OverridePercentageConstraints = openOrder.Order.OverridePercentageConstraints;
                    order.Rule80A = openOrder.Order.Rule80A;
                    order.AllOrNone = openOrder.Order.AllOrNone;
                    order.MinQty = openOrder.Order.MinQty;
                }

                if (openOrder.OrderState != null)
                {
                    order.Status = openOrder.OrderState.Status;
                    order.InitMargin = openOrder.OrderState.InitMargin;
                    order.MaintMargin = openOrder.OrderState.MaintMargin;
                    order.EquityWithLoan = openOrder.OrderState.EquityWithLoan;
                    order.Commission = openOrder.OrderState.Commission;
                    order.MinCommission = openOrder.OrderState.MinCommission;
                    order.MaxCommission = openOrder.OrderState.MaxCommission;
                    order.CommissionCurrency = openOrder.OrderState.CommissionCurrency;
                    order.WarningText = openOrder.OrderState.WarningText;
                }

                if (openOrder.OrderStatus != null)
                {
                    order.Filled = openOrder.OrderStatus.Filled;
                    order.Remaining = openOrder.OrderStatus.Remaining;
                    order.AvgFillPrice = openOrder.OrderStatus.AvgFillPrice;
                    order.LastFillPrice = openOrder.OrderStatus.LastFillPrice;
                    order.WhyHeld = openOrder.OrderStatus.WhyHeld;
                }

                return order;
            }

            return null;
        }

        public static Trading.Trade ConvertExecutionToTrade(Messages.ExecutionMessage execution)
        {
            if (execution != null)
            {
                Trading.Trade trade = new Trading.Trade();

                trade.Contract = ConvertContract(execution.Contract);
                
                if (execution.Execution != null)
                {
                    trade.OrderId = execution.Execution.OrderId;
                    trade.ClientId = execution.Execution.ClientId;
                    trade.ExecId = execution.Execution.ExecId;
                    trade.Time = execution.Execution.Time;
                    trade.AcctNumber = execution.Execution.AcctNumber;
                    trade.Exchange = execution.Execution.Exchange;
                    trade.Side = execution.Execution.Side;
                    trade.Shares = execution.Execution.Shares;
                    trade.Price = execution.Execution.Price;
                    trade.PermId = execution.Execution.PermId;
                    trade.Liquidation = execution.Execution.Liquidation;
                    trade.CumQty = execution.Execution.CumQty;
                    trade.AvgPrice = execution.Execution.AvgPrice;
                    trade.OrderRef = execution.Execution.OrderRef;
                    trade.EvRule = execution.Execution.EvRule;
                    trade.EvMultiplier = execution.Execution.EvMultiplier;
                    trade.ModelCode = execution.Execution.ModelCode;
                }

                if (execution.Commission != null)
                {
                    trade.Commission = execution.Commission.Commission;
                    trade.Currency = execution.Commission.Currency;
                    trade.RealizedPNL = execution.Commission.RealizedPNL;
                    trade.Yield = execution.Commission.Yield;
                    trade.YieldRedemptionDate = execution.Commission.YieldRedemptionDate;
                }

                return trade;
            }

            return null;
        }

        public static Trading.Position ConvertPosition(Messages.PositionMessage position)
        {
            if (position != null)
            {
                Trading.Position pos = new Trading.Position();
                pos.Contract = ConvertContract(position.Contract);
                pos.Account = position.Account;
                pos.Quantity = position.Position;
                pos.AverageCost = position.AverageCost;
                return pos;
            }

            return null;
        }

        public static Trading.PositionPortfolio ConvertPositionPortfolio(Messages.UpdatePortfolioMessage portfolio)
        {
            if (portfolio != null)
            {
                Trading.PositionPortfolio pos = new Trading.PositionPortfolio();
                pos.Contract = ConvertContract(portfolio.Contract);
                pos.Account = portfolio.AccountName;
                pos.Quantity = portfolio.Position;
                pos.AverageCost = portfolio.AverageCost;
                pos.MarketPrice = portfolio.MarketPrice;
                pos.MarketValue = portfolio.MarketValue;
                pos.RealizedPNL = portfolio.RealizedPNL;
                pos.UnrealizedPNL = portfolio.UnrealizedPNL;
                
                return pos;
            }

            return null;
        }

        public Trading.Order GetOpenOrder(string userName, int orderId)
        {
            try
            {
                IBUser user = _userManager.GetUser(userName);
                if (user != null)
                {
                    Messages.OpenOrderMessage openOrder = user.GetOpenOrderMessage(orderId, user.ClientId);
                    if (openOrder != null)
                    {
                        return ConvertOpenOrderMessageToOrder(openOrder);
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public Trading.Order[] GetAllOpenOrders(string userName)
        {
            try
            {
                IBUser user = _userManager.GetUser(userName);

                if (user != null)
                {
                    Messages.OpenOrderMessage[] openOrders = user.GetAllOpenOrders();
                    if (openOrders != null)
                    {
                        return openOrders.Select(p => ConvertOpenOrderMessageToOrder(p)).ToArray();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public Trading.Order[] RefreshAllOpenOrders(string userName)
        {
            try
            {
                IBUser user = _userManager.GetUser(userName);

                if (user != null)
                {
                    Messages.OpenOrderMessage[] openOrders = user.RefreshAllOpenOrders();
                    if (openOrders != null)
                    {
                        return openOrders.Select(p => ConvertOpenOrderMessageToOrder(p)).ToArray();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public AccountSummary[] GetAccountSummary(string userName)
        {
            try
            {
                IBUser user = _userManager.GetUser(userName);
                if (user != null)
                {
                    Messages.AccountSummaryMessage[] accountSummarys = user.GetAccountSummary();
                    if (accountSummarys != null)
                    {
                        Dictionary<string, Messages.AccountSummaryMessage[]> dictMessages = (from p in accountSummarys
                                                                                           group p by p.Account into g
                                                                                            select g).ToDictionary(k => k.Key, v => v.ToArray());
                        List<AccountSummary> accountSummaryList = new List<AccountSummary>();

                        foreach (string account in dictMessages.Keys)
                        {
                            Messages.AccountSummaryMessage[] messages = dictMessages[account];
                            AccountSummary accountSummary = new AccountSummary();
                            Dictionary<string, System.Reflection.PropertyInfo> dictProperties = typeof(AccountSummary).GetProperties().ToDictionary(k => k.Name, v => v);
                            accountSummary.UserName = userName;
                            foreach (Messages.AccountSummaryMessage message in messages)
                            {
                                System.Reflection.PropertyInfo property = null;
                                if (dictProperties.TryGetValue(message.Tag, out property))
                                {
                                    if (property.GetType() == typeof(string))
                                    {
                                        property.SetValue(accountSummary, message.Value);
                                    }
                                    else if (property.GetType() == typeof(double))
                                    {
                                        double val;
                                        if (double.TryParse(message.Value, out val))
                                        {
                                            property.SetValue(accountSummary, val);
                                        }
                                    }
                                }
                            }

                            accountSummaryList.Add(accountSummary);
                        }

                        return accountSummaryList.ToArray();
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _Logger.Error(ex);
                throw ex;
            }
        }

        public bool UnsubscribeAccountUpdates(string userName)
        {
            IBUser user = _userManager.GetUser(userName);
            if (user != null)
            {
                return user.UnsubscribeAccountUpdates();
            }

            return false;
        }

        public bool SubscribeAccountUpdates(string userName, string account)
        {
            IBUser user = _userManager.GetUser(userName);
            if (user != null)
            {
                return user.SubscribeAccountUpdates(account);
            }

            return false;
        }

        public PositionPortfolio[] GetPortfolios(string userName)
        {
            IBUser user = _userManager.GetUser(userName);
            if (user != null)
            {
                Messages.UpdatePortfolioMessage[] portfolios = user.GetPortfolios();
                if (portfolios != null)
                {
                    return portfolios.Select(p => ConvertPositionPortfolio(p)).ToArray();
                }
            }

            return null;
        }

        public Trading.Trade[] GetAllTrades(string userName)
        {
            IBUser user = _userManager.GetUser(userName);
            if (user != null)
            {
                Messages.ExecutionMessage[] executions = user.GetAllExecutions();
                if (executions != null)
                {
                    return executions.Select(p => ConvertExecutionToTrade(p)).ToArray();
                }
            }

            return null;
        }

        public Trading.Trade[] FilterTrades(string userName, TradeFilter tradeFilter)
        {
            IBUser user = _userManager.GetUser(userName);
            if (user != null)
            {
                ExecutionFilter filter = new ExecutionFilter(tradeFilter.ClientId, tradeFilter.AcctCode, tradeFilter.Time, tradeFilter.Symbol, tradeFilter.SecType, tradeFilter.Exchange, tradeFilter.Side);
                Messages.ExecutionMessage[] executions = user.FilterExecutions(filter);
                if (executions != null)
                {
                    return executions.Select(p => ConvertExecutionToTrade(p)).ToArray();
                }
            }

            return null;
        }

        public string GetAccountUpdateTime(string userName)
        {
            IBUser user = _userManager.GetUser(userName);
            if (user != null)
            {
                return user.GetAccountUpdateTime();
            }

            return string.Empty;
        }

        public Trading.Position[] GetPositions(string userName)
        {
            IBUser user = _userManager.GetUser(userName);
            if (user != null)
            {
                Messages.PositionMessage[] positions = user.GetPositions();
                if (positions != null)
                {
                    return positions.Select(p => ConvertPosition(p)).ToArray();
                }
            }

            return null;
        }
    }
}
