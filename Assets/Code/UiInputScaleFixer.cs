using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
#endif

namespace Code
{
    public class UiInputScaleFixer : MonoBehaviour
    {
#if ENABLE_INPUT_SYSTEM
        private InputSystemUIInputModule inputModule;
        private InputActionAsset runtimeActions;

        private void Awake()
        {
            inputModule = FindFirstObjectByType<InputSystemUIInputModule>();
            if (inputModule == null)
            {
                return;
            }

            Apply();
        }

        private void Apply()
        {
            var systemWidth = Display.main != null ? Display.main.systemWidth : 0;
            var systemHeight = Display.main != null ? Display.main.systemHeight : 0;
            var scaleX = systemWidth > 0 ? (float)systemWidth / Screen.width : 1f;
            var scaleY = systemHeight > 0 ? (float)systemHeight / Screen.height : 1f;

            runtimeActions = ScriptableObject.CreateInstance<InputActionAsset>();
            var map = new InputActionMap("UI");

            var point = map.AddAction("Point", InputActionType.PassThrough, "<Mouse>/position", processors: $"scaleVector2(x={scaleX},y={scaleY})");
            var leftClick = map.AddAction("LeftClick", InputActionType.PassThrough, "<Mouse>/leftButton");
            var rightClick = map.AddAction("RightClick", InputActionType.PassThrough, "<Mouse>/rightButton");
            var middleClick = map.AddAction("MiddleClick", InputActionType.PassThrough, "<Mouse>/middleButton");
            var scroll = map.AddAction("ScrollWheel", InputActionType.PassThrough, "<Mouse>/scroll");

            runtimeActions.AddActionMap(map);
            runtimeActions.Enable();

            inputModule.point = InputActionReference.Create(point);
            inputModule.leftClick = InputActionReference.Create(leftClick);
            inputModule.rightClick = InputActionReference.Create(rightClick);
            inputModule.middleClick = InputActionReference.Create(middleClick);
            inputModule.scrollWheel = InputActionReference.Create(scroll);
        }
#endif
    }
}
