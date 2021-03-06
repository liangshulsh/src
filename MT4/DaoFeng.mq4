//+------------------------------------------------------------------+
//|                                                      DaoFeng.mq4 |
//|                                                          Skywolf |
//|                                             https://www.mql5.com |
//+------------------------------------------------------------------+
#property copyright "Skywolf"
#property link      "https://www.mql5.com"
#property version   "1.00"
#property strict
//--- input parameters
input int      PeriodK=8;
input int      PeriodD=3;
input int      PeriodSlow=5;
input int      StartHour=8;
input int      StartMin=59;
input int      EndHour=20;
input int      EndMin=59;
input int      LongTermMAPeriod=144;
 
input double LotsRisk           = 0.1;        // 1 to *10
input double MaxRisk            = 0.05;

input double MinLots            = 0.01;

input int    SlipPage           = 3;

input int ExecuteSecondLimit = 5;

input int MagicNum = 20181026; 

#define DIRECTION_NONE 0
#define DIRECTION_BUY 1
#define DIRECTION_SELL -1

#define DAOFENG_NONE -1
#define DAOFENG_GOLD_CROSS 1
#define DAOFENG_TREND_UP 2
#define DAOFENG_TREND_SUPER_BUY 3

#define DAOFENG_DEAD_CROSS 11
#define DAOFENG_TREND_DOWN 12
#define DAOFENG_TREND_SUPER_SELL 13

int DaoFengHighBar = 80;
int DaoFengLowBar = 20;


static bool Bar_Executed = false;

//+------------------------------------------------------------------+
//| Expert initialization function                                   |
//+------------------------------------------------------------------+
int OnInit()
  {
//--- create timer
   EventSetTimer(1);
      
//---
   return(INIT_SUCCEEDED);
  }
//+------------------------------------------------------------------+
//| Expert deinitialization function                                 |
//+------------------------------------------------------------------+
void OnDeinit(const int reason)
  {
//--- destroy timer
   EventKillTimer();
      
  }
//+------------------------------------------------------------------+
//| Expert tick function                                             |
//+------------------------------------------------------------------+
void OnTick()
  {
//---
   Execute();
   ShowClock();
  }
//+------------------------------------------------------------------+
//| Timer function                                                   |
//+------------------------------------------------------------------+
void OnTimer()
  {
//---
   Execute();
   ShowClock();
  }
//+------------------------------------------------------------------+
//| ChartEvent function                                              |
//+------------------------------------------------------------------+
void OnChartEvent(const int id,
                  const long &lparam,
                  const double &dparam,
                  const string &sparam)
  {
//---
   
  }
  
void Execute()
{
   //-------------------------------------
   // verify state
   //-------------------------------------
   if(!IsTradeAllowed()) 
   {
      Print("Trade is not allowed");
      return;
   }
   
   if(!IsConnected()) 
   {
      Print("It is not connected.");
      return;
   }
   
   if(IsStopped()) 
   {
      Print("It is stopped.");
      return;
   }
     
   if(IsTradeContextBusy()) 
     {
      Print("Trade context is busy!");
      return;
     }
     
   if(!IsDateTimeEnabled(TimeCurrent())) 
     {
         CloseAllOrders();
         return;
     }
     
   int barShift = GetRunStrategyBarShift();
   
   if (barShift >= 0)
   {
      Bar_Executed = RunStrategy(barShift);
   }
}

bool RunStrategy(int shift)
{
   int status = GetDaoFengStatusFinal(shift);
   
   if (status == DAOFENG_NONE)
   {
      Comment("Dao Feng indicator is error at ", TimeCurrent());
      Print("Dao Feng indicator is error at ", TimeCurrent());
      return false;
   }
   
   int daofengDirection = DaoFengStatusToDirection(status);
   double lots = 0;
   
   // for test
   //DaoFengOpenTrade(shift, daofengDirection, "Dao Feng Trend Down");   
   bool correct_handled = true;
   
   if (GetOrderCount() > 0)
   {
         // how many orders already open?
      int curOrder, numOrders = OrdersTotal();
      
      int count = 0;
      if(numOrders > 0) 
        { 
         for(curOrder = 0; curOrder < numOrders; curOrder++) 
           {
              if(GetOrderByPos(curOrder))
              {
                 int direction = OrderTypeDirection();
                 if (direction != daofengDirection)
                 {
                     correct_handled = CloseOrder();
                     if (!correct_handled)
                     {
                        return correct_handled;
                     }
                 }
                 else
                 {
                     if (direction > 0)
                     {
                        lots = lots + OrderLots();
                     }
                     else if (direction < 0)
                     {
                        lots = lots - OrderLots();
                     }
                 }
              }
           }
        }   
   }
   
   correct_handled = true;
   
   if (status == DAOFENG_GOLD_CROSS)
   {
      if (lots < MinLots)
      {
         correct_handled = DaoFengOpenTrade(shift, daofengDirection, "Dao Feng Gold Cross");      
      }
   }
   else if (status == DAOFENG_DEAD_CROSS)
   {
      if (lots > -MinLots)
      {
         correct_handled = DaoFengOpenTrade(shift, daofengDirection, "Dao Feng Dead Cross");      
      }
   }
   else if (status == DAOFENG_TREND_UP)
   {
      if (lots < MinLots)
      {
         datetime lastCrossTime = GetDaoFengLastCrossTime(shift);
         if (TimeHour(lastCrossTime) >= StartHour)
         {
            correct_handled = DaoFengOpenTrade(shift, daofengDirection, "Dao Feng Trend Up");            
         }
      }
   }
   else if (status == DAOFENG_TREND_DOWN)
   {
      if (lots > -MinLots)
      {
         datetime lastCrossTime = GetDaoFengLastCrossTime(shift);
         if (TimeHour(lastCrossTime) >= StartHour)
         {
            correct_handled = DaoFengOpenTrade(shift, daofengDirection, "Dao Feng Trend Down");
         }
      }
   }
   
   return correct_handled;  
}

bool DaoFengOpenTrade(int shift, int direction, string comments)
{
   double stoploss = FindStopLossByDaoFeng(direction, shift);
   double numLots = CalculateLots(shift, direction, stoploss);
   if (stoploss > 0 && numLots > 0)
   {
      if (OpenOrder(direction, numLots, stoploss, 0.0, comments) == -1)
      {
         IsError("Open Trade Error");
         return false;
      }
   }
   else
   {
      Print("error:", comments, "stop loss:", stoploss, " lots:", numLots);
      return false;
   } 
   
   return true;
}

int DaoFengStatusToDirection(int status)
{
   if (status == DAOFENG_GOLD_CROSS ||
       status == DAOFENG_TREND_UP ||
       status == DAOFENG_TREND_SUPER_BUY)
   {
         return 1;
   }
   else if (status == DAOFENG_DEAD_CROSS ||
       status == DAOFENG_TREND_DOWN ||
       status == DAOFENG_TREND_SUPER_SELL)
   {
      return -1;
   }
   
   return 0;
}

int GetDaoFengStatusFinal(int shift)
{
   int status = DAOFENG_NONE;
   if (IsFirstBarOfDay())
   {
      status = GetDaoFengAdjstedStatusForFirstBar(shift);
   }
   else
   {
      status = GetDaoFengStatus(shift);
   }
   
   return status;
}

int GetDaoFengAdjstedStatusForFirstBar(int shift)
{
   int curStatus = GetDaoFengStatus(shift);
   
   bool found1 = false;
   bool found2 = false;
   double main1 = 0;
   double main2 = 0;

   int diff = shift;    
   
   if (curStatus == DAOFENG_TREND_UP)
   {  
      while (!found1 || !found2)
      {
         double curMain = GetDaoFeng(MODE_MAIN, diff);
         double curSignal = GetDaoFeng(MODE_SIGNAL, diff);
         double preMain = GetDaoFeng(MODE_MAIN, diff+1);
         double preSignal = GetDaoFeng(MODE_SIGNAL, diff+1);
         
         if (curMain < 0 || curSignal < 0 || preMain < 0 || preSignal < 0)
         {
            break;
         }
         
         if (curMain >= curSignal && preMain < preSignal)
         {         
            if (!found1)
            {
               main1 = preMain;
               found1 = true;
            }
            else if (!found2)
            {
               main2 = preMain;
               found2 = true;
            }
         }        
         diff++;
      }
      
      if (found1 && found2)
      {
         if (main1 > main2)
         {
            curStatus = DAOFENG_GOLD_CROSS;
         }
         else
         {
            curStatus = DAOFENG_TREND_UP;
         }
      }
      else
      {
         curStatus = DAOFENG_NONE;
      }
   }
   else if (curStatus == DAOFENG_TREND_DOWN)
   {
      while (!found1 || !found2)
      {
         double curMain = GetDaoFeng(MODE_MAIN, diff);
         double curSignal = GetDaoFeng(MODE_SIGNAL, diff);
         double preMain = GetDaoFeng(MODE_MAIN, diff+1);
         double preSignal = GetDaoFeng(MODE_SIGNAL, diff+1);
         
         if (curMain < 0 || curSignal < 0 || preMain < 0 || preSignal < 0)
         {
            break;
         }
         
         if (curMain <= curSignal && preMain > preSignal)
         {         
            if (!found1)
            {
               main1 = preMain;
               found1 = true;
            }
            else if (!found2)
            {
               main2 = preMain;
               found2 = true;
            }
         }        
         diff++;
      }
      
      if (found1 && found2)
      {
         if (main1 > main2)
         {
            curStatus = DAOFENG_TREND_DOWN;
         }
         else
         {
            curStatus = DAOFENG_DEAD_CROSS;
         }
      }
      else
      {
         curStatus = DAOFENG_NONE;
      }      
   }
   
   return curStatus;
}

double FindStopLossByDaoFeng(int direction, int shift)
{
   double lowest = 100000;
   double highest = -100000;
   bool found = false;
   int diff = shift;   
   double price = PriceClose(direction);
   
   while (!found)
   {
      double curMain = GetDaoFeng(MODE_MAIN, diff);
      double curSignal = GetDaoFeng(MODE_SIGNAL, diff);
      double preMain = GetDaoFeng(MODE_MAIN, diff+1);
      double preSignal = GetDaoFeng(MODE_SIGNAL, diff+1);
      double curhigh = GetHigh(diff);
      double curlow = GetLow(diff);
      
      if (curMain < 0 || curSignal < 0 || preMain < 0 || preSignal < 0 || curhigh < 0 || curlow < 0)
      {
         break;
      }
      
      if (curlow < lowest)
      {
         lowest = curlow;
      }
      
      if (curhigh > highest)
      {
         highest = curhigh;
      }
      
      if ((direction > 0 && curMain <= curSignal && preMain > preSignal) || 
          (curMain >= curSignal && preMain < preSignal))
      {
         found = true;
      }
      
      diff++;
   }
     
   lowest = lowest - SlipPage * PricePoint();
   highest = highest + SlipPage * PricePoint();
    
   if (found)
   {
      if (direction > 0)
      {
         if ((price - lowest) / PricePoint() < 10)
         {
            lowest = price - 10 * PricePoint();
         }
         
         return lowest;
      }
      else
      {
         if ((highest - price) / PricePoint() < 10)
         {
            highest = price + 10 * PricePoint();
         }
         
         return highest;
      }
   }
   
   return -1;
}

double PricePoint()
{
   return Point * 10;
}

datetime GetDaoFengLastCrossTime(int shift)
{
   bool found = false;
   int diff = shift;
   
   while (!found)
   {
      double curMain = GetDaoFeng(MODE_MAIN, diff);
      double curSignal = GetDaoFeng(MODE_SIGNAL, diff);
      double preMain = GetDaoFeng(MODE_MAIN, diff+1);
      double preSignal = GetDaoFeng(MODE_SIGNAL, diff+1);
      
      if (curMain < 0 || curSignal < 0 || preMain < 0 || preSignal < 0)
      {
         break;
      }
      
      
      
      if (curMain > DaoFengLowBar && curSignal > DaoFengLowBar && (preMain <= DaoFengLowBar || preSignal <= DaoFengLowBar))
      {
         found = true;
         break;
      }
      else if (curMain < DaoFengHighBar && curSignal < DaoFengHighBar && (preMain >= DaoFengHighBar || preSignal >= DaoFengHighBar))
      {
         found = true;
         break;
      }
      else if (curMain > DaoFengLowBar && curSignal > DaoFengLowBar && curMain < DaoFengHighBar && curSignal < DaoFengHighBar &&
               preMain > DaoFengLowBar && preSignal > DaoFengLowBar && preMain < DaoFengHighBar && preSignal < DaoFengHighBar)
      {      
         if (curMain >= curSignal && preMain < preSignal)
         {
            found = true;
            break;
         }
         else if (curMain <= curSignal && preMain > preSignal)
         {
            found = true;
            break;
         }
      }
      
      diff++;
   }
   
   if (found)
   {
      return iTime(Symbol(), Period(), diff);
   }
   
   return D'1970.01.01 00:00';  
}

int GetDaoFengStatus(int shift)
{
   double curMain = GetDaoFeng(MODE_MAIN, shift);
   double curSignal = GetDaoFeng(MODE_SIGNAL, shift);
   double preMain = GetDaoFeng(MODE_MAIN, shift+1);
   double preSignal = GetDaoFeng(MODE_SIGNAL, shift+1);
   
   int status = DAOFENG_NONE;
   if (curMain > 0 && curSignal > 0 && preMain > 0 && preSignal > 0)
   {
      if (curMain >= DaoFengLowBar && curSignal >= DaoFengLowBar)
      {
         if (curMain <= DaoFengHighBar && curSignal <= DaoFengHighBar)
         {
            if (preMain < DaoFengLowBar || preSignal < DaoFengLowBar)
            {
               status = DAOFENG_GOLD_CROSS;
            }
            else if (preMain > DaoFengHighBar || preSignal > DaoFengHighBar)
            {
               status = DAOFENG_DEAD_CROSS;
            }
            else // (preMain >= DaoFengLowBar && preSignal >= DaoFengLowBar && preMain <= DaoFengHighBar && preSignal <= DaoFengHighBar)
            {
               if (curMain >= curSignal && preMain < preSignal)
               {
                  status = DAOFENG_GOLD_CROSS;
               }
               else if (curMain <= curSignal && preMain > preSignal)
               {
                  status = DAOFENG_DEAD_CROSS;
               }
               else if (curMain >= curSignal && preMain >= preSignal)
               {
                  status = DAOFENG_TREND_UP; 
               }
               else // (curMain <= curSignal && preMain <= preSignal)
               {
                  status = DAOFENG_TREND_DOWN;
               }
            }
         }
         else
         {
            status = DAOFENG_TREND_SUPER_BUY;
         }
      }
      else
      {
         status = DAOFENG_TREND_SUPER_SELL;
      }
   }
   
   return status;
}

double GetLongTermMA(int shift)
{
   double value = iMA(Symbol(), Period(), LongTermMAPeriod, 0, MODE_SMA, PRICE_CLOSE, shift); 
   if (IsError("Get Long Term MA"))
   {
      return (-1);
   }
   
   return value;
}

double GetHigh(int shift)
{
   double value = iHigh(Symbol(), Period(), shift);
   if (IsError("Get High"))
   {
      return (-1);
   }
   
   return value;
}

double GetLow(int shift)
{
   double value = iLow(Symbol(), Period(), shift);
   if (IsError("Get Low"))
   {
      return (-1);
   }
   
   return value;
}

double GetDaoFeng(int mode, int shift)
{
   double value=iStochastic(Symbol(), Period(), PeriodK, PeriodD, PeriodSlow, MODE_LWMA, 0, mode, shift);
   if(IsError("Get Dao Feng Indicator")) 
   {
      return (-1);
   }
   
   return value;
}

int GetRunStrategyBarShift()
   {
      int diff = -1;
      int secondsLeave = GetSecondsLeave();
      if (Bar_Executed == false)
      {
         if (secondsLeave <= ExecuteSecondLimit)
         {
            diff = 0;
            //Bar_Executed = true;
         }
         else if ((Period() * 60 - secondsLeave) < ExecuteSecondLimit)
         {
            diff = 1;
            //Bar_Executed = true;
         }
      }
      else
      {
         if (secondsLeave > ExecuteSecondLimit && (Period() * 60 - secondsLeave) > ExecuteSecondLimit)
         {
            Bar_Executed = false;
         }
      }
      
      return diff;
   }

bool IsDateTimeEnabled(datetime t)
  {
   int day_w=TimeDayOfWeek(t);
   int hour = TimeHour(t);
   int minute = TimeMinute(t);
   int currtime = hour * 100 + minute;
   int starttime = StartHour * 100 + StartMin;
   int endtime = EndHour * 100 + EndMin;
   
   return(day_w > 0 && day_w < 6 && currtime >= starttime && currtime <= endtime);
  }
  
bool IsFirstBarOfDay()
{
   datetime t = TimeCurrent();
   int hour = TimeHour(t);
   int minute = TimeHour(t);
   int second = TimeSeconds(t);
   
   int starttime = StartHour * 3600 + (StartMin + 1) * 60;
   int curtime = hour * 3600 + minute * 60 + second;
   
   if (MathAbs(starttime-curtime) < 60)
   {
      return true;
   }
   
   return false;
}

//+------------------------------------------------------------------+

int GetSecondsLeave()
{
   int seconds = 100;
   switch(Period())
   {      
   case 1:      
      {         
         seconds=60-Seconds();         
         break;      
      }      
   case 5:      
      {         
         seconds=Period()*60-Minute()%5*60-Seconds();
         break;      
      }            
   case 15:      
      {         
         seconds=Period()*60-Minute()%15*60-Seconds();         
         break;      
      }      
   case 30:       
      {         
         seconds=Period()*60-Minute()%30*60-Seconds();
         break;      
      }        
   case 60:      
      {         
         seconds=Period()*60-Minute()*60-Seconds();         
         break;      
      }      
   case 240:      
      {        
         seconds=Period()*60-Hour()%4*3600-Minute()*60-Seconds();         
         break;      
      }      
   case 1440:      
      {         
         seconds=Period()*60-Hour()*3600-Minute()*60-Seconds();         
         break;      
      }      
   case 10080:      
      {         
         seconds=Period()*60-TimeDayOfWeek(TimeCurrent())*1440*60-Hour()*3600-Minute()*60-Seconds();
         break;      
      }      
   case 43200:      
      {         
         seconds=Period()*60-TimeDay(TimeCurrent())*1440*60-Hour()*3600-Minute()*60-Seconds();         
         break;      
      }      
   default:      
      {         
         string c="failed to get leaving time！！！";         
         Comment(c);        
         break;      
      }         
   }
   return seconds;
}

double CalculateLots(int shift, int direction, double stop_loss = 0)
{
   if (direction == 0)
   {
      return 0;
   }
   
   double freeMargin = AccountFreeMargin();
   
   double maxRiskValue = freeMargin * MaxRisk;
   
   double margin = freeMargin * LotsRisk;
   double standardLots = MathRound((margin * AccountLeverage() / 100000.0) / MinLots) * MinLots;
   double standardMaxStopLossPoint = maxRiskValue / (standardLots * 100000 * PricePoint());
   
   double lots = standardLots;
   
   double stoplosspoints = MathAbs(MathRound((PriceClose(direction) - stop_loss) / PricePoint()));
   
   if (stoplosspoints > standardMaxStopLossPoint)
   {
      lots = MathRound(standardMaxStopLossPoint / stoplosspoints / MinLots) * MinLots;
   }
   
   double MA = GetLongTermMA(shift);
   
   if ((direction < 0 && PriceClose(direction) > MA) ||
       (direction > 0 && PriceClose(direction) < MA))
   {
      lots = MathRound(lots / 2 / MinLots) * MinLots;
   }
   
   return lots;
}

// =================================================
// order functions
// =================================================

// -------------------------------------------------
// OpenOrder
// -------------------------------------------------
int OpenOrder(int direction, double numlots, double stop_loss = 0, double take_profit = 0, string comment = "") 
  { 
   double price;
   
   //calc order bounds 
   price = PriceOpen(direction);
   
   if (take_profit == 0)
   {
      if (direction > 0)
      {
         take_profit = price + PricePoint() * 2000;
      }
      else if (direction < 0)
      {
         take_profit = price - PricePoint() * 2000;
      }
   }
   // take_profit = price + TakeProfit * Point * direction; 
   
   //if(StopLoss > 0)
     //{
      //stop_loss = PriceClose(direction) - StopLoss * Point * direction; 
      //stop_loss = stop_loss * (1/(numlots/0.1)); // convert numloss points to amounts
     //}      

   return(OrderSend(Symbol(), DirectionOrderType(direction), 
     numlots, price, SlipPage, stop_loss, take_profit,  
     comment, MagicNum, 0, ColorOpen(direction)));
  }

// -------------------------------------------------
//  CloseOrder()
// -------------------------------------------------

void CloseAllOrders()
{
   // how many orders already open?
   int curOrder, numOrders = OrdersTotal();
   
   //-------------------------------------
   // check open orders status 
   //-------------------------------------
   
   if(numOrders > 0) 
     { 
      for(curOrder = 0; curOrder < numOrders; curOrder++) 
        {
         if(GetOrderByPos(curOrder))
           {
            if(OrderMagicNumber() == MagicNum) 
              {
               CloseOrder();
              }
           }
        }
     } 
}

int GetOrderCount()
{
   // how many orders already open?
   int curOrder, numOrders = OrdersTotal();
   
   //-------------------------------------
   // check open orders status 
   //-------------------------------------
   int count = 0;
   if(numOrders > 0) 
     { 
      for(curOrder = 0; curOrder < numOrders; curOrder++) 
        {
         if(GetOrderByPos(curOrder))
           {
              count++;
           }
        }
     }   
   return count;
}

int GetOrderDirection()
{
   // how many orders already open?
   int curOrder, numOrders = OrdersTotal();
   
   //-------------------------------------
   // check open orders status 
   //-------------------------------------
   int count = 0;
   if(numOrders > 0) 
     { 
      for(curOrder = 0; curOrder < numOrders; curOrder++) 
        {
         if(GetOrderByPos(curOrder))
           {
              return OrderTypeDirection();
           }
        }
     }   
     
   return 0;
}

bool CloseOrder()
{
   int direction = OrderTypeDirection();
   if(direction == 0) return false;

   if (!OrderClose(OrderTicket(), OrderLots(), PriceClose(direction), SlipPage, ColorClose(direction)))
   {
      Print("Order Close Failed, ticket:", OrderTicket(), " order lots:", OrderLots(), " price:", PriceClose(direction), " slip page:", SlipPage);
      return false;
   }
   
   return true;
}
  
// -------------------------------------------------
// GetOrderByPos
// -------------------------------------------------
bool GetOrderByPos(int pos)
  {
   return(OrderSelect(pos, SELECT_BY_POS, MODE_TRADES) 
      && (OrderType() <= OP_SELL) 
      && OrderSymbol() == Symbol()
      && OrderMagicNumber() == MagicNum);
  }
  
  // =================================================
// trading direction symmetry
// =================================================
 
// -------------------------------------------------
// OrderType <--> Direction
// -------------------------------------------------
int OrderTypeDirection()
  {
   return(1 - 2 * (OrderType() % 2));
  }

int DirectionOrderType(int direction)
  {
   return(iif(direction > 0, OP_BUY, OP_SELL));
  }

bool IsOrderDirection(int direction)
  {
   return(direction == 0 || direction == OrderTypeDirection());
  }
  
// -------------------------------------------------
// Color Open / Close
// -------------------------------------------------
color ColorOpen(int direction)
  {
   return(iif(direction > 0, Green, Red));
  }

color ColorClose(int direction)
  {
   return(Violet);
  }
  
// -------------------------------------------------
// PriceOpen / Close
// -------------------------------------------------

double PriceOpen(int direction)
  {
   return(iif(direction > 0, Ask, Bid));
  }  

double PriceClose(int direction)
  {
   return(iif(direction > 0, Bid, Ask));
  }  

// =================================================
// logical
// =================================================

int iif(bool condition, int ifTrue, int ifFalse)
  {
   if(condition) return(ifTrue);
   return(ifFalse);
  }
  
double iif(bool condition, double ifTrue, double ifFalse)
  {
   if(condition) return(ifTrue);
   return(ifFalse);
  }
 
string iifStr(bool condition, string ifTrue, string ifFalse)
  {
   if(condition) return(ifTrue);
   return(ifFalse);
  }
  
  // =================================================
// date / time
// =================================================
      
// -------------------------------------------------
// Order Ages 
// -------------------------------------------------

double OrderAgePeriods()
  {
   return(OrderAgeSeconds() / PeriodSecond());
  }

double OrderAgeDays()
  {
   return(OrderAgeHours() / 24);
  }
         
double OrderAgeHours()
  {
   return(OrderAgeMinutes() / 60);
  }
            
double OrderAgeMinutes()
  {
   return(OrderAgeSeconds() / 60);
  }
         
double OrderAgeSeconds()
  {
   return(TimeCurrent() - OrderOpenTime());
  }              

int PeriodSecond()
  {
   return(Period() * 60);
  }
  
// -------------------------------------------------
// Names of objects
// -------------------------------------------------
string OrderTypeName(int OrdType)
  {
   switch(OrdType)
     {
      case OP_BUY:         return("BUY");
      case OP_SELL:        return("SELL");
      case OP_BUYLIMIT:    return("BUYLIMIT");
      case OP_SELLLIMIT:   return("SELLLIMIT");
      case OP_BUYSTOP:     return("BUYSTOP");
      case OP_SELLSTOP:    return("SELLSTOP");
		default:		         return("UnknownOrder");
     }
  }

string PeriodName(int PerNum)
  {
	switch(PerNum)
     {
		case PERIOD_M1:    return("M1");
		case PERIOD_M5:    return("M5");
		case PERIOD_M15:   return("M15");
		case PERIOD_M30:   return("M30");
		case PERIOD_H1:    return("H1");
		case PERIOD_H4:    return("H4");
		case PERIOD_D1:    return("D1");
		case PERIOD_W1:    return("W1");
		case PERIOD_MN1:   return("M1");
		default:		       return("UnknownPeriod");
	  }
  }
int PeriodIndex(int PerNum)
  {
	switch(PerNum)
     {
		case PERIOD_M1:    return(0);
		case PERIOD_M5:    return(1);
		case PERIOD_M15:   return(2);
		case PERIOD_M30:   return(3);
		case PERIOD_H1:    return(4);
		case PERIOD_H4:    return(5);
		case PERIOD_D1:    return(6);
		case PERIOD_W1:    return(7);
		case PERIOD_MN1:   return(8);
		default:		       return(-1);
	  }
  }

// =================================================
// error handling
// =================================================

// -------------------------------------------------
// IsError()
// -------------------------------------------------
bool IsError(string Whose="Raptor V1")  
  {
   int ierr = GetLastError(); 
 //bool result = (ierr!= 0);
   bool result = (ierr > 1);
   if(result) Print(Whose, " error = ", ierr, "; desc = ", ErrorDescription(ierr));
   return(result);
  }
  
//+------------------------------------------------------------------+
//| return error description                                         |
//+------------------------------------------------------------------+
string ErrorDescription(int error_code)
  {
   string error_string;
//----
   switch(error_code)
     {
      //---- codes returned from trade server
      case 0:
      case 1:   error_string="no error";                                                  break;
      case 2:   error_string="common error";                                              break;
      case 3:   error_string="invalid trade parameters";                                  break;
      case 4:   error_string="trade server is busy";                                      break;
      case 5:   error_string="old version of the client terminal";                        break;
      case 6:   error_string="no connection with trade server";                           break;
      case 7:   error_string="not enough rights";                                         break;
      case 8:   error_string="too frequent requests";                                     break;
      case 9:   error_string="malfunctional trade operation (never returned error)";      break;
      case 64:  error_string="account disabled";                                          break;
      case 65:  error_string="invalid account";                                           break;
      case 128: error_string="trade timeout";                                             break;
      case 129: error_string="invalid price";                                             break;
      case 130: error_string="invalid stops";                                             break;
      case 131: error_string="invalid trade volume";                                      break;
      case 132: error_string="market is closed";                                          break;
      case 133: error_string="trade is disabled";                                         break;
      case 134: error_string="not enough money";                                          break;
      case 135: error_string="price changed";                                             break;
      case 136: error_string="off quotes";                                                break;
      case 137: error_string="broker is busy (never returned error)";                     break;
      case 138: error_string="requote";                                                   break;
      case 139: error_string="order is locked";                                           break;
      case 140: error_string="long positions only allowed";                               break;
      case 141: error_string="too many requests";                                         break;
      case 145: error_string="modification denied because order too close to market";     break;
      case 146: error_string="trade context is busy";                                     break;
      case 147: error_string="expirations are denied by broker";                          break;
      case 148: error_string="amount of open and pending orders has reached the limit";   break;
      //---- mql4 errors
      case 4000: error_string="no error (never generated code)";                          break;
      case 4001: error_string="wrong function pointer";                                   break;
      case 4002: error_string="array index is out of range";                              break;
      case 4003: error_string="no memory for function call stack";                        break;
      case 4004: error_string="recursive stack overflow";                                 break;
      case 4005: error_string="not enough stack for parameter";                           break;
      case 4006: error_string="no memory for parameter string";                           break;
      case 4007: error_string="no memory for temp string";                                break;
      case 4008: error_string="not initialized string";                                   break;
      case 4009: error_string="not initialized string in array";                          break;
      case 4010: error_string="no memory for array\' string";                             break;
      case 4011: error_string="too long string";                                          break;
      case 4012: error_string="remainder from zero divide";                               break;
      case 4013: error_string="zero divide";                                              break;
      case 4014: error_string="unknown command";                                          break;
      case 4015: error_string="wrong jump (never generated error)";                       break;
      case 4016: error_string="not initialized array";                                    break;
      case 4017: error_string="dll calls are not allowed";                                break;
      case 4018: error_string="cannot load library";                                      break;
      case 4019: error_string="cannot call function";                                     break;
      case 4020: error_string="expert function calls are not allowed";                    break;
      case 4021: error_string="not enough memory for temp string returned from function"; break;
      case 4022: error_string="system is busy (never generated error)";                   break;
      case 4050: error_string="invalid function parameters count";                        break;
      case 4051: error_string="invalid function parameter value";                         break;
      case 4052: error_string="string function internal error";                           break;
      case 4053: error_string="some array error";                                         break;
      case 4054: error_string="incorrect series array using";                             break;
      case 4055: error_string="custom indicator error";                                   break;
      case 4056: error_string="arrays are incompatible";                                  break;
      case 4057: error_string="global variables processing error";                        break;
      case 4058: error_string="global variable not found";                                break;
      case 4059: error_string="function is not allowed in testing mode";                  break;
      case 4060: error_string="function is not confirmed";                                break;
      case 4061: error_string="send mail error";                                          break;
      case 4062: error_string="string parameter expected";                                break;
      case 4063: error_string="integer parameter expected";                               break;
      case 4064: error_string="double parameter expected";                                break;
      case 4065: error_string="array as parameter expected";                              break;
      case 4066: error_string="requested history data in update state";                   break;
      case 4099: error_string="end of file";                                              break;
      case 4100: error_string="some file error";                                          break;
      case 4101: error_string="wrong file name";                                          break;
      case 4102: error_string="too many opened files";                                    break;
      case 4103: error_string="cannot open file";                                         break;
      case 4104: error_string="incompatible access to a file";                            break;
      case 4105: error_string="no order selected";                                        break;
      case 4106: error_string="unknown symbol";                                           break;
      case 4107: error_string="invalid price parameter for trade function";               break;
      case 4108: error_string="invalid ticket";                                           break;
      case 4109: error_string="trade is not allowed in the expert properties";            break;
      case 4110: error_string="longs are not allowed in the expert properties";           break;
      case 4111: error_string="shorts are not allowed in the expert properties";          break;
      case 4200: error_string="object is already exist";                                  break;
      case 4201: error_string="unknown object property";                                  break;
      case 4202: error_string="object is not exist";                                      break;
      case 4203: error_string="unknown object type";                                      break;
      case 4204: error_string="no object name";                                           break;
      case 4205: error_string="object coordinates error";                                 break;
      case 4206: error_string="no specified subwindow";                                   break;
      default:   error_string="unknown error";
     }
//----
   return(error_string);
  }
  
void ShowClock()
  {
//---
      switch(Period())
      {      
      case 1:      
         {         
            int a=60-Seconds();         
            Comment(a);         
            break;      
         }      
      case 5:      
         {         
            int a=Period()*60-Minute()%5*60-Seconds();         
            Comment(a/60,":",a%60);         
            break;      
         }            
      case 15:      
         {         
            int a=Period()*60-Minute()%15*60-Seconds();         
            Comment(a/60,":",a%60);         
            break;      
         }      
      case 30:       
         {         
            int a=Period()*60-Minute()%30*60-Seconds();         
            Comment(a/60,":",a%60);         
            break;      
         }        
      case 60:      
         {         
            int a=Period()*60-Minute()*60-Seconds();         
            Comment(a/60,":",a%60);         
            break;      
         }      
      case 240:      
         {        
            int a=Period()*60-Hour()%4*3600-Minute()*60-Seconds();         
            Comment(a/3600,":",a%3600/60,":",a%3600%60);         
            break;      
         }      
      case 1440:      
         {         
            int a=Period()*60-Hour()*3600-Minute()*60-Seconds();         
            Comment(a/3600,":",a%3600/60,":",a%60);         
            break;      
         }      
      case 10080:      
         {         
            int a=Period()*60-TimeDayOfWeek(TimeCurrent())*1440*60-Hour()*3600-Minute()*60-Seconds();
            Comment(a/86400,":",a%86400/3600,":",a%86400%3600/60,":",a%86400%3600%60);         
            break;      
         }      
      case 43200:      
         {         
            int a=Period()*60-TimeDay(TimeCurrent())*1440*60-Hour()*3600-Minute()*60-Seconds();         
            Comment(a/86400,":",a%86400/3600,":",a%86400%3600/60,":",a%86400%3600%60);         
            break;      
         }      
      default:      
         {         
            string c="failed！！！";         
            Comment(c);        
            break;      
         }         
      }   
  }