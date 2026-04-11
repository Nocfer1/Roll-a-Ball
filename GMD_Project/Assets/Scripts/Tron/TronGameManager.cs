using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tron
{
    public class TronGameManager : MonoBehaviour
    {
        [Header("Intro UI")]
        public GameObject introPanel;
        public Button startButton;
        public Button backToLobbyIntroButton;

        [Header("Gameplay UI")]
        public TextMeshProUGUI p1LivesText;
        public TextMeshProUGUI p2LivesText;
        public TextMeshProUGUI roundMessageText;

        [Header("End UI")]
        public TextMeshProUGUI resultText;
        public Button rematchButton;
        public Button backToLobbyEndButton;

        [Header("Spawner")]
        public TronPlayerSpawner playerSpawner;

        private int player1Lives = 3;
        private int player2Lives = 3;

        private bool roundActive = false;
        private bool matchEnded = false;
        private bool roundResolving = false;

        private bool player1Crashed = false;
        private bool player2Crashed = false;

        private void Start()
        {
            if (introPanel != null) introPanel.SetActive(true);

            if (resultText != null) resultText.gameObject.SetActive(false);
            if (rematchButton != null) rematchButton.gameObject.SetActive(false);
            if (backToLobbyEndButton != null) backToLobbyEndButton.gameObject.SetActive(false);

            if (roundMessageText != null) roundMessageText.gameObject.SetActive(false);

            if (p1LivesText != null) p1LivesText.gameObject.SetActive(false);
            if (p2LivesText != null) p2LivesText.gameObject.SetActive(false);
        }

        public void StartMatch()
        {
            if (introPanel != null) introPanel.SetActive(false);

            if (p1LivesText != null) p1LivesText.gameObject.SetActive(true);
            if (p2LivesText != null) p2LivesText.gameObject.SetActive(true);

            UpdateLivesUI();
            StartCoroutine(StartRoundSequence());
        }

        private IEnumerator StartRoundSequence()
        {
            roundActive = false;
            roundResolving = false;

            player1Crashed = false;
            player2Crashed = false;

            if (playerSpawner != null)
            {
                playerSpawner.ClearSpawnedPlayers();
            }

            if (roundMessageText != null)
            {
                roundMessageText.gameObject.SetActive(true);

                roundMessageText.text = "3";
                yield return new WaitForSeconds(0.6f);

                roundMessageText.text = "2";
                yield return new WaitForSeconds(0.6f);

                roundMessageText.text = "1";
                yield return new WaitForSeconds(0.6f);

                roundMessageText.text = "GO!";
                yield return new WaitForSeconds(0.5f);

                roundMessageText.gameObject.SetActive(false);
            }

            if (playerSpawner != null)
            {
                playerSpawner.SpawnPlayersForRound();
            }

            roundActive = true;
        }

        public void ReportPlayerCrash(int playerID)
        {
            if (matchEnded) return;

            if (playerID == 1) player1Crashed = true;
            if (playerID == 2) player2Crashed = true;

            if (!roundResolving)
            {
                roundActive = false;
                roundResolving = true;
                StartCoroutine(ResolveRoundSequence());
            }
        }

        private IEnumerator ResolveRoundSequence()
        {
            yield return new WaitForSeconds(0.05f);

            if (player1Crashed) player1Lives--;
            if (player2Crashed) player2Lives--;

            if (player1Lives < 0) player1Lives = 0;
            if (player2Lives < 0) player2Lives = 0;

            UpdateLivesUI();

            if (roundMessageText != null)
            {
                roundMessageText.gameObject.SetActive(true);

                if (player1Crashed && player2Crashed)
                    roundMessageText.text = "BOTH LOSE 1 LIFE";
                else if (player1Crashed)
                    roundMessageText.text = "PLAYER 1 LOSES 1 LIFE";
                else if (player2Crashed)
                    roundMessageText.text = "PLAYER 2 LOSES 1 LIFE";
            }

            yield return new WaitForSeconds(1.0f);

            if (player1Lives == 0 && player2Lives == 0)
            {
                EndMatch("DRAW!");
                yield break;
            }

            if (player1Lives == 0)
            {
                EndMatch("PLAYER 2 WINS!");
                yield break;
            }

            if (player2Lives == 0)
            {
                EndMatch("PLAYER 1 WINS!");
                yield break;
            }

            StartCoroutine(StartRoundSequence());
        }

        private void EndMatch(string result)
        {
            matchEnded = true;
            roundActive = false;
            roundResolving = false;

            if (playerSpawner != null)
            {
                playerSpawner.ClearSpawnedPlayers();
            }

            if (roundMessageText != null)
            {
                roundMessageText.gameObject.SetActive(false);
            }

            if (resultText != null)
            {
                resultText.gameObject.SetActive(true);
                resultText.text = result;
            }

            if (rematchButton != null) rematchButton.gameObject.SetActive(true);
            if (backToLobbyEndButton != null) backToLobbyEndButton.gameObject.SetActive(true);
        }

        private void UpdateLivesUI()
        {
            if (p1LivesText != null) p1LivesText.text = "P1 Lives: " + player1Lives;
            if (p2LivesText != null) p2LivesText.text = "P2 Lives: " + player2Lives;
        }

        public void ReturnToLobby()
        {
            SceneManager.LoadScene("Lobby");
        }

        public void Rematch()
        {
            SceneManager.LoadScene("TronMinigame");
        }
    }
}