using FishNet;
using FishNet.Connection;
using FishNet.Managing.Client;
using FishNet.Managing.Server;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Managing.Client;
using FishNet.Managing.Server;
using FishNet.Object;
using FishNet.Managing.Observing;
using Game.PlayerSystem;

namespace Game.Utils
{
    public static class NetworkExtensions
    {

        public static MyClient GetLocalPlayer()
        {
            if (InstanceFinder.ClientManager == null)
            {
                Debug.LogError("Client Manager is Null");
                return null;
            }

            MyClient player = null;

            foreach (var obj in InstanceFinder.ClientManager.Connection.Objects)
            {
                if (obj.gameObject.CompareTag("Player"))
                    player = obj.GetComponent<MyClient>();
            }

            return player;
        }


        public static T GetPlayer<T>(this NetworkBehaviour behaviour, int objectId) where T : Component
        {

            if (InstanceFinder.IsServerStarted)
            {
                ServerManager serverManager = InstanceFinder.ServerManager;
                if (serverManager == null || !serverManager.Objects.Spawned.ContainsKey(objectId))
                    return default;

                T networkObject = serverManager.Objects.Spawned[objectId].GetComponent<T>();

                return networkObject;
            }
            else if (InstanceFinder.IsClientStarted)
            {
                ClientManager clientManager = InstanceFinder.ClientManager;
                if (clientManager == null || !clientManager.Objects.Spawned.ContainsKey(objectId))
                    return default;

                T networkObject = clientManager.Objects.Spawned[objectId].GetComponent<T>();

                return networkObject;
            }

            return default;
        }

        public static bool TryGetNetworkObjectFromObjectId(this int objectId, out NetworkObject networkObject)
        {
            networkObject = null;
            if (InstanceFinder.IsServerStarted)
            {
                ServerManager serverManager = InstanceFinder.ServerManager;
                if (serverManager == null || !serverManager.Objects.Spawned.ContainsKey(objectId))
                    return false;

                networkObject = serverManager.Objects.Spawned[objectId];
                return true;
            }
            else if (InstanceFinder.IsClientStarted)
            {
                ClientManager clientManager = InstanceFinder.ClientManager;
                if (clientManager == null || !clientManager.Objects.Spawned.ContainsKey(objectId))
                    return false;

                networkObject = clientManager.Objects.Spawned[objectId];
                return true;
            }

            return false;
        }
    }
}