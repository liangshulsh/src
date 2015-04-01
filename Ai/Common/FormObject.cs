// Ai Software Library.

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Ai.Common {
    public sealed class FormObject : DataObject {
        int _id = 0;
        int _formID = 0;
        string _name = "";
        ObjectLanguageManager _olm;
        public FormObject(Connection conn) : base(conn) {
            setTmp("ID", _id);
            setTmp("Name", _name);
            setTmp("FormID", _formID);
            _olm = new ObjectLanguageManager(conn);
            _olm.TableName = "TblFormObjectDetail";
            _olm.KeyName = "formObjectID";
            _olm.addField("formObjectText", "Text");
            _olm.addField("formObjectToolTip", "ToolTip");
            _olm.addField("formObjectToolTipText", "ToolTipText");
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
        public int FormID {
            get { return _formID; }
            set {
                if (_formID != value) {
                    setTmp("FormID", _formID);
                    _formID = value;
                }
            }
        }
        public ObjectLanguageManager LanguageManager { get { return _olm; } }
        public static bool keyIsExist(Connection conn, object[] keys) {
            if (keys == null) return false;
            if (keys.Length < 1) return false;
            string strSQL;
            if (keys.Length == 1) strSQL = "SELECT COUNT(1) FROM TblFormObject WHERE formObjectName='" +
                Connection.formatValue((string)keys[0]) + "'";
            else if (keys.Length == 2) strSQL = "SELECT COUNT(1) FROM TblFormObject WHERE formObjectName='" +
                Connection.formatValue((string)keys[0]) + "' AND formObjectID<>" + (int)keys[1];
            else if (keys.Length == 2) strSQL = "SELECT COUNT(1) FROM TblFormObject WHERE formObjectName='" +
                Connection.formatValue((string)keys[0]) + "' AND formObjectID<>" + (int)keys[1] +
                " AND formID=" + (int)keys[2];
            else return false;
            return executeCommand(strSQL, conn);
        }
        public override bool undoValue(string propName) {
            switch (propName) { 
                case "Name":
                    _name = (string)getTmp(propName);
                    return true;
                case "FormID":
                    _formID = (int)getTmp(propName);
                    return true;
                default:
                    return false;
            }
        }
        public override bool save() {
            if (Connection.formatValue(_name) == "") return false;
            if (_formID < 1) return false;
            if (keyIsExist(_connection, new object[] { _name, 0, _formID })) return false;
            string strSQL = "SELECT sqcFormObjectID.nextVal FROM dual";
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = dr.GetInt32(0);
                    dr.Close();
                    dr.Dispose();
                    strSQL = "INSERT INTO TblFormObject VALUES(" + _id + ", " + _formID + ", '" +
                        Connection.formatValue(_name) + "')";
                    if (_connection.executeSQL(strSQL, Common.getCaller())) {
                        _olm.ObjectID = _id;
                        return true;
                    } else return false;
                }
                dr.Close();
                dr.Dispose();
            }
            return false;
        }
        public override bool load(object key) {
            if (key == null) return false;
            if ((int)key < 1) return false;
            string strSQL = "SELECT * FROM TblFormObject WHERE formObjectID=" + (int)key;
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = dr.GetInt32(0);
                    _formID = dr.GetInt32(1);
                    _name = dr.GetString(2);
                    dr.Close();
                    dr.Dispose();
                    _olm.ObjectID = _id;
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
            if (keyIsExist(_connection, new object[] { _name, _id, _formID })) return false;
            string strSQL = "UPDATE TblFormObject SET formObjectName='" + Connection.formatValue(_name)
                + "' WHERE formObjectID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
        public override bool remove() {
            if (_id < 1) return false;
            string strSQL = "DELETE FROM TblFormObject WHERE formObjectID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
    }
}