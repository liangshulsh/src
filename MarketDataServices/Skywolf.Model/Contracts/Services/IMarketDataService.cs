using Skywolf.Contracts.DataContracts.MarketData;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

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
        TimeSeriesDataOutput GetTimeSeriesData(TimeSeriesDataInput input, string datasource);

        [OperationContract]
        Quote[] GetStockBatchQuote(string[] symbols, string datasource);

        [OperationContract]
        IDictionary<string, StockBar[]> GetStockHistoryPrices(string[] symbols, BarFrequency frequency, DateTime? startDate, DateTime? endDate, int outputCount, bool isAdjustedValue, string datasource);

        [OperationContract]
        IDictionary<string, CryptoBar[]> GetCryptoHistoryPrices(string[] symbols, string market, BarFrequency frequency, DateTime? startDate, DateTime? endDate, int outputCount, string datasource);

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
    }
}
