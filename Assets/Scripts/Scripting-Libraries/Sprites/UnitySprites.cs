using MSW.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace MSW.Unity.Sprites
{
    public class UnitySprites : MSWUnityLibrary
    {
        [SerializeField] private SpriteDatabase[] sprites;
        private Dictionary<string, SpriteDatabase> databases;
        private SpriteCanvas canvas;

        private void Awake()
        {
            if(databases == null)
            {
                this.Setup();
            }
        }

        private void Setup()
        {
            this.databases = new Dictionary<string, SpriteDatabase>();
            foreach (SpriteDatabase db in sprites)
            {
                this.databases[db.GetName()] = db;
            }

            this.canvas = this.GetComponentInChildren<SpriteCanvas>(true);
            this.canvas.gameObject.SetActive(false);
        }

        public override void Cleanup()
        {
            if(this.canvas)
            {
                this.canvas.gameObject.SetActive(false);
            }
        }

        [MSWFunction("The {0} looks {1}.")]
        [MSWFunction("{0} looks {1}.")]
        public object ShowSprite(Context context, string character, string spriteName)
        {
            if(this.databases == null || this.canvas == null)
            {
                this.Setup();
            }

            if(!this.databases.TryGetValue(character, out SpriteDatabase db))
            {
                return null;
            }

            this.canvas.gameObject.SetActive(true);
            this.canvas.ShowSprite(db.GetSprite(spriteName));

            return null;
        }
    }
}
