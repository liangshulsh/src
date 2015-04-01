using System;
using System.Drawing;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Ai.Common {
    public sealed class User : DataObject {
        string _name = "";
        int _typeID = 0;
        string _password = "";
        string _passwordHint = "";
        bool _active = true;
        bool _loggedIn = false;
        Image _img = null;
        public User(Connection conn) : base(conn) {
            setTmp("Name", _name);
            setTmp("TypeID", _typeID);
            setTmp("Password", _password);
            setTmp("PasswordHint", _passwordHint);
            setTmp("Active", _active);
        }
        public string Name {
            get { return _name; }
            set {
                if (_name != value) {
                    setTmp("Name", _name);
                    _name = value;
                }
            }
        }
        public int TypeID {
            get { return _typeID; }
            set {
                if (_typeID != value) {
                    setTmp("TypeID", _typeID);
                    _typeID = value;
                }
            }
        }
        public string Password {
            get { return _password; }
            set {
                if (_password != value) {
                    setTmp("Password", _password);
                    _password = value;
                }
            }
        }
        public string PasswordHint {
            get { return _passwordHint; }
            set {
                if (_passwordHint != value) {
                    setTmp("PasswordHint", _passwordHint);
                    _passwordHint = value;
                }
            }
        }
        public bool Active {
            get { return _active; }
            set {
                if (_active != value) {
                    setTmp("Active", _active);
                    _active = value;
                }
            }
        }
        public bool LoggedIn {
            get { return _loggedIn; }
            set { _loggedIn = value; }
        }
        public Image Image {
            get { return _img; }
            set { _img = value; }
        }
        public static bool keyIsExist(Connection conn, object[] keys) {
            if (keys == null) return false;
            if (keys.Length == 0) return false;
            string strSQL;
            if (keys.Length == 1) strSQL = "SELECT COUNT(1) FROM TblUser WHERE userName='" +
                Connection.formatValue((string)keys[0]) + "'";
            else return false;
            return executeCommand(strSQL, conn);

        }
        public override bool undoValue(string propName) {
            switch (propName) { 
                case "Name":
                    _name = (string)getTmp(propName);
                    return true;
                case "TypeID":
                    _typeID = (int)getTmp(propName);
                    return true;
                case "Password":
                    _password = (string)getTmp(propName);
                    return true;
                case "PasswordHint":
                    _passwordHint = (string)getTmp(propName);
                    return true;
                default :
                    return false;
            }
        }
        public override bool save() {
            if (Connection.formatValue(_name) == "") return false;
            if (keyIsExist(_connection, new object[] { _name })) return false;
            string strSQL = "INSERT INTO TblUser VALUES('" + Connection.formatValue(_name) +
                "', " + _typeID + ", '" + Common.huffman.encrypt(_password) + "', '" +
                Connection.formatValue(_passwordHint) + "', " + (_active ? 1 : 0) + ", 0)";
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
        public override bool load(object key) {
            if (Connection.formatValue((string)key) == "") return false;
            string strSQL = "SELECT * FROM TblUser WHERE userName='" + Connection.formatValue((string)key) + "'";
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _name = dr.GetString(0);
                    _typeID = dr.GetInt32(1);
                    _password = Common.huffman.decrypt(dr.GetString(2));
                    _passwordHint = dr.GetString(3);
                    _active = (dr.GetInt32(4) == 1 ? true : false);
                    _loggedIn = (dr.GetInt32(5) == 1 ? true : false);
                    dr.Close();
                    dr.Dispose();
                    return true;
                }
                dr.Close();
                dr.Dispose();
            }
            return false;
        }
        public override bool modify() {
            if (Connection.formatValue(_name) == "") return false;
            if (keyIsExist(_connection, new object[] { _name })) return false;
            string strSQL = "UPDATE TblUser SET (userName, typeID, userPassword, passwordHint, userActive, userLoggedIn) = ('" +
                Connection.formatValue(_name) + "', '" + Common.huffman.encrypt(_password) + "', '" +
                Connection.formatValue(_passwordHint) + "', " + (_active ? 1 : 0) + ", " + (_loggedIn ? 1 : 0) + ")";
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
        public override bool remove() {
            if (Connection.formatValue(_name) == "") return false;
            string strSQL = "DELETE FROM TblUser WHERE userName='" + Connection.formatValue(_name) + "'";
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
    }
}