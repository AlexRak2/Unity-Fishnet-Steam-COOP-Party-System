using System;
using System.Collections;
using UnityEngine;

namespace Game
{
    public class BootManager : MonoBehaviour
    {
        private void Start()
        {
            ScenesManager.ChangeScene(EScenes.MainMenu);
        }
    }
}