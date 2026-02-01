using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code
{
    public class UiClickValidator : MonoBehaviour
    {
        private const string ResultKey = "ui_click_validation_result";
        private const string DetailsKey = "ui_click_validation_details";

        private void Start()
        {
            var results = new List<string>();
            var passed = true;

            passed &= ValidateButton("Slot1Button", results);
            passed &= ValidateButton("ClickButton", results);

            var summary = passed ? "PASS" : "FAIL";
            var details = string.Join("\n", results);

            PlayerPrefs.SetString(ResultKey, summary);
            PlayerPrefs.SetString(DetailsKey, details);
            PlayerPrefs.Save();

            if (passed)
            {
                Debug.Log($"[UiClickValidator] {summary}\n{details}");
            }
            else
            {
                Debug.LogError($"[UiClickValidator] {summary}\n{details}");
            }

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public static bool TryReadResult(out string summary, out string details)
        {
            summary = PlayerPrefs.GetString(ResultKey, string.Empty);
            details = PlayerPrefs.GetString(DetailsKey, string.Empty);
            return !string.IsNullOrWhiteSpace(summary);
        }

        private static bool ValidateButton(string name, List<string> results)
        {
            var buttonObject = GameObject.Find(name);
            if (buttonObject == null)
            {
                results.Add($"{name}: not found");
                return false;
            }

            var hit = RaycastAtCenter(buttonObject);
            if (!hit.hasHit)
            {
                results.Add($"{name}: raycast miss");
                return false;
            }

            var ok = IsSameOrChild(hit.hitObject, buttonObject);
            results.Add($"{name}: hit {hit.hitObject.name} -> {(ok ? "OK" : "MISMATCH")}");
            return ok;
        }

        private static (bool hasHit, GameObject hitObject) RaycastAtCenter(GameObject target)
        {
            var eventSystem = EventSystem.current;
            if (eventSystem == null)
            {
                return (false, null);
            }

            var raycaster = FindFirstObjectByType<GraphicRaycaster>();
            if (raycaster == null)
            {
                return (false, null);
            }

            var rectTransform = target.GetComponent<RectTransform>();
            if (rectTransform == null)
            {
                return (false, null);
            }

            var canvas = raycaster.GetComponentInParent<Canvas>();
            var worldPoint = rectTransform.TransformPoint(rectTransform.rect.center);
            var screenPoint = RectTransformUtility.WorldToScreenPoint(canvas != null ? canvas.worldCamera : null, worldPoint);

            var pointerData = new PointerEventData(eventSystem) { position = screenPoint };
            var results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);
            if (results.Count == 0)
            {
                return (false, null);
            }

            return (true, results.First().gameObject);
        }

        private static bool IsSameOrChild(GameObject candidate, GameObject expectedRoot)
        {
            if (candidate == expectedRoot)
            {
                return true;
            }

            return candidate.transform.IsChildOf(expectedRoot.transform);
        }
    }
}
