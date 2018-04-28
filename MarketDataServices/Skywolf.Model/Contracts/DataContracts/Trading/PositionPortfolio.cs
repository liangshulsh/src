using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.Trading
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class PositionPortfolio : Position
    {
        [DataMember]
        public double MarketPrice { get; set; }

        [DataMember]
        public double MarketValue { get; set; }

        [DataMember]
        public double UnrealizedPNL { get; set; }

        [DataMember]
        public double RealizedPNL { get; set; }
    }
}
