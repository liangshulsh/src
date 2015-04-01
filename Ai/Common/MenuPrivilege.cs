// Ai Software Library.

using System;
using System.Data;
using System.Data.Common;
using System.Collections.Generic;

namespace Ai.Common {
    public sealed class MenuPrivilege {
        int _menuID = 0;
        int _userTypeID = 0;
        bool _visible = true;
        bool _enabled = true;
        bool _allowInsert = true;
        bool _allowEdit = true;
        bool _allowDelete = true;
        int _allowPrint = 1;
        Connection _conn;
        public MenuPrivilege(Connection conn) { _conn = conn; }
        public int MenuID {
            get { return _menuID; }
            set { _menuID = value; }
        }
        public int UserTypeID {
            get { return _userTypeID; }
            set { _userTypeID = value; }
        }
        public bool Visible {
            get { return _visible; }
            set { _visible = value; }
        }
        public bool Enabled {
            get { return _enabled; }
            set { _enabled = value; }
        }
        public bool AllowInsert {
            get { return _allowInsert; }
            set { _allowInsert = value; }
        }
        public bool AllowEdit {
            get { return _allowEdit; }
            set { _allowEdit = value; }
        }
        public bool AllowDelete {
            get { return _allowDelete; }
            set { _allowDelete = value; }
        }
        public int AllowPrint {
            get { return _allowPrint; }
            set { _allowPrint = value; }
        }
        public static bool keyIsExist(Connection conn, object[] key) {
            if (key == null) return false;
            string strSQL = "";
            if (key.Length == 2) strSQL = "SELECT COUNT(1) FROM TblMenuPrivilege WHERE menuID=" + (int)key[0] +
                    " AND userTypeID=" + (int)key[1];
            else return false;
            int rowCount = 0;
            if (conn.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = conn.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    rowCount = dr.GetInt32(0);
                }
                dr.Close();
                dr.Dispose();
            }
            return rowCount > 0;
        }
        internal bool save() {
            if (_userTypeID < 1 || _menuID < 1) return false;
            string strSQL;
            if (keyIsExist(_conn, new object[] { _menuID, _userTypeID })) strSQL = "UPDATE TblMenuPrivilege SET " +
                "(menuVisible, menuEnabled, allowInsert, allowEdit, allowDelete, allowPrint)=" +
                "(" + (_visible ? 1 : 0) + ", " + (_enabled ? 1 : 0) + ", " + (_allowInsert ? 1 : 0) + ", " + (_allowEdit ? 1 : 0) +
                ", " + (_allowDelete ? 1 : 0) + ", " + _allowPrint + ") WHERE menuID=" + _menuID + " AND userTypeID=" + _userTypeID;
            else strSQL = "INSERT INTO TblMenuPrivilege VALUES(" + _menuID + ", " + _userTypeID + ", " +
                (_visible ? 1 : 0) + ", " + (_enabled ? 1 : 0) + ", " + (_allowInsert ? 1 : 0) + ", " + (_allowEdit ? 1 : 0) +
                ", " + (_allowDelete ? 1 : 0) + ", " + _allowPrint + ")";
            return _conn.executeSQL(strSQL, Common.getCaller());
        }
        internal bool load(int menuID, int userTypeID) {
            if (menuID < 1 || userTypeID < 1) return false;
            string strSQL = "SELECT * FROM TblMenuPrivilege WHERE menuID=" + menuID + " AND userTypeID=" + userTypeID;
            if (_conn.executeData(strSQL, Common.getCaller())) {
                DbDataReader dr = _conn.Reader;
                if (dr.HasRows) {
                    dr.Read();
                    _visible = dr.GetInt32(2) == 1;
                    _enabled = dr.GetInt32(3) == 1;
                    _allowInsert = dr.GetInt32(4) == 1;
                    _allowEdit = dr.GetInt32(5) == 1;
                    _allowDelete = dr.GetInt32(6) == 1;
                    _allowPrint = dr.GetInt32(7);
                    dr.Close();
                    dr.Dispose();
                    return true;
                }
                dr.Close();
                dr.Dispose();
            }
            return false;
        }
        internal bool load() { return load(_menuID, _userTypeID); }
    }
    public sealed class MenuPrivilegeManger { 
    }
}