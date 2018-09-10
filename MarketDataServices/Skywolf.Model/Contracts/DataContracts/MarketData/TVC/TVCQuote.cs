using System;
using System.Runtime.Serialization;

namespace Skywolf.Contracts.DataContracts.MarketData.TVC
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class TVCQuote
    {
        [DataMember]
        public string ch { get; set; }

        [DataMember]
        public string chp { get; set; }

        [DataMember]
        public string short_name { get; set; }

        [DataMember]
        public string exchange { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public string lp { get; set; }

        [DataMember]
        public string ask { get; set; }

        [DataMember]
        public string bid { get; set; }

        [DataMember]
        public double? spread { get; set; }

        [DataMember]
        public string open_price { get; set; }

        [DataMember]
        public string high_price { get; set; }

        [DataMember]
        public string low_price { get; set; }

        [DataMember]
        public string prev_close_price { get; set; }

        [DataMember]
        public string volume { get; set; }
    }
}
