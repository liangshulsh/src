import datetime as dt

class IBPosition(object):
    """description of class"""
    Fund = None
    Strategy = None
    Folder = None
    Contract = None
    Account = None
    Quantity = None
    AverageCost = None

    def __repr__(self):
        return "[%s,%s,%s,%s,%s,%f,%f]" % (self.Fund, self.Strategy, self.Folder, self.Contract, self.Account, self.Quantity, self.AverageCost)

    def set_value(self, field, value):
        def to_int(val):
            ival = None
            try:
                ival = int(val)
            except:
                pass
            return ival

        def to_float(val):
            fval = None
            try:
                fval = float(val)
            except:
                pass
            return fval

        def to_str(val):
            return str(val)
        
        def to_datetime(val):
            try:
                return dt.datetime.strptime(val, "%Y-%m-%d %H:%M:%S")
            except:
                pass
            return None

        set_functions = {
            'Account' : to_str,
            'Fund' : to_str,
            'Strategy' : to_str,
            'Folder' : to_str,
            'Contract' : to_str,
            'Quantity' : to_float,
            'AverageCost' : to_float}

        if field in set_functions:
            self.__dict__[field] = set_functions[field](value)

class IBPositionPortfolio(IBPosition):
    MarketPrice = None
    MarketValue = None
    UnrealizedPNL = None
    RealizedPNL = None

    def __repr__(self):
        return "[%s,%s,%s,%s,%s,%f,%f,%f,%f,%f,%f]" % (self.Fund, self.Strategy, self.Folder, self.Contract, self.Account, self.Quantity, self.AverageCost,self.MarketPrice, self.MarketValue, self.UnrealizedPNL, self.RealizedPNL)

    def set_value(self, field, value):
        def to_int(val):
            ival = None
            try:
                ival = int(val)
            except:
                pass
            return ival

        def to_float(val):
            fval = None
            try:
                fval = float(val)
            except:
                pass
            return fval

        def to_str(val):
            return str(val)
        
        def to_datetime(val):
            try:
                return dt.datetime.strptime(val, "%Y-%m-%d %H:%M:%S")
            except:
                pass
            return None

        set_functions = {
            'Account' : to_str,
            'Fund' : to_str,
            'Strategy' : to_str,
            'Folder' : to_str,
            'Contract' : to_str,
            'Quantity' : to_float,
            'AverageCost' : to_float,
            'MarketPrice' : to_float,
            'MarketValue' : to_float,
            'UnrealizedPNL' : to_float,
            'RealizedPNL' : to_float}

        if field in set_functions:
            self.__dict__[field] = set_functions[field](value)