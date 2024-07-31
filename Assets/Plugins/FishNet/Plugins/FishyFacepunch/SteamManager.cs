using FishNet;
using FishNet.Transporting;
using Steamworks;
using System;
using System.Net;
using UnityEngine;

public class SteamManager : MonoBehaviour
{
    #region Serialized.
    [Header("Steam Settings")]
    /// <summary>
    /// Steam application Id.
    /// </summary>
    [Tooltip("Steam application Id.")]
    [SerializeField]
    private uint _steamAppID = 480;

    /// <summary>
    /// SSteam Mod Directory.
    /// </summary>
    [Tooltip("Steam Mod Directory.")]
    [SerializeField]
    private string _modDir = string.Empty;

    /// <summary>
    /// Steam Game Description.
    /// </summary>
    [Tooltip("Steam Game Description.")]
    [SerializeField]
    private string _gameDesc = string.Empty;

    /// <summary>
    /// Steam version.
    /// </summary>
    [Tooltip("Steam version.")]
    [SerializeField]
    private string _version = string.Empty;

    [Header("Server Settings")]
    /// <summary>
    /// Servername.
    /// </summary>
    [Tooltip("Server Name")]
    [SerializeField]
    private string _serverName = string.Empty;

    /// <summary>
    /// Server Password.
    /// </summary>
    [Tooltip("Server Password.")]
    [SerializeField]
    private string _password = string.Empty;

    /// <summary>
    /// Steam Server Query Port.
    /// </summary>
    [Tooltip("Server Query Port.")]
    [SerializeField]
    private ushort _queryPort = 27016;

    /// <summary>
    /// Server VAC Secure.
    /// </summary>
    [Tooltip("Server VAC Secure.")]
    [SerializeField]
    private bool _vac = true;

    /// <summary>
    /// Server as Dedicated Server.
    /// </summary>
    [Tooltip("Server as Dedicated Server.")]
    [SerializeField]
    private bool _ds = true;
    #endregion

    void Start()
    {
#if UNITY_SERVER
        InstanceFinder.ServerManager.StartConnection();
        InstanceFinder.ServerManager.OnServerConnectionState += OnServerConnectionState;
#endif
    }

    private void OnServerConnectionState(FishNet.Transporting.ServerConnectionStateArgs state)
    {
        if (state.ConnectionState == FishNet.Transporting.LocalConnectionState.Started)
        {
            var serverInit = new SteamServerInit(_modDir, _gameDesc)
            {
                IpAddress = IPAddress.Parse(InstanceFinder.TransportManager.Transport.GetServerBindAddress(IPAddressType.IPv4)),
                GamePort = InstanceFinder.TransportManager.Transport.GetPort(),
                QueryPort = _queryPort,
                Secure = _vac,
                DedicatedServer = _ds,
                VersionString = _version,
            };
            serverInit.WithRandomSteamPort();

            try
            {
                SteamServer.Init(1280590, serverInit, true);
                SteamServer.ServerName = _serverName;
                SteamServer.MaxPlayers = InstanceFinder.TransportManager.Transport.GetMaximumClients();
                SteamServer.Passworded = !string.IsNullOrEmpty(_password);

                SteamServer.DedicatedServer = _ds;
                SteamServer.AutomaticHeartbeats = true;

                SteamServer.LogOnAnonymous();

                SteamServer.OnSteamServersConnected += OnSteamServersConnected;
                SteamServer.OnSteamServersDisconnected += OnSteamServersDisconnected;
                SteamServer.OnSteamServerConnectFailure += OnSteamServerConnectFailure;

                if (!SteamServer.IsValid)
                {
                    Debug.LogWarning("Couldn't initialize server");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Couldn't initialize Steam server (" + ex.Message + ")");
                Application.Quit();
            }
        }
    }

    private void OnSteamServersConnected()
    {
        Debug.Log("Dedicated Server connected to Steam successfully");
    }

    private void OnSteamServerConnectFailure(Result result, bool stilltrying)
    {
        Debug.Log("Dedicated Server failed to connect to Steam");
    }

    private void OnSteamServersDisconnected(Result result)
    {
        Debug.Log("Dedicated Server got logged out of Steam");
    }
}
