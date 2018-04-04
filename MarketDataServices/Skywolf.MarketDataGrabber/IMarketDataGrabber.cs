using Skywolf.Contracts.DataContracts.MarketData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.MarketDataGrabber
{
    public interface IMarketDataGrabber
    {
        TimeSeriesDataOutput GetTimeSeriesData(TimeSeriesDataInput input);
        Quote[] StockBatchQuote(IEnumerable<string> symbols);
    }
}
