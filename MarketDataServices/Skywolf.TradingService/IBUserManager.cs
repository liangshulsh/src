using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.TradingService
{
    public class IBUserManager
    {
        static ILog _log = log4net.LogManager.GetLogger(typeof(IBUserManager));

        static int _nextClientId = 1;
        static object _clientIdLockObj = new object();

        private int GetNextClientId()
        {
            lock (_clientIdLockObj)
            {
                int id = _nextClientId;
                _nextClientId++;
                return id;
            }
        }

        protected ConcurrentDictionary<string, IBUser> _IBUserMap = new ConcurrentDictionary<string, IBUser>();

        public IBUserManager()
        {

        }

        public IBUser AddUser(string username, string host, int port)
        {
            if (string.IsNullOrEmpty(username))
            {
                return null;
            }

            IBUser user = null;
            username = username.ToUpper().Trim();
            if (_IBUserMap.ContainsKey(username))
            {
                user = _IBUserMap[username];
            }
            else
            {
                user = new IBUser(host, port, GetNextClientId(), _log);
                if (user.Connect())
                {
                    _IBUserMap[username] = user;
                }
                else
                {
                    user = null;
                }
            }

            return user;
        }

        public void RemoveUser(string username)
        {
            IBUser user = GetUser(username);

            if (user != null)
            {
                user.UnsubscribeAccountUpdates();
                user.Disconnect();
                _IBUserMap.TryRemove(username, out user);
            }
        }

        public IBUser GetUser(string username)
        {
            if (_IBUserMap.ContainsKey(username))
            {
                return _IBUserMap[username];
            }

            return null;
        }
    }
}
