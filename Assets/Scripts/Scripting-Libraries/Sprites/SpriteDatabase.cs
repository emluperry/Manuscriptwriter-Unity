using System.Collections.Generic;
using UnityEngine;

namespace MSW.Unity.Sprites
{
    [CreateAssetMenu(fileName = "SpriteDatabase", menuName = "Scriptable Objects/Sprites/SpriteDatabase")]
    public class SpriteDatabase : ScriptableObject
    {
        [SerializeField] private string SubjectName = "Name";
        [SerializeField] private SpriteID[] sprites;
        private Dictionary<string, SpriteID> spriteDictionary;

        private void SetupDictionary()
        {
            this.spriteDictionary = new Dictionary<string, SpriteID>();
            foreach(SpriteID sprite in sprites)
            {
                this.spriteDictionary[sprite.GetName()] = sprite;
            }
        }

        public string GetName()
        {
            return this.SubjectName;
        }

        public Sprite GetSprite(string spriteName)
        {
            if(this.spriteDictionary == null)
            {
                this.SetupDictionary();
            }

            return this.spriteDictionary[spriteName].GetSprite();
        }
    }
}
