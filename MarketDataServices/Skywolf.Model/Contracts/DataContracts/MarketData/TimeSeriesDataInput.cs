using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.MarketData
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    [KnownType(typeof(CryptoTimeSeriesDataInput))]
    public class TimeSeriesDataInput
    {
        [DataMember]
        public string Symbol { get; set; }

        [DataMember]
        public BarFrequency Frequency { get; set; }

        [DataMember]
        public long OutputCount { get; set; }

        [DataMember]
        public bool IsAdjustedValue { get; set; }
    }
}
