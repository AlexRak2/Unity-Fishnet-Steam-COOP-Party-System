#if !FishyFacepunch
using FishNet.Managing;
using FishNet.Transporting;
using Steamworks;
using System;
using UnityEngine;

namespace FishyFacepunch
{
    public class FishyFacepunch : Transport
    {
        ~FishyFacepunch()
        {
            Shutdown();
        }

        #region Public.
        [System.NonSerialized]
        public ulong LocalUserSteamID;
        #endregion

        #region Serialized.
        /// <summary>
        /// Steam application Id.
        /// </summary>
        [Tooltip("Steam application Id.")]
        [SerializeField]
        private uint _steamAppID = 480;

        [Header("Server")]
        /// <summary>
        /// Address server should bind to.
        /// </summary>
        [Tooltip("Address server should bind to.")]
        [SerializeField]
        private string _serverBindAddress = string.Empty;
        /// <summary>
        /// Port to use.
        /// </summary>
        [Tooltip("Port to use.")]
        [SerializeField]
        private ushort _port = 27015;
        /// <summary>
        /// Maximum number of players which may be connected at once.
        /// </summary>
        [Tooltip("Maximum number of players which may be connected at once.")]
        [Range(1, ushort.MaxValue)]
        [SerializeField]
        private ushort _maximumClients = 16;

        [Header("Client")]
        /// <summary>
        /// Address client should connect to.
        /// </summary>
        [Tooltip("Address client should connect to.")]
        [SerializeField]
        private string _clientAddress = string.Empty;

        [Tooltip("Timeout for connecting in seconds.")]
        [SerializeField]
        private int _timeout = 25;
        #endregion

        #region Private.
        /// <summary>
        /// MTUs for each channel.
        /// </summary>
        private int[] _mtus;
        /// <summary>
        /// Client for the transport.
        /// </summary>
        private Client.ClientSocket _client = new Client.ClientSocket();
        /// <summary>
        /// Client when acting as host.
        /// </summary>
        private Client.ClientHostSocket _clientHost = new Client.ClientHostSocket();
        /// <summary>
        /// Server for the transport.
        /// </summary>
        private Server.ServerSocket _server = new Server.ServerSocket();
        #endregion

        #region Const.
        /// <summary>
        /// Id to use for client when acting as host.
        /// </summary>
        internal const int CLIENT_HOST_ID = short.MaxValue;
        #endregion

        #region Initialization and Unity.
        public override void Initialize(NetworkManager networkManager, int transportIndex)
        {
            base.Initialize(networkManager, transportIndex);

            CreateChannelData();

#if !UNITY_SERVER
            SteamClient.Init(_steamAppID, true);
            SteamNetworking.AllowP2PPacketRelay(true);
#endif
            _clientHost.Initialize(this);
            _client.Initialize(this);
            _server.Initialize(this);
        }

        private void OnDestroy()
        {
            Shutdown();
        }

        private void Update()
        {
            _clientHost.CheckSetStarted();
        }
        #endregion

        #region Setup.
        /// <summary>
        /// Creates ChannelData for the transport.
        /// </summary>
        private void CreateChannelData()
        {
            _mtus = new int[2]
            {
                1048576,
                1200
            };
        }

        /// <summary>
        /// Tries to initialize steam network access.
        /// </summary>
        private void InitializeRelayNetworkAccess()
        {
#if !UNITY_SERVER
            SteamNetworkingUtils.InitRelayNetworkAccess();
            LocalUserSteamID = Steamworks.SteamClient.SteamId.Value;
#endif
        }
        #endregion

        #region ConnectionStates.
        /// <summary>
        /// Gets the IP address of a remote connection Id.
        /// </summary>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public override string GetConnectionAddress(int connectionId)
        {
            return _server.GetConnectionAddress(connectionId);
        }
        /// <summary>
        /// Called when a connection state changes for the local client.
        /// </summary>
        public override event Action<ClientConnectionStateArgs> OnClientConnectionState;
        /// <summary>
        /// Called when a connection state changes for the local server.
        /// </summary>
        public override event Action<ServerConnectionStateArgs> OnServerConnectionState;
        /// <summary>
        /// Called when a connection state changes for a remote client.
        /// </summary>
        public override event Action<RemoteConnectionStateArgs> OnRemoteConnectionState;
        /// <summary>
        /// Gets the current local ConnectionState.
        /// </summary>
        /// <param name="server">True if getting ConnectionState for the server.</param>
        public override LocalConnectionState GetConnectionState(bool server)
        {
            if (server)
                return _server.GetLocalConnectionState();
            else
                return _client.GetLocalConnectionState();
        }
        /// <summary>
        /// Gets the current ConnectionState of a remote client on the server.
        /// </summary>
        /// <param name="connectionId">ConnectionId to get ConnectionState for.</param>
        public override RemoteConnectionState GetConnectionState(int connectionId)
        {
            return _server.GetConnectionState(connectionId);
        }
        /// <summary>
        /// Handles a ConnectionStateArgs for the local client.
        /// </summary>
        /// <param name="connectionStateArgs"></param>
        public override void HandleClientConnectionState(ClientConnectionStateArgs connectionStateArgs)
        {
            OnClientConnectionState?.Invoke(connectionStateArgs);
        }
        /// <summary>
        /// Handles a ConnectionStateArgs for the local server.
        /// </summary>
        /// <param name="connectionStateArgs"></param>
        public override void HandleServerConnectionState(ServerConnectionStateArgs connectionStateArgs)
        {
            OnServerConnectionState?.Invoke(connectionStateArgs);
        }
        /// <summary>
        /// Handles a ConnectionStateArgs for a remote client.
        /// </summary>
        /// <param name="connectionStateArgs"></param>
        public override void HandleRemoteConnectionState(RemoteConnectionStateArgs connectionStateArgs)
        {
            OnRemoteConnectionState?.Invoke(connectionStateArgs);
        }
        #endregion

        #region Iterating.
        /// <summary>
        /// Processes data received by the socket.
        /// </summary>
        /// <param name="server">True to process data received on the server.</param>
        public override void IterateIncoming(bool server)
        {
            if (server)
            {
                _server.IterateIncoming();

            }
            else
            {
                _client.IterateIncoming();
                _clientHost.IterateIncoming();
            }
        }

        /// <summary>
        /// Processes data to be sent by the socket.
        /// </summary>
        /// <param name="server">True to process data received on the server.</param>
        public override void IterateOutgoing(bool server)
        {
            if (server)
                _server.IterateOutgoing();
            else
                _client.IterateOutgoing();
        }
        #endregion

        #region ReceivedData.
        /// <summary>
        /// Called when client receives data.
        /// </summary>
        public override event Action<ClientReceivedDataArgs> OnClientReceivedData;
        /// <summary>
        /// Handles a ClientReceivedDataArgs.
        /// </summary>
        /// <param name="receivedDataArgs"></param>
        public override void HandleClientReceivedDataArgs(ClientReceivedDataArgs receivedDataArgs)
        {
            OnClientReceivedData?.Invoke(receivedDataArgs);
        }
        /// <summary>
        /// Called when server receives data.
        /// </summary>
        public override event Action<ServerReceivedDataArgs> OnServerReceivedData;
        /// <summary>
        /// Handles a ClientReceivedDataArgs.
        /// </summary>
        /// <param name="receivedDataArgs"></param>
        public override void HandleServerReceivedDataArgs(ServerReceivedDataArgs receivedDataArgs)
        {
            OnServerReceivedData?.Invoke(receivedDataArgs);
        }
        #endregion

        #region Sending.
        /// <summary>
        /// Sends to the server or all clients.
        /// </summary>
        /// <param name="channelId">Channel to use.</param>
        /// /// <param name="segment">Data to send.</param>
        public override void SendToServer(byte channelId, ArraySegment<byte> segment)
        {
            _client.SendToServer(channelId, segment);
            _clientHost.SendToServer(channelId, segment);
        }
        /// <summary>
        /// Sends data to a client.
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="segment"></param>
        /// <param name="connectionId"></param>
        public override void SendToClient(byte channelId, ArraySegment<byte> segment, int connectionId)
        {
            _server.SendToClient(channelId, segment, connectionId);
        }
        #endregion

        #region Configuration.
        /// <summary>
        /// Returns the maximum number of clients allowed to connect to the server. If the transport does not support this method the value -1 is returned.
        /// </summary>
        /// <returns></returns>
        public override int GetMaximumClients()
        {
            return _server.GetMaximumClients();
        }
        /// <summary>
        /// Sets maximum number of clients allowed to connect to the server. If applied at runtime and clients exceed this value existing clients will stay connected but new clients may not connect.
        /// </summary>
        /// <param name="value"></param>
        public override void SetMaximumClients(int value)
        {
            _server.SetMaximumClients(value);
        }
        /// <summary>
        /// Sets which address the client will connect to.
        /// </summary>
        /// <param name="address"></param>
        public override void SetClientAddress(string address)
        {
            _clientAddress = address;
        }
        public override void SetServerBindAddress(string address, IPAddressType addressType)
        {
            _serverBindAddress = address;
        }
        /// <summary>
        /// Sets which port to use.
        /// </summary>
        /// <param name="port"></param>
        public override void SetPort(ushort port)
        {
            _port = port;
        }
        /// <summary>
        /// Returns the adjusted timeout as float
        /// </summary>
        /// <param name="asServer"></param>
        public override float GetTimeout(bool asServer)
        {
            return _timeout;
        }
        #endregion

        #region Start and stop.
        /// <summary>
        /// Starts the local server or client using configured settings.
        /// </summary>
        /// <param name="server">True to start server.</param>
        public override bool StartConnection(bool server)
        {
            Debug.Log("StartConnection fishy server: " + server);

            if (server)
                return StartServer();
            else
                return StartClient(_clientAddress);
        }

        /// <summary>
        /// Stops the local server or client.
        /// </summary>
        /// <param name="server">True to stop server.</param>
        public override bool StopConnection(bool server)
        {
            if (server)
                return StopServer();
            else
                return StopClient();
        }

        /// <summary>
        /// Stops a remote client from the server, disconnecting the client.
        /// </summary>
        /// <param name="connectionId">ConnectionId of the client to disconnect.</param>
        /// <param name="immediately">True to abrutly stp the client socket without waiting socket thread.</param>
        public override bool StopConnection(int connectionId, bool immediately)
        {
            return StopClient(connectionId, immediately);
        }

        /// <summary>
        /// Stops both client and server.
        /// </summary>
        public override void Shutdown()
        {
            //Stops client then server connections.
            StopConnection(false);
            StopConnection(true);
        }

        #region Privates.
        /// <summary>
        /// Starts server.
        /// </summary>
        /// <returns>True if there were no blocks. A true response does not promise a socket will or has connected.</returns>
        private bool StartServer()
        {
            bool clientRunning = false;
#if !UNITY_SERVER
            if (!SteamClient.IsValid)
            {
                Debug.LogError("Steam Facepunch not initialized. Server could not be started.");
                return false;
            }
            //if (_client.GetLocalConnectionState() != LocalConnectionState.Stopped)
            //{
            //    Debug.LogError("Server cannot run while client is running.");
            //    return false;
            //}

            clientRunning = (_client.GetLocalConnectionState() != LocalConnectionState.Stopped);
            /* If remote _client is running then stop it
             * and start the client host variant. */
            if (clientRunning)
                _client.StopConnection();
#endif

            _server.ResetInvalidSocket();
            if (_server.GetLocalConnectionState() != LocalConnectionState.Stopped)
            {
                Debug.LogError("Server is already running.");
                return false;
            }
            InitializeRelayNetworkAccess();

            bool result = _server.StartConnection(_serverBindAddress, _port, _maximumClients);

            //If need to restart client.
            if (result && clientRunning)
                StartConnection(false);

            return result;
        }

        /// <summary>
        /// Stops server.
        /// </summary>
        private bool StopServer()
        {
            return _server.StopConnection();
        }

        /// <summary>
        /// Starts the client.
        /// </summary>
        /// <param name="address"></param>
        /// <returns>True if there were no blocks. A true response does not promise a socket will or has connected.</returns>
        private bool StartClient(string address)
        {
            if (!SteamClient.IsValid)
            {
                Debug.LogError("Steam Facepunch not initialized. Client could not be started.");
                return false;
            }

            //If not acting as a host.
            if (_server.GetLocalConnectionState() == LocalConnectionState.Stopped)
            {
                if (_client.GetLocalConnectionState() != LocalConnectionState.Stopped)
                {
                    Debug.LogError("Client is already running.");
                    return false;
                }
                //Stop client host if running.
                if (_clientHost.GetLocalConnectionState() != LocalConnectionState.Stopped)
                    _clientHost.StopConnection();
                //Initialize.
                InitializeRelayNetworkAccess();

                _client.StartConnection(address, _port);
            }
            //Acting as host.
            else
            {
                _clientHost.StartConnection(_server);
            }

            return true;
        }

        /// <summary>
        /// Stops the client.
        /// </summary>
        private bool StopClient()
        {
            bool result = false;
            result |= _client.StopConnection();
            result |= _clientHost.StopConnection();
            return result;
        }

        /// <summary>
        /// Stops a remote client on the server.
        /// </summary>
        /// <param name="connectionId"></param>
        /// <param name="immediately">True to abrutly stp the client socket without waiting socket thread.</param>
        private bool StopClient(int connectionId, bool immediately)
        {
            return _server.StopConnection(connectionId);
        }
        #endregion
        #endregion

        #region Channels.
        /// <summary>
        /// Gets the MTU for a channel. This should take header size into consideration.
        /// For example, if MTU is 1200 and a packet header for this channel is 10 in size, this method should return 1190.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public override int GetMTU(byte channel)
        {
            if (channel >= _mtus.Length)
            {
                Debug.LogError($"Channel {channel} is out of bounds.");
                return 0;
            }

            return _mtus[channel];
        }
        #endregion
    }
}
#endif // !DISABLESTEAMWORKS