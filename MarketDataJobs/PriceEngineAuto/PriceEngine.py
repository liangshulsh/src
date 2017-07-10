from DataSourceHandler import DataSourceHandler
from GoogleDataSourceHandler import GoogleDataSourceHandler
from YahooDataSourceHandler import YahooDataSourceHandler
from MarketData import MarketData
import pandas
import datetime
import logging

#x = MarketData()
#x.open()
#priceRules = x.getPricingRulesObj(True)
#sids = [priceRule.SID for priceRule in priceRules]
#stocks = x.getStockTermsObj(sids)
#stockDict = {}
#for stock in stocks:
#    stockDict[stock.SID] = stock

#for priceRule in priceRules:
#    if priceRule.Ticker != None:
#        print("load historical prices for " + priceRule.Ticker)
#        issueDate = stockDict[priceRule.SID].IssueDate
#        web = WebGrabberYahoo()
#        web.loadHistoricalData(datetime.datetime(issueDate.year, issueDate.month, issueDate.day), datetime.datetime(2017,7,9), priceRule.Ticker)
#        if (web.historicalPrices != None):
#            print("upsert historical prices for " + priceRule.Ticker)
#            x.upsertAdjPrices(priceRule.SID, pandas.DataFrame(web.historicalPrices))
#        if (web.dividends != None):
#            print("upsert dividend for " + priceRule.Ticker)
#            x.upsertStockDividend(priceRule.SID, pandas.DataFrame(web.dividends))
#        if (web.splits != None):
#            print("upsert stock split for " + priceRule.Ticker)
#            x.upsertStockSplit(priceRule.SID, pandas.DataFrame(web.splits))

class PriceEngine(object):
    type = 'refresh'
    asofdate = datetime.datetime.today()

    def __init__(self,type,asofdate):
        self.type = type
        self.asofdate = asofdate
        self.logfilename = "priceengine_{0}_{1}.log".format(datetime.datetime.today().strftime('%Y%m%d'), int(datetime.datetime.now().timestamp()))
        self.initLogger()
        self.source = {}
        self.source["google"] = GoogleDataSourceHandler(asofdate, type, self.logger)
        self.source["yahoo"] = YahooDataSourceHandler(asofdate, type, self.logger)

    def initLogger(self):
        self.logger = logging.getLogger("PriceEngine")
        self.logger.setLevel(logging.DEBUG)

        # create console handler and set level to debug
        ch = logging.StreamHandler()
        ch.setLevel(logging.DEBUG)

        # create formatter
        formatter = logging.Formatter('%(asctime)s - %(name)s - %(levelname)s - %(message)s')

        # add formatter to ch
        ch.setFormatter(formatter)

        # add ch to logger
        self.logger.addHandler(ch)
        fh = logging.FileHandler(self.logfilename)
        fh.setLevel(logging.DEBUG)
        fh.setFormatter(formatter)
        self.logger.addHandler(fh)

    def run(self):
        model = None
        try:
            self.logger.info("log file name:" + self.logfilename)
            self.logger.info("type:" + self.type)
            self.logger.info("asofdate:" + self.asofdate.strftime("%Y-%m-%d"))
        
            model = self.source["yahoo"].getMarketData()

            self.logger.info("Load priceing rules.")
            priceRules = model.getPricingRulesObj(True)
            sids = [priceRule.SID for priceRule in priceRules]

            self.logger.info("Load stock terms.")
            stocks = model.getStockTermsObj(sids)
            stockLatestAdjPrices = model.getLatestAdjClosePrice(sids)
            stockLatestPrices = model.getLatestClosePrice(sids)
            stockDict = {}
            stockLatestAdjPriceDict = {}
            stockLatestPriceDict = {}
            if stockLatestAdjPrices != None:
                for price in stockLatestAdjPrices:
                    stockLatestAdjPriceDict[price.SID] = price
            if stockLatestPrices != None:
                for price in stockLatestPrices:
                    stockLatestPriceDict[price.SID] = price
            for stock in stocks:
                stockDict[stock.SID] = stock
                if stock.SID in stockLatestAdjPriceDict:
                    stock.LatestAdjPriceDate = stockLatestAdjPriceDict[stock.SID].AsOfDate
                    stock.LatestAdjClosePrice = stockLatestAdjPriceDict[stock.SID].Value
                else:
                    stock.LatestAdjPriceDate = None
                if stock.SID in stockLatestPriceDict:
                    stock.LatestPriceDate = stockLatestPriceDict[stock.SID].AsOfDate
                    stock.LatestClosePrice = stockLatestPriceDict[stock.SID].Value
                else:
                    stock.LatestPriceDate = None

            for priceRule in priceRules:
                if priceRule.Ticker != None:
                    self.logger.info("load historical prices for " + priceRule.Ticker)
                    instrument = stockDict[priceRule.SID]
                    self.source[priceRule.DataSource].loadPrices(priceRule, instrument)
        finally:
            if (model is not None):
                model.close()

if __name__ == '__main__':
    engine = PriceEngine('refresh', datetime.datetime(2017,7,8))
    engine.run()