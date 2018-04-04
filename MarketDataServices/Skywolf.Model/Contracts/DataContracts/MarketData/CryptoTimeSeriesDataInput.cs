using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.MarketData
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class CryptoTimeSeriesDataInput : TimeSeriesDataInput
    {
        [DataMember]
        public string Market { get; set; }
    }
}
