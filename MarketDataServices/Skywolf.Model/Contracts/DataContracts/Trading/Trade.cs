using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.Trading
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class Trade
    {
        [DataMember]
        public string Fund { get; set; }

        [DataMember]
        public string Strategy { get; set; }

        [DataMember]
        public string Folder { get; set; }

        [DataMember]
        public Contract Contract { get; set; }

        [DataMember]
        public string AsOfDate { get; set; }

        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public string ExecId { get; set; }

        [DataMember]
        public string Time { get; set; }

        [DataMember]
        public string AcctNumber { get; set; }

        [DataMember]
        public string Exchange { get; set; }

        [DataMember]
        public string Side { get; set; }

        [DataMember]
        public double Shares { get; set; }

        [DataMember]
        public double Price { get; set; }

        [DataMember]
        public int PermId { get; set; }

        [DataMember]
        public int Liquidation { get; set; }

        [DataMember]
        public double CumQty { get; set; }

        [DataMember]
        public double AvgPrice { get; set; }

        [DataMember]
        public string OrderRef { get; set; }

        [DataMember]
        public string EvRule { get; set; }

        [DataMember]
        public double EvMultiplier { get; set; }

        [DataMember]
        public string ModelCode { get; set; }

        [DataMember]
        public double Commission { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public double RealizedPNL { get; set; }

        [DataMember]
        public double Yield { get; set; }

        [DataMember]
        public int YieldRedemptionDate { get; set; }
    }
}
