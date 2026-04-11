using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Runner
{
    public class RunnerGameManager : MonoBehaviour
    {
        public static RunnerGameManager Instance;

        [Header("Players")]
        public RunnerPlayerHealth player1Health;
        public RunnerPlayerHealth player2Health;

        [Header("UI")]
        public TextMeshProUGUI player1LivesText;
        public TextMeshProUGUI player2LivesText;
        public TextMeshProUGUI progressText;
        public TextMeshProUGUI resultText;

        public Button backToLobbyButton;
        public Button rematchButton;

        [Header("Game Settings")]
        public float runDuration = 30f;

        private float elapsedTime = 0f;
        private bool gameEnded = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            if (resultText != null)
                resultText.gameObject.SetActive(false);

            if (backToLobbyButton != null)
                backToLobbyButton.gameObject.SetActive(false);

            if (rematchButton != null)
                rematchButton.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (!gameEnded)
            {
                elapsedTime += Time.deltaTime;
                UpdateUI();
                CheckEndConditions();
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    ReturnToLobby();
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    Rematch();
                }
            }
        }

        private void UpdateUI()
        {
            if (player1Health != null && player1LivesText != null)
                player1LivesText.text = "P1 Lives: " + player1Health.currentLives;

            if (player2Health != null && player2LivesText != null)
                player2LivesText.text = "P2 Lives: " + player2Health.currentLives;

            float progress = GetCurrentProgressPercent();

            if (progressText != null)
                progressText.text = "Progress: " + progress.ToString("F0") + "%";
        }

        private void CheckEndConditions()
        {
            if (player1Health == null || player2Health == null) return;

            bool bothDead = player1Health.isDead && player2Health.isDead;
            bool reachedEnd = elapsedTime >= runDuration;

            if (bothDead || reachedEnd)
            {
                EndGame();
            }
        }

        public float GetCurrentProgressPercent()
        {
            return Mathf.Clamp01(elapsedTime / runDuration) * 100f;
        }

        private void EndGame()
        {
            if (gameEnded) return;

            gameEnded = true;

            float finalProgressPercent = GetCurrentProgressPercent();

            float p1Progress = player1Health.isDead ? player1Health.deathProgressPercent : finalProgressPercent;
            float p2Progress = player2Health.isDead ? player2Health.deathProgressPercent : finalProgressPercent;

            int p1Score = Mathf.RoundToInt(p1Progress * 10f) + (player1Health.currentLives * 100) - (player1Health.hitsReceived * 25);
            int p2Score = Mathf.RoundToInt(p2Progress * 10f) + (player2Health.currentLives * 100) - (player2Health.hitsReceived * 25);

            string winnerText;

            if (p1Score > p2Score)
                winnerText = "Winner: Player 1";
            else if (p2Score > p1Score)
                winnerText = "Winner: Player 2";
            else
                winnerText = "Winner: Draw";

            if (resultText != null)
            {
                resultText.gameObject.SetActive(true);
                resultText.text =
                    "RESULTS\n\n" +
                    "P1 Score: " + p1Score + "\n" +
                    "P2 Score: " + p2Score + "\n\n" +
                    winnerText + "\n\n" +
                    "Enter = Lobby | R = Rematch";
            }

            if (backToLobbyButton != null)
                backToLobbyButton.gameObject.SetActive(true);

            if (rematchButton != null)
                rematchButton.gameObject.SetActive(true);
        }

        public void ReturnToLobby()
        {
            SceneManager.LoadScene("Lobby");
        }

        public void Rematch()
        {
            SceneManager.LoadScene("RunnerMinigame");
        }
    }
}