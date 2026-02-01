using System.Globalization;
using System.Text;
using UnityEngine;

namespace Code
{
    public static class SaveSystem
    {
        private const string SlotPrefix = "pharma_slot_";
        private const string CurrencyKey = "currency";
        private const string PerClickKey = "per_click";
        private const string PerSecondKey = "per_second";
        private const string UpgradeIdsKey = "upgrade_ids";
        private const string UpgradePrefix = "upgrade_";

        public static void Save(GameState state, int slot)
        {
            if (state == null)
            {
                return;
            }

            var slotKey = GetSlotPrefix(slot);

            PlayerPrefs.SetString(slotKey + CurrencyKey, state.Currency.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.SetString(slotKey + PerClickKey, state.PerClick.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.SetString(slotKey + PerSecondKey, state.PerSecond.ToString(CultureInfo.InvariantCulture));

            var builder = new StringBuilder();
            var upgrades = state.GetUpgrades();
            for (var i = 0; i < upgrades.Count; i++)
            {
                var upgrade = upgrades[i];
                if (upgrade == null || string.IsNullOrWhiteSpace(upgrade.id))
                {
                    continue;
                }

                if (builder.Length > 0)
                {
                    builder.Append('|');
                }

                builder.Append(upgrade.id);
                PlayerPrefs.SetInt(slotKey + UpgradePrefix + upgrade.id, upgrade.level);
            }

            PlayerPrefs.SetString(slotKey + UpgradeIdsKey, builder.ToString());
            PlayerPrefs.Save();
        }

        public static bool Load(GameState state, int slot)
        {
            if (state == null)
            {
                return false;
            }

            var slotKey = GetSlotPrefix(slot);

            if (!PlayerPrefs.HasKey(slotKey + CurrencyKey))
            {
                return false;
            }

            state.SetCurrency(ParseDouble(PlayerPrefs.GetString(slotKey + CurrencyKey), state.Currency));
            state.SetPerClick(ParseDouble(PlayerPrefs.GetString(slotKey + PerClickKey), state.PerClick));
            state.SetPerSecond(ParseDouble(PlayerPrefs.GetString(slotKey + PerSecondKey), state.PerSecond));

            var ids = PlayerPrefs.GetString(slotKey + UpgradeIdsKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(ids))
            {
                var split = ids.Split('|');
                foreach (var id in split)
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        continue;
                    }

                    var level = PlayerPrefs.GetInt(slotKey + UpgradePrefix + id, 0);
                    state.SetUpgradeLevel(id, level);
                }
            }

            return true;
        }

        public static bool HasSave(int slot)
        {
            return PlayerPrefs.HasKey(GetSlotPrefix(slot) + CurrencyKey);
        }

        private static string GetSlotPrefix(int slot)
        {
            if (slot < 1)
            {
                slot = 1;
            }

            return SlotPrefix + slot + "_";
        }

        private static double ParseDouble(string value, double fallback)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return fallback;
        }
    }
}
