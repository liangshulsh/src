using System;
using System.Runtime.Serialization;

namespace Skywolf.Contracts.DataContracts.MarketData.TVC
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class TVCCalendar
    {
        [DataMember]
        public DateTime AsOfDate { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Exchange { get; set; }

        [DataMember]
        public string Holiday { get; set; }

        [DataMember]
        public string EarlyClose { get; set; }
    }
}
