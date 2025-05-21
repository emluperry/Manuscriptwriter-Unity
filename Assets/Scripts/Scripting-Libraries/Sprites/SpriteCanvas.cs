using UnityEngine;

namespace MSW.Unity.Sprites
{
    public class SpriteCanvas : MonoBehaviour
    {
        [SerializeField] private SpriteData spriteObject;

        public virtual void ShowSprite(Sprite sprite)
        {
            this.spriteObject.SetSprite(sprite);
        }
    }
}
