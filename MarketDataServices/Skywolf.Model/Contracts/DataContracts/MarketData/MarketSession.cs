using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.MarketData
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class MarketSession
    {
        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public DateTime OpenTime { get; set; }

        [DataMember]
        public DateTime CloseTime { get; set; }
    }
}
