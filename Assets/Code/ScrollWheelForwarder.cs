using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Code
{
    public class ScrollWheelForwarder : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private float wheelMultiplier = 1.5f;

        public void Initialize(ScrollRect target, float multiplier)
        {
            scrollRect = target;
            wheelMultiplier = multiplier;
        }

        private void Awake()
        {
            if (scrollRect == null)
            {
                scrollRect = GetComponentInChildren<ScrollRect>();
            }
        }

        private void Update()
        {
            if (scrollRect == null || !scrollRect.gameObject.activeInHierarchy)
            {
                return;
            }

            var delta = GetScrollDelta();
            if (Mathf.Abs(delta) < 0.01f)
            {
                return;
            }

            var position = scrollRect.verticalNormalizedPosition;
            position += delta * wheelMultiplier * 0.1f;
            scrollRect.verticalNormalizedPosition = Mathf.Clamp01(position);
        }

        private float GetScrollDelta()
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current == null)
            {
                return 0f;
            }

            return Mouse.current.scroll.ReadValue().y / 120f;
#else
            return Input.mouseScrollDelta.y;
#endif
        }
    }
}
