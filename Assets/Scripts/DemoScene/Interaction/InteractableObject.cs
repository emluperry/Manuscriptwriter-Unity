using System;
using System.Collections.Generic;
using MSW.Events;
using MSW.Unity.Events;
using MSW.Unity;
using UnityEngine;

namespace Demo.Interaction
{
    [RequireComponent(typeof(Descriptor))]
    public class InteractableObject : MonoBehaviour
    {
        private Descriptor objectDescriptor;
        
        [Header("UI References")]
        [SerializeField] private InteractionIcon interactionIcon;
        [SerializeField] private SpriteRenderer iconRenderer;

        [SerializeField] private UnityMSWEvent OnInteract;

        protected virtual void Awake()
        {
            iconRenderer.sprite = interactionIcon.InteractionImage;
            iconRenderer.enabled = false;

            this.objectDescriptor = this.GetComponent<Descriptor>();
        }
        
        public virtual void StartInteract(string interactor)
        {
            iconRenderer.enabled = false;
            OnInteract?.FireEvent(this, new RunnerEventArgs(new List<object>() {interactor, this.objectDescriptor.ObjectName}));
            iconRenderer.enabled = true;
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
