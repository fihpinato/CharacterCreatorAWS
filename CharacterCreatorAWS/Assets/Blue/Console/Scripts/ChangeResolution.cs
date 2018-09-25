using UnityEngine;
using UnityEngine.UI;

// By @Bullrich
namespace Blue.Console
{
    public class ChangeResolution : MonoBehaviour
    {
        public bool applyResolution;

        private readonly Vector2
            WIDTH_RESOLUTION = new Vector2(1800, 600),
            HEIGHT_RESOLUTION = new Vector2(800, 600);

        private float screenWidth;

        private CanvasScaler _scaler;

        public void Awake()
        {
            _scaler = GetComponent<CanvasScaler>();
            screenWidth = Screen.width;
        }

        public void Update()
        {
            if (!applyResolution || Screen.width == screenWidth) return;

            _scaler.referenceResolution =
                Screen.width > Screen.height ? WIDTH_RESOLUTION : HEIGHT_RESOLUTION;
            screenWidth = Screen.width;
        }
    }
}