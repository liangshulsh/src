import requests
import pandas as pd
import datetime
import simplejson as j

class WebGrabberYahoo(object):
    historicalPriceUrlPattern = "https://finance.yahoo.com/quote/{0}/history?period1={1}&period2={2}&interval=1d&filter=history&frequency=1d"
    historicalPrices = None
    dividends = None
    splits = None

    def loadHistoricalData(self,startdate, enddate, ticker):
        startTs = (startdate + datetime.timedelta(days=-1) - datetime.datetime(1970,1,1)).total_seconds()
        endTs = (enddate + datetime.timedelta(days=1) - datetime.datetime(1970,1,1)).total_seconds()
        url = self.historicalPriceUrlPattern.format(ticker, int(startTs), int(endTs))
        result = requests.get(url)
        if result != None and result.status_code == 200:
            histPriceIdx = result.text.find('HistoricalPriceStore')
            if (histPriceIdx > 0):
                leftScropeIdx = result.text.find('[', histPriceIdx)
                rightScropeIdx = result.text.find(']', leftScropeIdx)
                histPriceJs = result.text[leftScropeIdx:rightScropeIdx+1]
                histPrices = j.loads(histPriceJs)
                self.historicalPrices = {'date' : [], 'open' : [], 'high' : [], 'low' : [], 'close' : [], 'volume' : [], 'unadjclose' : []}
                self.dividends = {'date' : [], 'amount' : [], 'data' : []}
                self.splits = {'date' : [], 'numerator' : [], 'denominator' : [], 'splitRatio' : [], 'data' : []}
                
                for histItem in histPrices:
                    if 'type' in histItem:
                        #{"date":1045578600,"numerator":1,"denominator":2,"splitRatio":"2\\u002F1","type":"SPLIT","data":"2\\u002F1"}
                        if (histItem['type'] == 'SPLIT'):
                            if ('date' in histItem):
                                self.splits['date'].append(datetime.datetime(1970,1,1) + datetime.timedelta(seconds=int(histItem['date'])))
                            else:
                                self.splits['date'].append(None)

                            if ('numerator' in histItem):
                                self.splits['numerator'].append(histItem['numerator'])
                            else:
                                self.splits['numerator'].append(None)

                            if ('denominator' in histItem):
                                self.splits['denominator'].append(histItem['denominator'])
                            else:
                                self.splits['denominator'].append(None)

                            if ('splitRatio' in histItem):
                                self.splits['splitRatio'].append(histItem['splitRatio'])
                            else:
                                self.splits['splitRatio'].append(None)

                            if ('data' in histItem):
                                self.splits['data'].append(histItem['data'])
                            else:
                                self.splits['data'].append(None)

                        #{"amount":0.39,"date":1494941400,"type":"DIVIDEND","data":0.39}
                        elif (histItem['type'] == 'DIVIDEND'):
                            if ('date' in histItem):
                                self.dividends['date'].append(datetime.datetime(1970,1,1) + datetime.timedelta(seconds=int(histItem['date'])))
                            else:
                                self.dividends['date'].append(None)

                            if ('amount' in histItem):
                                self.dividends['amount'].append(histItem['amount'])
                            else:
                                self.dividends['amount'].append(None)

                            if ('data' in histItem):
                                self.dividends['data'].append(histItem['data']);
                            else:
                                self.dividends['data'].append(None)
                    else:
                        #{"date":669393000,"open":1.3645833730697632,"high":1.4027777910232544,"low":1.34375,"close":0.9227131009101868,"volume":92505600,"unadjclose":99}
                        if ('date' in histItem):
                            self.historicalPrices['date'].append(datetime.datetime(1970,1,1) + datetime.timedelta(seconds=int(histItem['date'])))
                        else:
                            self.historicalPrices['date'].append(None)

                        if ('open' in histItem):
                            self.historicalPrices['open'].append(histItem['open'])
                        else:
                            self.historicalPrices['open'].append(None)

                        if ('high' in histItem):
                            self.historicalPrices['high'].append(histItem['high'])
                        else:
                            self.historicalPrices['high'].append(None)

                        if ('low' in histItem):
                            self.historicalPrices['low'].append(histItem['low'])
                        else:
                            self.historicalPrices['low'].append(None)

                        if ('close' in histItem):
                            self.historicalPrices['close'].append(histItem['close'])
                        else:
                            self.historicalPrices['close'].append(None)

                        if ('volume' in histItem):
                            self.historicalPrices['volume'].append(histItem['volume'])
                        else:
                            self.historicalPrices['volume'].append(None)

                        if ('unadjclose' in histItem):
                            self.historicalPrices['unadjclose'].append(histItem['unadjclose'])
                        else:
                            self.historicalPrices['unadjclose'].append(None)

if __name__ == '__main__':

    yahoo = WebGrabberYahoo()
    startdate = datetime.datetime(1900,1,1)
    enddate = datetime.datetime(2017,7,4)
    ticker = 'MSFT'
    yahoo.loadHistoricalData(startdate, enddate, ticker)
    histPrice = pd.DataFrame(yahoo.historicalPrices)
