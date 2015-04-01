using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Ai.Common {
    public sealed class Language : DataObject {
        int _id = 0;
        string _name = "";
        string _cultureCode = "";
        public Language(Connection conn) : base(conn) {
            setTmp("Name", _name);
            setTmp("CultureCode", _cultureCode);
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
        public string CultureCode {
            get { return _cultureCode; }
            set {
                if (_cultureCode != value) {
                    setTmp("CultureCode", _cultureCode);
                    _cultureCode = value;
                }
            }
        }
        public static bool keyIsExist(Connection conn, object[] keys) {
            if (keys == null) return false;
            if (keys.Length == 0) return false;
            string strSQL;
            if (keys.Length == 1) strSQL = "SELECT COUNT(1) FROM TblLanguage WHERE langName='" + Connection.formatValue((string)keys[0]) + "'";
            if (keys.Length == 2) strSQL = "SELECT COUNT(1) FROM TblLanguage WHERE langName='" +
                Connection.formatValue((string)keys[0]) + "' AND langID<>" + (int)keys[1];
            else return false;
            return executeCommand(strSQL, conn);
        }
        public override bool undoValue(string propName) {
            switch (propName) {
                case "Name":
                    _name = (string)getTmp(propName);
                    return true;
                case "CultureCode":
                    _cultureCode = (string)getTmp(propName);
                    return true;
                default:
                    return false;
            }
        }
        public override bool save() {
            if (Connection.formatValue(_name) == "") return false;
            if (keyIsExist(_connection, new object[] { _name })) return false;
            string strSQL = "SELECT sqcLangID.nextVal FROM DUAL";
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = dr.GetInt32(0);
                    dr.Close();
                    dr.Dispose();
                    strSQL = "INSERT INTO TblLanguage VALUES(" + _id + ", '" +
                        Connection.formatValue(_name) + "', '" + Connection.formatValue(_cultureCode) + "')";
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
            string strSQL = "SELECT * FROM TblLanguage WHERE langID=" + (int)key;
            if (_connection.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _connection.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _id = (int)key;
                    _name = dr.GetString(1);
                    _cultureCode = dr.GetString(2);
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
            string strSQL = "UPDATE TblLanguage SET (langName, cultureCode) = ('" +
                Connection.formatValue(_name) + "', '" + Connection.formatValue(_cultureCode) +
                "') WHERE langID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
        public override bool remove() {
            if (_id < 1) return false;
            string strSQL = "DELETE FROM TblLanguage WHERE langID=" + _id;
            return _connection.executeSQL(strSQL, Common.getCaller());
        }
    }
    public sealed class LanguageManager : IDisposable {
        Connection _conn;
        Dictionary<string, Language> _langs = new Dictionary<string, Language>();
        public LanguageManager(Connection conn) { _conn = conn; }
        public void load() {
            _langs.Clear();
            string strSQL = "SELECT langID FROM TblLanguage ORDER BY langName ASC";
            if (_conn.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _conn.Reader;
                while (dr.Read()) {
                    Language aLang = new Language(_conn);
                    if (aLang.load(dr.GetInt32(0))) _langs.Add(aLang.Name.ToLower(), aLang);
                }
                dr.Close();
                dr.Dispose();
            }
        }
        public Language getLanguage(string name) {
            if (_langs.ContainsKey(name.ToLower())) return _langs[name.ToLower()];
            return null;
        }
        public Language addLanguage(string name, string culture) {
            Language aLang = new Language(_conn);
            aLang.Name = name;
            aLang.CultureCode = culture;
            if (!aLang.save()) return null;
            _langs.Add(aLang.Name.ToLower(), aLang);
            return aLang;
        }
        public Language setLanguage(int id, string name, string culture) {
            Language aLang = null;
            string key = "";
            foreach (Language l in _langs.Values) {
                if (l.ID == id) {
                    aLang = l;
                    key = aLang.Name.ToLower();
                    break;
                }
            }
            if (aLang != null) {
                aLang.Name = name;
                aLang.CultureCode = culture;
                if (!aLang.modify()) return null;
                _langs.Remove(key);
                _langs.Add(aLang.Name.ToLower(), aLang);
            }
            return aLang;
        }
        public bool removeLanguage(int id) {
            Language aLang = null;
            foreach (Language l in _langs.Values) {
                if (l.ID == id) {
                    aLang = l;
                    break;
                }
            }
            if (aLang == null) return false;
            string key = aLang.Name.ToLower();
            if (!aLang.remove()) return false;
            _langs.Remove(key);
            return true;
        }
        public bool removeLanguage(string name) {
            if (!_langs.ContainsKey(name.ToLower())) return false;
            Language aLang = _langs[name.ToLower()];
            if (!aLang.remove()) return false;
            string key = aLang.Name.ToLower();
            _langs.Remove(key);
            return true;
        }
        public Language[] Languages {
            get {
                List<Language> ls = new List<Language>();
                foreach (Language l in _langs.Values) ls.Add(l);
                return ls.ToArray();
            }
        }
        #region IDisposable Members
        bool _disposed = false;
        public void Dispose() {
            if (!_disposed) {
                if (_langs != null) {
                    _langs.Clear();
                    _langs = null;
                }
            }
        }
        #endregion
    }
    public sealed class ObjectLanguageManager {
        public sealed class ObjectLanguage {
            ObjectLanguageManager _manager = null;
            int _langID = 0;
            Dictionary<string, string> _properties = new Dictionary<string, string>();
            public ObjectLanguage(ObjectLanguageManager manager) {
                _manager = manager;
            }
            public int LanguageID {
                get { return _langID; }
                set { _langID = value; }
            }
            public void setProperties(string propName, string value) {
                if (_properties.ContainsKey(propName.ToLower())) _properties[propName.ToLower()] = value;
                else _properties.Add(propName.ToLower(), value);
            }
            public string getProperties(string propName) {
                if (_properties.ContainsKey(propName.ToLower())) return _properties[propName.ToLower()];
                else return "";
            }
            public bool save() {
                if (_manager._tableName == "") return false;
                if (_manager._objectID < 1) return false;
                if (_langID < 1) return false;
                string strSQL = "SELECT COUNT(1) FROM " + _manager._tableName +
                    " WHERE keyName=" + _manager._objectID + " AND langID=" + _langID;
                if (_manager._conn.executeData(strSQL, Common.getCaller())) {
                    DbDataReader dr = _manager._conn.Reader;
                    if (dr.HasRows) {
                        dr.Read();
                        int rowCount = dr.GetInt32(0);
                        FieldInfo[] fs = _manager.Fields;
                        string strs = "";
                        dr.Close();
                        dr.Dispose();
                        if (rowCount > 0) {
                            strSQL = "UPDATE " + _manager._tableName + " SET (";
                            foreach (FieldInfo fi in fs) {
                                if (strs != "") strs = ", " + strs;
                                strs = strs + fi.FieldName;
                            }
                            strSQL = strSQL + strs + ") = (";
                            strs = "";
                            foreach (FieldInfo fi in fs) {
                                if (strs != "") strs = ", " + strs;
                                strs = strs + "'" + Connection.formatValue(getProperties(fi.MapName)) + "'";
                            }
                            strSQL = strSQL + strs + ") WHERE " + _manager._keyName + "=" + _manager._objectID + " AND langID=" + _langID;
                        } else {
                            strSQL = "INSERT INTO " + _manager._tableName + " (" + _manager._keyName + ", langID, ";
                            strs = "";
                            foreach (FieldInfo fi in fs) {
                                if (strs != "") strs = ", " + strs;
                                strs = strs + fi.FieldName;
                            }
                            strSQL = strSQL + strs + ") VALUES(" + _manager._objectID + ", " + _langID;
                            strs = "";
                            foreach (FieldInfo fi in fs) {
                                if (strs != "") strs = ", " + strs;
                                strs = strs + "'" + Connection.formatValue(getProperties(fi.MapName)) + "'";
                            }
                            strSQL = strSQL + strs + ")";
                        }
                        return _manager._conn.executeSQL(strSQL, Common.getCaller());
                    }
                    dr.Close();
                    dr.Dispose();
                }
                return false;
            }
            public bool load() {
                if (_manager._tableName == "") return false;
                if (_manager._objectID < 1) return false;
                if (_langID < 1) return false;
                FieldInfo[] fs = _manager.Fields;
                string strs = "";
                string strSQL = "SELECT (";
                foreach (FieldInfo fi in fs) {
                    if (strs != "") strs = ", " + strs;
                    strs = strs + fi.FieldName;
                }
                strSQL = strSQL + strs + ") FROM " + _manager._tableName + " WHERE " + _manager._keyName + "=" + _manager._objectID +
                    " AND langID=" + _langID;
                if (_manager._conn.executeData(strSQL, Common.getCaller())) {
                    DbDataReader dr = _manager._conn.Reader;
                    if (dr.HasRows) {
                        dr.Read();
                        int i = 0;
                        foreach (FieldInfo fi in fs) {
                            setProperties(fi.MapName, dr.GetString(i));
                            i++;
                        }
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
        public sealed class FieldInfo {
            string _fieldName = "";
            string _mapName = "";
            public FieldInfo() { }
            public FieldInfo(string fieldName) {
                _fieldName = fieldName;
            }
            public FieldInfo(string fieldName, string mapName) {
                _fieldName = fieldName;
                _mapName = mapName;
            }
            public string FieldName {
                get { return _fieldName; }
                set { _fieldName = value; }
            }
            public string MapName {
                get { return _mapName; }
                set { _mapName = value; }
            }
        }
        Connection _conn;
        List<FieldInfo> _fields = new List<FieldInfo>();
        List<ObjectLanguage> _languages = new List<ObjectLanguage>();
        int _objectID = 0;
        string _tableName = "";
        string _keyName = "";
        public ObjectLanguageManager(Connection conn) {
            _conn = conn;
        }
        public ObjectLanguageManager(Connection conn, string tableName, string keyName, int objectID) {
            _conn = conn;
            _tableName = tableName;
            _keyName = keyName;
            _objectID = objectID;
        }
        public int ObjectID {
            get { return _objectID; }
            set { _objectID = value; }
        }
        public string TableName {
            get { return _tableName; }
            set { _tableName = value; }
        }
        public string KeyName {
            get { return _keyName; }
            set { _keyName = value; }
        }
        public FieldInfo[] Fields { get { return _fields.ToArray(); } }
        public ObjectLanguage[] Languages {
            get { return _languages.ToArray(); }
        }
        public FieldInfo addField(string name, string mapName) {
            bool found = false;
            FieldInfo aField = null;
            foreach (FieldInfo fi in _fields) {
                if (string.Compare(fi.FieldName, name, true) == 0) {
                    aField = fi;
                    found = true;
                    break;
                }
            }
            if (!found) aField = new FieldInfo();
            aField.FieldName = name;
            aField.MapName = mapName;
            if (!found) _fields.Add(aField);
            return aField;
        }
        public FieldInfo addField(FieldInfo fi) { return addField(fi.FieldName, fi.MapName); }
        public FieldInfo getField(string name) {
            foreach (FieldInfo fi in _fields) {
                if (string.Compare(fi.FieldName, name, true) == 0) return fi;
            }
            return null;
        }
        public FieldInfo getMappedField(string mapName) {
            foreach (FieldInfo fi in _fields) {
                if (string.Compare(fi.MapName, mapName, true) == 0) return fi;
            }
            return null;
        }
        public ObjectLanguage addLanguage(int langID, string[] properties) {
            if (langID < 1) return null;
            if (properties == null) return null;
            if (properties.Length != _fields.Count) return null;
            ObjectLanguage aLang = null;
            bool found = false;
            foreach (ObjectLanguage ol in _languages) {
                if (ol.LanguageID == langID) {
                    found = true;
                    aLang = ol;
                }
            }
            if (!found) aLang = new ObjectLanguage(this);
            int i = 0;
            foreach (FieldInfo fi in _fields) aLang.setProperties(fi.MapName, properties[i]);
            if (aLang.save()) {
                if (!found) _languages.Add(aLang);
                return aLang;
            } else {
                return null;
            }
        }
        public ObjectLanguage getLanguage(int langID) {
            if (langID < 1) return null;
            foreach (ObjectLanguage ol in _languages) {
                if (ol.LanguageID == langID) return ol;
            }
            return null;
        }
        public bool loadLanguages() {
            if (_tableName == "") return false;
            if (_keyName == "") return false;
            if (_objectID < 1) return false;
            _languages.Clear();
            string strSQL = "SELECT langID FROM " + _tableName + " WHERE " + _keyName + "=" + _objectID;
            if (_conn.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _conn.Reader;
                while (dr.Read()) {
                    ObjectLanguage ol = new ObjectLanguage(this);
                    ol.LanguageID = dr.GetInt32(0);
                    if (ol.load()) _languages.Add(ol);
                }
                dr.Close();
                dr.Dispose();
                return true;
            }
            return false;
        }
    }
}