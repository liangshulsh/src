import datetime as dt

class IBOrder(object):
    """description of class"""
    Account = None
    Fund = None
    Strategy = None
    Folder = None
    Contract = None
    AsOfDate = None
    OrderId = None
    ClientId = None
    PermId = None
    Action = None
    TotalQuantity = None
    OrderType = None
    LimitPrice = None
    AuxPrice = None
    Tif = None
    ActiveStartTime = None
    ActiveStopTime = None
    OcaGroup = None
    OcaType = None
    OrderRef = None
    Transmit = None
    ParentId = None
    BlockOrder = None
    SweepToFill = None
    DisplaySize = None
    TriggerMethod = None
    OutsideRth = None
    Hidden = None
    GoodAfterTime = None
    GoodTillDate = None
    OverridePercentageConstraints = None
    Rule80A = None
    AllOrNone = None
    MinQty = None
    Status = None
    InitMargin = None
    MaintMargin = None
    EquityWithLoan = None
    Commission = None
    MinCommission = None
    MaxCommission = None
    CommissionCurrency = None
    WarningText = None
    Filled = None
    Remaining = None
    AvgFillPrice = None
    LastFillPrice = None
    WhyHeld = None


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
            'AsOfDate' : to_datetime,
            'OrderId' : to_int,
            'ClientId' : to_int,
            'PermId' : to_int,
            'Action' : to_str,
            'TotalQuantity' : to_float,
            'OrderType' : to_str,
            'LimitPrice' : to_float,
            'AuxPrice' : to_float,
            'Tif' : to_str,
            'ActiveStartTime' : to_datetime,
            'ActiveStopTime' : to_datetime,
            'OcaGroup' : to_str,
            'OcaType' : to_int,
            'OrderRef' : to_str,
            'Transmit' : to_int,
            'ParentId' : to_int,
            'BlockOrder' : to_int,
            'SweepToFill' : to_int,
            'DisplaySize' : to_int,
            'TriggerMethod' : to_int,
            'OutsideRth' : to_int,
            'Hidden' : to_int,
            'GoodAfterTime' : to_datetime,
            'GoodTillDate' : to_datetime,
            'OverridePercentageConstraints' : to_int,
            'Rule80A' : to_str,
            'AllOrNone' : to_int,
            'MinQty' : to_int,
            'Status' : to_str,
            'InitMargin' : to_str,
            'MaintMargin' : to_str,
            'EquityWithLoan' : to_str,
            'Commission' : to_float,
            'MinCommission' : to_float,
            'MaxCommission' : to_float,
            'CommissionCurrency' : to_str,
            'WarningText' : to_str,
            'Filled' : to_float,
            'Remaining' : to_float,
            'AvgFillPrice' : to_float,
            'LastFillPrice' : to_float,
            'WhyHeld' : to_str }

        if field in set_functions:
            self.__dict__[field] = set_functions[field](value)