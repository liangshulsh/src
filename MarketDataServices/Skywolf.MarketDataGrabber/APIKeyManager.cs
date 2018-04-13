using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Skywolf.MarketDataGrabber
{
    public class APIKeyManager
    {
        protected ConcurrentDictionary<string, APIKey> _APIKeys = new ConcurrentDictionary<string, APIKey>();
        protected object _updateKeyLockObj = new object();

        #region AV Calls

        protected APIKey GetIdioKey()
        {
            lock (_updateKeyLockObj)
            {
                APIKey key = (from p in _APIKeys.Values
                              orderby p.TimeEclipsed() descending
                              select p).FirstOrDefault();
                key.LastCallingTime = DateTime.Now;
                return key;
            }
        }

        public int GetKeyCount()
        {
            return _APIKeys.Count;
        }

        public T Call<T>(Func<string, T> callingFunc)
        {
            APIKey key = GetIdioKey();
            lock (key.LockObj)
            {
                return callingFunc(key.Key);
            }
        }

        #endregion

        #region Key Operations
        public void AddAPIKey(IEnumerable<string> apiKeys)
        {
            lock (_updateKeyLockObj)
            {
                if (apiKeys != null && apiKeys.Count() > 0)
                {
                    string[] existKeys = _APIKeys.Keys.ToArray();

                    string[] newKeys = apiKeys.Select(p => p.ToUpper()).ToArray();

                    foreach (string key in newKeys)
                    {
                        if (!existKeys.Contains(key))
                        {
                            _APIKeys[key] = new APIKey(key);
                        }
                    }
                }
            }
        }

        public void RemoveAPIKey(IEnumerable<string> apiKeys)
        {
            lock (_updateKeyLockObj)
            {
                if (apiKeys != null && apiKeys.Count() > 0)
                {
                    string[] existKeys = _APIKeys.Keys.ToArray();

                    string[] removeKeys = apiKeys.Select(p => p.ToUpper()).ToArray();

                    foreach (string key in existKeys)
                    {
                        if (removeKeys.Contains(key))
                        {
                            APIKey apiKey = null;
                            _APIKeys.TryRemove(key, out apiKey);
                        }
                    }
                }
            }
        }

        public void UpdateAPIKey(IEnumerable<string> apiKeys)
        {
            lock (_updateKeyLockObj)
            {
                if (apiKeys != null && apiKeys.Count() > 0)
                {
                    string[] existKeys = _APIKeys.Keys.ToArray();

                    string[] newKeys = apiKeys.Select(p => p.ToUpper()).ToArray();

                    foreach (string key in existKeys)
                    {
                        if (!newKeys.Contains(key))
                        {
                            APIKey apiKey = null;
                            _APIKeys.TryRemove(key, out apiKey);
                        }
                    }

                    existKeys = _APIKeys.Keys.ToArray();

                    foreach (string key in newKeys)
                    {
                        if (!existKeys.Contains(key))
                        {
                            _APIKeys[key] = new APIKey(key);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
