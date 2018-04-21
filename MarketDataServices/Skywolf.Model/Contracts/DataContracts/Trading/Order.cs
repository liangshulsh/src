using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.Trading
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class Order
    {
        [DataMember]
        public Contract Contract { get; set; }

        // main order fields
        [DataMember]
        public int OrderId { get; set; }

        [DataMember]
        public int ClientId { get; set; }

        [DataMember]
        public int PermId { get; set; }

        [DataMember]
        public string Action { get; set; }

        [DataMember]
        public double TotalQuantity { get; set; }

        [DataMember]
        public string OrderType { get; set; }

        [DataMember]
        public double LimitPrice { get; set; }

        [DataMember]
        public double AuxPrice { get; set; }

        // extended order fields
        // "Time in Force" - DAY, GTC, etc.
        [DataMember]
        public string Tif { get; set; }
        //GTC orders
        [DataMember]
        public string ActiveStartTime { get; set; }

        [DataMember]
        public string ActiveStopTime { get; set; }
        // one cancels all group name

        [DataMember]
        public string OcaGroup { get; set; }
        // 1 = CANCEL_WITH_BLOCK, 2 = REDUCE_WITH_BLOCK, 3 = REDUCE_NON_BLOCK

        [DataMember]
        public int OcaType { get; set; }

        [DataMember]
        public string OrderRef { get; set; }
        // if false, order will be created but not transmitted

        [DataMember]
        public bool Transmit { get; set; }
        // Parent order Id, to associate Auto STP or TRAIL orders with the original order.
        [DataMember]
        public int ParentId { get; set; }
        [DataMember]
        public bool BlockOrder { get; set; }
        [DataMember]
        public bool SweepToFill { get; set; }
        [DataMember]
        public int DisplaySize { get; set; }
        // 0=Default, 1=Double_Bid_Ask, 2=Last, 3=Double_Last, 4=Bid_Ask, 7=Last_or_Bid_Ask, 8=Mid-point
        [DataMember]
        public int TriggerMethod { get; set; }
        [DataMember]
        public bool OutsideRth { get; set; }
        [DataMember]
        public bool Hidden { get; set; }
        [DataMember]
        // FORMAT: 20060505 08:00:00 {time zone}
        public string GoodAfterTime { get; set; }
        [DataMember]
        // FORMAT: 20060505 08:00:00 {time zone}
        public string GoodTillDate { get; set; }
        [DataMember]
        public bool OverridePercentageConstraints { get; set; }
        // Individual = 'I', Agency = 'A', AgentOtherMember = 'W', IndividualPTIA = 'J', AgencyPTIA = 'U', AgentOtherMemberPTIA = 'M', IndividualPT = 'K', AgencyPT = 'Y', AgentOtherMemberPT = 'N'
        [DataMember]
        public string Rule80A { get; set; }
        [DataMember]
        public bool AllOrNone { get; set; }
        [DataMember]
        public int MinQty { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string InitMargin { get; set; }
        [DataMember]
        public string MaintMargin { get; set; }
        [DataMember]
        public string EquityWithLoan { get; set; }
        [DataMember]
        public double Commission { get; set; }
        [DataMember]
        public double MinCommission { get; set; }
        [DataMember]
        public double MaxCommission { get; set; }
        [DataMember]
        public string CommissionCurrency { get; set; }
        [DataMember]
        public string WarningText { get; set; }
        [DataMember]
        public double Filled { get; set; }
        [DataMember]
        public double Remaining { get; set; }
        [DataMember]
        public double AvgFillPrice { get; set; }
        [DataMember]
        public double LastFillPrice { get; set; }
        [DataMember]
        public string WhyHeld { get; set; }
    }
}
