using System;
using System.Runtime.Serialization;

namespace Skywolf.Contracts.DataContracts.MarketData.TVC
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    [KnownType(typeof(TVCHistoryResponse))]
    [KnownType(typeof(TVCQuotesResponse))]
    [KnownType(typeof(TVCSymbolResponse))]
    [KnownType(typeof(TVCQuoteResponse))]
    public class TVCResponse
    {
        [DataMember]
        public string s { get; set; }
    }
}
