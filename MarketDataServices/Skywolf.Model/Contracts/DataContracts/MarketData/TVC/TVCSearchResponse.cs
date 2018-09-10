using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Skywolf.Contracts.DataContracts.MarketData.TVC
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class TVCSearchResponse : TVCResponse
    {
        [DataMember]
        public string symbol { get; set; }

        [DataMember]
        public string ticker { get; set; }

        [DataMember]
        public string type { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public string exchange { get; set; }

        [DataMember]
        public string full_name { get; set; }
    }
}
