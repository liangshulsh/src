import pyodbc
import pandas as pd
import pandas.io.sql as psql
import tempfile
import datetime
import os
import math

class DataModelBase(object):

    __connectionString = ""
    __connection = None

    def __init__(self, connectionString):
        self.__connectionString = connectionString

    def open(self):
        self.__connection = pyodbc.connect(self.__connectionString)

    def read(self, query):
        return pd.read_sql(query,self.__connection)
    
    def DataFrameToObjects(self, data, typeName):
        if (isinstance(data, pd.DataFrame)):
            attributions = {}
            columns = data.columns.values.tolist()
            for col in columns:
                attributions[col] = None
            DataType = type(typeName, (), attributions)
            objects = []
            for row in data.values.tolist():
                obj = DataType()
                for idx, col in enumerate(columns):
                    obj.__dict__[col] = row[idx]
                objects.append(obj)
            return objects
        return None

    def readObjects(self, query, typeName):
        data = self.read(query)
        return self.DataFrameToObjects(data, typeName)

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
        cursor.execute(query)
        self.__connection.commit()

    def insert(self, data, tableName):
        if isinstance(data, pd.DataFrame):
            sep = ','
            insertStmt = 'insert into ' + tableName + ' (' + sep.join(["[" + v +"]" for v in data.columns.values.tolist()]) + ') values ('
            typeStyle = []
            for type in data.dtypes:
                if (type.name[0:3] == 'int' or type.name[0:5] == 'float'):
                    typeStyle.append(1)
                elif (type.name[0:8] == 'datetime'):
                    typeStyle.append(2)
                else:
                    typeStyle.append(0)

            try:
                insertCount = 0
                cursor = self.__connection.cursor()
                for row in data.values.tolist():
                    valueStmt = []
                    for idx, style in enumerate(typeStyle):
                        if int(style) == 0:
                            valueStmt.append("'{0}'".format(row[idx]))
                        elif (int(style) == 1):
                            v = "{0}".format(row[idx])
                            if v == "nan":
                                v = "null"
                            valueStmt.append(v)
                        elif (int(style) == 2):
                            valueStmt.append("'{0}'".format(row[idx].strftime('%Y-%m-%d %H:%M:%S')))
                    cursor.execute(insertStmt + sep.join(valueStmt) + ")")
                    insertCount = insertCount + 1
                    if (insertCount % 100 == 0):
                        self.__connection.commit()
                        insertCount = 0

                if insertCount > 0:
                    self.__connection.commit()
            except Exception as e:
                print (e)

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
    
