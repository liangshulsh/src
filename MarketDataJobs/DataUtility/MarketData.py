import pandas as pd
from DataModelBase import DataModelBase
import datetime

class MarketData(DataModelBase):
    MarketDataConnectionString = 'DRIVER={SQL Server Native Client 11.0};SERVER=LAPTOP-N2OTR2I1\\SKYWOLFDB;DATABASE=MarketData;UID=liangshu;PWD=optimus'

    def __init__(self):
        super(MarketData, self).__init__(self.MarketDataConnectionString)

    def getPricingRulesObj(self, active):
        query = "select * from [price].[vw_PricingRule]"
        if active:
            query += " where Active = 1"
        return self.readObjects(query, "PricingRule")

    def getStockTermsObj(self, sids):
        query = """select [SID]
                  ,[Name]
                  ,[FullName]
                  ,[SubType]
                  ,[Country]
                  ,[Currency]
                  ,[IssueDate]
                  ,[Usr]
                  ,[TS]
                  ,[AssetClass]
              FROM [MarketData].[sec].[Stock] where SID in ({0})""".format(",".join([str(sid) for sid in sids]))
        return self.readObjects(query, "Stock")

    def getLatestAdjClosePrice(self, sids):
        query = """select [SID]
                  ,[AsOfDate]
                  ,[Value] from
            (SELECT [SID]
                  ,[AsOfDate]
                  ,[Value]
	              ,ROW_NUMBER() over(PARTITION BY [SID] ORDER BY AsOfDate DESC) Num
              FROM [MarketData].[price].[AdjPrices]
              where PriceTypeId = 1) as  t
              where Num = 1 and [SID] in ({0})""".format(",".join([str(sid) for sid in sids]))
        return self.readObjects(query, "LatestPrice")

    def getLatestClosePrice(self, sids):
        query = """select [SID]
                  ,[AsOfDate]
                  ,[Value] from
            (SELECT [SID]
                  ,[AsOfDate]
                  ,[Value]
	              ,ROW_NUMBER() over(PARTITION BY [SID] ORDER BY AsOfDate DESC) Num
              FROM [MarketData].[price].[Prices]
              where PriceTypeId = 1) as  t
              where Num = 1 and [SID] in ({0})""".format(",".join([str(sid) for sid in sids]))
        return self.readObjects(query, "LatestPrice")

    def upsertStockDividend(self, sid, dividends):
        if isinstance(dividends, pd.DataFrame) and len(dividends.index) > 0:
            minDate = dividends['date'].min()
            maxDate = dividends['date'].max()
            delQuery = "delete from [MarketData].[sec].[Stock_Dividend] where SID = {0} and AsOfDate >= '{1}' and AsOfDate <= '{2}'".format(sid, minDate.strftime("%Y-%m-%d"), maxDate.strftime("%Y-%m-%d"))
            self.execute(delQuery)
            divs = {'SID' : [], 'AsOfDate' : [], 'Amount' : [] }
            divsDict = {}
            if (isinstance(dividends, pd.DataFrame)):
                divsDict = dividends.to_dict(orient='list')
            else:
                divsDict = dividends

            for idx, dDate in enumerate(divsDict['date']):
                strDate = dDate.strftime("%Y-%m-%d")
                divs['SID'].append(sid)
                divs['AsOfDate'].append(strDate)
                divs['Amount'].append(float(divsDict['amount'][idx]))

            self.insert(pd.DataFrame(divs), '[MarketData].[sec].[Stock_Dividend]')

    def upsertStockSplit(self, sid, splits):
        if isinstance(splits, pd.DataFrame) and len(splits.index) > 0:
            minDate = splits['date'].min()
            maxDate = splits['date'].max()
            delQuery = "delete from [MarketData].[sec].[Stock_Split] where SID = {0} and AsOfDate >= '{1}' and AsOfDate <= '{2}'".format(sid, minDate.strftime("%Y-%m-%d"), maxDate.strftime("%Y-%m-%d"))
            self.execute(delQuery)
            stockSplits = {'SID' : [], 'AsOfDate' : [], 'Numerator' : [], 'Denominator' : [], 'SplitRatio' : []}
            splitsDict = {}
            if (isinstance(splits, pd.DataFrame)):
                splitsDict = splits.to_dict(orient='list')
            else:
                splitsDict = splits

            for idx, dDate in enumerate(splitsDict['date']):
                strDate = dDate.strftime("%Y-%m-%d")
                stockSplits['SID'].append(sid)
                stockSplits['AsOfDate'].append(strDate)
                stockSplits['Numerator'].append(float(splitsDict['numerator'][idx]))
                stockSplits['Denominator'].append(float(splitsDict['denominator'][idx]))
                stockSplits['SplitRatio'].append(str(splitsDict['splitRatio'][idx]))

            self.insert(pd.DataFrame(stockSplits), '[MarketData].[sec].[Stock_Split]')

    def upsertAdjPrices(self, sid, stockPrices):
        self.upsertPricesTable(sid, stockPrices, '[MarketData].[price].[AdjPrices]')

    def upsertPrices(self, sid, stockPrices):
        self.upsertPricesTable(sid, stockPrices, '[MarketData].[price].[Prices]')

    def upsertPricesTable(self, sid, stockPrices, tablename):
        if isinstance(stockPrices, pd.DataFrame) and len(stockPrices.index) > 0:
            minDate = stockPrices['date'].min()
            maxDate = stockPrices['date'].max()
            delQuery = "delete from {0} where SID = {3} and AsOfDate >= '{1}' and AsOfDate <= '{2}' and PriceTypeId in (1,2,3,4,6)".format(tablename, minDate.strftime("%Y-%m-%d"), maxDate.strftime("%Y-%m-%d"), sid)
            self.execute(delQuery)
            prices = {'SID' : [], 'AsOfDate' : [], 'PriceTypeId' : [], 'Value' : [] }

            pircesDict = {}
            if (isinstance(stockPrices, pd.DataFrame)):        
                pricesDict = stockPrices.to_dict(orient='list')
            else:
                pricesDict = stockPrices

            for idx, dDate in enumerate(pricesDict['date']):
                strDate = dDate.strftime("%Y-%m-%d")
                if (pricesDict['close'][idx] != None and pricesDict['open'][idx] != None and
                    pricesDict['high'][idx] != None and pricesDict['low'][idx] != None and
                    pricesDict['volume'][idx] != None):
                    prices['SID'].append(sid)
                    prices['AsOfDate'].append(strDate)
                    prices['PriceTypeId'].append(1)
                    prices['Value'].append(float(pricesDict['close'][idx]))
                    prices['SID'].append(sid)
                    prices['AsOfDate'].append(strDate)
                    prices['PriceTypeId'].append(2)
                    prices['Value'].append(float(pricesDict['open'][idx]))
                    prices['SID'].append(sid)
                    prices['AsOfDate'].append(strDate)
                    prices['PriceTypeId'].append(3)
                    prices['Value'].append(float(pricesDict['high'][idx]))
                    prices['SID'].append(sid)
                    prices['AsOfDate'].append(strDate)
                    prices['PriceTypeId'].append(4)
                    prices['Value'].append(float(pricesDict['low'][idx]))
                    prices['SID'].append(sid)
                    prices['AsOfDate'].append(strDate)
                    prices['PriceTypeId'].append(6)
                    prices['Value'].append(float(pricesDict['volume'][idx]))
            self.insert(pd.DataFrame(prices), tablename)

if __name__ == '__main__':
    a = MarketData()
    a.open()
    #result = a.executeQuery('select * from MarketData.sec.Stock')
    instIds = pd.DataFrame({ 'SID' : [1,2,3,4,5],
                    'DataSource' : ['yahoo','yahoo','yahoo','yahoo','yahoo'],
                    'IDTypeId' : [1,1,1,1,1],
                    'IDValue' : ['MSFT','Yahoo','FB','Google','AMZN'],
                    'Usr': 'shu.liang',
                    'TS': datetime.datetime.now()})
    instIds = instIds[['SID','DataSource','IDTypeId','IDValue','Usr','TS']]
    
    print(instIds)
    a.insert(instIds, '[MarketData].[sec].[InstrumentID]')
    a.close()
    