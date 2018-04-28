import pandas as pd
from DataModelBase import DataModelBase
import datetime

class MarketData(DataModelBase):
    #MarketDataConnectionString = 'DRIVER={SQL Server Native Client 11.0};SERVER=LAPTOP-N2OTR2I1\\SKYWOLFDB;DATABASE=MarketData;UID=liangshu;PWD=optimus'
    MarketDataConnectionString = 'DRIVER={SQL Server Native Client 11.0};SERVER=SKYWOLFSERVER1;DATABASE=MarketData;UID=liangshu;PWD=optimus'

    def __init__(self):
        super(MarketData, self).__init__(self.MarketDataConnectionString)

    def getPricingRulesObj(self, active, sources = None, sids = None):
        query = "select * from [price].[vw_PricingRule]"
        if active or sources is not None or sids is not None:
            query += " where "
            conditions = []
            if active:
                conditions.append("Active = 1")
            if sources is not None:
                conditions.append("DataSource in (" + ",".join(["'" + s + "'" for s in sources]) + ")")
            if sids is not None:
                conditions.append("SID in (" + ",".join([str(s) for s in sids]) + ")")
            query += " and ".join(conditions)

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

    def createStock(self, name, fullname, subtype, country, currency, issuedate, assetclass, sector, industry):
        assetclassvalue = 'null'
        sectorvalue = 'null'
        industryvalue = 'null'

        if assetclass is not None:
            assetclassvalue = "'{0}'".format(assetclass)
        if sector is not None:
            sectorvalue = "'{0}'".format(sector)
        if industry is not None:
            industryvalue = "'{0}'".format(industry)
        
        startdate = "null"
        if issuedate is not None:
            startdate = "'" + issuedate.strftime("%Y-%m-%d") + "'"

        insertquery = """INSERT INTO [sec].[Stock]
           ([Name]
           ,[FullName]
           ,[SubType]
           ,[Country]
           ,[Currency]
           ,[IssueDate]
           ,[AssetClass]
           ,[Sector]
           ,[Industry]) values ('{0}','{1}','{2}','{3}','{4}',{5},{6},{7},{8})""".format(name, fullname, subtype, country, currency, startdate, assetclassvalue, sectorvalue, industryvalue)
        self.execute(insertquery)
        selectquery = """SELECT [SID]
      ,[Name]
      ,[FullName]
      ,[SubType]
      ,[Country]
      ,[Currency]
      ,[IssueDate]
      ,[Usr]
      ,[TS]
      ,[AssetClass]
      ,[Sector]
      ,[Industry]
  FROM [MarketData].[sec].[Stock]
  where Name = '{0}'""".format(name)
        stocks = self.readObjects(selectquery,"Stock")
        if stocks is not None and len(stocks) > 0:
            return stocks[0]
        return None

    def createTicker(self, sid, source, ticker):
        insertquery = """
            delete from [sec].[InstrumentID] where [SID] = {0} and [DataSource] = '{1}' and [IDTypeId] = 1
            INSERT INTO [sec].[InstrumentID]
           ([SID]
           ,[DataSource]
           ,[IDTypeId]
           ,[IDValue]) values ({0},'{1}',1,'{2}')""".format(sid, source, ticker)
        self.execute(insertquery)
        
    def createPricingRule(self, sid, asofdate, source, timezone, active):
        insertquery = """
           delete from [price].[PricingRule] where [SID] = {0} and [DataSource] = '{2}' and [TimeZone] = '{3}'
           INSERT INTO [price].[PricingRule]
           ([SID]
           ,[AsOfDate]
           ,[DataSource]
           ,[TimeZone]
           ,[Active])
     VALUES
           ({0},'{1}','{2}','{3}',{4})""".format(sid, asofdate.strftime("%Y-%m-%d"), source, timezone, active)
        self.execute(insertquery)

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
    