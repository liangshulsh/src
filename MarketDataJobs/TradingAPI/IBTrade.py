import datetime as dt

class IBTrade(object):
    """description of class"""
    Fund = None
    Strategy = None
    Folder = None
    Contract = None
    AsOfDate = None
    OrderId = None
    ClientId = None
    ExecId = None
    Time = None
    AcctNumber = None
    Exchange = None
    Side = None
    Shares = None
    Price = None
    PermId = None
    Liquidation = None
    CumQty = None
    AvgPrice = None
    OrderRef = None
    EvRule = None
    EvMultiplier = None
    ModelCode = None
    Commission = None
    Currency = None
    RealizedPNL = None
    Yield = None
    YieldRedemptionDate = None

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
            'Fund' : to_str,
            'Strategy' : to_str,
            'Folder' : to_str,
            'Contract' : to_str,
            'AsOfDate' : to_datetime,
            'OrderId' : to_int,
            'ClientId' : to_int,
            'ExecId' : to_int,
            'Time' : to_str,
            'AcctNumber' : to_str,
            'Exchange' : to_str,
            'Side' : to_str,
            'Shares' : to_float,
            'Price' : to_float,
            'PermId' : to_int,
            'Liquidation' : to_int,
            'CumQty' : to_float,
            'AvgPrice' : to_float,
            'OrderRef' : to_str,
            'EvRule' : to_str,
            'EvMultiplier' : to_float,
            'ModelCode' : to_str,
            'Commission' : to_float,
            'Currency' : to_str,
            'RealizedPNL' : to_float,
            'Yield' : to_float,
            'YieldRedemptionDate' : to_int }

        if field in set_functions:
            self.__dict__[field] = set_functions[field](value)