using System;
using System.Collections.Generic;
using MSW.Events;
using MSW.Unity.Events;
using UnityEngine;

namespace Demo.Interaction
{
    public class InteractableObject : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private InteractionIcon interactionIcon;
        [SerializeField] private SpriteRenderer iconRenderer;

        [SerializeField] private UnityMSWEvent OnInteract;

        protected virtual void Awake()
        {
            iconRenderer.sprite = interactionIcon.InteractionImage;
            iconRenderer.enabled = false;
        }
        
        public virtual void StartInteract(string interactor)
        {
            // TODO: Get argument details from separate component/internal variables?
            OnInteract?.FireEvent(this, new RunnerEventArgs(new List<object>() {interactor, this.gameObject.name}));
        }

        public virtual void OnOverlap()
        {
            if(!iconRenderer)
            {
                return;
            }

            iconRenderer.enabled = true;
        }

        public virtual void StopOverlap()
        {
            if (!iconRenderer)
            {
                return;
            }

            iconRenderer.enabled = false;
        }
    }
}
