// Ai Software Library.

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Ai.Common {
    public sealed class Province : DataObject {
        int _id = 0;
        int _countryID = 0;
        string _name = "";
        public Province(Connection conn) : base(conn) {
            setTmp("ID", _id);
            setTmp("Name", _name);
            setTmp("CountryID", _countryID);
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
        public int CountryID {
            get { return _countryID; }
            set {
                if (_countryID != value) {
                    setTmp("CountryID", _countryID);
                    _countryID = value;
                }
            }
        }
        public override bool undoValue(string propName) {
            if (propName == "Name") {
                _name = (string)getTmp("Name");
                return true;
            } else if (propName == "CountryID") {
                _countryID = (int)getTmp("CountryID");
                return true;
            }
            return false;
        }
        public static bool keyIsExist(Connection conn, object[] keys) {
            if (keys == null) return false;
            if (keys.Length < 1) return false;
            string strSQL;
            if (keys.Length == 1) strSQL = "SELECT COUNT(1) FROM TblProvince WHERE provinceName='" +
                Connection.formatValue((string)keys[0]) + "'";
            else if (keys.Length == 2) strSQL = "SELECT COUNT(1) FROM TblProvince WHERE provinceName='" +
                Connection.formatValue((string)keys[0]) + "' AND provinceID<>" + (int)keys[1];
            else if (keys.Length == 3) strSQL = "SELECT COUNT(1) FROM TblProvince WHERE provinceName='" +
                Connection.formatValue((string)keys[0]) + "' AND provinceID<>" + (int)keys[1] +
                " AND countryID=" + (int)keys[2];
            else return false;
            return executeCommand(strSQL, conn);
        }
        public override bool save() {
            if (Connection.formatValue(_name) == "") return false;
            if (_countryID < 1) return false;
            if (keyIsExist(_connection, new object[] { _name, 0, _countryID })) return false;
            string strSQL = "SELECT sqcProvinceID.nextVal FROM DUAL";
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = dr.GetInt32(0);
                    dr.Close();
                    dr.Dispose();
                    strSQL = "INSERT INTO TblProvince VALUES(" + _id +
                        ", " + _countryID + ", '" + Connection.formatValue(_name) + "')";
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
            string strSQL = "SELECT * FROM TblProvince WHERE provinceID=" + (int)key;
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = (int)key;
                    _countryID = dr.GetInt32(1);
                    _name = dr.GetString(2);
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
            if (_countryID < 1) return false;
            if (keyIsExist(_connection, new object[] { _name, _id, _countryID })) return false;
            string strSQL = "UPDATE TblProvince SET (provinceName, countryID) = ('" +
                Connection.formatValue(_name) + "', " + _countryID + ") WHERE provinceID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
        public override bool remove() {
            if (_id < 1) return false;
            string strSQL = "DELETE FROM TblProvince WHERE countryID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
    }
}