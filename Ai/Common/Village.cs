// Ai Software Library.

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Ai.Common {
    public sealed class Village : DataObject {
        int _id = 0;
        int _districtID = 0;
        string _name = "";
        string _postalCode = "";
        public Village(Connection conn) : base(conn) {
            setTmp("ID", _id);
            setTmp("Name", _name);
            setTmp("DistrictID", _districtID);
            setTmp("PostalCode", _postalCode);
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
        public int DistrictID {
            get { return _districtID; }
            set {
                if (_districtID != value) {
                    setTmp("DistrictID", _districtID);
                    _districtID = value;
                }
            }
        }
        public string PostalCode {
            get { return _postalCode; }
            set {
                if (_postalCode != value) {
                    setTmp("PostalCode", _postalCode);
                    _postalCode = value;
                }
            }
        }
        public override bool undoValue(string propName) {
            if (propName == "Name") {
                _name = (string)getTmp("Name");
                return true;
            } else if (propName == "DistrictID") {
                _districtID = (int)getTmp("DistrictID");
                return true;
            }
            return false;
        }
        public static bool keyIsExist(Connection conn, object[] keys) {
            if (keys == null) return false;
            if (keys.Length < 1) return false;
            string strSQL;
            if (keys.Length == 1) strSQL = "SELECT COUNT(1) FROM TblVillage WHERE villageName='" +
                Connection.formatValue((string)keys[0]) + "'";
            else if (keys.Length == 2) strSQL = "SELECT COUNT(1) FROM TblVillage WHERE villageName='" +
                Connection.formatValue((string)keys[0]) + "' AND villageID<>" + (int)keys[1];
            else if (keys.Length == 3) strSQL = "SELECT COUNT(1) FROM TblVillage WHERE villageName='" +
                Connection.formatValue((string)keys[0]) + "' AND villageID<>" + (int)keys[1] +
                " AND districtID=" + (int)keys[2];
            else return false;
            return executeCommand(strSQL, conn);
        }
        public override bool save() {
            if (Connection.formatValue(_name) == "") return false;
            if (_districtID < 1) return false;
            if (keyIsExist(_connection, new object[] { _name, 0, _districtID })) return false;
            string strSQL = "SELECT sqcVillageID.nextVal FROM DUAL";
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = dr.GetInt32(0);
                    dr.Close();
                    dr.Dispose();
                    strSQL = "INSERT INTO TblVillage VALUES(" + _id +
                        ", " + _districtID + ", '" + Connection.formatValue(_name) + "', '" +
                        Connection.formatValue(_postalCode) + "')";
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
            string strSQL = "SELECT * FROM TblVillage WHERE villageID=" + (int)key;
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = (int)key;
                    _districtID = dr.GetInt32(1);
                    _name = dr.GetString(2);
                    _postalCode = dr.GetString(3);
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
            if (_districtID < 1) return false;
            if (keyIsExist(_connection, new object[] { _name, _id, _districtID })) return false;
            string strSQL = "UPDATE TblVillage SET (villageName, districtID, postalCode) = ('" +
                Connection.formatValue(_name) + "', " + _districtID + ", '" +
                Connection.formatValue(_postalCode) + "') WHERE villageID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
        public override bool remove() {
            if (_id < 1) return false;
            string strSQL = "DELETE FROM TblVillage WHERE villageID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
    }
}