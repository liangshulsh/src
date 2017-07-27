import pandas
import datetime
import MarketData
import Research

class DataSourceHandler(object):
    marketData = None
    research = None

    def __init__(self, asofdate, type, datasource,logger):
        self.type = type
        self.asofdate = asofdate
        self.datasource = datasource
        self.logger = logger

    def getMarketData(self):
        if (DataSourceHandler.marketData == None):
            DataSourceHandler.marketData = MarketData.MarketData()
            DataSourceHandler.marketData.open()
        return DataSourceHandler.marketData

    def getResearch(self):
        if (DataSourceHandler.research == None):
            DataSourceHandler.research = Research.Research()
            DataSourceHandler.research.open()
        return DataSourceHandler.research


