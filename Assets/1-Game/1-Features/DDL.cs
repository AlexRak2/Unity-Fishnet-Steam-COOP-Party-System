using System;
using UnityEngine;

namespace Game
{
    public class DDL : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
    }
}