using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MSW.Unity.Dialogue
{
    [RequireComponent(typeof(CanvasGroup))]
    public class EmoteCanvas : MonoBehaviour
    {
        [SerializeField] private Image emoteImage;
        [SerializeField] private float stayDuration = 3f;
        private CanvasGroup canvasGroup;
        private Queue<Sprite> spriteQueue;
        
        private Coroutine emoteCoroutine;

        private void Awake()
        {
            this.canvasGroup = this.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        public void Emote(Sprite emotion)
        {
            if (this.emoteCoroutine != null)
            {
                spriteQueue.Enqueue(emotion);
                return;
            }
            
            // show the emote canvas
            canvasGroup.alpha = 1;

            spriteQueue = new Queue<Sprite>();
            spriteQueue.Enqueue(emotion);
            emoteCoroutine = StartCoroutine(c_EmoteRoutine());
        }
        
        private IEnumerator c_EmoteRoutine()
        {
            while (spriteQueue.Count > 0)
            {
                Sprite emotion = spriteQueue.Dequeue();
                // set the text on the emote canvas to line
                this.emoteImage.sprite = emotion;
            
                yield return new WaitForSeconds(stayDuration);
            }

            this.emoteCoroutine = null;
            canvasGroup.alpha = 0;
        }
    }
}
