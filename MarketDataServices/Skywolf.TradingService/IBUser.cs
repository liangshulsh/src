using log4net;
using Skywolf.IBApi;
using Skywolf.TradingService.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skywolf.TradingService
{
    public class IBUser
    {
        private const int ACCOUNT_ID_BASE = 50000000;

        private const int ACCOUNT_SUMMARY_ID = ACCOUNT_ID_BASE + 1;

        public const int CONTRACT_ID_BASE = 60000000;

        private int _contractDetailNextId = CONTRACT_ID_BASE + 1;

        private const string ACCOUNT_SUMMARY_TAGS = "AccountType,NetLiquidation,TotalCashValue,SettledCash,AccruedCash,BuyingPower,EquityWithLoanValue,PreviousEquityWithLoanValue,"
             + "GrossPositionValue,ReqTEquity,ReqTMargin,SMA,InitMarginReq,MaintMarginReq,AvailableFunds,ExcessLiquidity,Cushion,FullInitMarginReq,FullMaintMarginReq,FullAvailableFunds,"
             + "FullExcessLiquidity,LookAheadNextChange,LookAheadInitMarginReq ,LookAheadMaintMarginReq,LookAheadAvailableFunds,LookAheadExcessLiquidity,HighestSeverity,DayTradesRemaining,Leverage";

        private const int TIME_OUT_VALUE = 30000;

        protected ILog _log;
        protected IBClient _ibClient;

        protected string _host;
        protected int _port;
        protected int _clientId;
        protected bool _isConnected = false;


        private object _connectLockObj = new object();
        private List<string> _managedAccounts = new List<string>();

        public bool IsConnected
        {
            get { return _isConnected; }
            set { _isConnected = value; }
        }

        public int ClientId
        {
            get { return _clientId; }
        }

        private EventWaitHandle _accountSummaryWait = null;
        private object _accountSummaryLockObj = new object();
        private List<AccountSummaryMessage> _accountSummary = new List<AccountSummaryMessage>();

        private object _updateAccountLockObj = new object();
        private bool _accountUpdateRequestActive = false;
        private string _currentAccountSubscribedToTupdate = string.Empty;
        private List<AccountValueMessage> _accountValues = new List<AccountValueMessage>();
        private List<UpdatePortfolioMessage> _portfolios = new List<UpdatePortfolioMessage>();

        private EventWaitHandle _positionWait = null;
        private object _positionLockObj = new object();
        private List<PositionMessage> _positions = new List<PositionMessage>();

        private string _lastUpdateTime = string.Empty;

        private object _placeOrderLockObj = new object();

        private EventWaitHandle _openOrderWait = null;
        private object _refreshOpenOrderLockObj = new object();
        private object _openOrderLockObj = new object();
        private List<OpenOrderMessage> _openOrders = new List<OpenOrderMessage>();

        private EventWaitHandle _executionWait = null;
        private object _filterExecutionLockObj = new object();
        private object _executionLockObj = new object();
        private List<ExecutionMessage> _executions = new List<ExecutionMessage>();
        private Dictionary<int, List<ExecutionMessage>> _requestExecutions = new Dictionary<int, List<ExecutionMessage>>();
        private int _executionReqNextId = 1000;

        private EventWaitHandle _contractDetailWait = null;
        private object _contractDetailLockObj = new object();
        private ConcurrentDictionary<string, ContractDetailsMessage> _contractDetails = new ConcurrentDictionary<string, ContractDetailsMessage>();

        private EReaderMonitorSignal _signal = new EReaderMonitorSignal();

        public IBUser(string host, int port, int clientId, ILog log)
        {
            _log = log;
            _host = host;
            _port = port;
            _clientId = clientId;

            _ibClient = new IBClient(_signal);
            _ibClient.Error += ibClient_Error;
            _ibClient.ConnectionClosed += ibClient_ConnectionClosed;
            _ibClient.NextValidId += ibClient_UpdateConnectionStatus;
            _ibClient.ManagedAccounts += ibClient_UpdateManagedAccounts;
            _ibClient.AccountSummary += ibClient_HandleAccountSummary;
            _ibClient.AccountSummaryEnd += ibClient_HandleAccountSummaryEnd;
            _ibClient.Position += ibClient_HandlePosition;
            _ibClient.PositionEnd += ibClient_HandlePositionEnd;
            _ibClient.UpdateAccountValue += ibClient_HandleAccountValue;
            _ibClient.UpdatePortfolio += ibClient_HandlePortfolioValue;
            _ibClient.UpdateAccountTime += ibClient_HandleLastUpdateTime;
            _ibClient.OpenOrder += ibClient_HandleOpenOrder;
            _ibClient.OrderStatus += ibClient_HandleOrderStatus;
            _ibClient.OpenOrderEnd += ibClient_HandleOpenOrderEnd;
            _ibClient.ExecDetails += ibClient_HandleExecutionMessage;
            _ibClient.ExecDetailsEnd += ibClient_HandleExecutionEnd;
            _ibClient.CommissionReport += commissionReport => ibClient_HandleCommissionMessage(new CommissionMessage(commissionReport));
            _ibClient.ContractDetails += ibClient_HandleContractDataMessage;
            _ibClient.ContractDetailsEnd += ibClient_HandleContractDataEnd;

        }

        #region PlaceOrder

        public int PlaceOrder(Contract contract, Order order)
        {
            lock (_placeOrderLockObj)
            {
                int orderId = 0;
                if (order.OrderId != 0)
                {
                    _ibClient.ClientSocket.placeOrder(order.OrderId, contract, order);
                    orderId = order.OrderId;
                }
                else
                {
                    _ibClient.ClientSocket.placeOrder(_ibClient.NextOrderId, contract, order);
                    orderId = _ibClient.NextOrderId;
                    _ibClient.NextOrderId++;
                }

                return orderId;
            }
        }

        public bool CancelOrder(int orderId)
        {
            int clientId = _clientId;
            OpenOrderMessage openOrder = GetOpenOrderMessage(orderId, clientId);
            if (openOrder != null)
            {
                _ibClient.ClientSocket.cancelOrder(openOrder.OrderId);
                return true;
            }

            return false;
        }

        public OpenOrderMessage GetOpenOrderMessage(int orderId, int clientId)
        {
            lock (_openOrderLockObj)
            {
                for (int i = 0; i < _openOrders.Count; i++)
                {
                    if (_openOrders[i].Order.OrderId == orderId && _openOrders[i].Order.ClientId == clientId)
                        return _openOrders[i];
                }
            }

            return null;
        }

        public OpenOrderMessage[] GetAllOpenOrders()
        {
            lock(_openOrderLockObj)
            {
                return _openOrders.ToArray();
            }
        }

        public OpenOrderMessage[] RefreshAllOpenOrders()
        {
            lock (_refreshOpenOrderLockObj)
            {
                if (_openOrderWait != null)
                {
                    try
                    {
                        _openOrderWait.Set();
                    }
                    catch (Exception)
                    {

                    }
                    _openOrderWait = null;
                }

                if (_openOrderWait == null)
                {
                    _openOrderWait = new ManualResetEvent(false);
                    _ibClient.ClientSocket.reqAllOpenOrders();

                    _openOrderWait.WaitOne(TIME_OUT_VALUE);
                }

                return GetAllOpenOrders();
            }
        }

        public ExecutionMessage[] GetAllExecutions()
        {
            lock (_executionLockObj)
            {
                return _executions.ToArray();
            }
        }

        public ExecutionMessage[] FilterExecutions(ExecutionFilter execFilter)
        {

            if (execFilter == null)
            {
                execFilter = new ExecutionFilter();
            }

            lock (_filterExecutionLockObj)
            {
                if (_executionWait != null)
                {
                    try
                    {
                        _executionWait.Set();
                    }
                    catch (Exception)
                    {

                    }
                    _executionWait = null;
                }

                List<ExecutionMessage> reqExecutions = new List<ExecutionMessage>();

                if (_executionWait == null)
                {
                    _executionWait = new ManualResetEvent(false);
                    int reqId = _executionReqNextId;
                    _executionReqNextId++;
                    _requestExecutions[reqId] = reqExecutions;
                    _ibClient.ClientSocket.reqExecutions(reqId, execFilter);
                    _executionWait.WaitOne(TIME_OUT_VALUE);
                    _requestExecutions.Remove(reqId);
                }

                return reqExecutions.ToArray();
            }
        }

        private void ibClient_HandleExecutionEnd(int reqId)
        {
            if (_executionWait != null)
            {
                _executionWait.Set();
                _executionWait = null;
            }
        }

        private void ibClient_HandleCommissionMessage(CommissionMessage message)
        {
            lock (_executionLockObj)
            {
                for (int i = 0; i < _executions.Count; i++)
                {
                    if (_executions[i].Execution != null && _executions[i].Execution.ExecId == message.CommissionReport.ExecId)
                    {
                        _executions[i].Commission = message.CommissionReport;
                    }
                }

                foreach (var executions in _requestExecutions.Values)
                {
                    foreach (var execution in executions)
                    {
                        if (execution.Execution != null && execution.Execution.ExecId == message.CommissionReport.ExecId)
                        {
                            execution.Commission = message.CommissionReport;
                        }
                    }
                }
            }
        }

        private void ibClient_HandleOpenOrder(OpenOrderMessage openOrder)
        {
            UpdateLiveOrders(openOrder);
        }

        private void ibClient_HandleOpenOrderEnd()
        {
           if (_openOrderWait != null)
            {
                _openOrderWait.Set();
                _openOrderWait = null;
            }
        }

        private void ibClient_HandleExecutionMessage(ExecutionMessage message)
        {
            lock (_executionLockObj)
            {
                UpdateExecutionMessageToDatabase(message);
                List<ExecutionMessage> executions;
                if (_requestExecutions.ContainsKey(message.ReqId))
                {
                    executions = _requestExecutions[message.ReqId];
                }
                else
                {
                    executions = _executions;
                }
                
                for (int i = 0; i < executions.Count; i++)
                {
                    if (executions[i].Execution != null && executions[i].Execution.ExecId == message.Execution.ExecId)
                    {
                        executions[i] = message;
                    }
                }

                executions.Add(message);
            }
        }

        private void UpdateExecutionMessageToDatabase(ExecutionMessage executionMessage)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    new DatabaseRepository.TradeDatabase().Trade_Upsert(IBTradingService.ConvertExecutionToTrade(executionMessage));
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);
                    _log.Error(ex.StackTrace);
                }
            });
        }

        private void ibClient_HandleOrderStatus(OrderStatusMessage statusMessage)
        {
            lock (_openOrderLockObj)
            {
                for (int i = 0; i < _openOrders.Count; i++)
                {
                    if (_openOrders[i].Order.PermId == statusMessage.PermId)
                    {
                        _openOrders[i].OrderStatus = statusMessage;
                        UpdateOrderMessageToDatabase(_openOrders[i]);
                        return;
                    }
                }
            }
        }

        private void UpdateLiveOrders(OpenOrderMessage orderMessage)
        {
            lock (_openOrderLockObj)
            {
                UpdateOrderMessageToDatabase(orderMessage);
                for (int i = 0; i < _openOrders.Count; i++)
                {
                    if (_openOrders[i].Order.OrderId == orderMessage.OrderId)
                    {
                        _openOrders[i] = orderMessage;
                        return;
                    }
                }
                _openOrders.Add(orderMessage);
            }
        }

        private void UpdateOrderMessageToDatabase(OpenOrderMessage orderMessage)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    new DatabaseRepository.TradeDatabase().Order_Upsert(IBTradingService.ConvertOpenOrderMessageToOrder(orderMessage));
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);
                    _log.Error(ex.StackTrace);
                }
            });
        }

        #endregion

        #region ContractDetails

        public ContractDetails GetContractDetails(Contract contract)
        {
            if (contract == null)
            {
                return null;
            }

            if (_contractDetails.ContainsKey(contract.ToString()))
            {
                return _contractDetails[contract.ToString()].ContractDetails;
            }
            else
            {
                lock (_contractDetailLockObj)
                {
                    if (_contractDetailWait != null)
                    {
                        _contractDetailWait.Set();
                        _contractDetailWait = null;
                    }

                    int reqId = _contractDetailNextId;
                    _contractDetailNextId++;
                    _contractDetailWait = new ManualResetEvent(false);
                    _ibClient.ClientSocket.reqContractDetails(reqId, contract);
                    _contractDetailWait.WaitOne(TIME_OUT_VALUE);
                    return (from p in _contractDetails
                     where p.Value.RequestId == reqId
                     select p.Value.ContractDetails).FirstOrDefault();
                }
            }
        }
        
        private void ibClient_HandleContractDataMessage(ContractDetailsMessage message)
        {
            if (message.ContractDetails != null && message.ContractDetails.Summary != null)
            {
                _contractDetails[message.ContractDetails.Summary.ToString()] = message;
            }
        }

        private void ibClient_HandleContractDataEnd(int reqId)
        {
            if (_contractDetailWait != null)
            {
                _contractDetailWait.Set();
                _contractDetailWait = null;
            }
        }

        #endregion

        #region Connection
        public void Disconnect()
        {
            if (IsConnected)
            {
                IsConnected = false;
                _ibClient.ClientSocket.eDisconnect();
            }
        }

        public bool Connect()
        {
            lock (_connectLockObj)
            {
                if (!IsConnected)
                {
                    int port;
                    string host = _host;

                    if (host == null || host.Equals(""))
                        host = "127.0.0.1";
                    try
                    {
                        port = _port;
                        _ibClient.ClientId = _clientId;
                        _ibClient.ClientSocket.eConnect(host, port, _ibClient.ClientId);

                        var reader = new EReader(_ibClient.ClientSocket, _signal);

                        reader.Start();
                        EventWaitHandle wait = new ManualResetEvent(false);
                        new Thread(() => { while (_ibClient.ClientSocket.IsConnected()) { _signal.waitForSignal(); reader.processMsgs(); wait.Set(); } }) { IsBackground = true }.Start();
                        wait.WaitOne(TIME_OUT_VALUE);
                    }
                    catch (Exception)
                    {
                        HandleErrorMessage(new ErrorMessage(-1, -1, "Please check your connection attributes."));
                    }
                }
            }

            return IsConnected;
        }

        private void ibClient_UpdateManagedAccounts(ManagedAccountsMessage message)
        {
            _managedAccounts = message.ManagedAccounts;
        }

        void ibClient_ConnectionClosed()
        {
            IsConnected = false;
            ibClient_UpdateConnectionStatus(new ConnectionStatusMessage(false));
        }

        private void ibClient_UpdateConnectionStatus(ConnectionStatusMessage statusMessage)
        {
            IsConnected = statusMessage.IsConnected;
        }

        #endregion

        #region Account Summary

        public string[] GetManagedAccount()
        {
            if (IsConnected)
            {
                _managedAccounts.ToArray();
            }

            return null;
        }

        public AccountSummaryMessage[] GetAccountSummary()
        {
            lock (_accountSummaryLockObj)
            {
                if (_accountSummaryWait != null)
                {
                    try
                    {
                        _ibClient.ClientSocket.cancelAccountSummary(ACCOUNT_SUMMARY_ID);
                        _accountSummaryWait.Set();
                    }
                    catch (Exception)
                    {

                    }
                    _accountSummaryWait = null;
                }

                if (_accountSummaryWait == null)
                {
                    _accountSummaryWait = new ManualResetEvent(false);
                    _ibClient.ClientSocket.reqAccountSummary(ACCOUNT_SUMMARY_ID, "All", ACCOUNT_SUMMARY_TAGS);
                    
                    _accountSummaryWait.WaitOne(TIME_OUT_VALUE);
                }

                return _accountSummary.ToArray();
            }
        }

        private void ibClient_HandleAccountSummaryEnd(AccountSummaryEndMessage summaryEndMessage)
        {
            if (_accountSummaryWait != null)
            {
                _accountSummaryWait.Set();
                _accountSummaryWait = null;
            }
        }

        private void ibClient_HandleAccountSummary(AccountSummaryMessage summaryMessage)
        {
            for (int i = 0; i < _accountSummary.Count; i++)
            {
                if (_accountSummary[i].Account == summaryMessage.Account && _accountSummary[i].Tag.Equals(summaryMessage.Tag) && _accountSummary[i].Account.Equals(summaryMessage.Account))
                {
                    _accountSummary[i].Value = summaryMessage.Value;
                    _accountSummary[i].Currency = summaryMessage.Currency;
                    return;
                }
            }

            _accountSummary.Add(new AccountSummaryMessage(summaryMessage.RequestId, summaryMessage.Account, summaryMessage.Tag, summaryMessage.Value, summaryMessage.Currency));
        }

        #endregion

        #region Account Update

        public bool UnsubscribeAccountUpdates()
        {
            if (_accountUpdateRequestActive)
            {
                lock (_updateAccountLockObj)
                {
                    if (_accountUpdateRequestActive)
                    {
                        _ibClient.ClientSocket.reqAccountUpdates(false, _currentAccountSubscribedToTupdate);
                        _currentAccountSubscribedToTupdate = string.Empty;
                        _accountUpdateRequestActive = false;
                        return true;
                    }
                }
            }

            return false;
        }

        public bool SubscribeAccountUpdates(string account)
        {
            if (string.IsNullOrWhiteSpace(account) || _managedAccounts == null || _managedAccounts.Count == 0)
            {
                return false;
            }

            lock (_updateAccountLockObj)
            {
                if (!_accountUpdateRequestActive)
                {
                    if (string.IsNullOrWhiteSpace(account))
                    {
                        _currentAccountSubscribedToTupdate = _managedAccounts.First();
                    }
                    else
                    {
                        _currentAccountSubscribedToTupdate = account;
                    }

                    _accountUpdateRequestActive = true;
                    _accountValues.Clear();
                    _portfolios.Clear();
                    _ibClient.ClientSocket.reqAccountUpdates(true, _currentAccountSubscribedToTupdate);
                    return true;
                }
            }

            return false;
        }

        public AccountValueMessage[] GetAccountValues()
        {
            lock (_updateAccountLockObj)
            {
                if (_accountValues != null)
                {
                    return _accountValues.ToArray();
                }
            }

            return null;
        }

        public UpdatePortfolioMessage[] GetPortfolios()
        {
            lock (_updateAccountLockObj)
            {
                if (_portfolios != null)
                {
                    return _portfolios.ToArray();
                }
            }

            return null;
        }

        public string GetAccountUpdateTime()
        {
            return _lastUpdateTime;
        }

        private void ibClient_HandleAccountValue(AccountValueMessage accountValueMessage)
        {
            lock (_updateAccountLockObj)
            {
                for (int i = 0; i < _accountValues.Count; i++)
                {
                    if (_accountValues[i].AccountName == accountValueMessage.AccountName && _accountValues[i].Key == accountValueMessage.Key)
                    {
                        _accountValues[i].Value = accountValueMessage.Value;
                        _accountValues[i].Currency = accountValueMessage.Currency;
                        return;
                    }
                }
                _accountValues.Add(accountValueMessage);
            }
        }

        private void ibClient_HandlePortfolioValue(UpdatePortfolioMessage updatePortfolioMessage)
        {
            lock (_updateAccountLockObj)
            {
                for (int i = 0; i < _portfolios.Count; i++)
                {
                    if (_portfolios[i].AccountName == updatePortfolioMessage.AccountName && Utils.ContractToString(_portfolios[i].Contract) == Utils.ContractToString(updatePortfolioMessage.Contract))
                    {
                        _portfolios[i].Position = updatePortfolioMessage.Position;
                        _portfolios[i].MarketPrice = updatePortfolioMessage.MarketPrice;
                        _portfolios[i].MarketValue = updatePortfolioMessage.MarketValue;
                        _portfolios[i].AverageCost = updatePortfolioMessage.AverageCost;
                        _portfolios[i].UnrealizedPNL = updatePortfolioMessage.UnrealizedPNL;
                        _portfolios[i].RealizedPNL = updatePortfolioMessage.RealizedPNL;
                        return;
                    }
                }

                _portfolios.Add(updatePortfolioMessage);
            }
        }

        private void ibClient_HandleLastUpdateTime(UpdateAccountTimeMessage message)
        {
            _lastUpdateTime = message.Timestamp;
        }

        #endregion

        #region Positions

        public PositionMessage[] GetPositions()
        {
            lock (_positionLockObj)
            {
                if (_positionWait != null)
                {
                    try
                    {
                        _positionWait.Set();
                    }
                    catch (Exception)
                    {

                    }
                    _positionWait = null;
                }

                if (_positionWait == null)
                {
                    _positions.Clear();
                    _positionWait = new ManualResetEvent(false);
                    _ibClient.ClientSocket.reqPositions();

                    _positionWait.WaitOne(TIME_OUT_VALUE);
                }

                return _positions.ToArray();
            }
        }

        private void ibClient_HandlePosition(PositionMessage positionMessage)
        {
            for (int i = 0; i < _positions.Count; i++)
            {
                if (_positions[i].Account == positionMessage.Account && Utils.ContractToString(_positions[i].Contract) == Utils.ContractToString(positionMessage.Contract))
                {
                    _positions[i].Account = positionMessage.Account;
                    _positions[i].Position = positionMessage.Position;
                    _positions[i].AverageCost = positionMessage.AverageCost;
                    return;
                }
            }

            _positions.Add(positionMessage);
        }

        private void ibClient_HandlePositionEnd()
        {
            if (_positionWait != null)
            {
                _positionWait.Set();
                _positionWait = null;
            }
        }

        #endregion

        #region Error Handling
        void ibClient_Error(int id, int errorCode, string str, Exception ex)
        {
            if (ex != null)
            {
                _log.Error(_host + ":" + _port.ToString() + "|" + ex.Message);
                _log.Error(ex.StackTrace);
                return;
            }

            if (id == 0 || errorCode == 0)
            {
                _log.Error(_host + ":" + _port.ToString() + "|" + str);
                return;
            }

            ErrorMessage error = new ErrorMessage(id, errorCode, str);

            HandleErrorMessage(error);
        }

        private void HandleErrorMessage(ErrorMessage message)
        {
            _log.Error("Request " + message.RequestId + ", Code: " + message.ErrorCode + " - " + message.Message);
        }

        #endregion
    }
}
