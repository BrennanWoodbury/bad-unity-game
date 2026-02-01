namespace Code
{
    using UnityEngine;

    public class IdleIncome : MonoBehaviour
    {
        [SerializeField] private GameState gameState;
        [SerializeField] private float tickIntervalSeconds = 0.2f;

        private float timer;

        public void Bind(GameState state)
        {
            gameState = state;
        }

        private void Update()
        {
            if (gameState == null)
            {
                return;
            }

            if (gameState.PerSecond <= 0)
            {
                return;
            }

            timer += Time.deltaTime;
            if (timer < tickIntervalSeconds)
            {
                return;
            }

            // Batch ticks to keep income stable even if frame rate dips.
            var ticks = Mathf.FloorToInt(timer / tickIntervalSeconds);
            timer -= ticks * tickIntervalSeconds;
            var incomePerTick = gameState.PerSecond * tickIntervalSeconds;
            gameState.AddCurrency(incomePerTick * ticks);
        }
    }
}
