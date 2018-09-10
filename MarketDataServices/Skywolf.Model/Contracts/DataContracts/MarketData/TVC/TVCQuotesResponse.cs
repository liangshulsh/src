using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Skywolf.Contracts.DataContracts.MarketData.TVC
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class TVCQuotesResponse : TVCResponse
    {
        [DataMember]
        public List<TVCQuoteResponse> d { get; set; }
    }
}
