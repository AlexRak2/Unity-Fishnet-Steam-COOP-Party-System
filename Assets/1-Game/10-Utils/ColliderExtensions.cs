using UnityEngine;

namespace Game.Utils
{
    public static class ColliderExtensions
    {
        public static T GetComponentFromCollider<T>(this Collider collider)
        {
            if (collider == null)
                return default;
            // Get component in parent of the collider & also in his children

            T component = collider.GetComponentInParent<T>();
            if (component == null)
            {
                component = collider.GetComponentInChildren<T>();

                if (component == null)
                    component = collider.GetComponent<T>();


            }

            return component;
        }

        public static bool TryGetComponentFromCollider<T>(this Collider collider, out T component)
        {
            component = GetComponentFromCollider<T>(collider);
            if (component == null)
                return false;
            return true;
        }
    }
}