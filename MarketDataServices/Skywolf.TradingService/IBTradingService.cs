using System;
using System.Collections.Concurrent;
using System.Data;
using System.ServiceModel;
using System.Text;
using log4net;
using Skywolf.Contracts.Services;
using Skywolf.Utility;
using System.Globalization;
using System.Collections.Generic;
using Skywolf.Contracts.DataContracts.Instrument;

namespace Skywolf.TradingService
{
    [ServiceBehavior(Namespace = Constants.NAMESPACE, InstanceContextMode = InstanceContextMode.PerCall)]
    public class IBTradingService : EchoService, IIBTradingService
    {
        private static ILog _Logger;
        private static IBUserManager _userManager;
        static IBTradingService()
        {
            _Logger = LogManager.GetLogger(typeof(IBTradingService));
            _userManager = new IBUserManager();
        }

        public bool CreateUser(string username, string account, string host, int port)
        {
            IBUser user = _userManager.AddUser(username, host, port);

            if (user != null)
            {
                user.SubscribeAccountUpdates(account);
                return true;
            }

            return false;
        }

        public void RemoveUser(string username)
        {
            _userManager.RemoveUser(username);
        }


    }
}
