using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class CharacterSelectManager : MonoBehaviour
    {
        private int player1Index = 0;
        private int player2Index = 1;

        private CharacterType[] characters = { CharacterType.Alice, CharacterType.Billy, CharacterType.Charlie };

        private void Start()
        {
            GameManager.Instance.ResetReadyState();
            UpdateSelections();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                player1Index = (player1Index + characters.Length - 1) % characters.Length;
                UpdateSelections();
            }

            if (Input.GetKeyDown(KeyCode.D))
            {
                player1Index = (player1Index + 1) % characters.Length;
                UpdateSelections();
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                GameManager.Instance.player1Ready = true;
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                player2Index = (player2Index + characters.Length - 1) % characters.Length;
                UpdateSelections();
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                player2Index = (player2Index + 1) % characters.Length;
                UpdateSelections();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                GameManager.Instance.player2Ready = true;
            }

            if (GameManager.Instance.player1Ready && GameManager.Instance.player2Ready)
            {
                SceneManager.LoadScene("Lobby");
            }
        }

        private void UpdateSelections()
        {
            GameManager.Instance.player1Character = characters[player1Index];
            GameManager.Instance.player2Character = characters[player2Index];
        }
    }
}