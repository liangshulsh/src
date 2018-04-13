using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.Instrument
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class PricingRule
    {
        [DataMember]
        public long SID { get; set; }

        [DataMember]
        public DateTime AsOfDate { get; set; }

        [DataMember]
        public string Ticker { get; set; }

        [DataMember]
        public string DataSource { get; set; }

        [DataMember]
        public string TimeZone { get; set; }

        [DataMember]
        public bool Active { get; set; }

        [DataMember]
        public int Priority { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public DateTime? TS { get; set; }
    }
}
