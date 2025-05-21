using UnityEngine;
using UnityEngine.UI;

namespace MSW.Unity.Sprites
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(AspectRatioFitter))]
    public class SpriteData : MonoBehaviour
    {
        private Image spriteImageComponent = null;
        private AspectRatioFitter aspectRatioFitter = null;

        private void Awake()
        {
            spriteImageComponent = GetComponent<Image>();
            aspectRatioFitter = GetComponent<AspectRatioFitter>();

            if (spriteImageComponent == null)
            {
                Debug.LogError("Sprite attached to the sprite canvas has no image component!");
            }
        }

        public void SetSprite(Sprite sprite)
        {
            spriteImageComponent.sprite = sprite;

            aspectRatioFitter.aspectRatio = spriteImageComponent.sprite.rect.width / spriteImageComponent.sprite.rect.height;
        }
    }
}
