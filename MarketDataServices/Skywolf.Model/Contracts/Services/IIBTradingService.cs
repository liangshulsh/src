using Skywolf.Contracts.DataContracts.MarketData;
using Skywolf.Contracts.DataContracts.Instrument;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace Skywolf.Contracts.Services
{
    [ServiceContract(Namespace = Constants.NAMESPACE)]
    public interface IIBTradingService
    {
    }
}
