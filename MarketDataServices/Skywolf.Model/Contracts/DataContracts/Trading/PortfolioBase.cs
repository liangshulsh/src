using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.Trading
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class PortfolioBase
    {
        [DataMember]
        public string Fund { get; set; }

        [DataMember]
        public string Strategy { get; set; }

        [DataMember]
        public string Folder { get; set; }

        [DataMember]
        public long SID { get; set; }

        [DataMember]
        public string SecurityName { get; set; }
    }
}
