using UnityEngine;

namespace MSW.Unity.Sprites
{
    [CreateAssetMenu(fileName = "SpriteID", menuName = "Scriptable Objects/Sprites/Sprite ID")]
    public class SpriteID : ScriptableObject
    {
        [SerializeField] private string spriteName = "None";
        [SerializeField] private Sprite sprite;

        public string GetName()
        {
            return spriteName;
        }

        public Sprite GetSprite()
        {
            return sprite;
        }
    }
}
