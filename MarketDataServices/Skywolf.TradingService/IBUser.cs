using log4net;
using Skywolf.IBApi;
using Skywolf.TradingService.Messages;
using System;
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

        private EventWaitHandle _accountSummaryWait = null;
        private object _accountSummaryLockObj = new object();
        private List<AccountSummaryMessage> _accountSummary = new List<AccountSummaryMessage>();

        private List<AccountValueMessage> _accountValues = new List<AccountValueMessage>();
        private List<UpdatePortfolioMessage> _portfolios = new List<UpdatePortfolioMessage>();

        private List<PositionMessage> _positions = new List<PositionMessage>();

        private string _lastUpdateTime = string.Empty;

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

        }

        private void ibClient_HandleLastUpdateTime(UpdateAccountTimeMessage message)
        {
            _lastUpdateTime = message.Timestamp;
        }

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

        private void ibClient_HandleAccountValue(AccountValueMessage accountValueMessage)
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

        private void ibClient_HandlePortfolioValue(UpdatePortfolioMessage updatePortfolioMessage)
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

        }

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
