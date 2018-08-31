using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.Services.Restful
{
    [ServiceContract(Namespace = Constants.NAMESPACE)]
    public interface IIBTradingRfService
    {
        [OperationContract]
        [WebGet(UriTemplate = "createuser?username={username}&account={account}&host={host}&port={port}")]
        string CreateUser(string username, string account, string host, string port);

        [OperationContract]
        [WebGet(UriTemplate = "removeuser?username={username}")]
        string RemoveUser(string username);

        [OperationContract]
        [WebGet(UriTemplate = "placesimpleorder?username={username}&orderid={orderid}&securitytype={securitytype}&symbol={symbol}&currency={currency}&quantity={quantity}&ordertype={ordertype}&action={action}&limitprice={limitprice}&stopprice={stopprice}&fund={fund}&strategy={strategy}&folder={folder}")]
        string PlaceSimpleOrder(string username, string orderid, string securitytype, string symbol, string currency, string quantity, string ordertype, string action, string limitprice, string stopprice, string fund, string strategy, string folder);

        [OperationContract]
        [WebGet(UriTemplate = "cancelorder?username={username}&orderid={orderid}")]
        string CancelOrder(string username, string orderId);

        [OperationContract]
        [WebGet(UriTemplate = "getopenorder?username={username}&orderid={orderid}")]
        string GetOpenOrder(string username, string orderId);

        [OperationContract]
        [WebGet(UriTemplate = "getallopenorders?username={username}")]
        string GetAllOpenOrders(string username);

        [OperationContract]
        [WebGet(UriTemplate = "refreshallopenorders?username={username}")]
        string RefreshAllOpenOrders(string username);

        [OperationContract]
        [WebGet(UriTemplate = "getaccountsummary?username={username}")]
        string GetAccountSummary(string username);

        [OperationContract]
        [WebGet(UriTemplate = "unsubscribeaccountupdates?username={username}")]
        string UnsubscribeAccountUpdates(string username);

        [OperationContract]
        [WebGet(UriTemplate = "subscribeaccountupdates?username={username}&account={account}")]
        string SubscribeAccountUpdates(string username, string account);

        [OperationContract]
        [WebGet(UriTemplate = "getportfolios?username={username}")]
        string GetPortfolios(string username);

        [OperationContract]
        [WebGet(UriTemplate = "getalltrades?username={username}")]
        string GetAllTrades(string username);

        [OperationContract]
        [WebGet(UriTemplate = "filtertrades?username={username}&clientid={clientid}&acctcode={acctcode}&symbol={symbol}&sectype={sectype}&time={time}&exchange={exchange}&side={side}")]
        string FilterTrades(string username, string clientid, string acctcode, string symbol, string sectype, string time, string exchange, string side);

        [OperationContract]
        [WebGet(UriTemplate = "getaccountupdatetime?username={username}")]
        string GetAccountUpdateTime(string username);

        [OperationContract]
        [WebGet(UriTemplate = "getpositions?username={username}")]
        string GetPositions(string username);
    }
}
