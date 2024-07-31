using System;
using FishNet;
using FishNet.Managing.Scened;
using Game.PlayerSystem;
using Game.PopupSystem;
using Game.Utils;
using UnityEngine;

namespace Game.GameSystem
{
    public class GameManager : BaseNetworkBehaviour
    {
        public static GameManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        protected override void RegisterEvents()
        {
            
        }

        protected override void UnregisterEvents()
        {
            
        }
        
        public static void StartGame()
        {
            if (!IsAllPlayersReady())
            {
                PopupManager.Popup_Show(new PopupContent("CAN NOT START THE GAME", "ALL PLAYERS MUST BE READY TO START.", true));
                return;
            }
            
            ScenesManager.ChangeScene(EScenes.Game, true);
        }

        public static bool IsAllPlayersReady()
        {
            foreach (MyClient client in PlayerConnectionManager.Instance.AllClients)
            {
                if (!client.IsReady.Value)
                    return false;
            }

            return true;
        }
    }
}