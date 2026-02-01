using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace Code
{
    public class PharmaClickerBootstrap : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private GameState gameState;
        [SerializeField] private ClickerController clickerController;
        [SerializeField] private IdleIncome idleIncome;

        [Header("UI")]
        [SerializeField] private bool autoCreateUI = true;
        [SerializeField] private List<UpgradeDefinition> upgrades = new List<UpgradeDefinition>();

        private readonly List<UpgradeButton> upgradeButtons = new List<UpgradeButton>();
        private readonly List<Button> slotButtons = new List<Button>();
        private int selectedSlot = 1;

        private void Awake()
        {
            EnsureCoreSystems();

            if (autoCreateUI)
            {
                BuildUI();
            }
        }

        private void EnsureCoreSystems()
        {
            if (gameState == null)
            {
                var existing = FindFirstObjectByType<GameState>();
                if (existing != null)
                {
                    gameState = existing;
                }
                else
                {
                    var gameStateObject = new GameObject("GameState");
                    gameState = gameStateObject.AddComponent<GameState>();
                }
            }

            if (clickerController == null)
            {
                var existingClicker = FindFirstObjectByType<ClickerController>();
                if (existingClicker != null)
                {
                    clickerController = existingClicker;
                }
                else
                {
                    var systems = new GameObject("Systems");
                    clickerController = systems.AddComponent<ClickerController>();
                }
            }

            if (idleIncome == null)
            {
                var existingIdle = FindFirstObjectByType<IdleIncome>();
                if (existingIdle != null)
                {
                    idleIncome = existingIdle;
                }
                else
                {
                    if (clickerController == null)
                    {
                        var systems = new GameObject("Systems");
                        clickerController = systems.AddComponent<ClickerController>();
                        idleIncome = systems.AddComponent<IdleIncome>();
                    }
                    else
                    {
                        idleIncome = clickerController.gameObject.AddComponent<IdleIncome>();
                    }
                }
            }

            clickerController.Bind(gameState);
            idleIncome.Bind(gameState);
        }

        private void BuildUI()
        {
            EnsureEventSystem();
            var canvas = CreateCanvas();

            var root = CreatePanel(canvas.transform, "RootPanel");
            var rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;
            var rootImage = root.GetComponent<Image>();
            if (rootImage != null)
            {
                rootImage.color = new Color(0.05f, 0.06f, 0.07f, 1f);
            }

            var responsiveLayout = root.AddComponent<ResponsiveLayoutGroup>();
            responsiveLayout.Padding = new RectOffset(24, 24, 24, 24);
            responsiveLayout.Spacing = 20;
            responsiveLayout.ChildForceExpandWidth = true;
            responsiveLayout.ChildForceExpandHeight = true;
            responsiveLayout.ChildControlWidth = true;
            responsiveLayout.ChildControlHeight = true;

            var leftColumn = CreatePanel(root.transform, "LeftColumn");
            var leftLayout = leftColumn.AddComponent<VerticalLayoutGroup>();
            leftLayout.padding = new RectOffset(12, 12, 12, 12);
            leftLayout.spacing = 12;
            leftLayout.childControlHeight = true;
            leftLayout.childControlWidth = true;
            leftLayout.childForceExpandHeight = false;
            leftLayout.childForceExpandWidth = true;
            var leftElement = leftColumn.AddComponent<LayoutElement>();
            leftElement.flexibleWidth = 1;

            var rightColumn = CreatePanel(root.transform, "RightColumn");
            var rightRect = rightColumn.GetComponent<RectTransform>();
            rightRect.anchorMin = Vector2.zero;
            rightRect.anchorMax = Vector2.one;
            rightRect.offsetMin = Vector2.zero;
            rightRect.offsetMax = Vector2.zero;
            var rightLayout = rightColumn.AddComponent<VerticalLayoutGroup>();
            rightLayout.padding = new RectOffset(12, 12, 12, 12);
            rightLayout.spacing = 12;
            rightLayout.childControlHeight = true;
            rightLayout.childControlWidth = true;
            rightLayout.childForceExpandHeight = false;
            rightLayout.childForceExpandWidth = true;
            var rightElement = rightColumn.AddComponent<LayoutElement>();
            rightElement.flexibleWidth = 1;
            rightElement.flexibleHeight = 1;

            var menuBar = new GameObject("MenuBar", typeof(RectTransform));
            menuBar.transform.SetParent(leftColumn.transform, false);
            var menuLayout = menuBar.AddComponent<HorizontalLayoutGroup>();
            menuLayout.spacing = 8;
            menuLayout.childControlHeight = true;
            menuLayout.childControlWidth = true;
            menuLayout.childForceExpandHeight = false;
            menuLayout.childForceExpandWidth = false;

            CreateText(menuBar.transform, "SlotLabel", "Slot:", 14, TextAnchor.MiddleLeft, FontStyle.Bold);
            CreateSlotButton(menuBar.transform, 1);
            CreateSlotButton(menuBar.transform, 2);
            CreateSlotButton(menuBar.transform, 3);

            var saveButton = CreateButton(menuBar.transform, "SaveButton", "Save");
            var loadButton = CreateButton(menuBar.transform, "LoadButton", "Load");
            var exitButton = CreateButton(menuBar.transform, "ExitButton", "Exit");

            StyleMenuButton(saveButton);
            StyleMenuButton(loadButton);
            StyleMenuButton(exitButton);

            saveButton.GetComponent<Button>()?.onClick.AddListener(() =>
            {
                SaveSystem.Save(gameState, selectedSlot);
                UpdateSlotButtonVisuals();
            });
            loadButton.GetComponent<Button>()?.onClick.AddListener(() =>
            {
                if (SaveSystem.Load(gameState, selectedSlot))
                {
                    RefreshUpgradeButtons();
                }
            });
            exitButton.GetComponent<Button>()?.onClick.AddListener(HandleExit);

            UpdateSlotButtonVisuals();

            CreateText(leftColumn.transform, "Title", "OmniPharm Syndicate", 32, TextAnchor.MiddleLeft, FontStyle.Bold);
            CreateText(leftColumn.transform, "Subtitle", "Dominate healthcare through aggressive acquisition and R&D hype.", 16, TextAnchor.MiddleLeft, FontStyle.Italic);

            var currencyText = CreateText(leftColumn.transform, "CurrencyText", "Research Budget: 0", 24, TextAnchor.MiddleLeft, FontStyle.Bold);
            var perClickText = CreateText(leftColumn.transform, "PerClickText", "Per Click: 1", 16, TextAnchor.MiddleLeft, FontStyle.Normal);
            var perSecondText = CreateText(leftColumn.transform, "PerSecondText", "Per Sec: 0", 16, TextAnchor.MiddleLeft, FontStyle.Normal);

            var clickButton = CreateButton(leftColumn.transform, "ClickButton", "Synthesize Pill");
            var clickLayout = clickButton.AddComponent<LayoutElement>();
            clickLayout.preferredHeight = 64;
            var clickButtonComponent = clickButton.GetComponent<Button>();
            if (clickButtonComponent != null)
            {
                clickButtonComponent.onClick.AddListener(() => clickerController.Click());
            }

            CreateText(rightColumn.transform, "UpgradesHeader", "Executive Upgrades", 20, TextAnchor.MiddleLeft, FontStyle.Bold);
            var headerSpacer = new GameObject("HeaderSpacer", typeof(RectTransform), typeof(LayoutElement));
            headerSpacer.transform.SetParent(rightColumn.transform, false);
            headerSpacer.GetComponent<LayoutElement>().preferredHeight = 6;

            var scrollRoot = new GameObject("UpgradesScroll", typeof(RectTransform), typeof(ScrollRect));
            scrollRoot.transform.SetParent(rightColumn.transform, false);
            var scrollElement = scrollRoot.AddComponent<LayoutElement>();
            scrollElement.flexibleHeight = 1;
            scrollElement.flexibleWidth = 1;
            scrollElement.minHeight = 200;

            var viewport = new GameObject("Viewport", typeof(RectTransform), typeof(RectMask2D));
            viewport.transform.SetParent(scrollRoot.transform, false);

            var upgradesContainer = new GameObject("UpgradesContainer", typeof(RectTransform));
            upgradesContainer.transform.SetParent(viewport.transform, false);
            var upgradesLayout = upgradesContainer.AddComponent<VerticalLayoutGroup>();
            upgradesLayout.spacing = 8;
            upgradesLayout.childControlHeight = true;
            upgradesLayout.childControlWidth = true;
            upgradesLayout.childForceExpandHeight = false;
            upgradesLayout.childForceExpandWidth = true;

            var upgradesFitter = upgradesContainer.AddComponent<ContentSizeFitter>();
            upgradesFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            upgradesFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            var scrollRect = scrollRoot.GetComponent<ScrollRect>();
            scrollRect.viewport = viewport.GetComponent<RectTransform>();
            scrollRect.content = upgradesContainer.GetComponent<RectTransform>();
            scrollRect.horizontal = false;
            scrollRect.vertical = true;
            scrollRect.movementType = ScrollRect.MovementType.Clamped;
            scrollRect.scrollSensitivity = 40f;
            scrollRect.inertia = true;
            scrollRect.decelerationRate = 0.08f;

            var viewportRect = viewport.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.offsetMin = Vector2.zero;
            viewportRect.offsetMax = Vector2.zero;

            var contentRect = upgradesContainer.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.offsetMin = Vector2.zero;
            contentRect.offsetMax = Vector2.zero;

            var wheelForwarder = root.AddComponent<ScrollWheelForwarder>();
            wheelForwarder.Initialize(scrollRect, 2f);

            EnsureUpgradeDefaults();
            var lastDomain = string.Empty;
            foreach (var upgrade in upgrades)
            {
                if (upgrade == null)
                {
                    continue;
                }

                upgrade.EnsureId();
                var domain = upgrade.Domain;
                if (string.IsNullOrWhiteSpace(domain))
                {
                    domain = "General";
                }

                if (domain != lastDomain)
                {
                    CreateText(upgradesContainer.transform, $"Domain_{domain}", domain, 18, TextAnchor.MiddleLeft, FontStyle.Bold);
                    lastDomain = domain;
                }

                var upgradeGO = CreatePanel(upgradesContainer.transform, $"Upgrade_{upgrade.Id}");
                var upgradeLayoutElement = upgradeGO.AddComponent<LayoutElement>();
                upgradeLayoutElement.preferredHeight = 140;
                var upgradeLayout = upgradeGO.AddComponent<VerticalLayoutGroup>();
                upgradeLayout.padding = new RectOffset(12, 12, 12, 12);
                upgradeLayout.spacing = 4;
                upgradeLayout.childControlHeight = true;
                upgradeLayout.childControlWidth = true;
                upgradeLayout.childForceExpandHeight = false;
                upgradeLayout.childForceExpandWidth = true;

                var button = upgradeGO.AddComponent<Button>();
                var buttonImage = upgradeGO.GetComponent<Image>();
                if (buttonImage != null)
                {
                    buttonImage.color = new Color(0.18f, 0.18f, 0.2f, 0.92f);
                }

                var title = CreateText(upgradeGO.transform, "Title", upgrade.DisplayName, 18, TextAnchor.MiddleLeft, FontStyle.Bold);
                var description = CreateText(upgradeGO.transform, "Description", upgrade.Description, 14, TextAnchor.MiddleLeft, FontStyle.Normal);
                var level = CreateText(upgradeGO.transform, "Level", "Level: 0", 14, TextAnchor.MiddleLeft, FontStyle.Normal);
                var cost = CreateText(upgradeGO.transform, "Cost", "Cost: 0", 14, TextAnchor.MiddleLeft, FontStyle.Bold);

                var upgradeButton = upgradeGO.AddComponent<UpgradeButton>();
                var upgradeButtonField = typeof(UpgradeButton).GetField("button", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                upgradeButtonField?.SetValue(upgradeButton, button);

                var titleField = typeof(UpgradeButton).GetField("titleText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var descriptionField = typeof(UpgradeButton).GetField("descriptionText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var levelField = typeof(UpgradeButton).GetField("levelText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var costField = typeof(UpgradeButton).GetField("costText", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var stateField = typeof(UpgradeButton).GetField("gameState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var definitionField = typeof(UpgradeButton).GetField("definition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                titleField?.SetValue(upgradeButton, title);
                descriptionField?.SetValue(upgradeButton, description);
                levelField?.SetValue(upgradeButton, level);
                costField?.SetValue(upgradeButton, cost);
                stateField?.SetValue(upgradeButton, gameState);
                definitionField?.SetValue(upgradeButton, upgrade);

                upgradeButton.Initialize(gameState, upgrade);
                upgradeButtons.Add(upgradeButton);
            }

            var binderObject = new GameObject("UIBinder");
            binderObject.transform.SetParent(root.transform, false);
            var binder = binderObject.AddComponent<UIBinder>();
            binder.Bind(gameState, currencyText, perClickText, perSecondText);
            binder.SetLabels("Research Budget", "Per Click", "Per Sec");

            AddUiDebugOverlay(canvas.transform);
        }

        private void AddUiDebugOverlay(Transform parent)
        {
            var debugRoot = new GameObject("UiHitDebug", typeof(RectTransform));
            debugRoot.transform.SetParent(parent, false);
            var debugRect = debugRoot.GetComponent<RectTransform>();
            debugRect.anchorMin = new Vector2(0, 1);
            debugRect.anchorMax = new Vector2(0, 1);
            debugRect.pivot = new Vector2(0, 1);
            debugRect.anchoredPosition = new Vector2(12, -12);
            debugRect.sizeDelta = new Vector2(320, 80);

            var debugText = CreateText(debugRoot.transform, "DebugText", string.Empty, 12, TextAnchor.UpperLeft, FontStyle.Normal);
            debugText.color = new Color(1f, 0.8f, 0.4f, 1f);

            var debug = debugRoot.AddComponent<UiHitDebug>();
            debug.GetType();
            var outputField = typeof(UiHitDebug).GetField("output", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            outputField?.SetValue(debug, debugText);
        }

        private void RefreshUpgradeButtons()
        {
            foreach (var button in upgradeButtons)
            {
                if (button == null)
                {
                    continue;
                }

                button.SyncFromState();
            }
        }

        private void StyleMenuButton(GameObject buttonObject)
        {
            if (buttonObject == null)
            {
                return;
            }

            var image = buttonObject.GetComponent<Image>();
            if (image != null)
            {
                image.color = new Color(0.2f, 0.2f, 0.3f, 0.9f);
            }

            var layout = buttonObject.AddComponent<LayoutElement>();
            layout.preferredHeight = 34;
            layout.minWidth = 90;
        }

        private void CreateSlotButton(Transform parent, int slot)
        {
            var slotButtonObject = CreateButton(parent, $"Slot{slot}Button", $"{slot}");
            var button = slotButtonObject.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() =>
                {
                    selectedSlot = slot;
                    UpdateSlotButtonVisuals();
                });
            }

            var layout = slotButtonObject.AddComponent<LayoutElement>();
            layout.preferredHeight = 34;
            layout.minWidth = 36;
            layout.preferredWidth = 36;

            slotButtons.Add(button);
        }

        private void UpdateSlotButtonVisuals()
        {
            for (var index = 0; index < slotButtons.Count; index++)
            {
                var button = slotButtons[index];
                if (button == null)
                {
                    continue;
                }

                var slot = index + 1;
                var image = button.GetComponent<Image>();
                if (image == null)
                {
                    continue;
                }

                var isSelected = slot == selectedSlot;
                var hasSave = SaveSystem.HasSave(slot);
                if (isSelected)
                {
                    image.color = new Color(0.25f, 0.5f, 0.25f, 0.95f);
                }
                else if (hasSave)
                {
                    image.color = new Color(0.2f, 0.35f, 0.5f, 0.85f);
                }
                else
                {
                    image.color = new Color(0.2f, 0.2f, 0.3f, 0.75f);
                }
            }
        }

        private void HandleExit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void EnsureEventSystem()
        {
            var existing = FindFirstObjectByType<EventSystem>();
            if (existing != null)
            {
#if ENABLE_INPUT_SYSTEM
                if (existing.GetComponent<InputSystemUIInputModule>() == null)
                {
                    existing.gameObject.AddComponent<InputSystemUIInputModule>();
                }

                if (existing.GetComponent<UiInputScaleFixer>() == null)
                {
                    existing.gameObject.AddComponent<UiInputScaleFixer>();
                }

                var legacy = existing.GetComponent<StandaloneInputModule>();
                if (legacy != null)
                {
                    Destroy(legacy);
                }
#else
                if (existing.GetComponent<StandaloneInputModule>() == null)
                {
                    existing.gameObject.AddComponent<StandaloneInputModule>();
                }
#endif
                return;
            }

#if ENABLE_INPUT_SYSTEM
            var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule), typeof(UiInputScaleFixer));
#else
            var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
#endif
            eventSystemObject.transform.SetParent(null);
        }

        private Canvas CreateCanvas()
        {
            var existingCanvas = FindFirstObjectByType<Canvas>();
            if (existingCanvas != null)
            {
                ConfigureCanvas(existingCanvas);
                return existingCanvas;
            }

            var canvasObject = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            ConfigureCanvas(canvas);
            return canvas;
        }

        private void ConfigureCanvas(Canvas canvas)
        {
            if (canvas == null)
            {
                return;
            }

            var uiCamera = Camera.main;
            if (uiCamera != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = uiCamera;
                canvas.planeDistance = 1f;
            }
            else
            {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            canvas.pixelPerfect = false;

            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            var raycaster = canvas.GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                raycaster = canvas.gameObject.AddComponent<GraphicRaycaster>();
            }

            raycaster.ignoreReversedGraphics = true;
            raycaster.blockingObjects = GraphicRaycaster.BlockingObjects.None;
        }

        private GameObject CreatePanel(Transform parent, string name)
        {
            var panel = new GameObject(name, typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(parent, false);
            var image = panel.GetComponent<Image>();
            image.color = new Color(0.08f, 0.08f, 0.1f, 0.9f);
            return panel;
        }

        private Text CreateText(Transform parent, string name, string content, int fontSize, TextAnchor anchor, FontStyle style)
        {
            var textObject = new GameObject(name, typeof(RectTransform), typeof(Text));
            textObject.transform.SetParent(parent, false);
            var text = textObject.GetComponent<Text>();
            text.text = content;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = anchor;
            text.fontStyle = style;
            text.color = Color.white;
            return text;
        }

        private GameObject CreateButton(Transform parent, string name, string label)
        {
            var buttonObject = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);
            var image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.2f, 0.5f, 0.2f, 0.9f);

            var buttonText = CreateText(buttonObject.transform, "Text", label, 18, TextAnchor.MiddleCenter, FontStyle.Bold);
            buttonText.color = Color.white;

            return buttonObject;
        }

        private void EnsureUpgradeDefaults()
        {
            upgrades.RemoveAll(item => item == null);
            if (upgrades.Count > 0)
            {
                return;
            }

            upgrades.Add(CreateRuntimeUpgrade(
                "Influence",
                "ghostwritten_journals",
                "Ghostwritten Journals",
                "Place flattering studies to amplify market confidence.",
                20,
                1.12,
                1.2,
                0,
                0));

            upgrades.Add(CreateRuntimeUpgrade(
                "Legal",
                "patent_thicket",
                "Patent Thicket",
                "Stack filings to block competitors and push margins.",
                120,
                1.18,
                3,
                0,
                0));

            upgrades.Add(CreateRuntimeUpgrade(
                "PR",
                "pr_crisis_spin",
                "PR Crisis Spin Room",
                "Manage headlines to keep revenue humming.",
                220,
                1.16,
                0,
                3.5,
                0));

            upgrades.Add(CreateRuntimeUpgrade(
                "Sales",
                "off_label_blitz",
                "Off-Label Marketing Blitz",
                "Aggressive campaigns that boost both demand and retention.",
                420,
                1.17,
                1.5,
                2.5,
                0));

            upgrades.Add(CreateRuntimeUpgrade(
                "Regulatory",
                "regulatory_capture",
                "Regulatory Capture",
                "Streamline approvals to compound every spend.",
                800,
                1.2,
                0,
                6,
                0));

            upgrades.Add(CreateRuntimeUpgrade(
                "Influence",
                "influencer_doctor_tours",
                "Influencer Doctor Tours",
                "High-profile endorsements build steady demand.",
                1500,
                1.22,
                0,
                10,
                0));

            upgrades.Add(CreateRuntimeUpgrade(
                "Oncology",
                "confidential_cure",
                "Confidential Cure Protocol",
                "A breakthrough kept in-house to drive proprietary treatment lines.",
                5000,
                1.25,
                4,
                20,
                0));
        }

        private UpgradeDefinition CreateRuntimeUpgrade(
            string domain,
            string id,
            string displayName,
            string description,
            double baseCost,
            double costMultiplier,
            double perClickBonus,
            double perSecondBonus,
            int maxLevel)
        {
            var upgrade = ScriptableObject.CreateInstance<UpgradeDefinition>();
            upgrade.Configure(domain, id, displayName, description, baseCost, costMultiplier, perClickBonus, perSecondBonus, maxLevel);
            return upgrade;
        }
    }
}
