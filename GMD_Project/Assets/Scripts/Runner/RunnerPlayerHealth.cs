using UnityEngine;

namespace Runner
{
    public class RunnerPlayerHealth : MonoBehaviour
    {
        public int maxLives = 3;
        public int currentLives;

        public int playerID = 1;
        public bool isDead = false;
        public int hitsReceived = 0;

        public float deathProgressPercent = -1f;

        private void Start()
        {
            currentLives = maxLives;
        }

        public void TakeDamage(int damage)
        {
            if (isDead) return;

            hitsReceived += damage;
            currentLives -= damage;

            if (currentLives <= 0)
            {
                currentLives = 0;
                isDead = true;

                if (RunnerGameManager.Instance != null)
                {
                    deathProgressPercent = RunnerGameManager.Instance.GetCurrentProgressPercent();
                }
            }
        }
    }
}