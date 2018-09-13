using System;
using System.Runtime.Serialization;

namespace Skywolf.Contracts.DataContracts.MarketData.TVC
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class TVCCalendarResponse
    {
        [DataMember]
        public string data { get; set; }

        [DataMember]
        public int noresult { get; set; }

        [DataMember]
        public string timeframe { get; set; }

        [DataMember]
        public string dateFrom { get; set; }

        [DataMember]
        public string dateTo { get; set; }

        [DataMember]
        public int rows_num { get; set; }

        [DataMember]
        public int last_time_scope { get; set; }

        [DataMember]
        public bool bind_scroll_handler { get; set; }
    }
}
