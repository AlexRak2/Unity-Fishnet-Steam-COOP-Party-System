using UnityEngine;

namespace Game.Utils
{
    public abstract class BaseMonoBehaviour : MonoBehaviour
    {
        public void Awake()
        {
            RegisterEvents();
        }
        
        public void OnDestroy()
        {
            UnregisterEvents();
        }

        protected abstract void RegisterEvents();
        protected abstract void UnregisterEvents();
    }
}