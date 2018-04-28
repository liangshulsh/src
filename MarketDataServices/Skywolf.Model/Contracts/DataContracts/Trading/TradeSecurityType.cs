using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.Trading
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public enum TradeSecurityType
    {
        [EnumMember]
        NONE = -1,

        [EnumMember]
        Stock = 1,

        [EnumMember]
        FX = 2
    }
}
