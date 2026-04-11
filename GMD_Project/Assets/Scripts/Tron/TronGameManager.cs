using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tron
{
    public class TronGameManager : MonoBehaviour
    {
        public TextMeshProUGUI resultText;
        public Button backToLobbyButton;
        public Button rematchButton;

        private bool player1Dead = false;
        private bool player2Dead = false;
        private bool gameEnded = false;

        private void Start()
        {
            if (resultText != null)
                resultText.gameObject.SetActive(false);

            if (backToLobbyButton != null)
                backToLobbyButton.gameObject.SetActive(false);

            if (rematchButton != null)
                rematchButton.gameObject.SetActive(false);
        }

        public void RegisterDeath(int playerID)
        {
            if (gameEnded) return;

            if (playerID == 1) player1Dead = true;
            if (playerID == 2) player2Dead = true;

            CheckGameEnd();
        }

        private void CheckGameEnd()
        {
            if (!player1Dead && !player2Dead) return;

            gameEnded = true;

            string result;

            if (player1Dead && player2Dead)
                result = "DRAW!";
            else if (player1Dead)
                result = "PLAYER 2 WINS!";
            else
                result = "PLAYER 1 WINS!";

            if (resultText != null)
            {
                resultText.gameObject.SetActive(true);
                resultText.text = result;
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
            SceneManager.LoadScene("TronMinigame");
        }
    }
}