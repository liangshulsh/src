using System.ServiceModel;
using System.ServiceModel.Web;

namespace Skywolf.Contracts.Services.Restful
{
    [ServiceContract(Namespace = Constants.NAMESPACE)]
    public interface IMarketDataRfService
    {
        [OperationContract]
        [WebGet(UriTemplate = "getsidfromname?names={names}")]
        string GetSIDFromName(string names);

        [OperationContract]
        [WebGet(UriTemplate = "getnamefromsid?sids={sids}")]
        string GetNameFromSID(string sids);

        [OperationContract]
        [WebGet(UriTemplate = "gettimeseriesdata?symbol={symbol}&market={market}&frequency={frequency}&outputcount={outputcount}&isadjustedvalue={isadjustedvalue}&datasource={datasource}")]
        string GetTimeSeriesData(string symbol, string market, string frequency, string outputcount, string isadjustedvalue, string datasource);

        [OperationContract]
        [WebGet(UriTemplate = "getstockbatchquote?symbols={symbols}&datasource={datasource}")]
        string GetStockBatchQuote(string symbols, string datasource);

        [OperationContract]
        [WebGet(UriTemplate = "getstockhistoryprices?symbols={symbols}&frequency={frequency}&startdate={startdate}&enddate={enddate}&outputcount={outputcount}&isadjustedvalue={isadjustedvalue}&datasource={datasource}")]
        string GetStockHistoryPrices(string symbols, string frequency, string startdate, string enddate, string outputcount, string isadjustedvalue, string datasource);

        [OperationContract]
        [WebGet(UriTemplate = "getcryptohistoryprices?symbols={symbols}&market={market}&frequency={frequency}&startdate={startdate}&enddate={enddate}&outputcount={outputcount}&datasource={datasource}")]
        string GetCryptoHistoryPrices(string symbols, string market, string frequency, string startdate, string enddate, string outputcount, string datasource);

        [OperationContract]
        [WebGet(UriTemplate = "getlateststockhistoryprices?symbols={symbols}&frequency={frequency}&isadjustedvalue={isadjustedvalue}&datasource={datasource}")]
        string GetLatestStockHistoryPrices(string symbols, string frequency, string isadjustedvalue, string datasource);

        [OperationContract]
        [WebGet(UriTemplate = "getlatestcryptohistoryprices?symbols={symbols}&market={market}&frequency={frequency}&datasource={datasource}")]
        string GetLatestCryptoHistoryPrices(string symbols, string market, string frequency, string datasource);

        [OperationContract]
        [WebGet(UriTemplate = "va_getavailableapikey")]
        string VA_GetAvailableAPIKey();

        [OperationContract]
        [WebGet(UriTemplate = "va_addapikeys?apikeys={apikeys}")]
        string AV_AddAPIKeys(string apikeys);

        [OperationContract]
        [WebGet(UriTemplate = "va_removeapikeys?apikeys={apikeys}")]
        string AV_RemoveAPIKeys(string apikeys);

        [OperationContract]
        [WebGet(UriTemplate = "va_updateapikeys?apikeys={apikeys}")]
        string AV_UpdateAPIKeys(string apikeys);
    }
}
