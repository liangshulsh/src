// Ai Software Library

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Ai.Common {
    public class UserLog {
        Connection _conn;
        int _id = 0;
        string _userName = "";
        string _machineID = "";
        DateTime _start;
        DateTime _end;
        List<LogDetail> _details = new List<LogDetail>();
        public class LogDetail {
            int _id = 0;
            UserLog _owner = null;
            int _menuID = 0;
            string _sqlText = "";
            DateTime _logTime;
            public LogDetail(UserLog owner) {
                _owner = owner;
            }
            public int ID { get { return _id; } }
            public UserLog Owner { get { return _owner; } }
            public DateTime LogTime { get { return _logTime; } }
            public int MenuID {
                get { return _menuID; }
                set { _menuID = value; }
            }
            public string SQLText {
                get { return _sqlText; }
                set { _sqlText = value; }
            }
            internal bool save() {
                if (_owner == null) return false;
                if (_owner.ID == 0) return false;
                string strSQL = "SELECT sysdate FROM dual";
                if (_owner._conn.executeData(strSQL, Common.getCaller())) {
                    DbDataReader dr = _owner._conn.Reader;
                    if (dr.HasRows) {
                        dr.Read();
                        _logTime = dr.GetDateTime(0);
                        dr.Close();
                        dr.Dispose();
                        strSQL = "SELECT sqcUserLogDetailID.nextVal FROM dual";
                        if (_owner._conn.executeData(strSQL, Common.getCaller())) {
                            if (dr.HasRows) {
                                dr.Read();
                                _id = dr.GetInt32(0);
                                dr.Close();
                                dr.Dispose();
                                strSQL = "INSERT INTO TblUserLogDetail VALUES(" + _id + ", " +
                                    _owner.ID + ", " + _menuID + ", '" + Connection.formatValue(_sqlText) + "', to_date('" +
                                    _logTime.ToString("yyyy/MM/dd hh:mm:ss") + "', 'yyyy/MM/dd hh24:mm:ss'))";
                                return _owner._conn.executeSQL(strSQL, Common.getCaller());
                            }
                            dr.Close();
                            dr.Dispose();
                            return false;
                        }
                    }
                    dr.Close();
                    dr.Dispose();
                }
                return false;
            }
            internal bool load(int key) {
                if (key < 1) return false;
                if (_owner == null) return false;
                string strSQL = "SELECT * FROM TblUserLogDetail WHERE userLogDetailID=" + key;
                if (_owner._conn.executeData(strSQL, Common.getCaller())) {
                    DbDataReader dr = _owner._conn.Reader;
                    if (dr.HasRows) {
                        dr.Read();
                        _id = dr.GetInt32(0);
                        _menuID = dr.GetInt32(2);
                        _sqlText = dr.GetString(3);
                        _logTime = dr.GetDateTime(4);
                        dr.Close();
                        dr.Dispose();
                        return true;
                    }
                    dr.Close();
                    dr.Dispose();
                }
                return false;
            }
        }
        public UserLog(Connection conn) { 
            _conn = conn;
            _start = DateTime.Now;
            _end = DateTime.Now;
        }
        public int ID { get { return _id; } }
        public string Username {
            get { return _userName; }
            set { _userName = value; }
        }
        public string MachineID {
            get { return _machineID; }
            set { _machineID = value; }
        }
        public DateTime StartDate { get { return _start; } }
        public DateTime EndDate { get { return _end; } }
        public LogDetail[] Details { get { return _details.ToArray(); } }
        public bool addDetail(LogDetail detail) {
            if (detail.save()) {
                _details.Add(detail);
                return true;
            }
            return false;
        }
        public bool addDetail(int menuID, string sqlText) {
            LogDetail detail = new LogDetail(this);
            detail.MenuID = menuID;
            detail.SQLText = sqlText;
            return addDetail(detail);
        }
        public bool load(int logID) {
            if (logID < 1) return false;
            string strSQL = "SELECT * FROM TblUserLog WHERE userLogID=" + logID;
            if (_conn.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _conn.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = dr.GetInt32(0);
                    _userName = dr.GetString(1);
                    _machineID = dr.GetString(2);
                    _start = dr.GetDateTime(3);
                    _end = dr.GetDateTime(4);
                    dr.Close();
                    dr.Dispose();
                }
                dr.Close();
                dr.Dispose();
            }
            return false;
        }
        public bool loadDetails() {
            if (_id < 1) return false;
            string strSQL = "SELECT userLogDetailID FROM TblUserLogDetail WHERE userLogID=" + _id + " ORDER BY userLogDetailID ASC";
            if (_conn.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _conn.Reader;
                _details.Clear();
                while (dr.Read()) {
                    LogDetail detail = new LogDetail(this);
                    if (detail.load(dr.GetInt32(0))) _details.Add(detail);
                }
                dr.Close();
                dr.Dispose();
                return true;
            }
            return false;
        }
        public bool login(string username, string machineID) {
            if (Connection.formatValue(username) == "") return false;
            if (Connection.formatValue(machineID) == "") return false;
            string strSQL = "SELECT sysdate FROM dual";
            if (_conn.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _conn.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _start = dr.GetDateTime(0);
                    dr.Close();
                    dr.Dispose();
                    strSQL = "SELECT sqcUserLogID.nextVal FROM dual";
                    if (_conn.executeData(strSQL, Common.getCaller())) {
                        dr = _conn.Reader;
                        if (dr.HasRows) {
                            dr.Read();
                            _id = dr.GetInt32(0);
                            dr.Close();
                            dr.Dispose();
                            strSQL = "INSERT INTO TblUserLog VALUES(" + _id + ", '" +
                                Connection.formatValue(username) + "', '" +
                                Connection.formatValue(machineID) + "', to_date('" +
                                _start.ToString("yyyy/MM/ss HH:mm:ss") + "', 'yyyy/mm/dd hh24:mi:ss'), NULL)";
                            if (_conn.executeSQL(strSQL, Common.getCaller())) {
                                _details.Clear();
                                LogDetail detail = new LogDetail(this);
                                detail.MenuID = 0;
                                detail.SQLText = "Application log in";
                                if (detail.save()) {
                                    _details.Add(detail);
                                    return true;
                                } else return false;
                            } else return false;
                        }
                        dr.Close();
                        dr.Dispose();
                    }
                }
                dr.Close();
                dr.Dispose();
            }
            return false;
        }
        public bool login() { return login(_userName, _machineID); }
        public bool logout() {
            if (_id < 1) return false;
            string strSQL = "SELECT COUNT(1) FROM TblUserLog WHERE userLogID=" + _id;
            if (_conn.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _conn.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    int rowCount = dr.GetInt32(0);
                    dr.Close();
                    dr.Dispose();
                    if (rowCount < 1) return false;
                    strSQL = "SELECT sysdate FROM dual";
                    if (_conn.executeData(strSQL, Common.getCaller())) {
                        dr = _conn.Reader;
                        if (dr.HasRows) {
                            dr.Read();
                            _end = dr.GetDateTime(0);
                            dr.Close();
                            dr.Dispose();
                            strSQL = "UPDATE TblUserLog SET userLogEnd=" + _conn.formatDateTime(_end) +
                                " WHERE userLogID=" + _id;
                            if (_conn.executeSQL(strSQL, Common.getCaller())) {
                                LogDetail detail = new LogDetail(this);
                                detail.MenuID = 0;
                                detail.SQLText = "Application log out.";
                                if (detail.save()) {
                                    _details.Add(detail);
                                    return true;
                                } else return false;
                            } else return false;
                        }
                        dr.Close();
                        dr.Dispose();
                        return false;
                    }
                }
                dr.Close();
                dr.Dispose();
            }
            return false;
        }
    }
}