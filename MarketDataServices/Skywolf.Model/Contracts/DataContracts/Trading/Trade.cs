using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.Trading
{
    class Trade
    {
        public Contract Contract { get; set; }

        public int OrderId { get; set; }
        public int clientId { get; set; }
        public string execId { get; set; }
        public string time { get; set; }
        public string acctNumber { get; set; }
        public string exchange { get; set; }
        public string side { get; set; }
        public double shares { get; set; }
        public double price { get; set; }
        public int permId { get; set; }
        public int liquidation { get; set; }
        public double cumQty { get; set; }
        public double avgPrice { get; set; }
        public string orderRef { get; set; }
        public string evRule { get; set; }
        public double evMultiplier { get; set; }
        public string modelCode { get; set; }

        public double commission { get; set; }
        public string currency { get; set; }
        public double realizedPNL { get; set; }
        public double yield { get; set; }
        public int yieldRedemptionDate { get; set; }
    }
}
