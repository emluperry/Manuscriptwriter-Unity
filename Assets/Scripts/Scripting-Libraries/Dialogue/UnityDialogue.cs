using System;
using System.Collections.Generic;
using MSW.Events;
using MSW.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using MSW.Unity.Events;

namespace MSW.Unity.Dialogue
{
    public class UnityDialogue : MSWUnityLibrary
    {
        private DialogueCanvas canvas;
        [SerializeField] private EmotionObject[] emotions;

        #region MSW Events
        
        [MSWEvent("{0} speaks with {1}")]
        public UnityMSWEvent interactionEvent;

        #endregion

        private void Awake()
        {
            this.canvas = this.GetComponentInChildren<DialogueCanvas>(true);
            this.canvas.gameObject.SetActive(false);
        }

        #region MSW Functions

        [MSWFunction("{0} feels {1}.")]
        public object RunEmotion(Context context, string speaker, string emotionName)
        {
            Sprite emotionSprite = null;
            // identify emotion from the known list
            foreach (var emotion in this.emotions)
            {
                emotionSprite = emotion.GetSpriteFromAlias(emotionName);
                if (emotionSprite)
                {
                    break;
                }
            }
            
            // get the target of the bark from the speaker
            var target = this.GetObjectWithName(speaker);
            if (target == null)
            {
                return null;
            }
            var targetCanvas = target.GetComponentInChildren<EmoteCanvas>(true);
            if (targetCanvas == null)
            {
                return null;
            }

            targetCanvas.Emote(emotionSprite);

            return null;
        }
        
        [MSWFunction("{0} barks: {1}")]
        public object RunBark(Context context, string speaker, string line)
        {
            // get the target of the bark from the speaker
            var target = this.GetObjectWithName(speaker);
            if (target == null)
            {
                return null;
            }
            var targetCanvas = target.GetComponentInChildren<BarkCanvas>(true);
            if (targetCanvas == null)
            {
                return null;
            }
            
            targetCanvas.Bark(line);
            
            return null;
        }
        
        [MSWFunction("{0}: {1}")]
        public object RunDialogue(Context context, string speaker, string line)
        {
            if (canvas != null)
            {
                canvas.UpdateCanvas(speaker, line);
                context.WaitForEvent(canvas.ContinueAction);
            }
            return null;
        }

        #endregion

        public override void Cleanup()
        {
            if (canvas != null)
            {
                canvas.CleanupCanvas();
            }
        }
    }
}
