using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Skywolf.Client
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class ServiceFault
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string Message { get; set; }
    }
}

