import DataSourceHandler
from WebGrabberGoogle import WebGrabberGoogle
import datetime
import pandas

class GoogleDataSourceHandler(DataSourceHandler.DataSourceHandler):
    """description of class"""
    def __init__(self, asofdate, type, logger):
        super(GoogleDataSourceHandler, self).__init__(asofdate, type, "google", logger)

    def convertNameToTicker(self, name):
        if (name is not None and '^' not in name):
            return name.strip()
        return None

    def convertTickerToName(self, ticker):
        if (ticker is not None):
            return ticker.strip()
        return None

    def loadPrices(self, priceRule, instrument):
        try:
            if priceRule.Ticker != None:
                self.logger.info("{0} by {1} is loading prices".format(priceRule.Ticker, self.datasource))
                startdate = datetime.datetime(1900,1,1)
                if instrument.IssueDate is not None:
                    startdate = instrument.IssueDate
                if (self.type == "refresh" and instrument.LatestPriceDate is not None):
                    startdate = instrument.LatestPriceDate

                web = WebGrabberGoogle()
                web.loadHistoricalData(datetime.datetime(startdate.year, startdate.month, startdate.day), self.asofdate, priceRule.Ticker)
                if (web.historicalPrices != None):
                    self.logger.info("upsert historical prices:{0} records".format(len(web.historicalPrices["date"])))
                    self.getMarketData().upsertPrices(priceRule.SID, pandas.DataFrame(web.historicalPrices))
        except Exception as ex:
            self.logger.error(ex)
            raise ex
