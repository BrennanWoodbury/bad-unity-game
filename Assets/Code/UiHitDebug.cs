using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Code
{
    public class UiHitDebug : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster raycaster;
        [SerializeField] private EventSystem eventSystem;
        [SerializeField] private Text output;
#if ENABLE_INPUT_SYSTEM
        [SerializeField] private UnityEngine.InputSystem.UI.InputSystemUIInputModule inputModule;
#endif

        private readonly List<RaycastResult> results = new List<RaycastResult>();

        private void Awake()
        {
            if (eventSystem == null)
            {
                eventSystem = EventSystem.current;
            }

            if (raycaster == null)
            {
                raycaster = FindFirstObjectByType<GraphicRaycaster>();
            }

#if ENABLE_INPUT_SYSTEM
            if (inputModule == null)
            {
                inputModule = FindFirstObjectByType<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
            }
#endif
        }

        private void Update()
        {
            if (output == null || eventSystem == null || raycaster == null)
            {
                return;
            }

            var mousePos = GetMousePosition();
            results.Clear();
            var pointerData = new PointerEventData(eventSystem) { position = mousePos };
            raycaster.Raycast(pointerData, results);

            var hitName = results.Count > 0 ? results[0].gameObject.name : "(none)";
            var canvas = raycaster.GetComponentInParent<Canvas>();
            var scaleFactor = canvas != null ? canvas.scaleFactor : 1f;
            var systemWidth = Display.main != null ? Display.main.systemWidth : 0;
            var systemHeight = Display.main != null ? Display.main.systemHeight : 0;
#if ENABLE_INPUT_SYSTEM
            var modulePos = inputModule != null && inputModule.point != null && inputModule.point.action != null
                ? inputModule.point.action.ReadValue<Vector2>()
                : Vector2.zero;
            var moduleText = inputModule != null ? $"Module: {modulePos}" : "Module: (none)";
#else
            var moduleText = "Module: (legacy)";
#endif
            output.text = $"Mouse: {mousePos}\nHit: {hitName}\nCanvas scale: {scaleFactor:0.###}\nScreen: {Screen.width}x{Screen.height}\nSystem: {systemWidth}x{systemHeight}\n{moduleText}";
        }

        private static Vector2 GetMousePosition()
        {
#if ENABLE_INPUT_SYSTEM
            return Mouse.current != null ? Mouse.current.position.ReadValue() : new Vector2(Input.mousePosition.x, Input.mousePosition.y);
#else
            return Input.mousePosition;
#endif
        }
    }
}
