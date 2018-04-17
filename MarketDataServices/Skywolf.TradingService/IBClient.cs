using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Skywolf.IBApi;
using Skywolf.TradingService.Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Skywolf.TradingService
{
    public class IBClient : EWrapper
    {
        private EClientSocket clientSocket;
        private int nextOrderId;
        private int clientId;

        public Task<Contract> ResolveContractAsync(int conId, string refExch)
        {
            var reqId = new Random(DateTime.Now.Millisecond).Next();
            var resolveResult = new TaskCompletionSource<Contract>();
            var resolveContract_Error = new Action<int, int, string, Exception>((id, code, msg, ex) =>
            {
                if (reqId != id)
                    return;

                resolveResult.SetResult(null);
            });
            var resolveContract = new Action<ContractDetailsMessage>(msg =>
            {
                if (msg.RequestId == reqId)
                    resolveResult.SetResult(msg.ContractDetails.Summary);
            });
            var contractDetailsEnd = new Action<int>(id =>
            {
                if (reqId == id && !resolveResult.Task.IsCompleted)
                    resolveResult.SetResult(null);
            });

            var tmpError = Error;
            var tmpContractDetails = ContractDetails;
            var tmpContractDetailsEnd = ContractDetailsEnd;

            Error = resolveContract_Error;
            ContractDetails = resolveContract;
            ContractDetailsEnd = contractDetailsEnd;

            resolveResult.Task.ContinueWith(t =>
            {
                Error = tmpError;
                ContractDetails = tmpContractDetails;
                ContractDetailsEnd = tmpContractDetailsEnd;
            });

            ClientSocket.reqContractDetails(reqId, new Contract() { ConId = conId, Exchange = refExch });

            return resolveResult.Task;
        }

        public Task<Contract[]> ResolveContractAsync(string secType, string symbol, string currency, string exchange)
        {
            var reqId = new Random(DateTime.Now.Millisecond).Next();
            var res = new TaskCompletionSource<Contract[]>();
            var contractList = new List<Contract>();
            var resolveContract_Error = new Action<int, int, string, Exception>((id, code, msg, ex) =>
            {
                if (reqId != id)
                    return;

                res.SetResult(new Contract[0]);
            });
            var contractDetails = new Action<ContractDetailsMessage>(msg =>
            {
                if (reqId != msg.RequestId)
                    return;

                contractList.Add(msg.ContractDetails.Summary);
            });
            var contractDetailsEnd = new Action<int>(id =>
            {
                if (reqId == id)
                    res.SetResult(contractList.ToArray());
            });

            var tmpError = Error;
            var tmpContractDetails = ContractDetails;
            var tmpContractDetailsEnd = ContractDetailsEnd;

            Error = resolveContract_Error;
            ContractDetails = contractDetails;
            ContractDetailsEnd = contractDetailsEnd;

            res.Task.ContinueWith(t =>
            {
                Error = tmpError;
                ContractDetails = tmpContractDetails;
                ContractDetailsEnd = tmpContractDetailsEnd;
            });

            ClientSocket.reqContractDetails(reqId, new Contract() { SecType = secType, Symbol = symbol, Currency = currency, Exchange = exchange });

            return res.Task;
        }

        public int ClientId
        {
            get { return clientId; }
            set { clientId = value; }
        }

        public IBClient(EReaderSignal signal)
        {
            clientSocket = new EClientSocket(this, signal);
        }

        public EClientSocket ClientSocket
        {
            get { return clientSocket; }
            private set { clientSocket = value; }
        }

        public int NextOrderId
        {
            get { return nextOrderId; }
            set { nextOrderId = value; }
        }

        public event Action<int, int, string, Exception> Error;

        void EWrapper.error(Exception e)
        {
            var tmp = Error;

            if (tmp != null)
                tmp(0, 0, null, e);
        }

        void EWrapper.error(string str)
        {
            var tmp = Error;

            if (tmp != null)
                tmp(0, 0, str, null);
        }

        void EWrapper.error(int id, int errorCode, string errorMsg)
        {
            var tmp = Error;

            if (tmp != null)
                tmp(id, errorCode, errorMsg, null);
        }

        public event Action ConnectionClosed;

        void EWrapper.connectionClosed()
        {
            var tmp = ConnectionClosed;

            if (tmp != null)
                tmp();
        }

        public event Action<long> CurrentTime;

        void EWrapper.currentTime(long time)
        {
            var tmp = CurrentTime;

            if (tmp != null)
                tmp(time);
        }

        public event Action<TickPriceMessage> TickPrice;

        void EWrapper.tickPrice(int tickerId, int field, double price, TickAttrib attribs)
        {
            var tmp = TickPrice;

            if (tmp != null)
                tmp(new TickPriceMessage(tickerId, field, price, attribs));
        }

        public event Action<TickSizeMessage> TickSize;

        void EWrapper.tickSize(int tickerId, int field, int size)
        {
            var tmp = TickSize;

            if (tmp != null)
                tmp(new TickSizeMessage(tickerId, field, size));
        }

        public event Action<int, int, string> TickString;

        void EWrapper.tickString(int tickerId, int tickType, string value)
        {
            var tmp = TickString;

            if (tmp != null)
                tmp(tickerId, tickType, value);
        }

        public event Action<int, int, double> TickGeneric;

        void EWrapper.tickGeneric(int tickerId, int field, double value)
        {
            var tmp = TickGeneric;

            if (tmp != null)
                tmp(tickerId, field, value);
        }

        public event Action<int, int, double, string, double, int, string, double, double> TickEFP;

        void EWrapper.tickEFP(int tickerId, int tickType, double basisPoints, string formattedBasisPoints, double impliedFuture, int holdDays, string futureLastTradeDate, double dividendImpact, double dividendsToLastTradeDate)
        {
            var tmp = TickEFP;

            if (tmp != null)
                tmp(tickerId, tickType, basisPoints, formattedBasisPoints, impliedFuture, holdDays, futureLastTradeDate, dividendImpact, dividendsToLastTradeDate);
        }

        public event Action<int> TickSnapshotEnd;

        void EWrapper.tickSnapshotEnd(int tickerId)
        {
            var tmp = TickSnapshotEnd;

            if (tmp != null)
                tmp(tickerId);
        }

        public event Action<ConnectionStatusMessage> NextValidId;

        void EWrapper.nextValidId(int orderId)
        {
            var tmp = NextValidId;

            if (tmp != null)
                tmp(new ConnectionStatusMessage(true));

            NextOrderId = orderId;
        }

        public event Action<int, UnderComp> DeltaNeutralValidation;

        void EWrapper.deltaNeutralValidation(int reqId, UnderComp underComp)
        {
            var tmp = DeltaNeutralValidation;

            if (tmp != null)
                tmp(reqId, underComp);
        }

        public event Action<ManagedAccountsMessage> ManagedAccounts;

        void EWrapper.managedAccounts(string accountsList)
        {
            var tmp = ManagedAccounts;

            if (tmp != null)
                tmp(new ManagedAccountsMessage(accountsList));
        }

        public event Action<TickOptionMessage> TickOptionCommunication;

        void EWrapper.tickOptionComputation(int tickerId, int field, double impliedVolatility, double delta, double optPrice, double pvDividend, double gamma, double vega, double theta, double undPrice)
        {
            var tmp = TickOptionCommunication;

            if (tmp != null)
                tmp(new TickOptionMessage(tickerId, field, impliedVolatility, delta, optPrice, pvDividend, gamma, vega, theta, undPrice));
        }

        public event Action<AccountSummaryMessage> AccountSummary;

        void EWrapper.accountSummary(int reqId, string account, string tag, string value, string currency)
        {
            var tmp = AccountSummary;

            if (tmp != null)
                tmp(new AccountSummaryMessage(reqId, account, tag, value, currency));
        }

        public event Action<AccountSummaryEndMessage> AccountSummaryEnd;

        void EWrapper.accountSummaryEnd(int reqId)
        {
            var tmp = AccountSummaryEnd;

            if (tmp != null)
                tmp(new AccountSummaryEndMessage(reqId));
        }

        public event Action<AccountValueMessage> UpdateAccountValue;

        void EWrapper.updateAccountValue(string key, string value, string currency, string accountName)
        {
            var tmp = UpdateAccountValue;

            if (tmp != null)
                tmp(new AccountValueMessage(key, value, currency, accountName));
        }

        public event Action<UpdatePortfolioMessage> UpdatePortfolio;

        void EWrapper.updatePortfolio(Contract contract, double position, double marketPrice, double marketValue, double averageCost, double unrealizedPNL, double realizedPNL, string accountName)
        {
            var tmp = UpdatePortfolio;

            if (tmp != null)
                tmp(new UpdatePortfolioMessage(contract, position, marketPrice, marketValue, averageCost, unrealizedPNL, realizedPNL, accountName));
        }

        public event Action<UpdateAccountTimeMessage> UpdateAccountTime;

        void EWrapper.updateAccountTime(string timestamp)
        {
            var tmp = UpdateAccountTime;

            if (tmp != null)
                tmp(new UpdateAccountTimeMessage(timestamp));
        }

        public event Action<AccountDownloadEndMessage> AccountDownloadEnd;

        void EWrapper.accountDownloadEnd(string account)
        {
            var tmp = AccountDownloadEnd;

            if (tmp != null)
                tmp(new AccountDownloadEndMessage(account));
        }

        public event Action<OrderStatusMessage> OrderStatus;

        void EWrapper.orderStatus(int orderId, string status, double filled, double remaining, double avgFillPrice, int permId, int parentId, double lastFillPrice, int clientId, string whyHeld)
        {
            var tmp = OrderStatus;

            if (tmp != null)
                tmp(new OrderStatusMessage(orderId, status, filled, remaining, avgFillPrice, permId, parentId, lastFillPrice, clientId, whyHeld));
        }

        public event Action<OpenOrderMessage> OpenOrder;

        void EWrapper.openOrder(int orderId, Contract contract, Order order, OrderState orderState)
        {
            var tmp = OpenOrder;

            if (tmp != null)
                tmp(new OpenOrderMessage(orderId, contract, order, orderState));
        }

        public event Action OpenOrderEnd;

        void EWrapper.openOrderEnd()
        {
            var tmp = OpenOrderEnd;

            if (tmp != null)
                tmp();
        }

        public event Action<ContractDetailsMessage> ContractDetails;

        void EWrapper.contractDetails(int reqId, ContractDetails contractDetails)
        {
            var tmp = ContractDetails;

            if (tmp != null)
                tmp(new ContractDetailsMessage(reqId, contractDetails));
        }

        public event Action<int> ContractDetailsEnd;

        void EWrapper.contractDetailsEnd(int reqId)
        {
            var tmp = ContractDetailsEnd;

            if (tmp != null)
                tmp(reqId);
        }

        public event Action<ExecutionMessage> ExecDetails;

        void EWrapper.execDetails(int reqId, Contract contract, Execution execution)
        {
            var tmp = ExecDetails;

            if (tmp != null)
                tmp(new ExecutionMessage(reqId, contract, execution));
        }

        public event Action<int> ExecDetailsEnd;

        void EWrapper.execDetailsEnd(int reqId)
        {
            var tmp = ExecDetailsEnd;

            if (tmp != null)
                tmp(reqId);
        }

        public event Action<CommissionReport> CommissionReport;

        void EWrapper.commissionReport(CommissionReport commissionReport)
        {
            var tmp = CommissionReport;

            if (tmp != null)
                tmp(commissionReport);
        }

        public event Action<FundamentalsMessage> FundamentalData;

        void EWrapper.fundamentalData(int reqId, string data)
        {
            var tmp = FundamentalData;

            if (tmp != null)
                tmp(new FundamentalsMessage(data));
        }

        public event Action<HistoricalDataMessage> HistoricalData;

        void EWrapper.historicalData(int reqId, Bar bar)
        {
            var tmp = HistoricalData;

            if (tmp != null)
                tmp(new HistoricalDataMessage(reqId, bar));
        }

        public event Action<HistoricalDataEndMessage> HistoricalDataEnd;

        void EWrapper.historicalDataEnd(int reqId, string startDate, string endDate)
        {
            var tmp = HistoricalDataEnd;

            if (tmp != null)
                tmp(new HistoricalDataEndMessage(reqId, startDate, endDate));
        }

        public event Action<MarketDataTypeMessage> MarketDataType;

        void EWrapper.marketDataType(int reqId, int marketDataType)
        {
            var tmp = MarketDataType;

            if (tmp != null)
                tmp(new MarketDataTypeMessage(reqId, marketDataType));
        }

        public event Action<DeepBookMessage> UpdateMktDepth;

        void EWrapper.updateMktDepth(int tickerId, int position, int operation, int side, double price, int size)
        {
            var tmp = UpdateMktDepth;

            if (tmp != null)
                tmp(new DeepBookMessage(tickerId, position, operation, side, price, size, ""));
        }

        public event Action<DeepBookMessage> UpdateMktDepthL2;

        void EWrapper.updateMktDepthL2(int tickerId, int position, string marketMaker, int operation, int side, double price, int size)
        {
            var tmp = UpdateMktDepthL2;

            if (tmp != null)
                tmp(new DeepBookMessage(tickerId, position, operation, side, price, size, marketMaker));
        }

        public event Action<int, int, String, String> UpdateNewsBulletin;

        void EWrapper.updateNewsBulletin(int msgId, int msgType, String message, String origExchange)
        {
            var tmp = UpdateNewsBulletin;

            if (tmp != null)
                tmp(msgId, msgType, message, origExchange);
        }

        public event Action<PositionMessage> Position;

        void EWrapper.position(string account, Contract contract, double pos, double avgCost)
        {
            var tmp = Position;

            if (tmp != null)
                tmp(new PositionMessage(account, contract, pos, avgCost));
        }

        public event Action PositionEnd;

        void EWrapper.positionEnd()
        {
            var tmp = PositionEnd;

            if (tmp != null)
                tmp();
        }

        public event Action<RealTimeBarMessage> RealtimeBar;

        void EWrapper.realtimeBar(int reqId, long time, double open, double high, double low, double close, long volume, double WAP, int count)
        {
            var tmp = RealtimeBar;

            if (tmp != null)
                tmp(new RealTimeBarMessage(reqId, time, open, high, low, close, volume, WAP, count));
        }

        public event Action<string> ScannerParameters;

        void EWrapper.scannerParameters(string xml)
        {
            var tmp = ScannerParameters;

            if (tmp != null)
                tmp(xml);
        }

        public event Action<ScannerMessage> ScannerData;

        void EWrapper.scannerData(int reqId, int rank, ContractDetails contractDetails, string distance, string benchmark, string projection, string legsStr)
        {
            var tmp = ScannerData;

            if (tmp != null)
                tmp(new ScannerMessage(reqId, rank, contractDetails, distance, benchmark, projection, legsStr));
        }

        public event Action<int> ScannerDataEnd;

        void EWrapper.scannerDataEnd(int reqId)
        {
            var tmp = ScannerDataEnd;

            if (tmp != null)
                tmp(reqId);
        }

        public event Action<AdvisorDataMessage> ReceiveFA;

        void EWrapper.receiveFA(int faDataType, string faXmlData)
        {
            var tmp = ReceiveFA;

            if (tmp != null)
                tmp(new AdvisorDataMessage(faDataType, faXmlData));
        }

        public event Action<BondContractDetailsMessage> BondContractDetails;

        void EWrapper.bondContractDetails(int requestId, ContractDetails contractDetails)
        {
            var tmp = BondContractDetails;

            if (tmp != null)
                tmp(new BondContractDetailsMessage(requestId, contractDetails));
        }

        public event Action<string> VerifyMessageAPI;

        void EWrapper.verifyMessageAPI(string apiData)
        {
            var tmp = VerifyMessageAPI;

            if (tmp != null)
                tmp(apiData);
        }
        public event Action<bool, string> VerifyCompleted;

        void EWrapper.verifyCompleted(bool isSuccessful, string errorText)
        {
            var tmp = VerifyCompleted;

            if (tmp != null)
                tmp(isSuccessful, errorText);
        }

        public event Action<string, string> VerifyAndAuthMessageAPI;

        void EWrapper.verifyAndAuthMessageAPI(string apiData, string xyzChallenge)
        {
            var tmp = VerifyAndAuthMessageAPI;

            if (tmp != null)
                tmp(apiData, xyzChallenge);
        }

        public event Action<bool, string> VerifyAndAuthCompleted;

        void EWrapper.verifyAndAuthCompleted(bool isSuccessful, string errorText)
        {
            var tmp = VerifyAndAuthCompleted;

            if (tmp != null)
                tmp(isSuccessful, errorText);
        }

        public event Action<int, string> DisplayGroupList;

        void EWrapper.displayGroupList(int reqId, string groups)
        {
            var tmp = DisplayGroupList;

            if (tmp != null)
                tmp(reqId, groups);
        }

        public event Action<int, string> DisplayGroupUpdated;

        void EWrapper.displayGroupUpdated(int reqId, string contractInfo)
        {
            var tmp = DisplayGroupUpdated;

            if (tmp != null)
                tmp(reqId, contractInfo);
        }


        void EWrapper.connectAck()
        {
            if (ClientSocket.AsyncEConnect)
                ClientSocket.startApi();
        }

        public event Action<PositionMultiMessage> PositionMulti;

        void EWrapper.positionMulti(int reqId, string account, string modelCode, Contract contract, double pos, double avgCost)
        {
            var tmp = PositionMulti;

            if (tmp != null)
                tmp(new PositionMultiMessage(reqId, account, modelCode, contract, pos, avgCost));
        }

        public event Action<int> PositionMultiEnd;

        void EWrapper.positionMultiEnd(int reqId)
        {
            var tmp = PositionMultiEnd;

            if (tmp != null)
                tmp(reqId);
        }

        public event Action<AccountUpdateMultiMessage> AccountUpdateMulti;

        void EWrapper.accountUpdateMulti(int reqId, string account, string modelCode, string key, string value, string currency)
        {
            var tmp = AccountUpdateMulti;

            if (tmp != null)
                tmp(new AccountUpdateMultiMessage(reqId, account, modelCode, key, value, currency));
        }

        public event Action<int> AccountUpdateMultiEnd;

        void EWrapper.accountUpdateMultiEnd(int reqId)
        {
            var tmp = AccountUpdateMultiEnd;

            if (tmp != null)
                tmp(reqId);
        }

        public event Action<SecurityDefinitionOptionParameterMessage> SecurityDefinitionOptionParameter;

        void EWrapper.securityDefinitionOptionParameter(int reqId, string exchange, int underlyingConId, string tradingClass, string multiplier, HashSet<string> expirations, HashSet<double> strikes)
        {
            var tmp = SecurityDefinitionOptionParameter;

            if (tmp != null)
                tmp(new SecurityDefinitionOptionParameterMessage(reqId, exchange, underlyingConId, tradingClass, multiplier, expirations, strikes));
        }

        public event Action<int> SecurityDefinitionOptionParameterEnd;

        void EWrapper.securityDefinitionOptionParameterEnd(int reqId)
        {
            var tmp = SecurityDefinitionOptionParameterEnd;

            if (tmp != null)
                tmp(reqId);
        }

        public event Action<SoftDollarTiersMessage> SoftDollarTiers;

        void EWrapper.softDollarTiers(int reqId, SoftDollarTier[] tiers)
        {
            var tmp = SoftDollarTiers;

            if (tmp != null)
                tmp(new SoftDollarTiersMessage(reqId, tiers));
        }

        public event Action<FamilyCode[]> FamilyCodes;

        void EWrapper.familyCodes(FamilyCode[] familyCodes)
        {
            var tmp = FamilyCodes;

            if (tmp != null)
                tmp(familyCodes);
        }

        public event Action<SymbolSamplesMessage> SymbolSamples;

        void EWrapper.symbolSamples(int reqId, ContractDescription[] contractDescriptions)
        {
            var tmp = SymbolSamples;

            if (tmp != null)
                tmp(new SymbolSamplesMessage(reqId, contractDescriptions));
        }


        public event Action<DepthMktDataDescription[]> MktDepthExchanges;

        void EWrapper.mktDepthExchanges(DepthMktDataDescription[] depthMktDataDescriptions)
        {
            var tmp = MktDepthExchanges;

            if (tmp != null)
                tmp(depthMktDataDescriptions);
        }

        public event Action<TickNewsMessage> TickNews;

        void EWrapper.tickNews(int tickerId, long timeStamp, string providerCode, string articleId, string headline, string extraData)
        {
            var tmp = TickNews;

            if (tmp != null)
                tmp(new TickNewsMessage(tickerId, timeStamp, providerCode, articleId, headline, extraData));
        }

        public event Action<int, Dictionary<int, KeyValuePair<string, char>>> SmartComponents;

        void EWrapper.smartComponents(int reqId, Dictionary<int, KeyValuePair<string, char>> theMap)
        {
            var tmp = SmartComponents;

            if (tmp != null)
                tmp(reqId, theMap);
        }

        public event Action<TickReqParamsMessage> TickReqParams;

        void EWrapper.tickReqParams(int tickerId, double minTick, string bboExchange, int snapshotPermissions)
        {
            var tmp = TickReqParams;

            if (tmp != null)
                tmp(new TickReqParamsMessage(tickerId, minTick, bboExchange, snapshotPermissions));
        }

        public event Action<NewsProvider[]> NewsProviders;

        void EWrapper.newsProviders(NewsProvider[] newsProviders)
        {
            var tmp = NewsProviders;

            if (tmp != null)
                tmp(newsProviders);
        }

        public event Action<NewsArticleMessage> NewsArticle;

        void EWrapper.newsArticle(int requestId, int articleType, string articleText)
        {
            var tmp = NewsArticle;

            if (tmp != null)
                tmp(new NewsArticleMessage(requestId, articleType, articleText));
        }

        public event Action<HistoricalNewsMessage> HistoricalNews;

        void EWrapper.historicalNews(int requestId, string time, string providerCode, string articleId, string headline)
        {
            var tmp = HistoricalNews;

            if (tmp != null)
                tmp(new HistoricalNewsMessage(requestId, time, providerCode, articleId, headline));
        }

        public event Action<HistoricalNewsEndMessage> HistoricalNewsEnd;

        void EWrapper.historicalNewsEnd(int requestId, bool hasMore)
        {
            var tmp = HistoricalNewsEnd;

            if (tmp != null)
                tmp(new HistoricalNewsEndMessage(requestId, hasMore));
        }

        public event Action<HeadTimestampMessage> HeadTimestamp;

        void EWrapper.headTimestamp(int reqId, string headTimestamp)
        {
            var tmp = HeadTimestamp;

            if (tmp != null)
                tmp(new HeadTimestampMessage(reqId, headTimestamp));
        }

        public event Action<HistogramDataMessage> HistogramData;

        void EWrapper.histogramData(int reqId, HistogramEntry[] data)
        {
            var tmp = HistogramData;

            if (tmp != null)
                tmp(new HistogramDataMessage(reqId, data));
        }

        public event Action<HistoricalDataMessage> HistoricalDataUpdate;

        void EWrapper.historicalDataUpdate(int reqId, Bar bar)
        {
            var tmp = HistoricalDataUpdate;

            if (tmp != null)
                tmp(new HistoricalDataMessage(reqId, bar));
        }

        public event Action<int, int, string> RerouteMktDataReq;

        void EWrapper.rerouteMktDataReq(int reqId, int conId, string exchange)
        {
            var tmp = RerouteMktDataReq;

            if (tmp != null)
                tmp(reqId, conId, exchange);
        }

        public event Action<int, int, string> RerouteMktDepthReq;

        void EWrapper.rerouteMktDepthReq(int reqId, int conId, string exchange)
        {
            var tmp = RerouteMktDepthReq;

            if (tmp != null)
                tmp(reqId, conId, exchange);
        }

        public event Action<MarketRuleMessage> MarketRule;

        void EWrapper.marketRule(int marketRuleId, PriceIncrement[] priceIncrements)
        {
            var tmp = MarketRule;

            if (tmp != null)
                tmp(new MarketRuleMessage(marketRuleId, priceIncrements));
        }

        public event Action<PnLMessage> pnl;

        void EWrapper.pnl(int reqId, double dailyPnL, double unrealizedPnL)
        {
            var tmp = pnl;

            if (tmp != null)
                tmp(new PnLMessage(reqId, dailyPnL, unrealizedPnL));
        }

        public event Action<PnLSingleMessage> pnlSingle;

        void EWrapper.pnlSingle(int reqId, int pos, double dailyPnL, double unrealizedPnL, double value)
        {
            var tmp = pnlSingle;

            if (tmp != null)
                tmp(new PnLSingleMessage(reqId, pos, dailyPnL, unrealizedPnL, value));
        }
    }
}
