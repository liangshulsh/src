import pandas
import datetime
import MarketData

class DataSourceHandler(object):
    marketData = None
    def __init__(self, asofdate, type, datasource,logger):
        self.type = type
        self.asofdate = asofdate
        self.datasource = datasource
        self.logger = logger
        self.getMarketData()

    def getMarketData(self):
        if (DataSourceHandler.marketData == None):
            DataSourceHandler.marketData = MarketData.MarketData()
            DataSourceHandler.marketData.open()
        return DataSourceHandler.marketData



