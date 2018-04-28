using Skywolf.Contracts.DataContracts.MarketData;
using Skywolf.Contracts.DataContracts.Instrument;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Skywolf.Contracts.DataContracts.Trading;

namespace Skywolf.Contracts.Services
{
    [ServiceContract(Namespace = Constants.NAMESPACE)]
    public interface IIBTradingService
    {
        [OperationContract]
        bool CreateUser(string username, string account, string host, int port);

        [OperationContract]
        void RemoveUser(string username);

        [OperationContract]
        int PlaceSimpleOrder(SimpleOrder simpleOrder);

        [OperationContract]
        bool CancelOrder(string userName, int orderId);

        [OperationContract]
        Order GetOpenOrder(string userName, int orderId);

        [OperationContract]
        Order[] GetAllOpenOrders(string userName);

        [OperationContract]
        Order[] RefreshAllOpenOrders(string userName);

        [OperationContract]
        AccountSummary[] GetAccountSummary(string userName);

        [OperationContract]
        bool UnsubscribeAccountUpdates(string userName);

        [OperationContract]
        bool SubscribeAccountUpdates(string userName, string account);

        [OperationContract]
        PositionPortfolio[] GetPortfolios(string userName);

        [OperationContract]
        Trade[] GetAllTrades(string userName);

        [OperationContract]
        Trade[] FilterTrades(string userName, TradeFilter tradeFilter);

        [OperationContract]
        string GetAccountUpdateTime(string userName);

        [OperationContract]
        Position[] GetPositions(string userName);
    }
}
