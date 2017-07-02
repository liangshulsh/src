import pandas as pd
from DataModelBase import DataModelBase
import datetime

class MarketData(DataModelBase):
    MarketDataConnectionString = 'DRIVER={SQL Server Native Client 11.0};SERVER=LAPTOP-N2OTR2I1\\SKYWOLFDB;DATABASE=MarketData;UID=liangshu;PWD=optimus'

    def __init__(self):
        super(MarketData, self).__init__(self.MarketDataConnectionString)


if __name__ == '__main__':
    a = MarketData()
    a.open()
    #result = a.executeQuery('select * from MarketData.sec.Stock')
    instIds = pd.DataFrame({ 'SID' : [1,2,3,4,5],
                    'DataSource' : ['yahoo','yahoo','yahoo','yahoo','yahoo'],
                    'IDTypeId' : [1,1,1,1,1],
                    'IDValue' : ['MSFT','Yahoo','FB','Google','AMZN'],
                    'Usr': 'shu.liang',
                    'TS': datetime.datetime.now()})
    instIds = instIds[['SID','DataSource','IDTypeId','IDValue','Usr','TS']]
    
    print(instIds)
    a.insert(instIds, '[MarketData].[sec].[InstrumentID]')
    a.close()
    