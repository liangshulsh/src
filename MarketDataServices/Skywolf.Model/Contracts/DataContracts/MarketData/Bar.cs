using System;
using System.Runtime.Serialization;

namespace Skywolf.Contracts.DataContracts.MarketData
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    [KnownType(typeof(StockBar))]
    [KnownType(typeof(CryptoBar))]
    public class Bar
    {
        [DataMember]
        public DateTime AsOfDate { get; set; }

        [DataMember]
        public double? Open { get; set; }

        [DataMember]
        public double? High { get; set; }

        [DataMember]
        public double? Low { get; set; }

        [DataMember]
        public double? Close { get; set; }

        [DataMember]
        public decimal? Volume { get; set; }

        [DataMember]
        public DateTime? TS { get; set; }
    }

    [DataContract(Namespace = Constants.NAMESPACE)]
    public class StockBar : Bar
    {
        [DataMember]
        public double? AdjClose { get; set; }

        [DataMember]
        public double? DividendAmount { get; set; }

        [DataMember]
        public double? SplitCoefficient { get; set; }
    }

    [DataContract(Namespace = Constants.NAMESPACE)]
    public class CryptoBar : Bar
    {
        [DataMember]
        public decimal? MarketCap { get; set; }
    }
}
