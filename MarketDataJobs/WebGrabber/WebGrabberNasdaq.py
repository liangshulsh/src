import sys
import requests
import pandas as pd
import datetime
import simplejson as j
import io

class WebGrabberNasdaq(object):

    sourceId = 4

    stocklisturlPattern = "http://www.nasdaq.com/screening/companies-by-name.aspx?letter=0&exchange={0}&render=download"
    etflisturl = "http://www.nasdaq.com/investing/etfs/etf-finder-results.aspx?download=Yes"
    stocklistcolumns = ['Symbol','Name','LastSale','MarketCap','IPOyear','Sector','industry','Summary Quote']
    stocklistoutputcolumns = ['Name', 'FullName', 'LastPrice', 'MarketCap', 'IPOyear', 'Sector', 'Industry', 'SummaryQuote']
    etflistcolumns = ['Symbol','Name','LastSale','NetChange','PercentChange','1YrPercentChange']
    etflistoutputcolumns = ['Name', 'FullName', 'LastPrice', 'NetChange', 'PercentChange', 'PercentChange1Y']

    stocklist = None
    etflist = None
    def loadAllStockList(self):
        exchanges = ["nasdaq", "nyse", "amex"]
        frames = []
        for exchange in exchanges:
            frame = self.__getStockList(exchange)
            if frame is not None:
                frames.append(frame)
        result = pd.concat(frames, ignore_index = True)

        for col in result.columns:
            if (col not in self.stocklistcolumns):
                del result[col]
        result.columns = self.stocklistoutputcolumns
        result["AsOfDate"] = datetime.datetime.today()
        result.replace(['n/a'], [None])
        result["LastPrice"] = pd.to_numeric(result['LastPrice'], errors='coerce', downcast='float')
        result["IPOyear"] = pd.to_numeric(result['IPOyear'], errors='coerce', downcast='float')
        result["MarketCap"] =result['MarketCap'].apply(WebGrabberNasdaq.ConvertMarketCap, 1)
        result = result.drop_duplicates(subset=['Name'], keep='first')
        self.stocklist = result
        return result

    def ConvertMarketCap(marketCap):
        if marketCap is None:
            return None
        if type(marketCap) is float:
            return marketCap
        if type(marketCap) is str:
            if marketCap == 'n/a':
                return None
            elif marketCap[0] == '$':
                if marketCap[-1] == 'M':
                    num = marketCap[1:-1]
                    return float(num) * 1000000
                elif marketCap[-1] == 'B':
                    num = marketCap[1:-1]
                    return float(num) * 1000000000
        return None

    def __getStockList(self, exchange):
        stockresult = requests.get(self.stocklisturlPattern.format(exchange))
        if stockresult != None and stockresult.status_code == 200:
            stocklistdata = io.StringIO(stockresult.text)
            return pd.read_csv(stocklistdata)
        return None


    def loadAllETFList(self):
        etfresult = requests.get(self.etflisturl)
        if etfresult != None and etfresult.status_code == 200:
            etflistdata = io.StringIO(etfresult.text)
            etfpd = pd.read_csv(etflistdata)
            for col in etfpd.columns:
                if (col not in self.etflistcolumns):
                    del etfpd[col]
            etfpd.columns = self.etflistoutputcolumns
            etfpd["AsOfDate"] = datetime.datetime.today()
            etfpd.replace(['n/a'], [None])
            etfpd["LastPrice"] = pd.to_numeric(etfpd['LastPrice'], errors='coerce', downcast='float')
            etfpd["NetChange"] = pd.to_numeric(etfpd['NetChange'], errors='coerce', downcast='float')
            etfpd["PercentChange"] = pd.to_numeric(etfpd['PercentChange'], errors='coerce', downcast='float')
            etfpd["PercentChange1Y"] = pd.to_numeric(etfpd['PercentChange1Y'], errors='coerce', downcast='float')
            etfpd = etfpd.drop_duplicates(subset=['Name'], keep='first')
            self.etflist = etfpd
            return etfpd
        return None

if __name__ == '__main__':
    web = WebGrabberNasdaq()
    # test get all stock list
    stockList = web.loadAllStockList()
    print(stockList.head(5))

    # test get etf list
    etfList = web.loadAllETFList()
    print(etfList.head(5))
