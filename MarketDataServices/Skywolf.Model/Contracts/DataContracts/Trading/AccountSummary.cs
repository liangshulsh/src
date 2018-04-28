using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Contracts.DataContracts.Trading
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public class AccountSummary
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string Account { get; set; }

        [DataMember]
        public string AccountType { get; set; }

        [DataMember]
        public double NetLiquidation { get; set; }

        [DataMember]
        public double TotalCashValue { get; set; }

        [DataMember]
        public double SettledCash { get; set; }

        [DataMember]
        public double AccruedCash { get; set; }

        [DataMember]
        public double BuyingPower { get; set; }

        [DataMember]
        public double EquityWithLoanValue { get; set; }

        [DataMember]
        public double PreviousEquityWithLoanValue { get; set; }

        [DataMember]
        public double GrossPositionValue { get; set; }

        [DataMember]
        public double ReqTEquity { get; set; }

        [DataMember]
        public double ReqTMargin { get; set; }

        [DataMember]
        public double SMA { get; set; }

        [DataMember]
        public double InitMarginReq { get; set; }

        [DataMember]
        public double MaintMarginReq { get; set; }

        [DataMember]
        public double AvailableFunds { get; set; }

        [DataMember]
        public double ExcessLiquidity { get; set; }

        [DataMember]
        public double Cushion { get; set; }

        [DataMember]
        public double FullInitMarginReq { get; set; }

        [DataMember]
        public double FullMaintMarginReq { get; set; }

        [DataMember]
        public double FullAvailableFunds { get; set; }

        [DataMember]
        public double FullExcessLiquidity { get; set; }

        [DataMember]
        public double LookAheadNextChange { get; set; }

        [DataMember]
        public double LookAheadInitMarginReq { get; set; }

        [DataMember]
        public double LookAheadMaintMarginReq { get; set; }

        [DataMember]
        public double LookAheadAvailableFunds { get; set; }

        [DataMember]
        public double LookAheadExcessLiquidity { get; set; }

        [DataMember]
        public double HighestSeverity { get; set; }

        [DataMember]
        public double DayTradesRemaining { get; set; }

        [DataMember]
        public double Leverage { get; set; }
    }
}
