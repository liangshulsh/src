// Ai Software Library.

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Ai.Common {
    public sealed class Religion : DataObject {
        int _id = 0;
        string _name = "";
        ObjectLanguageManager _olm;
        public Religion(Connection conn) : base(conn) {
            setTmp("ID", _id);
            setTmp("Name", _name);
            _olm = new ObjectLanguageManager(conn);
            _olm.addField("religionName", "Name");
            _olm.TableName = "TblReligionName";
            _olm.KeyName = "religionID";
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
        public ObjectLanguageManager LanguageManager { get { return _olm; } }
        public static bool keyIsExist(Connection conn, object[] keys) {
            if (keys == null) return false;
            if (keys.Length < 1) return false;
            string strSQL;
            if (keys.Length == 1) strSQL = "SELECT COUNT(1) FROM TblReligion WHERE religionName='" +
                Connection.formatValue((string)keys[0]) + "'";
            else if (keys.Length == 2) strSQL = "SELECT COUNT(1) FROM TblReligion WHERE religionName='" +
                Connection.formatValue((string)keys[0]) + "' AND religionID<>" + (int)keys[1];
            else return false;
            return executeCommand(strSQL, conn);
        }
        public override bool undoValue(string propName) {
            switch (propName) { 
                case "Name":
                    _name = (string)getTmp(propName);
                    return true;
                default:
                    return false;
            }
        }
        public override bool save() {
            if (Connection.formatValue(_name) == "") return false;
            if (keyIsExist(_connection, new object[] { _name })) return false;
            string strSQL = "SELECT sqcReligionID.nextVal FROM dual";
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = dr.GetInt32(0);
                    dr.Close();
                    dr.Dispose();
                    strSQL = "INSERT INTO TblReligion VALUES(" + _id + ", '" +
                        Connection.formatValue(_name) + "')";
                    if (_connection.executeSQL(strSQL, Common.getCaller())) {
                        _olm.ObjectID = _id;
                        return true;
                    } else {
                        return false;
                    }
                }
                dr.Close();
                dr.Dispose();
            }
            return false;
        }
        public override bool load(object key) {
            if (key == null) return false;
            if ((int)key < 1) return false;
            string strSQL = "SELECT * FROM TblReligion WHERE religionID=" + (int)key;
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = dr.GetInt32(0);
                    _name = dr.GetString(1);
                    _olm.ObjectID = _id;
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
            if (Connection.formatValue(_name) == "") return false;
            if (keyIsExist(_connection, new object[] { _name, _id })) return false;
            string strSQL = "UPDATE TblReligion SET religionName='" +
                Connection.formatValue(_name) + "' WHERE religionID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
        public override bool remove() {
            if (_id < 1) return false;
            string strSQL = "DELETE FROM TblReligion WHERE religionID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
    }
}