using System;
using System.Runtime.Serialization;

namespace Skywolf.Contracts.DataContracts.MarketData.TVC
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class TVCQuoteResponse : TVCResponse
    {
        [DataMember]
        public string n { get; set; }

        [DataMember]
        public TVCQuote v { get; set; }
    }
}
