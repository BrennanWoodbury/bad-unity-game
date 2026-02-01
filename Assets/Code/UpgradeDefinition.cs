namespace Code
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Pharma Clicker/Upgrade Definition")]
    public class UpgradeDefinition : ScriptableObject
    {
        [SerializeField] private string domain;
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private string description;
        [SerializeField] private double baseCost = 10;
        [SerializeField] private double costMultiplier = 1.15;
        [SerializeField] private double perClickBonus = 1;
        [SerializeField] private double perSecondBonus;
        [SerializeField] private int maxLevel;

        public string Domain => string.IsNullOrWhiteSpace(domain) ? "General" : domain;
        public string Id => id;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public string Description => description;
        public double BaseCost => baseCost;
        public double CostMultiplier => costMultiplier <= 0 ? 1 : costMultiplier;
        public double PerClickBonus => perClickBonus;
        public double PerSecondBonus => perSecondBonus;
        public int MaxLevel => maxLevel;

        public void EnsureId()
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            id = name;
        }

        public void Configure(
            string newDomain,
            string newId,
            string newDisplayName,
            string newDescription,
            double newBaseCost,
            double newCostMultiplier,
            double newPerClickBonus,
            double newPerSecondBonus,
            int newMaxLevel)
        {
            domain = newDomain;
            id = newId;
            displayName = newDisplayName;
            description = newDescription;
            baseCost = newBaseCost;
            costMultiplier = newCostMultiplier;
            perClickBonus = newPerClickBonus;
            perSecondBonus = newPerSecondBonus;
            maxLevel = newMaxLevel;
        }
    }
}
