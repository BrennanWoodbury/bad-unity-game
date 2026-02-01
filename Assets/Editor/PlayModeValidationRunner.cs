#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Code.Editor
{
    public static class PlayModeValidationRunner
    {
        [MenuItem("Tools/Tests/Run UI Click Validation")]
        public static void RunUiClickValidation()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            EditorApplication.playModeStateChanged += OnPlayModeChanged;

            if (!EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = true;
            }
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                var validator = Object.FindFirstObjectByType<Code.UiClickValidator>();
                if (validator == null)
                {
                    var go = new GameObject("UiClickValidator");
                    go.AddComponent<Code.UiClickValidator>();
                }
            }
            else if (state == PlayModeStateChange.EnteredEditMode)
            {
                if (Code.UiClickValidator.TryReadResult(out var summary, out var details))
                {
                    if (summary == "PASS")
                    {
                        Debug.Log($"[UI Click Validation] {summary}\n{details}");
                    }
                    else
                    {
                        Debug.LogError($"[UI Click Validation] {summary}\n{details}");
                    }
                }

                EditorApplication.playModeStateChanged -= OnPlayModeChanged;
            }
        }
    }
}
#endif
