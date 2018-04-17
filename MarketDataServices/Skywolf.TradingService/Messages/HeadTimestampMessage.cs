using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skywolf.TradingService.Messages
{
    public class HeadTimestampMessage
    {
        public int ReqId { get; private set; }
        public string HeadTimestamp { get; private set; }

        public HeadTimestampMessage(int reqId, string headTimestamp)
        {
            this.ReqId = reqId;
            this.HeadTimestamp = headTimestamp;
        }
    }
}
