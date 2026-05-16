using Audio;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LobbyBackToCharacterSelect : MonoBehaviour
{
    private bool isReturning = false;

    private void Update()
    {
        if (isReturning)
            return;

        bool backPressed = false;

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            backPressed = true;
        }

        foreach (Gamepad gamepad in Gamepad.all)
        {
            if (gamepad.buttonNorth.wasPressedThisFrame)
            {
                backPressed = true;
                break;
            }
        }

        if (backPressed)
        {
            ReturnToCharacterSelect();
        }
    }

    private void ReturnToCharacterSelect()
    {
        isReturning = true;

        GameAudioManager.Instance?.PlayButtonBack();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetReadyState();
        }

        SceneManager.LoadScene("CharacterSelect");
    }
}