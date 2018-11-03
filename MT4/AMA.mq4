//+------------------------------------------------------------------+
//|                                                        AMA.mq4 |
//|                      Copyright ?2005, MetaQuotes Software Corp. |
//|                                       http://www.metaquotes.net/ |
//+------------------------------------------------------------------+
#property copyright "Copyright ?2005, MetaQuotes Software Corp."
#property link      "http://www.metaquotes.net/"

#property indicator_chart_window
#property indicator_buffers 1
#property indicator_color1 Yellow

//---- indicator parameters
extern int    AMAPeriod=10;
extern double    AMASlowest=60;
extern double    AMAFastest=2;

//---- buffers
double AMABuffer[];
double RETBuffer[];
double DMABuffer[];
//+------------------------------------------------------------------+
//| Custom indicator initialization function                         |
//+------------------------------------------------------------------+
int init()
  {
//---- indicators
   IndicatorBuffers(3);
   SetIndexStyle(0,DRAW_LINE);
   SetIndexBuffer(0, AMABuffer);
   SetIndexBuffer(1, RETBuffer);
   SetIndexBuffer(2, DMABuffer);
//----
   SetIndexDrawBegin(0,AMAPeriod);
//----
   return(0);
  }
//+------------------------------------------------------------------+
//| Bollinger Bands                                                  |
//+------------------------------------------------------------------+
int start()
  {
   int    i,counted_bars=IndicatorCounted();
//----
   if(Bars<=AMAPeriod) return(0);
//---- initial zero
   if(counted_bars<1)
      for(i=1;i<=AMAPeriod;i++)
        {
         AMABuffer[Bars-i]=EMPTY_VALUE;
         RETBuffer[Bars-i]=EMPTY_VALUE;
         DMABuffer[Bars-i]=EMPTY_VALUE;
        }
//----
   int limit=Bars-counted_bars;
   if (limit > 0)
   {
      int startPos = 0;
      if(counted_bars==0)
      {
         AMABuffer[limit - 1] = iClose(NULL, 0, limit - 1);
         startPos = limit - 2;
      }
      else
      {
         startPos = limit - 1;
      }
      
      for (i = startPos; i >= 0; i--)
      {
         RETBuffer[i] = iClose(NULL, 0, i) - iClose(NULL, 0, i+1);
      }
      
      //Print("Bars is " + Bars);
      
      for (i = startPos; i >= 0; i--)
      {
         if (i + AMAPeriod + 1 < Bars)
         {
            double dir1 = MathAbs(iClose(NULL,0,i) - iClose(NULL,0,i+AMAPeriod));
            double vir1 = 0;
            for (int j = 0; j < AMAPeriod; j++)
            {
               vir1 = vir1 + MathAbs(RETBuffer[i+j]);
            }
            double er1 = dir1 / vir1;
            double slow1 = 2/(AMASlowest+1);
            double fast1 = 2/(AMAFastest+1);
            double cs1 = er1*(fast1-slow1)+slow1;
            double cq1 = cs1*cs1;
            DMABuffer[i] = cq1*iClose(NULL,0,i) + (1-cq1)*DMABuffer[i+1];       
            AMABuffer[i] = (DMABuffer[i]*2+AMABuffer[i+1])/3;
         }
      }
   }
//----
   return(0);
  }
//+------------------------------------------------------------------+