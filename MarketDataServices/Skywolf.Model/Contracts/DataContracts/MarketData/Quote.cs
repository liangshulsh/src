using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.MarketData
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class Quote
    {
        [DataMember]
        public string Symbol { get; set; }

        [DataMember]
        public double Price { get; set; }

        [DataMember]
        public double Bid { get; set; }

        [DataMember]
        public double Ask { get; set; }

        [DataMember]
        public double Volume { get; set; }

        [DataMember]
        public DateTime TimeStamp { get; set; }

        [DataMember]
        public string TimeZone { get; set; }
    }
}
