// Ai Software Library.

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Ai.Common {
    public sealed class Country : DataObject {
        int _id = 0;
        string _name = "";
        public Country(Connection conn) : base(conn) {
            setTmp("ID", _id);
            setTmp("Name", _name);
        }
        public int ID { get { return _id; } }
        public string Name {
            get { return _name; }
            set {
                if (_name != value) {
                    setTmp("Name", _name);
                    _name = value;
                }
            }
        }
        public override bool undoValue(string propName) {
            if (propName == "Name") {
                _name = (string)getTmp("Name");
                return true;
            }
            return false;
        }
        public static bool keyIsExist(Connection conn, object[] keys) {
            if (keys == null) return false;
            if (keys.Length < 1) return false;
            string strSQL;
            if (keys.Length == 1) strSQL = "SELECT COUNT(1) FROM TblCountry WHERE countryName='" + Connection.formatValue((string)keys[0]) + "'";
            else if (keys.Length == 2) strSQL = "SELECT COUNT(1) FROM TblCountry WHERE countryName='" + Connection.formatValue((string)keys[0]) + "' AND countryID<>" + (int)keys[1];
            else return false;
            return executeCommand(strSQL, conn);
        }
        public override bool save() {
            if (Connection.formatValue(_name) == "") return false;
            if (keyIsExist(_connection, new object[] { _name })) return false;
            string strSQL = "SELECT sqcCountryID.nextVal FROM DUAL";
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = dr.GetInt32(0);
                    dr.Close();
                    dr.Dispose();
                    strSQL = "INSERT INTO TblCountry VALUES(" + _id +
                        ", '" + Connection.formatValue(_name) + "')";
                    return _connection.executeSQL(strSQL, Common.getCaller());
                }
                dr.Close();
                dr.Dispose();
            }
            return false;
        }
        public override bool load(object key) {
            if (key == null) return false;
            if ((int)key < 1) return false;
            string strSQL = "SELECT * FROM TblCountry WHERE countryID=" + (int)key;
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = (int)key;
                    _name = dr.GetString(1);
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
            if (_id < 1) return false;
            if (keyIsExist(_connection, new object[] { _name, _id })) return false;
            string strSQL = "UPDATE TblCountry SET countryName='" + Connection.formatValue(_name) + "' WHERE countryID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
        public override bool remove() {
            if (_id < 1) return false;
            string strSQL = "DELETE FROM TblCountry WHERE countryID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
    }
}