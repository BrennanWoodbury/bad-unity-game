namespace Code
{
    using UnityEngine;
    using UnityEngine.UI;

    public class UIBinder : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private Text currencyText;
        [SerializeField] private Text perClickText;
        [SerializeField] private Text perSecondText;
        [SerializeField] private string currencyLabel = "Research Budget";
        [SerializeField] private string perClickLabel = "Per Click";
        [SerializeField] private string perSecondLabel = "Per Sec";

        private void Update()
        {
            if (gameState == null)
            {
                return;
            }

            if (currencyText != null)
            {
                currencyText.text = currencyLabel + ": " + FormatCurrency(gameState.Currency);
            }

            if (perClickText != null)
            {
                perClickText.text = perClickLabel + ": " + FormatCurrency(gameState.PerClick);
            }

            if (perSecondText != null)
            {
                perSecondText.text = perSecondLabel + ": " + FormatCurrency(gameState.PerSecond);
            }
        }

        public void Bind(GameState state, Text currency, Text perClick, Text perSecond)
        {
            gameState = state;
            currencyText = currency;
            perClickText = perClick;
            perSecondText = perSecond;
        }

        public void SetLabels(string currency, string perClick, string perSecond)
        {
            currencyLabel = currency;
            perClickLabel = perClick;
            perSecondLabel = perSecond;
        }

        public static string FormatCurrency(double value)
        {
            if (value < 1_000)
            {
                return value.ToString("0");
            }

            if (value < 1_000_000)
            {
                return (value / 1_000).ToString("0.##") + "K";
            }

            if (value < 1_000_000_000)
            {
                return (value / 1_000_000).ToString("0.##") + "M";
            }

            if (value < 1_000_000_000_000)
            {
                return (value / 1_000_000_000).ToString("0.##") + "B";
            }

            return (value / 1_000_000_000_000).ToString("0.##") + "T";
        }
    }
}
