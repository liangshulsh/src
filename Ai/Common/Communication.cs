// Ai Software Library.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.Security;
using System.Security.Permissions;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Ai.Trade.Common {
    [DnsPermission(SecurityAction.Demand, Unrestricted = true)]
    public static class Communication {
        public const int PORTNUMBER = 13131;
        public static string getComputerName() {
            // System Informations
            string computerName = "";
            Type t = typeof(System.Windows.Forms.SystemInformation);
            PropertyInfo[] pi = t.GetProperties();
            foreach (PropertyInfo p in pi) {
                if (p.Name == "ComputerName") {
                    object pValue = p.GetValue(null, null);
                    computerName = pValue.ToString();
                }
            }
            return computerName;
        }
        public static string getIPAddress(string hostName) {
            try {
                string ipAddresses = "";
                IPAddress[] ip = Dns.GetHostEntry(hostName).AddressList;
                foreach (IPAddress ia in ip) {
                    if (ia.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork) {
                        ipAddresses = ia.ToString();
                        break;
                    }
                }
                return ipAddresses;
            } catch (Exception ex) {
                return ex.Message;
            }
        }
        public static Socket openSocket(string host) {
            Socket sck = null;
            IPHostEntry ih = Dns.GetHostEntry(host);
            foreach (IPAddress address in ih.AddressList) {
                IPEndPoint ipe = new IPEndPoint(address, PORTNUMBER);
                Socket tmpSocket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                tmpSocket.Connect(ipe);
                if (tmpSocket.Connected) {
                    sck = tmpSocket;
                    break;
                }
            }
            return sck;
        }
        /// <summary>
        /// http://www.codeproject.com/KB/IP/ListNetworkComputers.aspx
        /// </summary>
        public class EnumNetwork {
            #region Dll Imports
            /// <summary>
            /// Netapi32.dll : The NetServerEnum function lists all servers
            /// of the specified type that are visible in a domain. For example, an 
            /// application can call NetServerEnum to list all domain controllers only
            /// or all SQL servers only.
            ///	You can combine bit masks to list several types. For example, a value 
            /// of 0x00000003  combines the bit masks for SV_TYPE_WORKSTATION 
            /// (0x00000001) and SV_TYPE_SERVER (0x00000002)
            /// </summary>
            [DllImport("Netapi32", CharSet = CharSet.Auto, SetLastError = true),
            SuppressUnmanagedCodeSecurityAttribute]
            public static extern int NetServerEnum(
                string ServerNane, // must be null
                int dwLevel,
                ref IntPtr pBuf,
                int dwPrefMaxLen,
                out int dwEntriesRead,
                out int dwTotalEntries,
                int dwServerType,
                string domain, // null for login domain
                out int dwResumeHandle
                );
            /// <summary>
            /// Netapi32.dll : The NetApiBufferFree function frees 
            /// the memory that the NetApiBufferAllocate function allocates. 
            /// Call NetApiBufferFree to free the memory that other network 
            /// management functions return.
            /// </summary>
            [DllImport("Netapi32", SetLastError = true),
            SuppressUnmanagedCodeSecurityAttribute]
            public static extern int NetApiBufferFree(
                IntPtr pBuf);
            //create a _SERVER_INFO_100 STRUCTURE
            [StructLayout(LayoutKind.Sequential)]
            public struct _SERVER_INFO_100 {
                internal int sv100_platform_id;
                [MarshalAs(UnmanagedType.LPWStr)]
                internal string sv100_name;
            }
            #endregion
            #region Public Methods
            /// <summary>
            /// Uses the DllImport : NetServerEnum with all its required parameters
            /// (see http://msdn.microsoft.com/library/default.asp?url=/library/en-us/netmgmt/netmgmt/netserverenum.asp
            /// for full details or method signature) to retrieve a list of domain SV_TYPE_WORKSTATION
            /// and SV_TYPE_SERVER PC's
            /// </summary>
            /// <returns>Arraylist that represents all the SV_TYPE_WORKSTATION and SV_TYPE_SERVER
            /// PC's in the Domain</returns>
            public ArrayList getNetworkComputers() {
                //local fields
                ArrayList networkComputers = new ArrayList();
                const int MAX_PREFERRED_LENGTH = -1;
                int SV_TYPE_WORKSTATION = 1;
                int SV_TYPE_SERVER = 2;
                IntPtr buffer = IntPtr.Zero;
                IntPtr tmpBuffer = IntPtr.Zero;
                int entriesRead = 0;
                int totalEntries = 0;
                int resHandle = 0;
                int sizeofINFO = Marshal.SizeOf(typeof(_SERVER_INFO_100));
                try {
                    //call the DllImport : NetServerEnum with all its required parameters
                    //see http://msdn.microsoft.com/library/default.asp?url=/library/en-us/netmgmt/netmgmt/netserverenum.asp
                    //for full details of method signature
                    int ret = NetServerEnum(null, 100, ref buffer, MAX_PREFERRED_LENGTH,
                        out entriesRead,
                        out totalEntries, SV_TYPE_WORKSTATION | SV_TYPE_SERVER, null, out 
					    resHandle);
                    //if the returned with a NERR_Success (C++ term), =0 for C#
                    if (ret == 0) {
                        //loop through all SV_TYPE_WORKSTATION and SV_TYPE_SERVER PC's
                        for (int i = 0; i < totalEntries; i++) {
                            //get pointer to, Pointer to the buffer that received the data from
                            //the call to NetServerEnum. Must ensure to use correct size of 
                            //STRUCTURE to ensure correct location in memory is pointed to
                            tmpBuffer = new IntPtr((int)buffer + (i * sizeofINFO));
                            //Have now got a pointer to the list of SV_TYPE_WORKSTATION and 
                            //SV_TYPE_SERVER PC's, which is unmanaged memory
                            //Needs to Marshal data from an unmanaged block of memory to a 
                            //managed object, again using STRUCTURE to ensure the correct data
                            //is marshalled 
                            _SERVER_INFO_100 svrInfo = (_SERVER_INFO_100)Marshal.PtrToStructure(tmpBuffer, typeof(_SERVER_INFO_100));
                            //add the PC names to the ArrayList
                            networkComputers.Add(svrInfo.sv100_name);
                        }
                    }
                } catch (Exception) {
                    return null;
                } finally {
                    //The NetApiBufferFree function frees 
                    //the memory that the NetApiBufferAllocate function allocates
                    NetApiBufferFree(buffer);
                }
                //return entries found
                return networkComputers;
            }
            #endregion
        }
        public enum MessageType { Registering = 0, Message = 1 }
        public enum MessageDirection { Outgoing = 0, Incoming = 1 }
        public class ChatMessage {
            DateTime _messageTime;
            string _message = "";
            MessageType _type = MessageType.Message;
            MessageDirection _direction = MessageDirection.Outgoing;
            bool _sent = false;
            public ChatMessage() { }
            public ChatMessage(string message) { _message = message; }
            public DateTime MessageTime {
                get { return _messageTime; }
                set { _messageTime = value; }
            }
            public string Message {
                get { return _message; }
                set { _message = value; }
            }
            public MessageType Type {
                get { return _type; }
                set { _type = value; }
            }
            public bool Sent {
                get { return _sent; }
                set { _sent = value; }
            }
            public MessageDirection Direction {
                get { return _direction; }
                set { _direction = value; }
            }
            public static byte[] messageToBytes(MessageType type, string contents) {
                List<byte> result = new List<byte>();
                result.AddRange(BitConverter.GetBytes((int)type));
                if (contents != null) result.AddRange(BitConverter.GetBytes(contents.Length));
                else result.AddRange(BitConverter.GetBytes(0));
                if (contents != null) result.AddRange(Encoding.UTF8.GetBytes(contents));
                return result.ToArray();
            }
            public static byte[] messageToBytes(ChatMessage m) {
                List<byte> result = new List<byte>();
                if (m != null) {
                    result.AddRange(BitConverter.GetBytes((int)m.Type));
                    if (m.Message != null) result.AddRange(BitConverter.GetBytes(m.Message.Length));
                    else result.AddRange(BitConverter.GetBytes(0));
                    if (m.Message != null) result.AddRange(Encoding.UTF8.GetBytes(m.Message));
                }
                return result.ToArray();
            }
            public static ChatMessage bytesToMessage(byte[] data) {
                ChatMessage m = new ChatMessage();
                if (data.Length > 8) {
                    m.Type = (MessageType)BitConverter.ToInt32(data, 0);
                    int msgLength = BitConverter.ToInt32(data, 4);
                    if (msgLength > 0) m.Message = Encoding.UTF8.GetString(data, 8, msgLength);
                }
                return m;
            }
        }
        public enum ConnectionState { Online, Offline }
        // Refs : A Chat Application Using Asynchronous TCP Sockets @ http://www.codeproject.com/KB/IP/ChatAsynchTCPSockets.aspx
        public class ClientInfo {
            string _name = "";
            Socket _socket = null;
            Socket _clientSocket = null;
            string _status = "";
            Image _image = null;
            ConnectionState _state = ConnectionState.Offline;
            ChatMessage _currentMessage = null;
            byte[] _data = new byte[1024];
            bool _connected = false;
            List<ChatMessage> _messages = new List<ChatMessage>();
            string _errorMessage = "";
            // Server information
            string _serverName = "";
            string _serverStatus = "";
            ConnectionState _serverState = ConnectionState.Offline;
            // Public Events
            public event EventHandler<EventArgs> Connect;
            public event EventHandler<EventArgs> Send;
            public event EventHandler<EventArgs> Error;
            public ClientInfo() { }
            public ClientInfo(string name, Socket socket) {
                _name = name;
                _socket = socket;
            }
            public string Name {
                get { return _name; }
                set { _name = value; }
            }
            public Socket Socket {
                get { return _socket; }
                set { _socket = value; }
            }
            public string Status {
                get { return _status; }
                set { _status = value; }
            }
            public string IPAddress {
                get {
                    string ipa = "";
                    if (_socket != null) {
                        IPEndPoint iep = (IPEndPoint)_socket.RemoteEndPoint;
                        ipa = iep.Address.ToString();
                    }
                    return ipa;
                }
            }
            public Image Image {
                get { return _image; }
                set { _image = value; }
            }
            public ConnectionState State {
                get { return _state; }
                set { _state = value; }
            }
            public bool Connected { get { return _connected; } }
            public string ServerName {
                get { return _serverName; }
                set { _serverName = value; }
            }
            public string ServerStatus {
                get { return _serverStatus; }
                set { _serverStatus = value; }
            }
            public string ErrorMessage { get { return _errorMessage; } }
            public ConnectionState ServerState {
                get { return _serverState; }
                set { _serverState = value; }
            }
            public bool connectServer() {
                _errorMessage = "";
                if (_socket == null) return false;
                if (_clientSocket == null) _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try {
                    IPEndPoint iepRemote = (IPEndPoint)_socket.RemoteEndPoint;
                    IPEndPoint iepServer = new IPEndPoint(iepRemote.Address, PORTNUMBER);
                    _clientSocket.BeginConnect(iepServer, new AsyncCallback(_Connect), null);
                    return true;
                } catch (Exception ex) {
                    _errorMessage = ex.Message;
                    if (Error != null) Error(this, new EventArgs());
                    return false;
                }
            }
            public List<ChatMessage> Messages { get { return _messages; } }
            public bool sendMessage(ChatMessage m) {
                _errorMessage = "";
                try {
                    m.Direction = MessageDirection.Outgoing;
                    byte[] data = ChatMessage.messageToBytes(m);
                    _clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(_Send), null);
                    return true;
                } catch (Exception ex) {
                    _errorMessage = ex.Message;
                    if (Error != null) Error(this, new EventArgs());
                    return false;
                }
            }
            public bool sendMessage(string message, MessageType type) {
                ChatMessage m = new ChatMessage(message);
                m.Type = type;
                return sendMessage(m);
            }
            public bool sendMessage(string message) {
                ChatMessage m = new ChatMessage(message);
                m.Type = MessageType.Message;
                return sendMessage(m);
            }
            public bool updateServerStatus() {
                _errorMessage = "";
                try {
                    // Send server information to the server.
                    ChatMessage m = new ChatMessage();
                    m.Direction = MessageDirection.Outgoing;
                    m.Message = _serverName + "\n" + _serverStatus + "\n";
                    if (_serverState == ConnectionState.Offline) m.Message = m.Message + "Offline";
                    else m.Message = m.Message + "Online";
                    m.Type = MessageType.Registering;
                    byte[] data = ChatMessage.messageToBytes(m);
                    _clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(_Send), null);
                    return true;
                } catch (Exception ex) {
                    _errorMessage = ex.Message;
                    if (Error != null) Error(this, new EventArgs());
                    return false;
                }
            }
            public bool close() {
                _errorMessage = "";
                try {
                    _clientSocket.Close();
                    _connected = false;
                    return true;
                } catch (Exception ex) {
                    _errorMessage = ex.Message;
                    if (Error != null) Error(this, new EventArgs());
                    return false;
                }
            }
            private void _Connect(IAsyncResult ia) {
                _errorMessage = "";
                try {
                    _clientSocket.EndConnect(ia);
                    // Send server information to the server.
                    ChatMessage m = new ChatMessage();
                    m.Direction = MessageDirection.Outgoing;
                    m.Message = _serverName + "\n" + _serverStatus + "\n";
                    if (_serverState == ConnectionState.Offline) m.Message = m.Message + "Offline";
                    else m.Message = m.Message + "Online";
                    m.Type = MessageType.Registering;
                    byte[] data = ChatMessage.messageToBytes(m);
                    _clientSocket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(_Send), null);
                    _connected = true;
                    if (Connect != null) Connect(this, new EventArgs());
                } catch (Exception ex) {
                    _errorMessage = ex.Message;
                    if (Error != null) Error(this, new EventArgs());
                    _connected = false;
                }
            }
            private void _Send(IAsyncResult ia) {
                _errorMessage = "";
                try {
                    _clientSocket.EndSend(ia);
                    if (_currentMessage != null) {
                        if (_currentMessage.Type != MessageType.Registering) {
                            _currentMessage.Sent = true;
                            _messages.Add(_currentMessage);
                            if (Send != null) Send(this, new EventArgs());
                        }
                    }
                } catch (Exception ex) {
                    _errorMessage = ex.Message;
                    if (Error != null) Error(this, new EventArgs());
                }
            }
        }
        public class ClientEventArgs : EventArgs {
            ClientInfo _ci = null;
            public ClientEventArgs(ClientInfo ci) {
                _ci = ci;
            }
            public ClientInfo Client { get { return _ci; } }
        }
        public class MessageReceivedEventArgs : EventArgs {
            ClientInfo _ci = null;
            ChatMessage _message = null;
            public MessageReceivedEventArgs(ClientInfo ci, ChatMessage cm) : base() {
                _ci = ci;
                _message = cm;
            }
            public ClientInfo Client { get { return _ci; } }
            public ChatMessage Message { get { return _message; } }
        }
        public class Server {
            #region Public Events
            public event EventHandler<ClientEventArgs> ClientConnected;
            public event EventHandler<ClientEventArgs> ClientRegistered;
            public event EventHandler<MessageReceivedEventArgs> MessageReceived;
            public event EventHandler<EventArgs> Error;
            public event EventHandler<EventArgs> Open;
            #endregion
            Socket _socket;
            List<ClientInfo> _clients = new List<ClientInfo>();
            bool _listening = false;
            byte[] _data = new byte[1024];
            string _errorMessage = "";
            string _name = "";
            string _status = "";
            ConnectionState _state = ConnectionState.Offline;
            public Server() {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            public string Name {
                get { return _name; }
                set {
                    if (_name != value) {
                        _name = value;
                        foreach (ClientInfo ci in _clients) {
                            ci.ServerName = _name;
                            ci.updateServerStatus();
                        }
                    }
                }
            }
            public string Status {
                get { return _status; }
                set {
                    if (_status != value) {
                        foreach (ClientInfo ci in _clients) {
                            ci.ServerStatus = _status;
                            ci.updateServerStatus();
                        }
                    }
                }
            }
            public ConnectionState State {
                get { return _state; }
                set {
                    if (_state != value) {
                        _state = value;
                        foreach (ClientInfo ci in _clients) {
                            ci.ServerState = _state;
                            ci.updateServerStatus();
                        }
                    }
                }
            }
            public bool Listening { get { return _listening; } }
            public bool open() {
                _errorMessage = "";
                try {
                    IPEndPoint iep = new IPEndPoint(IPAddress.Any, PORTNUMBER);
                    _socket.Bind(iep);
                    _socket.Listen(4);
                    _listening = true;
                    _socket.BeginAccept(new AsyncCallback(_Accept), null);
                    if (Open != null) Open(this, new EventArgs());
                    return true;
                } catch (Exception ex) {
                    _errorMessage = ex.Message;
                    _listening = false;
                    if (Error != null) Error(this, new EventArgs());
                    return false;
                }
            }
            public bool close() {
                _errorMessage = "";
                try {
                    _socket.Close();
                    _listening = false;
                    return true;
                } catch (Exception ex) {
                    _errorMessage = ex.Message;
                    if (Error != null) Error(this, new EventArgs());
                    return false;
                }
            }
            public ClientInfo getClient(Socket socket) {
                ClientInfo result = null;
                if (socket != null) {
                    IPEndPoint iep = (IPEndPoint)socket.RemoteEndPoint;
                    foreach (ClientInfo ci in _clients) {
                        if (ci.IPAddress == iep.Address.ToString()) {
                            result = ci;
                            break;
                        }
                    }
                }
                return result;
            }
            public ClientInfo getClient(string address) {
                ClientInfo result = null;
                if (address != null) {
                    foreach (ClientInfo ci in _clients) {
                        if (ci.IPAddress == address) {
                            result = ci;
                            break;
                        }
                    }
                }
                return result;
            }
            private void _Accept(IAsyncResult ia) {
                try {
                    // Accept new client request.
                    Socket client = _socket.EndAccept(ia);
                    IPEndPoint iep = (IPEndPoint)client.RemoteEndPoint;
                    // Listens for another clients.
                    _socket.BeginAccept(new AsyncCallback(_Accept), null);
                    // Searching for existing client.
                    bool found = false;
                    foreach (ClientInfo ci in _clients) {
                        if (ci.IPAddress == iep.ToString()) {
                            ci.Socket = client;
                            ci.connectServer();
                            found = true;
                            if (ClientConnected != null) ClientConnected(this, new ClientEventArgs(ci));
                            ci.updateServerStatus();
                            break;
                        }
                    }
                    if (!found) {
                        ClientInfo nci = new ClientInfo();
                        nci.Socket = client;
                        nci.State = ConnectionState.Online;
                        nci.connectServer();
                        _clients.Add(nci);
                        if (ClientConnected != null) ClientConnected(this, new ClientEventArgs(nci));
                        nci.updateServerStatus();
                    }
                    // Continue listening for this client.
                    client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(_Receive), client);
                } catch (Exception ex) {
                    _errorMessage = ex.Message;
                    if (Error != null) Error(this, new EventArgs());
                }
            }
            private void _Receive(IAsyncResult ia) {
                try {
                    Socket client = (Socket)ia.AsyncState;
                    client.EndReceive(ia);
                    ClientInfo ci = getClient(client);
                    if (ci == null) return;
                    // Convert received data to ChatMessage
                    ChatMessage m = ChatMessage.bytesToMessage(_data);
                    m.MessageTime = DateTime.Now;
                    m.Direction = MessageDirection.Incoming;
                    if (m.Type == MessageType.Registering) {
                        string[] msgs = m.Message.Split(new char[] { });
                        ci.Name = msgs[0];
                        ci.Status = msgs[1];
                        if (msgs[2] == "Online") ci.State = ConnectionState.Online;
                        else ci.State = ConnectionState.Offline;
                        if (ClientRegistered != null) ClientRegistered(this, new ClientEventArgs(ci));
                    } else {
                        ci.Messages.Add(m);
                        if (MessageReceived != null) MessageReceived(this, new MessageReceivedEventArgs(ci, m));
                    }
                    // Continue listening for this client.
                    client.BeginReceive(_data, 0, _data.Length, SocketFlags.None, new AsyncCallback(_Receive), client);
                } catch (Exception ex) {
                    _errorMessage = ex.Message;
                    if (Error != null) Error(this, new EventArgs());
                }
            }
        }
    }
}