using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FishNet;
using FishNet.Transporting.Multipass;
using Game.PopupSystem;
using Game.Utils;
using Steamworks;
using Steamworks.Data;
using UnityEngine;

namespace Game.Steam
{
    public class SteamLobby : MonoBehaviour
    {
        public static SteamLobby Singleton;
        public static Lobby CurrentLobby;
        
        private void Start()
        {
            Singleton = this;
            
            SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
            SteamMatchmaking.OnLobbyDataChanged += OnLobbyDataUpdated;
            SteamMatchmaking.OnChatMessage += OnLobbyChatUpdate;
            SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
            
        }
       
        private void Update()
        {
           // SteamClient.RunCallbacks();
        }

        private void OnApplicationQuit()
        {
          //  SteamClient.Shutdown();
          LeaveLobby();
        }
        
        public async void CreateLobbyAsync()
        {
            PopupManager.Popup_Show(new PopupContent("CREATING LOBBY", ""));
            await SteamMatchmaking.CreateLobbyAsync(4);
        }

        public async void JoinLobbyAsync(Lobby lobby)
        {
            RoomEnter roomEnter = await lobby.Join();
            
            if(roomEnter == RoomEnter.Success)
                Debug.Log("Joined Successful");
            else
            {
                PopupManager.Popup_Show(new PopupContent("Failed to join", $"Reason: {roomEnter}", true));
            }


        }

        public async void FindLobby()
        {
            bool foundLobby = false;
            int maxAttempts = 5;
            
            for (int i = 0; i < maxAttempts; i++)
            {
                foreach (var lobby in await SteamExtension.GetLobbies())
                {
                    if(lobby.MemberCount >= 4 || foundLobby) continue;

                    foundLobby = true;
                    JoinLobbyAsync(lobby);
                    break;
                }
            }
        }

        public static void LeaveLobby()
        {
            CurrentLobby.Leave();
        }

        #region Steam CallBacks
        private void OnLobbyCreated(Result result, Lobby lobby)
        {
            if (result != Result.OK)
            {
                Debug.LogError("Failed to create Lobby");
                PopupManager.Popup_Show(new PopupContent("Failed to join", $"Reason: {result}", true));
                return;
            }

            CurrentLobby = lobby;
            CurrentLobby.SetData("Name", SteamClient.Name);
            CurrentLobby.SetData("Filter", "true");
            CurrentLobby.SetData("HostAddress", SteamClient.SteamId.ToString());
            
            InstanceFinder.ServerManager.StartConnection();
            InstanceFinder.ClientManager.StartConnection();
            
            Debug.Log($"Lobby Created.");
        }
        
        private void OnLobbyEntered(Lobby lobby)
        {
            CurrentLobby = lobby;

            string lobbyName = lobby.GetData("Name");
            string address = lobby.Owner.Id.ToString();

            Debug.Log($"Entered {lobbyName} lobby successfully! Address {address}  {lobby.GetData("HostAddress")}" );
            
            if(InstanceFinder.NetworkManager.IsClientStarted) return;

            Debug.Log(InstanceFinder.NetworkManager.IsClientStarted);
            InstanceFinder.ClientManager.StartConnection(address);
        }

        private void OnLobbyMemberJoined(Lobby lobby, Friend member)
        {
            Debug.Log($"{member.Name} joined the lobby.");
        }

        private void OnLobbyMemberLeave(Lobby lobby, Friend member)
        {
            Debug.Log($"{member.Name} left the lobby.");
        }
        
        private void OnLobbyDataUpdated(Lobby lobby)
        {
            string lobbyName = lobby.GetData("Name");
            
            Debug.Log($"Lobby Data Updated for Lobby {lobbyName}");
        }
        
        private void OnLobbyChatUpdate(Lobby lobby, Friend sender, string message)
        {
            // Handle different chat member state changes if needed
            Debug.Log($"Chat update: {sender.Name} : {message}");
        }

        private void OnLobbyInvite(Friend friend, Lobby lobby)
        {
            Debug.Log("Invited");
            string msg = $"{friend.Name} has invited you to their lobby.";
            PopupManager.Popup_Show(new PopupContent("FRIEND INVITE", msg, false,delegate { JoinLobbyAsync(lobby); }));
        }

        #endregion
    }
    
}