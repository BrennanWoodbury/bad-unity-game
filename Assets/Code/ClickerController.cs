namespace Code
{
    using UnityEngine;

    public class ClickerController : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private double clickMultiplier = 1;

        public void Bind(GameState state)
        {
            gameState = state;
        }

        public void Click()
        {
            if (gameState == null)
            {
                return;
            }

            var amount = gameState.PerClick * clickMultiplier;
            if (amount <= 0)
            {
                return;
            }

            gameState.AddCurrency(amount);
        }
    }
}
