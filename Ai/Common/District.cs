// Ai Software Library.

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Ai.Common {
    public sealed class District : DataObject {
        int _id = 0;
        int _cityID = 0;
        string _name = "";
        public District(Connection conn) : base(conn) {
            setTmp("ID", _id);
            setTmp("Name", _name);
            setTmp("CityID", _cityID);
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
        public int CityID {
            get { return _cityID; }
            set {
                if (_cityID != value) {
                    setTmp("CityID", _cityID);
                    _cityID = value;
                }
            }
        }
        public override bool undoValue(string propName) {
            if (propName == "Name") {
                _name = (string)getTmp("Name");
                return true;
            } else if (propName == "CityID") {
                _cityID = (int)getTmp("CityID");
                return true;
            }
            return false;
        }
        public static bool keyIsExist(Connection conn, object[] keys) {
            if (keys == null) return false;
            if (keys.Length < 1) return false;
            string strSQL;
            if (keys.Length == 1) strSQL = "SELECT COUNT(1) FROM TblDistrict WHERE districtName='" +
                Connection.formatValue((string)keys[0]) + "'";
            else if (keys.Length == 2) strSQL = "SELECT COUNT(1) FROM TblDistrict WHERE districtName='" +
                Connection.formatValue((string)keys[0]) + "' AND districtID<>" + (int)keys[1];
            else if (keys.Length == 3) strSQL = "SELECT COUNT(1) FROM TblDistrict WHERE districtName='" +
                Connection.formatValue((string)keys[0]) + "' AND districtID<>" + (int)keys[1] +
                " AND cityID=" + (int)keys[2];
            else return false;
            return executeCommand(strSQL, conn);
        }
        public override bool save() {
            if (Connection.formatValue(_name) == "") return false;
            if (_cityID < 1) return false;
            if (keyIsExist(_connection, new object[] { _name, 0, _cityID })) return false;
            string strSQL = "SELECT sqcDistrictID.nextVal FROM DUAL";
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = dr.GetInt32(0);
                    dr.Close();
                    dr.Dispose();
                    strSQL = "INSERT INTO TblDistrict VALUES(" + _id +
                        ", " + _cityID + ", '" + Connection.formatValue(_name) + "')";
                    return _connection.executeSQL(strSQL, Common.getCaller());
                }
                dr.Close();
                dr.Dispose();
            }
            return false;
        }
        public override bool load(object key) {
            if ((int)key < 1) return false;
            string strSQL = "SELECT * FROM TblDistrict WHERE districtID=" + (int)key;
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = (int)key;
                    _cityID = dr.GetInt32(1);
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
            if (_cityID < 1) return false;
            if (keyIsExist(_connection, new object[] { _name, _id, _cityID })) return false;
            string strSQL = "UPDATE TblDistrict SET (districtName, cityID) = ('" +
                Connection.formatValue(_name) + "', " + _cityID + ") WHERE districtID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
        public override bool remove() {
            if (_id < 1) return false;
            string strSQL = "DELETE FROM TblDistrict WHERE districtID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
    }
}