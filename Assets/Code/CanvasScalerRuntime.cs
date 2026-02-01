using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    [RequireComponent(typeof(CanvasScaler))]
    public class CanvasScalerRuntime : MonoBehaviour
    {
        private CanvasScaler scaler;
        private Vector2 lastResolution;

        private void Awake()
        {
            scaler = GetComponent<CanvasScaler>();
            Apply();
        }

        private void Update()
        {
            if (lastResolution.x != Screen.width || lastResolution.y != Screen.height)
            {
                Apply();
            }
        }

        private void Apply()
        {
            if (scaler == null)
            {
                return;
            }

            lastResolution = new Vector2(Screen.width, Screen.height);
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = lastResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }
    }
}
