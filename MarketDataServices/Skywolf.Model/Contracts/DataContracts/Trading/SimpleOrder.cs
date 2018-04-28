using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.Trading
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    [KnownType(typeof(SimpleMarketOrder))]
    [KnownType(typeof(SimpleLimitOrder))]
    [KnownType(typeof(SimpleStopOrder))]
    public class SimpleOrder
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public TradeSecurityType SecurityType { get; set; }

        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public string Symbol { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public double Quantity { get; set; }

        [DataMember]
        public TradeAction Action { get; set; }
    }

    [DataContract(Namespace = Constants.NAMESPACE)]
    public class SimpleMarketOrder : SimpleOrder
    {
    }

    [DataContract(Namespace = Constants.NAMESPACE)]
    public class SimpleLimitOrder : SimpleOrder
    {
        [DataMember]
        public double LimitPrice { get; set; }
    }

    [DataContract(Namespace = Constants.NAMESPACE)]
    public class SimpleStopOrder : SimpleOrder
    {
        [DataMember]
        public double StopPrice { get; set; }
    }
}
