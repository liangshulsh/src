import pyodbc
import pandas as pd
import pandas.io.sql as psql
import tempfile
import datetime
import os

class DataModelBase(object):

    __connectionString = ""
    __connection = None

    def __init__(self, connectionString):
        self.__connectionString = connectionString

    def open(self):
        self.__connection = pyodbc.connect(self.__connectionString)

    def read(self, query):
        return pd.read_sql(query,self.__connection)
        
    def close(self):
        if (self.__connection != None):
            self.__connection.close()
    
    def bulkInsert(self, data, tableName,includeIndex):
        if isinstance(data, pd.DataFrame):
            path = tempfile.gettempdir();
            
            filename = '{0}\\BulkInsert_{1}.csv'.format(path, datetime.datetime.now().timestamp())
            data.to_csv(filename,index=includeIndex)
            query = """BULK INSERT {0}
                            FROM '{1}'
                            WITH (
                            FIRSTROW = 2,
                            FIELDTERMINATOR = ',',
                            ROWTERMINATOR = '\n'
                            );"""
            
            cursor = self.__connection.cursor()
            cursor.execute(query.format(tableName, filename))
            self.__connection.commit()
            if os.path.exists(filename):
                os.remove(filename)

    def execute(self, query):
        cursor = self.__connection.cursor()
        cursor.execute(finalStmt)
        self.__connection.commit()

    def insert(self, data, tableName):
        if isinstance(data, pd.DataFrame):
            sep = ','
            insertStmt = 'insert into ' + tableName + ' (' + sep.join(data.columns.values.tolist()) + ') values ('
            typeStyle = []
            for type in data.dtypes:
                if (type.name[0:3] == 'int' or type.name[0:5] == 'float'):
                    typeStyle.append(1)
                elif (type.name[0:8] == 'datetime'):
                    typeStyle.append(2)
                else:
                    typeStyle.append(0)

            finalStmt=""

            for row in data.values.tolist():
                valueStmt = []
                for idx, style in enumerate(typeStyle):
                    if int(style) == 0:
                        valueStmt.append("'{0}'".format(row[idx]))
                    elif (int(style) == 1):
                        valueStmt.append("{0}".format(row[idx]))
                    elif (int(style) == 2):
                        valueStmt.append("'{0}'".format(row[idx].strftime('%Y-%m-%d %H:%M:%S')))

                finalStmt = finalStmt + insertStmt + sep.join(valueStmt) + ")\n"

            cursor = self.__connection.cursor()
            cursor.execute(finalStmt)
            self.__connection.commit()

if __name__ == '__main__':
    a = DataUtility()
    a.openMarketData()
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
    
