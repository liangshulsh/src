import datetime as dt

class IBAccount(object):
    """description of class"""
    UserName = None    Account = None    AccountType = None    NetLiquidation = None    TotalCashValue = None    SettledCash = None    AccruedCash = None    BuyingPower = None    EquityWithLoanValue = None    PreviousEquityWithLoanValue = None    GrossPositionValue = None    ReqTEquity = None    ReqTMargin = None    SMA = None    InitMarginReq = None    MaintMarginReq = None    AvailableFunds = None    ExcessLiquidity = None    Cushion = None    FullInitMarginReq = None    FullMaintMarginReq = None    FullAvailableFunds = None    FullExcessLiquidity = None    LookAheadNextChange = None    LookAheadInitMarginReq = None    LookAheadMaintMarginReq = None    LookAheadAvailableFunds = None    LookAheadExcessLiquidity = None    HighestSeverity = None    DayTradesRemaining = None    Leverage = None    def __repr__(self):
        properties = {}
        for key in self.__dict__:
            properties[key] = self.__dict__[key]
        return str(properties)

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
            'UserName' : to_str,            'Account' : to_str,            'AccountType' : to_str,            'NetLiquidation' : to_float,            'TotalCashValue' : to_float,            'SettledCash' : to_float,            'AccruedCash' : to_float,            'BuyingPower' : to_float,            'EquityWithLoanValue' : to_float,            'PreviousEquityWithLoanValue' : to_float,            'GrossPositionValue' : to_float,            'ReqTEquity' : to_float,            'ReqTMargin' : to_float,            'SMA' : to_float,            'InitMarginReq' : to_float,            'MaintMarginReq' : to_float,            'AvailableFunds' : to_float,            'ExcessLiquidity' : to_float,            'Cushion' : to_float,            'FullInitMarginReq' : to_float,            'FullMaintMarginReq' : to_float,            'FullAvailableFunds' : to_float,            'FullExcessLiquidity' : to_float,            'LookAheadNextChange' : to_float,            'LookAheadInitMarginReq' : to_float,            'LookAheadMaintMarginReq' : to_float,            'LookAheadAvailableFunds' : to_float,            'LookAheadExcessLiquidity' : to_float,            'HighestSeverity' : to_int,            'DayTradesRemaining' : to_int,            'Leverage' : to_float}

        if field in set_functions:
            self.__dict__[field] = set_functions[field](value)