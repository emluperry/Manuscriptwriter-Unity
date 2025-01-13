using System.Collections.Generic;
using UnityEngine;

namespace MSW.Unity.Dialogue
{
    [CreateAssetMenu(fileName = "NewEmotion", menuName = "Scriptable Objects/Interactions/Create Emotion")]
    public class EmotionObject : ScriptableObject
    {
        [SerializeField] private List<string> aliases;
        [SerializeField] private Sprite feelingSprite;

        public Sprite GetSpriteFromAlias(string emotion)
        {
            if (aliases.Contains(emotion))
            {
                return feelingSprite;
            }

            return null;
        }
    }
}
