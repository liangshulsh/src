// Ai Software Library.

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Ai.Common {
    /// <summary>
    /// Provides a base class for other objects that requires transaction with database.
    /// </summary>
    public abstract class DataObject : IDisposable {
        protected Connection _connection = null;
        protected Dictionary<string, object> _tmp = null;
        public DataObject(Connection connection) {
            _tmp = new Dictionary<string, object>();
            _connection = connection;
        }
        #region Public Properties
        /// <summary>
        /// Gets a Connection object used to communicate with the database.
        /// </summary>
        public Connection Connection { get { return _connection; } }
        #endregion
        #region Public Methods
        public abstract bool undoValue(string propName);
        public abstract bool save();
        public abstract bool load(object key);
        public abstract bool modify();
        public virtual bool modify(object newKey) { return true; }
        public abstract bool remove();
        #endregion
        #region Protected Methods
        protected void setTmp(string propName, object value) {
            if (_tmp.ContainsKey(propName)) _tmp[propName] = value;
            else _tmp.Add(propName, value);
        }
        protected object getTmp(string propName) {
            if (_tmp.ContainsKey(propName)) {
                object obj = _tmp[propName];
                _tmp.Remove(propName);
                return obj;
            }
            return null;
        }
        protected static bool executeCommand(string strSQL, Connection conn) {
            if (conn.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = conn.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    int recCount = dr.GetInt32(0);
                    dr.Close();
                    dr.Dispose();
                    return recCount > 0;
                }
                dr.Close();
                dr.Dispose();
            }
            return false;
        }
        #endregion
        #region IDisposable Members
        protected bool _disposed = false;
        public virtual void Dispose() {
            if (!_disposed) {
                if (_tmp != null) {
                    _tmp.Clear();
                    _tmp = null;
                }
                _disposed = true;
            }
        }
        #endregion
    }
}