using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Skywolf.Contracts.DataContracts.Trading;
using Skywolf.Contracts.DataContracts.Instrument;
using Skywolf.Utility;
using System.Collections.Concurrent;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace Skywolf.DatabaseRepository
{
    public class TradeDatabase
    {
        public void Trade_Upsert(Trade trade)
        {
            using (TradeDataContext context = new TradeDataContext())
            {
                DateTime asofdate;
                if(!TextUtility.ConvertStringToDateTime(trade.AsOfDate, out asofdate))
                {
                    asofdate = DateTime.MinValue;
                }

                DateTime tradeTime;
                if(!TextUtility.ConvertStringToDateTime(trade.Time, out tradeTime))
                {
                    tradeTime = DateTime.MinValue;
                }

                context.usp_Trade_Upsert(trade.AcctNumber, asofdate, trade.OrderId, trade.ClientId, trade.PermId, trade.ExecId, trade.Contract.SID, trade.Contract.Symbol, trade.Contract.SecType, tradeTime == DateTime.MinValue ? (DateTime?)null : tradeTime, trade.Exchange, trade.Contract.PrimaryExchange, trade.Currency, trade.Side, trade.Shares, trade.Price, trade.Liquidation, trade.CumQty, trade.AvgPrice, trade.OrderRef, trade.EvRule, trade.EvMultiplier, trade.ModelCode, trade.Commission, trade.RealizedPNL, trade.Yield, trade.YieldRedemptionDate);

                if (!string.IsNullOrEmpty(trade.Fund) || !string.IsNullOrEmpty(trade.Strategy) || !string.IsNullOrEmpty(trade.Folder))
                {
                    context.usp_StrategyFolder_Upsert(trade.AcctNumber, asofdate, trade.PermId, trade.Contract.SID, trade.Fund, trade.Strategy, trade.Folder);
                }

                context.SubmitChanges();
            }
        }

        public void Order_Upsert(Order order)
        {
            using (TradeDataContext context = new TradeDataContext())
            {
                DateTime asofdate;
                if (!TextUtility.ConvertStringToDateTime(order.AsOfDate, out asofdate))
                {
                    asofdate = DateTime.MinValue;
                }
                DateTime orderOpenTime;
                if (!TextUtility.ConvertStringToDateTime(order.ActiveStartTime, out orderOpenTime))
                {
                    orderOpenTime = DateTime.MinValue;
                }
                DateTime activeStartTime;
                if (!TextUtility.ConvertStringToDateTime(order.ActiveStartTime, out activeStartTime))
                {
                    activeStartTime = DateTime.MinValue;
                }
                DateTime activeStopTime;
                if (!TextUtility.ConvertStringToDateTime(order.ActiveStopTime, out activeStopTime))
                {
                    activeStopTime = DateTime.MinValue;
                }
                DateTime goodAfterDate;
                if (!TextUtility.ConvertStringToDateTime(order.GoodAfterTime, out goodAfterDate))
                {
                    goodAfterDate = DateTime.MinValue;
                }
                DateTime goodTilDate;
                if (!TextUtility.ConvertStringToDateTime(order.GoodTillDate, out goodTilDate))
                {
                    goodTilDate = DateTime.MinValue;
                }

                context.usp_Order_Upsert(order.Account, asofdate, order.OrderId, order.ClientId, order.PermId, order.Contract.SID, order.Contract.Symbol, order.Contract.SecType, orderOpenTime == DateTime.MinValue ? (DateTime?)null : orderOpenTime, order.Contract.Exchange, order.Contract.PrimaryExchange, order.Contract.Currency, order.Action, order.TotalQuantity, order.OrderType, order.LimitPrice, order.AuxPrice, order.Tif, activeStartTime == DateTime.MinValue ? (DateTime?)null : activeStartTime, activeStopTime == DateTime.MinValue ? (DateTime?)null : activeStopTime, order.OcaGroup, order.OcaType, order.OrderRef, order.Transmit, order.ParentId, order.BlockOrder, order.SweepToFill, order.DisplaySize, order.TriggerMethod, order.OutsideRth, order.Hidden, goodAfterDate == DateTime.MinValue ? (DateTime?)null : goodAfterDate, goodTilDate == DateTime.MinValue ? (DateTime?)null : goodTilDate, order.OverridePercentageConstraints, order.Rule80A, order.AllOrNone, order.MinQty, order.Status, order.InitMargin, order.MaintMargin, order.EquityWithLoan, order.Commission, order.MinCommission, order.MaxCommission, order.CommissionCurrency, order.WarningText, order.Filled, order.Remaining, order.AvgFillPrice, order.LastFillPrice, order.WhyHeld);

                if (!string.IsNullOrEmpty(order.Fund) || !string.IsNullOrEmpty(order.Strategy) || !string.IsNullOrEmpty(order.Folder))
                {
                    context.usp_StrategyFolder_Upsert(order.Account, asofdate, order.PermId, order.Contract.SID, order.Fund, order.Strategy, order.Folder);
                }

                context.SubmitChanges();
            }
        }
    }
}
