import pandas as pd
from DataModelBase import DataModelBase
import datetime

class Research(DataModelBase):
    """description of class"""
    ResearchConnectionString = 'DRIVER={SQL Server Native Client 11.0};SERVER=LAPTOP-N2OTR2I1\\SKYWOLFDB;DATABASE=Research;UID=liangshu;PWD=optimus'

    def __init__(self):
        super(Research, self).__init__(self.ResearchConnectionString)

    def upsertUSStockList(self, stocklist):
        if isinstance(stocklist, pd.DataFrame) and len(stocklist.index) > 0:
            delQuery = "delete from [Research].[sec].[Stock_USStockList_Nasdaq] where AsOfDate = '{0}'".format(stocklist['AsOfDate'][0].strftime("%Y-%m-%d"))
            self.execute(delQuery)
            self.insert(stocklist, '[Research].[sec].[Stock_USStockList_Nasdaq]')

    def upsertUSETFList(self, etflist):
        if isinstance(etflist, pd.DataFrame) and len(etflist.index) > 0:
            delQuery = "delete from [Research].[sec].[Stock_USETFList_Nasdaq] where AsOfDate = '{0}'".format(etflist['AsOfDate'][0].strftime("%Y-%m-%d"))
            self.execute(delQuery)
            self.insert(etflist, '[Research].[sec].[Stock_USETFList_Nasdaq]')

    def getNewETFs(self, asofdate):
        query = """SELECT [AsOfDate]
      ,[Name]
      ,[FullName]
      ,[LastPrice]
      ,[NetChange]
      ,[PercentChange]
      ,[PercentChange1Y]
      ,[TS]
  FROM [Research].[sec].[Stock_USETFList_Nasdaq]
  where AsOfDate = '{0}' and Name not in (select [Name] from MarketData.sec.Stock)""".format(asofdate.strftime("%Y-%m-%d"))
        return self.read(query)

    def getNewStocks(self, asofdate):
        query = """SELECT [AsOfDate]
      ,[Name]
      ,[FullName]
      ,[LastPrice]
      ,[MarketCap]
      ,[IPOyear]
      ,[Sector]
      ,[Industry]
      ,[SummaryQuote]
      ,[TS]
  FROM [Research].[sec].[Stock_USStockList_Nasdaq]
    where AsOfDate = '{0}' and Name not in (select [Name] from MarketData.sec.Stock)""".format(asofdate.strftime("%Y-%m-%d"))
        return self.read(query)