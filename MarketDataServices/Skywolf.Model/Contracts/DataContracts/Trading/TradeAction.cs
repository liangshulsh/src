using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.Trading
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public enum TradeAction
    {
        [EnumMember]
        NONE = -1,

        [EnumMember]
        BUY = 1,

        [EnumMember]
        SELL = 2
    }
}
