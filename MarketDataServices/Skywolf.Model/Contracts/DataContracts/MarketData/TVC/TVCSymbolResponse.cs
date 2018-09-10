using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace Skywolf.Contracts.DataContracts.MarketData.TVC
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class TVCSymbolResponse : TVCResponse
    {
        [DataMember]
        public string name { get; set; }

        [DataMember(Name = "exchange-traded")]
        public string exchange_traded { get; set; }

        [DataMember(Name = "exchange-listed")]
        public string exchange_listed { get; set; }

        [DataMember]
        public string timezone { get; set; }

        [DataMember]
        public double minmov { get; set; }

        [DataMember]
        public double minmov2 { get; set; }

        [DataMember]
        public double pricescale { get; set; }

        [DataMember]
        public double pointvalue { get; set; }

        [DataMember]
        public bool has_intraday { get; set; }

        [DataMember]
        public bool has_no_volume { get; set; }

        [DataMember]
        public double volume_precision { get; set; }

        [DataMember]
        public string ticker { get; set; }

        [DataMember]
        public string description { get; set; }

        [DataMember]
        public string type { get; set; }

        [DataMember]
        public bool has_daily { get; set; }

        [DataMember]
        public bool has_weekly_and_monthly { get; set; }

        [DataMember]
        public List<string> supported_resolutions { get; set; }

        [DataMember]
        public List<string> intraday_multipliers { get; set; }

        [DataMember]
        public string session { get; set; }
        
        [DataMember]
        public string data_status { get; set; }
    }
}
