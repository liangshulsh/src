using Skywolf.Contracts.DataContracts.MarketData;
using Skywolf.Contracts.DataContracts.Instrument;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;
using Skywolf.Contracts.DataContracts.MarketData.TVC;

namespace Skywolf.Contracts.Services
{
    [ServiceContract(Namespace = Constants.NAMESPACE)]
    public interface IMarketDataService
    {

        [OperationContract]
        IDictionary<string, long> GetSIDFromName(string[] names);

        [OperationContract]
        IDictionary<long, string> GetNameFromSID(long[] SIDs);

        [OperationContract]
        PricingRule[] GetPricingRule(bool active, string datasource);

        [OperationContract]
        TimeSeriesDataOutput GetTimeSeriesData(TimeSeriesDataInput input, string datasource);

        [OperationContract]
        Quote[] GetStockBatchQuote(string[] symbols, string datasource);

        [OperationContract]
        IDictionary<string, StockBar[]> GetStockHistoryPrices(string[] symbols, BarFrequency frequency, DateTime? startDate, DateTime? endDate, int outputCount, bool isAdjustedValue, string datasource);

        [OperationContract]
        IDictionary<string, CryptoBar[]> GetCryptoHistoryPrices(string[] symbols, string market, BarFrequency frequency, DateTime? startDate, DateTime? endDate, int outputCount, string datasource);

        [OperationContract]
        IDictionary<string, StockBar> GetLatestStockHistoryPrices(string[] symbols, BarFrequency frequency, bool isAdjustedValue, string datasource);

        [OperationContract]
        IDictionary<string, CryptoBar> GetLatestCryptoHistoryPrices(string[] symbols, string market, BarFrequency frequency, string datasource);

        [OperationContract]
        string[] VA_GetAvailableAPIKey();

        [OperationContract]
        bool VA_StorePrices(long SID, BarFrequency frequency, bool isAdjustedValue, Bar[] bars);

        [OperationContract]
        void AV_AddAPIKeys(string[] apiKeys);

        [OperationContract]
        void AV_RemoveAPIKeys(string[] apiKeys);

        [OperationContract]
        void AV_UpdateAPIKeys(string[] apiKeys);

        [OperationContract]
        TVCHistoryResponse TVC_GetHistoricalPrices(string symbol, BarFrequency frequency, DateTime from, DateTime to);

        [OperationContract]
        TVCQuotesResponse TVC_GetQuotes(IEnumerable<string> symbols);

        [OperationContract]
        TVCSymbolResponse TVC_GetSymbolInfo(string symbol);

        [OperationContract]
        TVCSearchResponse[] TVC_GetSymbolSearch(string query, string type, string exchange, int limit = 30);

        [OperationContract]
        TVCCalendar[] TVC_GetCalendars(DateTime fromDate, DateTime toDate, string country = "", string currentTab = "custom");

        [OperationContract]
        void TVC_StoreHolidays(IEnumerable<TVCCalendar> holidays);

        [OperationContract]
        void TVC_StoreQuotes(IEnumerable<TVCQuoteResponse> quotes);

        [OperationContract]
        void TVC_StoreSymbols(IEnumerable<TVCSymbolResponse> symbols);
    }
}
