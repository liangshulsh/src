import DataSourceHandler
from WebGrabberNasdaq import WebGrabberNasdaq
from MarketData import MarketData
import pandas
import datetime
import math

class NasdaqDataSourceHandler(DataSourceHandler.DataSourceHandler):
    """description of class"""
    def __init__(self, asofdate, type, logger):
        super(NasdaqDataSourceHandler,self).__init__(asofdate, type, "nasdaq", logger)

    def convertNameToTicker(self, name):
        return name.strip()

    def convertTickerToName(self, ticker):
        return ticker.strip()

    def loadUSSecurityList(self):
        try:                    
            web = WebGrabberNasdaq()
            self.logger.info("get all US Stock List from Nasdaq website.")
            stocklist = web.loadAllStockList()
            self.logger.info("get all US ETF List from Nasdaq website.")
            etflist = web.loadAllETFList()

            if (stocklist is not None):
                self.logger.info("upsert US Stock List to Research:{0} records".format(len(stocklist["Name"])))
                self.getResearch().upsertUSStockList(stocklist)
            if (etflist is not None):
                self.logger.info("upsert US ETF List to Research:{0} records".format(len(etflist["Name"])))
                self.getResearch().upsertUSETFList(etflist)
        except Exception as ex:
            self.logger.error(ex)
            raise ex
