using System;
using UnityEngine;

namespace Game
{
    public class SpawnCache : MonoBehaviour
    {
        public static SpawnCache Instance;
        public GameObject[] SpawnPoints;

        private void Awake()
        {
            Instance = this;
        }
    }
}