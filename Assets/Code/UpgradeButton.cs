namespace Code
{
    using System;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Button))]
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private UpgradeDefinition definition;
        [SerializeField] private Text titleText;
        [SerializeField] private Text descriptionText;
        [SerializeField] private Text costText;
        [SerializeField] private Text levelText;
        [SerializeField] private Button button;

        private int cachedLevel;

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            if (button != null)
            {
                button.onClick.AddListener(Purchase);
            }
        }

        private void OnEnable()
        {
            RefreshFromState();
            UpdateUI();
        }

        private void Update()
        {
            UpdateUI();
        }

        public void Initialize(GameState state, UpgradeDefinition upgrade)
        {
            gameState = state;
            definition = upgrade;
            RefreshFromState();
            UpdateUI();
        }

        public void RefreshFromState()
        {
            if (gameState == null || definition == null)
            {
                cachedLevel = 0;
                return;
            }

            cachedLevel = gameState.GetUpgradeLevel(definition.Id);
        }

        public void SyncFromState()
        {
            RefreshFromState();
            UpdateUI();
        }

        private void Purchase()
        {
            if (gameState == null || definition == null)
            {
                return;
            }

            if (definition.MaxLevel > 0 && cachedLevel >= definition.MaxLevel)
            {
                return;
            }

            var cost = GetCurrentCost();
            if (!gameState.TrySpend(cost))
            {
                return;
            }

            cachedLevel = gameState.IncrementUpgradeLevel(definition.Id);
            if (definition.PerClickBonus > 0)
            {
                gameState.AddPerClick(definition.PerClickBonus);
            }

            if (definition.PerSecondBonus > 0)
            {
                gameState.AddPerSecond(definition.PerSecondBonus);
            }

            UpdateUI();
        }

        private double GetCurrentCost()
        {
            if (definition == null)
            {
                return 0;
            }

            return definition.BaseCost * Math.Pow(definition.CostMultiplier, cachedLevel);
        }

        private void UpdateUI()
        {
            if (definition == null)
            {
                return;
            }

            if (titleText != null)
            {
                titleText.text = definition.DisplayName;
            }

            if (descriptionText != null)
            {
                descriptionText.text = definition.Description;
            }

            if (levelText != null)
            {
                levelText.text = definition.MaxLevel > 0
                    ? $"Level: {cachedLevel}/{definition.MaxLevel}"
                    : $"Level: {cachedLevel}";
            }

            var cost = GetCurrentCost();
            if (costText != null)
            {
                costText.text = "Cost: " + UIBinder.FormatCurrency(cost);
            }

            if (button != null && gameState != null)
            {
                var canBuy = gameState.Currency >= cost;
                if (definition.MaxLevel > 0 && cachedLevel >= definition.MaxLevel)
                {
                    canBuy = false;
                }

                button.interactable = canBuy;
            }
        }
    }
}
