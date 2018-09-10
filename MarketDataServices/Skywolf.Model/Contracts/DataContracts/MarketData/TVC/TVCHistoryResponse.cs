using System;
using System.Runtime.Serialization;

namespace Skywolf.Contracts.DataContracts.MarketData.TVC
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class TVCHistoryResponse : TVCResponse
    {
        [DataMember]
        public int[] t { get; set; }
        
        [DataMember]
        public double[] c { get; set; }
        
        [DataMember]
        public double[] o { get; set; }
        
        [DataMember]
        public double[] h { get; set; }
        
        [DataMember]
        public double[] l { get; set; }
        
        [DataMember]
        public double[] v { get; set; }
        
        [DataMember]
        public double[] vo { get; set; }
    }
}
