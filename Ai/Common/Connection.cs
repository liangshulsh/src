// Ai Software Library.

using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace Ai.Common {
    /// <summary>
    /// Provide common functions to establish connection to a database.
    /// </summary>
    public abstract class Connection : IDisposable {
        #region Public Events
        /// <summary>
        /// Occurs when the state of the connection has been changed.
        /// </summary>
        public event EventHandler<ConnectionEventArgs> StateChanged;
        /// <summary>
        /// Occurs when an error occured during executing a command.
        /// </summary>
        public event EventHandler<ConnectionEventArgs> CommandExecuted;
        /// <summary>
        /// Occurs before a command being executed.
        /// </summary>
        public event EventHandler<EventArgs> Executing;
        #endregion
        #region Protected Fields
        protected DbConnection _connection = null;
        protected string _serverAddress = "";
        protected string _serverPort = "";
        protected string _username = "";
        protected string _password = "";
        protected string _databaseName = "";
        protected string _caller = "";
        protected string _sqlCommand = "";
        protected int _errCode = 0;
        protected int _affectedRows = 0;
        protected string _errMsg = "";
        protected Dictionary<string, string> _params;
        protected DbDataReader _reader = null;
        protected DbCommand _command = null;
        #endregion
        public const string CompanyName = "Ai";
        public const string SubName = "Software";
        public string AppName = "";
        public string Version = "1.0";
        public string PublicName = "";
        public static string formatValue(string value) {
            string result = "";
            result = value.Replace("'", "''");
            result = result.Trim();
            return result;
        }
        public Connection() {
            _params = new Dictionary<string, string>();
        }
        protected void _StateChange(object sender, StateChangeEventArgs e) {
            if (StateChanged != null) StateChanged(e, new ConnectionEventArgs(e.CurrentState, _errCode, _errMsg, _affectedRows));
        }
        protected void _CommandExecuted(object sender) {
            if (CommandExecuted != null) {
                ConnectionEventArgs e = new ConnectionEventArgs(_connection.State, _errCode, _errMsg, _affectedRows);
                CommandExecuted(sender, e);
            }
        }
        protected void _Executing(object sender) {
            _errMsg = "";
            if (Executing != null) Executing(sender, new EventArgs());
        }
        #region Public Properties
        /// <summary>
        /// Gets or sets network address of the database server.
        /// </summary>
        public string ServerAddress {
            get { return _serverAddress; }
            set { _serverAddress = value; }
        }
        /// <summary>
        /// Gets or sets port number used to connect to the database server.
        /// </summary>
        public string ServerPort {
            get { return _serverPort; }
            set { _serverPort = value; }
        }
        /// <summary>
        /// Gets or sets the username used to connect to the database server.
        /// </summary>
        public string Username {
            get { return _username; }
            set { _username = value; }
        }
        /// <summary>
        /// Gets or sets password used to connect to the database server.
        /// </summary>
        public string Password {
            get { return _password; }
            set { _password = value; }
        }
        /// <summary>
        /// Gets or sets the database name to be connected.
        /// </summary>
        public string DatabaseName {
            get { return _databaseName; }
            set { _databaseName = value; }
        }
        /// <summary>
        /// Gets or sets the name who calls the process.
        /// </summary>
        public string Caller {
            get { return _caller; }
            set { _caller = value; }
        }
        /// <summary>
        /// Gets the last executed query command.
        /// </summary>
        public string SQLCommand { get { return _sqlCommand; } }
        /// <summary>
        /// Gets a value of the last error code raised by system or database server.
        /// </summary>
        public int ErrorCode { get { return _errCode; } }
        /// <summary>
        /// Gets a value indicating the number of rows affected by an execution of a SQL command.
        /// </summary>
        public int AffectedRows { get { return _affectedRows; } }
        /// <summary>
        /// Gets a collection of parameters used to execute a stored procedure.
        /// </summary>
        public DbParameterCollection Parameters { get { return _command.Parameters; } }
        /// <summary>
        /// Gets a value of the last error message raised by system or database server.
        /// </summary>
        public string ErrorMessage { get { return _errMsg; } }
        /// <summary>
        /// Gets the current state of the connection object.
        /// </summary>
        public ConnectionState State { get { return _connection.State; } }
        /// <summary>
        /// Gets the string used to open a database.
        /// </summary>
        public abstract string ConnectionString { get; }
        /// <summary>
        /// Gets a DbConnection object used to connect to database server.
        /// </summary>
        public DbConnection DBConnection { get { return _connection; } }
        /// <summary>
        /// Gets a DbDataReader object represent the data result from executing a query.
        /// </summary>
        public DbDataReader Reader { get { return _reader; } }
        /// <summary>
        /// Gets a DbCommand object used to execute commands using current connection.
        /// </summary>
        public DbCommand Command { get { return _command; } }
        #endregion
        #region Public Methods
        /// <summary>
        /// Adds a connection parameter.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">Value of the corresponding parameter name.</param>
        public void setParam(string paramName, string paramValue) {
            if (_params.ContainsKey(paramName.ToLower())) _params.Remove(paramName.ToLower());
            _params.Add(paramName.ToLower(), paramValue);
        }
        /// <summary>
        /// Gets a parameter value of the given parameter name.
        /// </summary>
        /// <param name="paramName">Name of the parameter to retrieve.</param>
        /// <returns>If exists, a string represent the value of the parameter, empty string, otherwise.</returns>
        public string getParam(string paramName) {
            if (_params.ContainsKey(paramName.ToLower())) return _params[paramName.ToLower()];
            return "";
        }
        /// <summary>
        /// Removes an existing parameter.
        /// </summary>
        /// <param name="paramName">The name of the parameter to be removed.</param>
        /// <returns>True, if the parameter successfully removed, false,otherwise.</returns>
        public bool removeParam(string paramName) {
            if (_params.ContainsKey(paramName.ToLower())) return _params.Remove(paramName.ToLower());
            return false;
        }
        /// <summary>
        /// Load the saved parameters setting from registry.
        /// </summary>
        /// <returns>True, if load succeeded, otherwise, false.</returns>
        public bool loadSetting() {
            string keyName = "SOFTWARE\\" + CompanyName + "\\" + SubName + "\\" + AppName + "\\" + Version;
            if ((string)Registry.GetValue("HKEY_LOCAL_MACHINE\\" + keyName, "PublicName", "") == "") return false;
            RegistryKey rk = Registry.LocalMachine.OpenSubKey(keyName);
            if (rk == null) return false;
            string[] valueNames = rk.GetValueNames();
            _params.Clear();
            foreach (string name in valueNames) {
                switch (name) {
                    case "PublicName":
                        PublicName = (string)rk.GetValue(name);
                        break;
                    case "ServerAddress":
                        _serverAddress = (string)rk.GetValue(name);
                        break;
                    case "ServerPort":
                        _serverPort = (string)rk.GetValue(name);
                        break;
                    case "Username":
                        _username = (string)rk.GetValue(name);
                        break;
                    case "Password":
                        _password = Common.huffman.decrypt((string)rk.GetValue(name));
                        break;
                    case "DatabaseName":
                        _databaseName = (string)rk.GetValue(name);
                        break;
                    default:
                        _params.Add(name, (string)rk.GetValue(name));
                        break;
                }
            }
            rk.Close();
            return true;
        }
        /// <summary>
        /// Saves current parameters setting to registry.
        /// </summary>
        /// <returns>True, if parameters setting successfully written, otherwise, false.</returns>
        public bool saveSetting() {
            string keyName = "SOFTWARE\\" + CompanyName + "\\" + SubName + "\\" + AppName + "\\" + Version;
            try {
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(keyName, true);
                if (rk == null) rk = Registry.LocalMachine.CreateSubKey(keyName);
                rk.SetValue("PublicName", PublicName);
                rk.SetValue("ServerAddress", _serverAddress);
                rk.SetValue("ServerPort", _serverPort);
                rk.SetValue("Username", _username);
                rk.SetValue("Password", Common.huffman.encrypt(_password));
                rk.SetValue("DatabaseName", _databaseName);
                foreach (string key in _params.Keys) rk.SetValue(key, _params[key]);
                return true;
            } catch (Exception) {
                return false;
            }
        }
        /// <summary>
        /// Open a connection to the database server.
        /// </summary>
        /// <returns>True, if connection has been established successfully, otherwise, false.</returns>
        public abstract bool open();
        /// <summary>
        /// Close the current connection to the database server.
        /// </summary>
        /// <returns>True, if the connection has been closed successfully, otherwise, false.</returns>
        public abstract bool close();
        /// <summary>
        /// Executes a Transact-SQL statement against the connection.
        /// </summary>
        /// <param name="command">SQL command to be executed.</param>
        /// <param name="caller">The name of the subroutine that calls this function.</param>
        /// <returns>True, if command executed successfully, false, otherwise.</returns>
        public abstract bool executeSQL(string command, string caller);
        /// <summary>
        /// Executes a Transact-SQL statement against the connection.
        /// </summary>
        /// <param name="command">SQL command to be executed.</param>
        /// <returns>True, if command executed successfully, false, otherwise.</returns>
        public abstract bool executeSQL(string command);
        /// <summary>
        /// Executes a SQL command and retrieves the resulting data through data reader.
        /// </summary>
        /// <param name="command">SQL command to be executed.</param>
        /// <param name="caller">The name of the subroutine that calls this function.</param>
        /// <returns>True, if command executed successfully, false, otherwise.</returns>
        public abstract bool executeData(string command, string caller);
        /// <summary>
        /// Executes a SQL command and retrieves the resulting data through data reader.
        /// </summary>
        /// <param name="command">SQL command to be executed.</param>
        /// <returns>True, if command executed successfully, false, otherwise.</returns>
        public abstract bool executeData(string command);
        /// <summary>
        /// Executes a sql command represent the name of the stored procedure.
        /// </summary>
        /// <param name="command">Name of the stored procedure.</param>
        /// <param name="spParams">Parameters needed to execute the procedure.</param>
        /// <param name="caller">The name of the subroutine that calls this function.</param>
        /// <returns>True, if command executed successfully, false, otherwise.</returns>
        public abstract bool executeSP(string command, DbParameterCollection spParams, string caller);
        /// <summary>
        /// Executes a sql command represent the name of the stored procedure.
        /// </summary>
        /// <param name="command">Name of the stored procedure.</param>
        /// <param name="spParams">Parameters needed to execute the procedure.</param>
        /// <returns>True, if command executed successfully, false, otherwise.</returns>
        public abstract bool executeSP(string command, DbParameterCollection spParams);
        /// <summary>
        /// Executes a sql command represent the name of the stored procedure.
        /// </summary>
        /// <param name="command">Name of the stored procedure.</param>
        /// <param name="caller">The name of the subroutine that calls this function.</param>
        /// <returns>True, if command executed successfully, false, otherwise.</returns>
        public abstract bool executeSP(string command, string caller);
        /// <summary>
        /// Executes a sql command represent the name of the stored procedure.
        /// </summary>
        /// <param name="command">Name of the stored procedure.</param>
        /// <returns>True, if command executed successfully, false, otherwise.</returns>
        public abstract bool executeSP(string command);
        /// <summary>
        /// Formats a date object to a string object, database specified.
        /// </summary>
        /// <param name="value">A date value to be formatted.</param>
        /// <returns>A string object represent the formatted date.</returns>
        public abstract string formatDate(DateTime value);
        /// <summary>
        /// Formats a date and time object to a string object, database specified.
        /// </summary>
        /// <param name="value">A date value to be formatted.</param>
        /// <returns>A string object represent the formatted date.</returns>
        public abstract string formatDateTime(DateTime value);
        #endregion
        #region IDisposable Members
        bool _disposed = false;
        public void Dispose() {
            if (!_disposed) {
                _params.Clear();
                if (_reader != null) {
                    _reader.Close();
                    _reader.Dispose();
                    _reader = null;
                }
                if (_command != null) {
                    _command.Cancel();
                    _command.Dispose();
                    _command = null;
                }
                if (_connection != null) {
                    // Close if connection is opened.
                    close();
                    _connection.Dispose();
                    _connection = null;
                }
                _params = null;
                _disposed = true;
            }
        }
        #endregion
    }
    /// <summary>
    /// Provide common functions to establish connection to a SQL Server database.
    /// </summary>
    public sealed class SQLConnection : Connection {
        public SQLConnection() : base() {
            _connection = new SqlConnection();
            _command = new SqlCommand();
            _command.Connection = _connection;
            // Configure default parameter for SQL server.
            _serverAddress = "(local)";
            _serverPort = "1433";
            setParam("enablemars", "False");
            // Attaching events.
            _connection.StateChange += _StateChange;
        }
        #region Specific Implementation
        /// <summary>
        /// Gets or sets a value indicating the connection should use MARS(multiple active result sets).
        /// </summary>
        public bool EnableMARS {
            get { return getParam("enablemars") == "True"; }
            set { setParam("enablemars", value.ToString()); }
        }
        #endregion
        public override string ConnectionString {
            get {
                string strConn = "";
                strConn = "Data Source=" + _serverAddress + "," + _serverPort;
                // Using custom network library
                if (_params.ContainsKey("network library")) strConn += ";Network Library=" + _params["network library"];
                strConn += ";Initial Catalog=" + _databaseName +
                    ";User ID=" + _username + ";Password=" + _password + ";";
                // Enable MARS
                if (getParam("enablemars") == "True") strConn += "MultipleActiveResultSets=true;";
                return strConn;
            }
        }
        public override bool open() {
            if (_connection.State != ConnectionState.Closed) {
                _errMsg = "The connection is not closed.";
                _CommandExecuted(this);
                return false;
            }
            try {
                _connection.ConnectionString = ConnectionString;
                _connection.Open();
                return true;
            } catch (Exception ex) {
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool close() {
            if (_connection.State != ConnectionState.Open) {
                _errMsg = "Connection is not open.";
                _CommandExecuted(this);
                return false;
            }
            try {
                _connection.Close();
                return true;
            } catch (Exception ex) {
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool executeSQL(string command, string caller) {
            _caller = caller;
            _command.CommandText = command;
            _command.CommandType = CommandType.Text;
            _Executing(this);
            try {
                _affectedRows = _command.ExecuteNonQuery();
                _CommandExecuted(this);
                return true;
            } catch (Exception ex) {
                _affectedRows = 0;
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool executeSQL(string command) {
            return executeSQL(command, "");
        }
        public override bool executeData(string command, string caller) {
            if (_reader != null) {
                _reader.Close();
                _reader.Dispose();
            }
            _caller = caller;
            _command.CommandText = command;
            _command.CommandType = CommandType.Text;
            _Executing(this);
            try {
                _reader = _command.ExecuteReader();
                _CommandExecuted(this);
                return true;
            } catch (Exception ex) {
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool executeData(string command) {
            return executeData(command, "");
        }
        public override bool executeSP(string command, DbParameterCollection spParams, string caller) {
            if (_reader != null) {
                _reader.Close();
                _reader.Dispose();
            }
            _caller = caller;
            _command.CommandText = command;
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.Clear();
            if (spParams != null) foreach (SqlParameter sp in spParams) { _command.Parameters.Add(sp); }
            _Executing(this);
            try {
                _reader = _command.ExecuteReader();
                _CommandExecuted(this);
                return true;
            } catch (Exception ex) {
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool executeSP(string command, DbParameterCollection spParams) {
            return executeSP(command, spParams, "");
        }
        public override bool executeSP(string command, string caller) {
            return executeSP(command, null, caller);
        }
        public override bool executeSP(string command) {
            return executeSP(command, null, "");
        }
        public override string formatDate(DateTime value) {
            return "";
        }
        public override string formatDateTime(DateTime value) {
            return "";
        }
    }
    /// <summary>
    /// Provide common functions to establish connection to an Oracle database.
    /// </summary>
    public sealed class OracleConnection : Connection {
        public OracleConnection() : base() {
            _connection = new System.Data.OracleClient.OracleConnection();
            _command = new OracleCommand();
            _command.Connection = _connection;
            // Attaching events.
            _connection.StateChange += _StateChange;
            // Configure default parameter for oracle.
            setParam("usetnsnames", "True");
            setParam("useconnectionpooling", "False");
            setParam("privilege", "Normal");
            setParam("serviceid", "");
        }
        #region Oracle Specific
        /// <summary>
        /// Privilege of the client used to connect to oracle.
        /// </summary>
        public enum ConnectPrivilege {
            Normal,
            SYSDBA,
            SYSOPER
        }
        /// <summary>
        /// Gets or sets a value indicating the connection should use tnsnames.ora file.
        /// </summary>
        public bool UseTnsnames {
            get { return getParam("usetnsnames") == "True"; }
            set { setParam("usetnsnames", value.ToString()); }
        }
        /// <summary>
        /// Gets or sets the value indicating the connection use pooling.
        /// </summary>
        public bool UseConnectionPooling {
            get { return getParam("useconnectionpooling") == "True"; }
            set { setParam("useconnectionpooling", value.ToString()); }
        }
        /// <summary>
        /// Gets or sets the privilege used to connect to oracle.
        /// </summary>
        public ConnectPrivilege Privilege {
            get {
                string strPrivilege = getParam("privilege");
                if (strPrivilege == "SYSDBA") return ConnectPrivilege.SYSDBA;
                else if (strPrivilege == "SYSOPER") return ConnectPrivilege.SYSOPER;
                else return ConnectPrivilege.Normal;
            }
            set { setParam("privilege", value.ToString()); }
        }
        /// <summary>
        /// Gets or sets the id of the oracle service used to connect to oracle database.
        /// </summary>
        public string ServiceID {
            get { return getParam("serviceid"); }
            set { setParam("serviceid", value); }
        }
        #endregion
        public override string ConnectionString {
            get {
                string strConn = "";
                if (getParam("usetnsnames") == "True") {
                    strConn = "Data Source=" + _serverAddress +
                        ";User ID=" + _username + ";Password=" + _password + ";";
                } else {
                    strConn = "Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=" + _serverAddress
                        + ")(PORT=" + _serverPort + "))(CONNECT_DATA=(SERVICE_NAME=" + getParam("serviceid")
                        + ")));User Id=" + _username + ";Password=" + _password + ";";
                }
                if (getParam("useconnectionpooling") == "True") {
                    if (_params.ContainsKey("min pool size")) strConn += "Min Pool Size=" + getParam("min pool size") + ";";
                    if (_params.ContainsKey("connection lifetime")) strConn += "Connection Lifetime=" + getParam("connection lifetime") + ";";
                    if (_params.ContainsKey("connection timeout")) strConn += "Connection Timeout=" + getParam("connection timeout") + ";";
                    if (_params.ContainsKey("incr pool size")) strConn += "Incr Pool Size=" + getParam("incr pool size") + ";";
                    if (_params.ContainsKey("decr pool size")) strConn += "Decr Pool Size=" + getParam("decr pool size") + ";";
                }
                if (getParam("privilege") != "Normal") strConn += "DBA Privilege=" + getParam("privilege") + ";";
                return strConn;
            }
        }
        public override bool open() {
            if (_connection.State != ConnectionState.Closed) {
                _errMsg = "The connection is not closed.";
                _CommandExecuted(this);
                return false;
            }
            try {
                _connection.ConnectionString = ConnectionString;
                _connection.Open();
                return true;
            } catch (Exception ex) {
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool close() {
            if (_connection.State != ConnectionState.Open) {
                _errMsg = "Connection is not open.";
                _CommandExecuted(this);
                return false;
            }
            try {
                _connection.Close();
                return true;
            } catch (Exception ex) {
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool executeSQL(string command, string caller) {
            _caller = caller;
            _command.CommandText = command;
            _command.CommandType = CommandType.Text;
            _Executing(this);
            try {
                _affectedRows = _command.ExecuteNonQuery();
                _CommandExecuted(this);
                return true;
            } catch (Exception ex) {
                _affectedRows = 0;
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool executeSQL(string command) {
            return executeSQL(command, "");
        }
        public override bool executeData(string command, string caller) {
            /*if (_reader != null) {
                _reader.Close();
                _reader.Dispose();
            }*/
            _caller = caller;
            _command.CommandText = command;
            _command.CommandType = CommandType.Text;
            _Executing(this);
            try {
                _reader = _command.ExecuteReader();
                _CommandExecuted(this);
                return true;
            } catch (Exception ex) {
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool executeData(string command) {
            return executeData(command, "");
        }
        public override bool executeSP(string command, DbParameterCollection spParams, string caller) {
            /*if (_reader != null) {
                _reader.Close();
                _reader.Dispose();
            }*/
            _caller = caller;
            _command.CommandText = command;
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.Clear();
            if (spParams != null) {
                foreach (DbParameter param in spParams) _command.Parameters.Add(param);
            }
            _Executing(this);
            try {
                _reader = _command.ExecuteReader();
                _CommandExecuted(this);
                return true;
            } catch (Exception ex) {
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool executeSP(string command, DbParameterCollection spParams) {
            return executeSP(command, spParams, "");
        }
        public override bool executeSP(string command, string caller) {
            return executeSP(command, null, caller);
        }
        public override bool executeSP(string command) {
            return executeSP(command, null, "");
        }
        public override string formatDate(DateTime value) {
            return value.ToString("dd-MMM-yyyy");
        }
        public override string formatDateTime(DateTime value) {
            return "to_date('" + value.ToString("yyyy/MM/dd HH:mm:ss") + "', 'yyyy/mm/dd hh24:mi:ss')";
        }
        /// <summary>
        /// Gets TNSNames file path from system path.
        /// http://www.codeproject.com/Articles/30962/TNSNames-Reader
        /// </summary>
        /// <returns>A string represent the file path of tnmsnames.ora file.</returns>
        public static string getTNSNamesFilePath() {
            string systemPath = Environment.GetEnvironmentVariable("Path");
            Regex reg = new Regex("[a-zA-Z]:\\\\[a-zA-Z0-9\\\\]*(oracle|app)[a-zA-Z0-9_.\\\\]*(?=bin)");
            MatchCollection col = reg.Matches(systemPath);

            string subpath = "network\\ADMIN\\tnsnames.ora";
            foreach (Match match in col) {
                string path = match.ToString() + subpath;
                if (File.Exists(path)) return path;
            }
            return string.Empty;
        }
        /// <summary>
        /// Get the list of tns names stored in TNSNames.ora file.
        /// http://www.codeproject.com/Articles/30962/TNSNames-Reader
        /// </summary>
        /// <returns>A list of tns names from TNSNames.ora file</returns>
        public static List<string> getTNSNames(string tnsnamesfile) {
            List<string> result = new List<string>();
            string regPattern = @"[\n][\s]*[^\(][a-zA-Z0-9_.]+[\s]*=[\s]*\(";
            string regPatternName = @"[a-zA-Z0-9_.]+";
            if (!tnsnamesfile.Equals("")) {
                FileInfo fInfo = new FileInfo(tnsnamesfile);
                if (fInfo.Exists) {
                    if (fInfo.Length > 0) {
                        string contents = File.ReadAllText(fInfo.FullName);
                        int tnsCount = Regex.Matches(contents, regPattern).Count;
                        MatchCollection col = Regex.Matches(contents, regPattern);
                        foreach (Match match in col) {
                            Regex n = new Regex(regPatternName);
                            result.Add(n.Match(match.ToString()).ToString());
                        }
                    }
                }
            }
            return result;
        }
    }
    /* For MySQL Database
    /// <summary>
    /// Provide common functions to establish connection to a MySQL database.
    /// </summary>
    public sealed class MySQLConnection : Connection {
        public MySQLConnection() : base() {
            _connection = new MySql.Data.MySqlClient.MySqlConnection();
            _command = new MySqlCommand();
            _command.Connection = _connection;
            // Attaching events.
            _connection.StateChange += _StateChange;
            // Configure default parameter for MySQL.
            setParam("useencryption", "False");
        }
        #region MySQL Specific
        /// <summary>
        /// Gets or sets a value indicating transfer data between client and server should use SSL encryption.
        /// </summary>
        public bool UseEncryption {
            get { return getParam("useencryption") == "True"; }
            set { setParam("useencryption", value.ToString()); }
        }
        #endregion
        public override string ConnectionString {
            get {
                string strConn = "Server=" + _serverAddress
                    + ";Database=" + _databaseName + ";Uid=" + _username + ";Pwd=" + _password + ";";
                if (getParam("useencryption") == "True") strConn += "Encryption=True;";
                return strConn;
            }
        }
        public override bool open() {
            if (_connection.State != ConnectionState.Closed) {
                _errMsg = "The connection is not closed.";
                _CommandExecuted(this);
                return false;
            }
            try {
                _connection.ConnectionString = ConnectionString;
                _connection.Open();
                return true;
            } catch (Exception ex) {
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool close() {
            if (_connection.State != ConnectionState.Open) {
                _errMsg = "Connection is not open.";
                _CommandExecuted(this);
                return false;
            }
            try {
                _connection.Close();
                return true;
            } catch (Exception ex) {
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool executeSQL(string command, string caller) {
            _caller = caller;
            _command.CommandText = command;
            _command.CommandType = CommandType.Text;
            _Executing(this);
            try {
                _affectedRows = _command.ExecuteNonQuery();
                _CommandExecuted(this);
                return true;
            } catch (Exception ex) {
                _affectedRows = 0;
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool executeSQL(string command) {
            return executeSQL(command, "");
        }
        public override bool executeData(string command, string caller) {
            if (_reader != null) {
                _reader.Close();
                _reader.Dispose();
            }
            _caller = caller;
            _command.CommandText = command;
            _command.CommandType = CommandType.Text;
            _Executing(this);
            try {
                _reader = _command.ExecuteReader();
                _CommandExecuted(this);
                return true;
            } catch (Exception ex) {
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool executeData(string command) {
            return executeData(command, "");
        }
        public override bool executeSP(string command, DbParameterCollection spParams, string caller) {
            if (_reader != null) {
                _reader.Close();
                _reader.Dispose();
            }
            _caller = caller;
            _command.CommandText = command;
            _command.CommandType = CommandType.StoredProcedure;
            _command.Parameters.Clear();
            if (spParams != null) { foreach (DbParameter param in spParams) _command.Parameters.Add(param); }
            _Executing(this);
            try {
                _reader = _command.ExecuteReader();
                _CommandExecuted(this);
                return true;
            } catch (Exception ex) {
                _errMsg = ex.Message;
                _CommandExecuted(this);
                return false;
            }
        }
        public override bool executeSP(string command, DbParameterCollection spParams) {
            return executeSP(command, spParams, "");
        }
        public override bool executeSP(string command, string caller) {
            return executeSP(command, null, "");
        }
        public override bool executeSP(string command) {
            return executeSP(command, null, "");
        }
    }
    */
    /// <summary>
    /// Provides data for events raised by Connection class.
    /// </summary>
    public class ConnectionEventArgs : EventArgs {
        ConnectionState _state;
        int _errCode = 0;
        string _errMsg = "";
        int _affectedRows = 0;
        public ConnectionEventArgs(ConnectionState state, int errorCode, string errorMsg, int affectedRows) : base() {
            _state = state;
            _errCode = errorCode;
            _errMsg = errorMsg;
            _affectedRows = affectedRows;
        }
        public ConnectionState State { get { return _state; } }
        public int ErrorCode { get { return _errCode; } }
        public string ErrorMessage { get { return _errMsg; } }
        public int AffectedRows { get { return _affectedRows; } }
    }
}