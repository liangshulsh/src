
import requests
import pandas as pd
from bs4 import BeautifulSoup
from IBOrder import IBOrder
from IBTrade import IBTrade
from IBAccount import IBAccount
from IBPosition import IBPosition, IBPositionPortfolio

CONNECTION_DEFAULT = "http://localhost:8065/Skywolf_DEV/IBTradingRfService/"

class IBTradingAPI(object):
    _username = None
    _host = None
    _port = None
    _connection = None
    _account = None

    def __init__(self, username, account = None, host = "127.0.0.1", port = 4002, connection=CONNECTION_DEFAULT):
        self._host = host
        self._port = port
        self._username = username
        self._connection = connection
        self._account = account

    def get_request(self, url):
        result = requests.get(self._connection + url)
        if result != None and result.status_code == 200:
            text =  BeautifulSoup(result.text, 'lxml')
            str = text.find('string')
            return str.text
        return None

    #[WebGet(UriTemplate = "createuser?username={username}&account={account}&host={host}&port={port}")]
    #string CreateUser(string username, string account, string host, string port);
    def create_user(self):
        result = self.get_request("createuser?username=%s&account=%s&host=%s&port=%d" % (self._username, self._account, self._host, self._port))
        if result == 'OK':
            return True
        return False

    #[WebGet(UriTemplate = "removeuser?username={username}")]
    #string RemoveUser(string username);
    def remove_user(self):
        result = self.get_request("removeuser?username=%s" % (self._username))
        if result == 'OK':
            return True
        return False

    #[WebGet(UriTemplate = "placesimpleorder?username={username}&orderid={orderid}&securitytype={securitytype}&symbol={symbol}&currency={currency}&quantity={quantity}&ordertype={ordertype}&action={action}&limitprice={limitprice}&stopprice={stopprice}&fund={fund}&strategy={strategy}&folder={folder}")]
    #string PlaceSimpleOrder(string username, string orderid, string securitytype, string symbol, string currency, string quantity, string ordertype, string action, string limitprice, string stopprice, string fund, string strategy, string folder);
    #ordertype:MKT,LMT,STP
    #action:BUY,SELL
    #securitytype:FX,STK
    def place_simple_order(self, orderid, securitytype, symbol, currency, quantity, ordertype, action, limitprice, stopprice, fund, strategy, folder):
        result = self.get_request("placesimpleorder?username=%s&orderid=%d&securitytype=%s&symbol=%s&currency=%s&quantity=%f&ordertype=%s&action=%s&limitprice=%f&stopprice=%f&fund=%s&strategy=%s&folder=%s" % (self._username, orderid, securitytype, symbol, currency, quantity, ordertype, action, limitprice, stopprice, fund, strategy, folder))
        try:
            return int(result)
        except:
            pass

        return None

    #[WebGet(UriTemplate = "cancelorder?username={username}&orderid={orderid}")]
    #string CancelOrder(string username, string orderId);
    def cancel_order(self, orderid):
        result = self.get_request("cancelorder?username=%s&orderid=%d" % (self._username, orderid))
        if result == 'OK':
            return True
        return False

    #[WebGet(UriTemplate = "getopenorder?username={username}&orderid={orderid}")]
    #string GetOpenOrder(string username, string orderId);
    def get_open_order(self, orderid):
        result = self.get_request("getopenorder?username=%s&orderid=%d" % (self._username, orderid))
        orders = self.parse_objects(IBOrder,result)
        if orders is not None and len(orders) > 0:
            return orders[0]
        return None

    def parse_objects(self, IBType, result):
        if len(result) > 0:
            lines = result.split('\n')
            if len(lines) >= 2:
                fields = lines[0].split(',')
                orders = []
                for i in range(len(lines)):
                    if i > 0 and lines[i] is not None and len(lines[i].strip()) > 1:
                        values = lines[i].split(',')
                        order = IBType()
                        for i in range(len(fields)):
                            field = ''
                            val = ''
                            if fields[i] is not None:
                                field = fields[i].strip()
                            if values[i] is not None:
                                val = values[i].strip()
                            order.set_value(field, val)
                        orders.append(order)
                return orders
        return None
  
    #[WebGet(UriTemplate = "getallopenorders?username={username}")]
    #string GetAllOpenOrders(string username);
    def get_all_open_orders(self):
        result = self.get_request("getallopenorders?username=%s" % (self._username))
        orders = self.parse_objects(IBOrder, result)
        return orders

    #[WebGet(UriTemplate = "refreshallopenorders?username={username}")]
    #string RefreshAllOpenOrders(string username);
    def refresh_all_open_orders(self):
        result = self.get_request("refreshallopenorders?username=%s" % (self._username))
        orders = self.parse_objects(IBOrder, result)
        return orders

    #[WebGet(UriTemplate = "getaccountsummary?username={username}")]
    #string GetAccountSummary(string username);
    def get_account_summary(self):
        result = self.get_request("getaccountsummary?username=%s" % (self._username))
        accounts = self.parse_objects(IBAccount, result)
        if accounts is not None and len(accounts) > 0:
            return accounts[0]
        return None

    #[WebGet(UriTemplate = "unsubscribeaccountupdates?username={username}")]
    #string UnsubscribeAccountUpdates(string username);
    def unsubscribe_account_updates(self):
        result = self.get_request("unsubscribeaccountupdates?username=%s" % (self._username))
        if result == 'OK':
            return True
        return False

    #[WebGet(UriTemplate = "subscribeaccountupdates?username={username}&account={account}")]
    #string SubscribeAccountUpdates(string username, string account);
    def subcribe_account_updates(self):
        result = self.get_request("subscribeaccountupdates?username=%s&account=%s" % (self._username, self._account))
        if result == 'OK':
            return True
        return False

    #[WebGet(UriTemplate = "getportfolios?username={username}")]
    #string GetPortfolios(string username);
    def get_portfolios(self):
        result = self.get_request("getportfolios?username=%s" % (self._username))
        portfolios = self.parse_objects(IBPositionPortfolio, result)
        return portfolios

    #[WebGet(UriTemplate = "getalltrades?username={username}")]
    #string GetAllTrades(string username);
    def get_all_trades(self):
        result = self.get_request("getalltrades?username=%s" % (self._username))
        trades = self.parse_objects(IBTrade, result)
        return trades

    #[WebGet(UriTemplate = "getaccountupdatetime?username={username}")]
    #string GetAccountUpdateTime(string username);
    def get_account_update_time(self):
        result = self.get_request("getaccountupdatetime?username=%s" % (self._username))
        return result
    
    #[WebGet(UriTemplate = "getpositions?username={username}")]
    #string GetPositions(string username);
    def get_positions(self):
        result = self.get_request("getpositions?username=%s" % (self._username))
        positions = self.parse_objects(IBPosition, result)
        return positions

if __name__ == '__main__':

    trade = IBTradingAPI('ying0806')
    if (trade.create_user()):
        account = trade.get_account_summary()
        print(account)
