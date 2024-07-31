using System;
using FishNet;
using FishNet.Connection;
using FishNet.Editing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using Game.PlayerSystem;
using Game.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class PlayerSpawnManager : BaseMonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        
        protected override void RegisterEvents()
        {
            PlayerConnectionManager.S_OnConnect += S_OnConnect;
            
            if(InstanceFinder.SceneManager != null)
                InstanceFinder.SceneManager.OnLoadEnd += OnSceneLoaded;
        }
        
        protected override void UnregisterEvents()
        {
            PlayerConnectionManager.S_OnConnect -= S_OnConnect;
            
            if(InstanceFinder.SceneManager != null)
                InstanceFinder.SceneManager.OnLoadEnd -= OnSceneLoaded;
        }
        
        private void S_OnConnect(NetworkConnection conn)
        {
            int index = PlayerConnectionManager.Instance.AllClients.Count;

            Transform spawnPoint = SpawnCache.Instance.SpawnPoints[index].transform;
            
            NetworkObject nob = InstanceFinder.NetworkManager.GetPooledInstantiated(_playerPrefab, spawnPoint.position, spawnPoint.rotation, true);
            InstanceFinder.ServerManager.Spawn(nob, conn);
            
        }

        //spawn players in each scene load
        public static void SpawnPlayer(int clientId, int index)
        {
            if (NetworkExtensions.TryGetNetworkObjectFromObjectId(clientId, out NetworkObject netObj))
            {
                MyClient client = netObj.GetComponent<MyClient>();
                
                Transform spawnPoint = SpawnCache.Instance.SpawnPoints[index].transform;
                client.S_SetPosition(spawnPoint.position, spawnPoint.rotation, true);
            }
        }
        
        private void OnSceneLoaded(SceneLoadEndEventArgs obj)
        {
            if(obj.LoadedScenes.Length == 0) return;
            
            if (obj.LoadedScenes[0].name == EScenes.Game.ToString())
            {
                int index = 0;
                foreach (var client in PlayerConnectionManager.Instance.AllClients)
                {
                    SpawnPlayer(client.ObjectId, index);
                    index++;
                }
            }        
        }
    }
}