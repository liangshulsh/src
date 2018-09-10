import pandas as pd
#import pandas_datareader as web

class WebGrabberGoogle(object):
    sourceId = 2
    historicalPrices = None

    def loadHistoricalData(self,startdate, enddate, ticker):
        data = web.DataReader(ticker, "google", startdate, enddate)

        if data is not None:
            data = data.reset_index(level=0)
            data.columns = [x.lower() for x in data.columns]
            self.historicalPrices = data.to_dict(orient='list')

