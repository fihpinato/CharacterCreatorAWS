using UnityEngine;
using UnityEngine.UI;

// by @Bullrich

namespace Blue.Console
{
    public class ToggleIcon : MonoBehaviour
    {
        public Sprite toggleSprite;
        private Sprite originalSprite;

        public void ChangeSprite()
        {
            Image _imageContainer = GetComponent<Image>();
            if (originalSprite == null)
            {
                originalSprite = _imageContainer.sprite;
                _imageContainer.sprite = toggleSprite;
            }
            else
            {
                if (_imageContainer.sprite == originalSprite)
                    _imageContainer.sprite = toggleSprite;
                else
                    _imageContainer.sprite = originalSprite;
            }
        }
    }
}