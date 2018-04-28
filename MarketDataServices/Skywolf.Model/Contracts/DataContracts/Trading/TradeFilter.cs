using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.Trading
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class TradeFilter
    {
        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public string AcctCode { get; set; }

        [DataMember]
        public string Time { get; set; }

        [DataMember]
        public string Symbol { get; set; }

        [DataMember]
        public string SecType { get; set; }

        [DataMember]
        public string Exchange { get; set; }

        [DataMember]
        public string Side { get; set; }
    }
}
