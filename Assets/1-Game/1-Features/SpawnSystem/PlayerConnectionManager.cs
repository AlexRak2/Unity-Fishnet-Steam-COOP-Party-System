using System;
using System.Collections.Generic;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Client;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Transporting;
using Game.PlayerSystem;
using Game.Utils;
using Unity.VisualScripting;

namespace Game
{
    public enum EServerState
    {
        MainMenu, InGame
    }

    public class PlayerConnectionManager : BaseMonoBehaviour
    {
        public static PlayerConnectionManager Instance;
        
        public static Action<NetworkConnection> S_OnConnect;
        public static Action<NetworkConnection> S_OnDisconnect;

        public List<MyClient> AllClients = new List<MyClient>();
        
        private ServerManager _serverManager;
        
        protected override void RegisterEvents()
        {
            Instance = this;
            
            _serverManager = InstanceFinder.ServerManager;
            
            if (_serverManager != null)
                _serverManager.OnRemoteConnectionState += OnRemoteConnectionState;
        }
        
        protected override void UnregisterEvents()
        {
            if (_serverManager != null)
                _serverManager.OnRemoteConnectionState -= OnRemoteConnectionState;        
        }

        private void OnRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs remoteConnectionStateArgs)
        {
            if(remoteConnectionStateArgs.ConnectionState == RemoteConnectionState.Started)
                S_OnConnect?.Invoke(connection);
            else
            {
                S_OnDisconnect?.Invoke(connection);
            }
        }
    }   
}