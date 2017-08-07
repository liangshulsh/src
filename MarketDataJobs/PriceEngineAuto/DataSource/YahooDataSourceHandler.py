import DataSourceHandler
from WebGrabberYahoo import WebGrabberYahoo
from MarketData import MarketData
import pandas
import datetime
import math

class YahooDataSourceHandler(DataSourceHandler.DataSourceHandler):

    def __init__(self, asofdate, type, logger):
        super(YahooDataSourceHandler,self).__init__(asofdate, type, "yahoo", logger)

    def getIssueDate(self, ticker):
        try:
            web = WebGrabberYahoo()
            web.loadHistoricalData(datetime.datetime(1900,1,1), datetime.datetime.today(), ticker)
            if web.historicalPrices != None:
                return min(web.historicalPrices['date'])
            return None
        except Exception as ex:
            self.logger.error(ex)
            raise ex
    
    def convertNameToTicker(self, name):
        if (name is not None and '^' not in name):
            return name.strip().replace('.', '-').replace(' ', '.')
        return None

    def convertTickerToName(self, ticker):
        if (ticker is not None):
            return ticker.strip().replace('.', ' ').replace('-', '.')
        return None

    def loadPrices(self, priceRule, instrument):
        try:
            if priceRule.Ticker != None:
                self.logger.info("{0} by {1} is loading Adjustment prices".format(priceRule.Ticker, self.datasource))
                startdate = datetime.datetime(1900,1,1)
                if (instrument.IssueDate is not None):
                    startdate = instrument.IssueDate
                if (self.type == "refresh" and instrument.LatestAdjPriceDate != None):
                    startdate = instrument.LatestAdjPriceDate
                    
                web = WebGrabberYahoo()
                web.loadHistoricalData(datetime.datetime(startdate.year, startdate.month, startdate.day), self.asofdate, priceRule.Ticker)
                if (web.historicalPrices != None):
                    if (self.type == "refresh" and instrument.LatestAdjPriceDate is not None):
                        dates = [datetime.date(d.year,d.month,d.day) for d in web.historicalPrices['date']]
                        if (abs(web.historicalPrices['close'][dates.index(startdate)] - instrument.LatestAdjClosePrice) > 0.000001):
                            web = WebGrabberYahoo()
                            startdate = datetime.datetime(instrument.IssueDate.year, instrument.IssueDate.month, instrument.IssueDate.day)
                            self.logger.info("{0} by {1} is re-loading Adjustment prices".format(priceRule.Ticker, self.datasource))
                            web.loadHistoricalData(startdate, self.asofdate, priceRule.Ticker)
                    self.logger.info("upsert historical prices:{0} records".format(len(web.historicalPrices["date"])))
                    self.getMarketData().upsertAdjPrices(priceRule.SID, pandas.DataFrame(web.historicalPrices))
                if (web.dividends != None):
                    self.logger.info("upsert dividends:{0} records".format(len(web.historicalPrices["date"])))
                    self.getMarketData().upsertStockDividend(priceRule.SID, pandas.DataFrame(web.dividends))
                if (web.splits != None):
                    self.logger.info("upsert stock split:{0} records".format(len(web.historicalPrices["date"])))
                    self.getMarketData().upsertStockSplit(priceRule.SID, pandas.DataFrame(web.splits))
        except Exception as ex:
            self.logger.error(ex)
            raise ex
