from DataSourceHandler import DataSourceHandler
from GoogleDataSourceHandler import GoogleDataSourceHandler
from YahooDataSourceHandler import YahooDataSourceHandler
from NasdaqDataSourceHandler import NasdaqDataSourceHandler
from MarketData import MarketData
import pandas
import datetime
import logging
import sys

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
        self.source["nasdaq"] = NasdaqDataSourceHandler(asofdate, type, self.logger)

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

    def loadsecurities(self):
        # load US Security

        asofdate = datetime.datetime.today()
        datasource = self.source["nasdaq"]
        researchModel = datasource.getResearch()
        datasource.loadUSSecurityList()

        #newETFs = researchModel.getNewETFs(asofdate)
        #if newETFs is not None:
        #    self.logger.info("new ETFs: {0} records".format(len(newETFs.Index)))
        #    rowcnt = len(newETFs.Index)
        #    rowidx = 0
        #    if rowcnt > 0:
        #        for row in newETFs.values.tolist():
        #            rowidx += 1
        #            try:
        #                self.logger.info("create ETF {0}|{3} ({1}/{2})".format(row['Name'], rowidx, rowcnt, row['FullName']))
        #                self.createStock(row['Name'], row['FullName'], 'ETF', 'US', 'USD', None, None, None)
        #            except Exception as ex:
        #                self.logger.error(ex)

        #newStocks = researchModel.getNewStocks(asofdate)
        #if newStocks is not None:
        #    self.logger.info("new Stocks: {0} records".format(len(newStocks.Index)))
        #    rowcnt = len(newStocks.Index)
        #    rowidx = 0
        #    if rowcnt > 0:
        #        for row in newStocks.values.tolist():
        #            rowidx += 1
        #            try:
        #                self.logger.info("create Stock {0}|{3} ({1}/{2})".format(row['Name'], rowidx, rowcnt, row['FullName']))
        #                self.createStock(row['Name'], row['FullName'], 'Stock', 'US', 'USD', 'Equity', row['Sector'], row['Industry'])
        #            except Exception as ex:
        #                self.logger.error(ex)

    #def createStock(self, name, fullname, subtype, country, currency, assetclass, sector, industry):
    #    datasource = self.source["nasdaq"]
    #    marketDataModel = datasource.getMarketData()
    #    startdate = datetime.datetime(1900,1,1)


    #    stocks = marketDataModel.createStock(name, fullname, subtype, country, currency, issuedate, assetclass, sector, industry):
    def run(self, sources=None, sids=None):
        model = None
        try:
            self.logger.info("log file name:" + self.logfilename)
            self.logger.info("type:" + self.type)
            self.logger.info("asofdate:" + self.asofdate.strftime("%Y-%m-%d"))

            model = self.source["yahoo"].getMarketData()
            self.logger.info("Load priceing rules.")
            priceRules = model.getPricingRulesObj(True, sources, sids)
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
    asofdate = datetime.datetime.today()
    if (len(sys.argv) > 1):
        asofdate = datetime.datetime.strptime(sys.argv[1], "%Y%m%d")
    #engine = PriceEngine('full', asofdate)
    #engine.run(sids = [200000007,200000006], sources=['yahoo'])

    engine = PriceEngine('refresh', asofdate)
    engine.loadsecurities()
    engine.run()