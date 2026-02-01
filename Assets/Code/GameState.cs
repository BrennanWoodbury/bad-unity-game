namespace Code
{
    using System.Collections.Generic;
    using UnityEngine;

    public class GameState : MonoBehaviour
    {
        [SerializeField] private double currency;
        [SerializeField] private double perClick = 1;
        [SerializeField] private double perSecond;
        [SerializeField] private List<UpgradeProgress> upgrades = new List<UpgradeProgress>();

        public double Currency => currency;
        public double PerClick => perClick;
        public double PerSecond => perSecond;

        public System.Collections.Generic.IReadOnlyList<UpgradeProgress> GetUpgrades()
        {
            return upgrades;
        }

        public void AddCurrency(double amount)
        {
            if (amount <= 0)
            {
                return;
            }

            currency += amount;
        }

        public void SetCurrency(double value)
        {
            currency = value < 0 ? 0 : value;
        }

        public bool TrySpend(double amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            if (currency < amount)
            {
                return false;
            }

            currency -= amount;
            return true;
        }

        public void SetPerClick(double value)
        {
            perClick = value < 0 ? 0 : value;
        }

        public void SetPerSecond(double value)
        {
            perSecond = value < 0 ? 0 : value;
        }

        public void AddPerClick(double amount)
        {
            if (amount <= 0)
            {
                return;
            }

            perClick += amount;
        }

        public void AddPerSecond(double amount)
        {
            if (amount <= 0)
            {
                return;
            }

            perSecond += amount;
        }

        public int GetUpgradeLevel(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return 0;
            }

            var entry = upgrades.Find(progress => progress.id == id);
            return entry == null ? 0 : Mathf.Max(0, entry.level);
        }

        public void SetUpgradeLevel(string id, int level)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }

            level = Mathf.Max(0, level);
            var entry = upgrades.Find(progress => progress.id == id);
            if (entry == null)
            {
                upgrades.Add(new UpgradeProgress { id = id, level = level });
                return;
            }

            entry.level = level;
        }

        public int IncrementUpgradeLevel(string id)
        {
            var level = GetUpgradeLevel(id) + 1;
            SetUpgradeLevel(id, level);
            return level;
        }

        [System.Serializable]
        public class UpgradeProgress
        {
            public string id;
            public int level;
        }
    }
}
