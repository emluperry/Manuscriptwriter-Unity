using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Demo.UI
{
    public class BarkCanvas : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textBox;
        private CanvasGroup canvasGroup;
        private Queue<string> barkQueue;
        
        private Coroutine barkCoroutine;

        private void Awake()
        {
            this.canvasGroup = this.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        public void Bark(string line)
        {
            if (this.barkCoroutine != null)
            {
                barkQueue.Enqueue(line);
                return;
            }
            
            // show the bark canvas
            canvasGroup.alpha = 1;

            barkQueue = new Queue<string>();
            barkQueue.Enqueue(line);
            barkCoroutine = StartCoroutine(c_BarkRoutine());
        }
        
        private IEnumerator c_BarkRoutine()
        {
            while (barkQueue.Count > 0)
            {
                string line = barkQueue.Dequeue();
                // set the text on the bark canvas to line
                this.textBox.text = line;
                
                // determine how long to wait for
                float barkTime = line.Length * 0.1f;
            
                yield return new WaitForSeconds(barkTime);
            }

            this.barkCoroutine = null;
            canvasGroup.alpha = 0;
        }
    }
}
