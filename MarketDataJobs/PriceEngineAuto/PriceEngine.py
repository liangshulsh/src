from DataSourceHandler import DataSourceHandler
from GoogleDataSourceHandler import GoogleDataSourceHandler
from YahooDataSourceHandler import YahooDataSourceHandler
from NasdaqDataSourceHandler import NasdaqDataSourceHandler
from MarketData import MarketData
import pandas
import datetime
import logging
import sys

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
    
    def createnewsecurities(self,asofdate):
        datasource = self.source["nasdaq"]
        researchModel = datasource.getResearch()
        newETFs = researchModel.getNewETFs(asofdate)
        if newETFs is not None:
            self.logger.info("new ETFs: {0} records".format(len(newETFs.index)))
            rowcnt = len(newETFs.index)
            rowidx = 0
            if rowcnt > 0:
                for row in newETFs.values.tolist():
                    rowidx += 1
                    try:
                        self.logger.info("create ETF {0}|{3} ({1}/{2})".format(row[1], rowidx, rowcnt, row[2]))
                        self.createStock(row[1], row[2], 'ETF', 'US', 'USD', None, None, None)
                    except Exception as ex:
                        self.logger.error(ex)

        newStocks = researchModel.getNewStocks(asofdate)
        if newStocks is not None:
            self.logger.info("new Stocks: {0} records".format(len(newStocks.index)))
            rowcnt = len(newStocks.index)
            rowidx = 0
            if rowcnt > 0:
                for row in newStocks.values.tolist():
                    rowidx += 1
                    try:
                        if (row[1] is not None and '^' not in row[1]):
                            self.logger.info("create Stock {0}|{3} ({1}/{2})".format(row[1], rowidx, rowcnt, row[2]))
                            self.createStock(row[1].strip(), row[2], 'Stock', 'US', 'USD', 'Equity', row[6], row[7])
                    except Exception as ex:
                        self.logger.error(ex)

    def createStock(self, name, fullname, subtype, country, currency, assetclass, sector, industry):
        nasdaq = self.source["nasdaq"]
        yahoo = self.source["yahoo"]
        google = self.source["google"]
        marketDataModel = nasdaq.getMarketData()
        issuedate = yahoo.getIssueDate(yahoo.convertNameToTicker(name))
        stock = marketDataModel.createStock(name, fullname, subtype, country, currency, issuedate, assetclass, sector, industry)
        if (stock is not None):
            for s in self.source.values():
                t = s.convertNameToTicker(stock.Name)
                if t is not None:
                    marketDataModel.createTicker(stock.SID, s.datasource, t)

            isactive = 1
            if (issuedate is None):
                isactive = 0
            marketDataModel.createPricingRule(stock.SID, datetime.datetime.today(), "yahoo", country, isactive)
            marketDataModel.createPricingRule(stock.SID, datetime.datetime.today(), "google", country, isactive)            
        return stock

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

            ruleCnt = 1
            for priceRule in priceRules:
                if priceRule.Ticker != None:
                    try:
                        self.logger.info("({1}/{2})load historical prices for {0}".format(priceRule.Ticker, ruleCnt, len(priceRules)))
                        instrument = stockDict[priceRule.SID]
                        self.source[priceRule.DataSource].loadPrices(priceRule, instrument)
                    except Exception as ex:
                        pass
                ruleCnt = ruleCnt + 1

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
    #engine.loadsecurities()
    #engine.createnewsecurities(asofdate)
    engine.run()