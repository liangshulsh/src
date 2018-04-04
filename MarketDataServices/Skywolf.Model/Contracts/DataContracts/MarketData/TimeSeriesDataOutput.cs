using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.MarketData
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    [KnownType(typeof(CryptoTimeSeriesDataOutput))]
    public class TimeSeriesDataOutput
    {
        [DataMember]
        public string Symbol { get; set; }

        [DataMember]
        public Bar[] Data { get; set; }

        [DataMember]
        public string TimeZone { get; set; }
    }
}
