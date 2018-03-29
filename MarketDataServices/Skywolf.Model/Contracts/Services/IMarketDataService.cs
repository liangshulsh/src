using System.ServiceModel;
using System.ServiceModel.Web;

namespace Skywolf.Contracts.Services
{
    [ServiceContract(Namespace = Constants.NAMESPACE)]
    public interface IMarketDataService
    {
        [OperationContract]
        [WebGet(UriTemplate = "test?quotedate={quoteDate}&betaname={betaName}")]
        string Test(string quoteDate, string betaName);
    }
}
