using System;
using UnityEngine;

namespace Demo.Entity
{
    public class CollisionHandler : MonoBehaviour
    {
        public Action<Collider> TriggerEnter;
        public Action<Collider> TriggerExit;

        private void OnTriggerEnter(Collider collision)
        {
            TriggerEnter?.Invoke(collision);
        }

        private void OnTriggerExit(Collider collision)
        {
            TriggerExit?.Invoke(collision);
        }
    }
}
