using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.Trading
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class Contract
    {
        [DataMember]
        public long SID { get; set; }

        [DataMember]
        public int ContractId { get; set; }

        [DataMember]
        public string Symbol { get; set; }

        [DataMember]
        public string SecType { get; set; }

        [DataMember]
        public string LastTradeDateOrContractMonth { get; set; }

        [DataMember]
        public double Strike { get; set; }

        [DataMember]
        public string Right { get; set; }

        [DataMember]
        public string Multiplier { get; set; }

        [DataMember]
        public string Exchange { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public string LocalSymbol { get; set; }

        [DataMember]
        public string PrimaryExchange { get; set; }

        [DataMember]
        public string TradingClass { get; set; }

        [DataMember]
        public string SecIdType { get; set; }

        [DataMember]
        public string SecId { get; set; }

        [DataMember]
        public string ComboLegsDescription { get; set; }

        public override string ToString()
        {
            return SecType + " " + Symbol + " " + Currency + " " + Exchange;
        }
    }
}
