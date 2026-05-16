using System.Collections;
using Audio;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tron
{
    public class TronGameManager : MonoBehaviour
    {
        [Header("Intro UI")]
        public GameObject introPanel;
        public RectTransform introPanelRect;
        public Button startButton;
        public Button backToLobbyIntroButton;

        [Header("Menu Input")]
        public float menuMoveDeadZone = 0.55f;
        public float menuResetDeadZone = 0.25f;

        private bool menuStickReady = true;
        
        [Header("Menu Texts")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI p1PreviewText;
        public TextMeshProUGUI p2PreviewText;

        [Header("Menu Layout")]
        public RectTransform startButtonRect;
        public RectTransform backToLobbyIntroButtonRect;
        public RectTransform resultTextRect;

        public Vector2 introPanelSize = new Vector2(700f, 400f);
        public Vector2 resultPanelSize = new Vector2(700f, 500f);

        public Vector2 introStartButtonPosition = new Vector2(-120f, -120f);
        public Vector2 introBackButtonPosition = new Vector2(120f, -120f);

        public Vector2 resultStartButtonPosition = new Vector2(-120f, -190f);
        public Vector2 resultBackButtonPosition = new Vector2(120f, -190f);

        public Vector2 resultTextPosition = new Vector2(0f, 35f);
        public Vector2 resultTextSize = new Vector2(620f, 260f);

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

        private bool matchStarted = false;
        private bool matchEnded = false;
        private bool roundResolving = false;
        private bool gamePaused = false;
        private bool startSequenceRunning = false;

        private bool player1Crashed = false;
        private bool player2Crashed = false;

        private void Start()
        {
            Time.timeScale = 0f;
            ShowIntroMenu();
        }

        private void Update()
        {
            HandleArcadeInput();
        }

        private void HandleArcadeInput()
        {
            bool confirmPressed = Keyboard.current != null &&
                                  (Keyboard.current.enterKey.wasPressedThisFrame ||
                                   Keyboard.current.spaceKey.wasPressedThisFrame);

            bool backPressed = Keyboard.current != null &&
                               Keyboard.current.escapeKey.wasPressedThisFrame;

            bool startPressed = Keyboard.current != null &&
                                Keyboard.current.pKey.wasPressedThisFrame;

            float menuHorizontal = 0f;

            foreach (Gamepad pad in Gamepad.all)
            {
                if (pad.buttonSouth.wasPressedThisFrame)
                    confirmPressed = true;

                if (pad.buttonEast.wasPressedThisFrame)
                    backPressed = true;

                if (pad.startButton.wasPressedThisFrame)
                    startPressed = true;

                float stickX = pad.leftStick.x.ReadValue();
                float dpadX = pad.dpad.x.ReadValue();

                float currentHorizontal = Mathf.Abs(dpadX) > Mathf.Abs(stickX) ? dpadX : stickX;

                if (Mathf.Abs(currentHorizontal) > Mathf.Abs(menuHorizontal))
                    menuHorizontal = currentHorizontal;
            }

            bool menuIsOpen = introPanel != null && introPanel.activeSelf;

            if (menuIsOpen)
            {
                HandleMenuNavigation(menuHorizontal);

                if (confirmPressed || startPressed)
                {
                    ClickSelectedButton();
                    return;
                }

                if (backPressed)
                {
                    ReturnToLobby();
                    return;
                }

                return;
            }

            if (matchStarted && !matchEnded)
            {
                if (startPressed)
                    TogglePause();
            }
        }

        private void HandleMenuNavigation(float horizontal)
        {
            if (EventSystem.current == null)
                return;

            if (EventSystem.current.currentSelectedGameObject == null)
            {
                FocusButton(startButton);
            }

            if (!menuStickReady)
            {
                if (Mathf.Abs(horizontal) <= menuResetDeadZone)
                    menuStickReady = true;

                return;
            }

            if (horizontal >= menuMoveDeadZone)
            {
                FocusButton(backToLobbyIntroButton);
                menuStickReady = false;
            }
            else if (horizontal <= -menuMoveDeadZone)
            {
                FocusButton(startButton);
                menuStickReady = false;
            }
        }

        private void ClickSelectedButton()
        {
            if (EventSystem.current == null)
            {
                if (startButton != null)
                    startButton.onClick.Invoke();

                return;
            }

            GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

            if (selectedObject == null)
            {
                FocusButton(startButton);
                selectedObject = startButton.gameObject;
            }

            Button selectedButton = selectedObject.GetComponent<Button>();

            if (selectedButton == null)
                selectedButton = selectedObject.GetComponentInParent<Button>();

            if (selectedButton != null && selectedButton.interactable)
            {
                selectedButton.onClick.Invoke();
            }
        }

        private void FocusButton(Button button)
        {
            if (button == null || EventSystem.current == null)
                return;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(button.gameObject);
            button.Select();
        }
        
        private void ShowIntroMenu()
        {
            Time.timeScale = 0f;

            matchStarted = false;
            matchEnded = false;
            gamePaused = false;
            roundResolving = false;
            startSequenceRunning = false;

            player1Lives = 3;
            player2Lives = 3;

            if (introPanel != null)
                introPanel.SetActive(true);

            if (introPanelRect != null)
                introPanelRect.sizeDelta = introPanelSize;

            if (titleText != null)
            {
                titleText.gameObject.SetActive(true);
                titleText.text = "Neon Rush";
            }

            if (p1PreviewText != null)
            {
                p1PreviewText.gameObject.SetActive(true);
                p1PreviewText.text = "P1: ALICE";
            }

            if (p2PreviewText != null)
            {
                p2PreviewText.gameObject.SetActive(true);
                p2PreviewText.text = "P2: BILLY";
            }

            if (resultText != null)
                resultText.gameObject.SetActive(false);

            if (roundMessageText != null)
                roundMessageText.gameObject.SetActive(false);

            if (p1LivesText != null)
                p1LivesText.gameObject.SetActive(false);

            if (p2LivesText != null)
                p2LivesText.gameObject.SetActive(false);

            if (startButton != null)
            {
                startButton.gameObject.SetActive(true);
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(StartMatch);
                SetButtonLabel(startButton, "Start");
            }

            if (backToLobbyIntroButton != null)
            {
                backToLobbyIntroButton.gameObject.SetActive(true);
                backToLobbyIntroButton.onClick.RemoveAllListeners();
                backToLobbyIntroButton.onClick.AddListener(ReturnToLobby);
                SetButtonLabel(backToLobbyIntroButton, "Back to Lobby");
            }
            
            
            if (startButtonRect != null)
                startButtonRect.anchoredPosition = introStartButtonPosition;

            if (backToLobbyIntroButtonRect != null)
                backToLobbyIntroButtonRect.anchoredPosition = introBackButtonPosition;

            if (rematchButton != null && rematchButton != startButton)
                rematchButton.gameObject.SetActive(false);

            if (backToLobbyEndButton != null && backToLobbyEndButton != backToLobbyIntroButton)
                backToLobbyEndButton.gameObject.SetActive(false);

            if (playerSpawner != null)
                playerSpawner.ClearSpawnedPlayers();
            
            FocusButton(startButton);
            
        }

        public void StartMatch()
        {
            if (startSequenceRunning || matchStarted)
                return;

            StartCoroutine(StartMatchSequence());
        }

        private IEnumerator StartMatchSequence()
        {
            startSequenceRunning = true;

            Time.timeScale = 1f;

            matchStarted = true;
            matchEnded = false;
            gamePaused = false;

            player1Lives = 3;
            player2Lives = 3;

            if (introPanel != null)
                introPanel.SetActive(false);

            if (resultText != null)
                resultText.gameObject.SetActive(false);

            if (p1LivesText != null)
                p1LivesText.gameObject.SetActive(true);

            if (p2LivesText != null)
                p2LivesText.gameObject.SetActive(true);

            UpdateLivesUI();

            yield return StartCoroutine(StartRoundSequence());

            startSequenceRunning = false;
        }

        private IEnumerator StartRoundSequence()
        {
            roundResolving = false;

            player1Crashed = false;
            player2Crashed = false;

            if (playerSpawner != null)
                playerSpawner.ClearSpawnedPlayers();

            if (roundMessageText != null)
            {
                roundMessageText.gameObject.SetActive(true);

                roundMessageText.text = "3";
                GameAudioManager.Instance?.PlayCountdownTick();
                yield return new WaitForSeconds(0.6f);

                roundMessageText.text = "2";
                GameAudioManager.Instance?.PlayCountdownTick();
                yield return new WaitForSeconds(0.6f);

                roundMessageText.text = "1";
                GameAudioManager.Instance?.PlayCountdownTick();
                yield return new WaitForSeconds(0.6f);

                roundMessageText.text = "GO!";
                GameAudioManager.Instance?.PlayCountdownGo();
                yield return new WaitForSeconds(0.5f);

                roundMessageText.gameObject.SetActive(false);
            }

            if (playerSpawner != null)
                playerSpawner.SpawnPlayersForRound();
        }

        private void TogglePause()
        {
            if (gamePaused)
                ResumeGame();
            else
                PauseGame();
        }

        private void PauseGame()
        {
            if (!matchStarted || matchEnded)
                return;

            gamePaused = true;
            Time.timeScale = 0f;

            if (introPanel != null)
                introPanel.SetActive(true);

            if (introPanelRect != null)
                introPanelRect.sizeDelta = introPanelSize;

            if (titleText != null)
            {
                titleText.gameObject.SetActive(true);
                titleText.text = "PAUSED";
            }

            if (p1PreviewText != null)
                p1PreviewText.gameObject.SetActive(false);

            if (p2PreviewText != null)
                p2PreviewText.gameObject.SetActive(false);

            if (resultText != null)
                resultText.gameObject.SetActive(false);

            if (startButton != null)
            {
                startButton.gameObject.SetActive(true);
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(ResumeGame);
                SetButtonLabel(startButton, "Resume");
            }

            if (backToLobbyIntroButton != null)
            {
                backToLobbyIntroButton.gameObject.SetActive(true);
                backToLobbyIntroButton.onClick.RemoveAllListeners();
                backToLobbyIntroButton.onClick.AddListener(ReturnToLobby);
                SetButtonLabel(backToLobbyIntroButton, "Back to Lobby");
            }

            if (startButtonRect != null)
                startButtonRect.anchoredPosition = introStartButtonPosition;

            if (backToLobbyIntroButtonRect != null)
                backToLobbyIntroButtonRect.anchoredPosition = introBackButtonPosition;
            
            FocusButton(startButton);
        }

        private void ResumeGame()
        {
            if (!gamePaused)
                return;

            gamePaused = false;
            Time.timeScale = 1f;

            if (introPanel != null)
                introPanel.SetActive(false);
        }

        public void ReportPlayerCrash(int playerID)
        {
            if (matchEnded)
                return;

            if (playerID == 1)
                player1Crashed = true;

            if (playerID == 2)
                player2Crashed = true;

            if (!roundResolving)
            {
                GameAudioManager.Instance?.PlayTronHit();

                roundResolving = true;
                StartCoroutine(ResolveRoundSequence());
            }
        }

        private IEnumerator ResolveRoundSequence()
        {
            yield return new WaitForSeconds(0.05f);

            if (player1Crashed)
                player1Lives--;

            if (player2Crashed)
                player2Lives--;

            player1Lives = Mathf.Max(0, player1Lives);
            player2Lives = Mathf.Max(0, player2Lives);

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
            matchStarted = false;
            roundResolving = false;
            gamePaused = false;

            Time.timeScale = 0f;

            if (playerSpawner != null)
                playerSpawner.ClearSpawnedPlayers();

            if (roundMessageText != null)
                roundMessageText.gameObject.SetActive(false);

            if (p1LivesText != null)
                p1LivesText.gameObject.SetActive(false);

            if (p2LivesText != null)
                p2LivesText.gameObject.SetActive(false);

            if (introPanel != null)
                introPanel.SetActive(true);

            if (introPanelRect != null)
                introPanelRect.sizeDelta = resultPanelSize;
            
            if (result == "DRAW!")
                GameAudioManager.Instance?.PlayDraw();
            else
                GameAudioManager.Instance?.PlayWin();
            if (titleText != null)
            {
                titleText.gameObject.SetActive(true);
                titleText.text = "Neon Rush";
            }

            if (p1PreviewText != null)
                p1PreviewText.gameObject.SetActive(false);

            if (p2PreviewText != null)
                p2PreviewText.gameObject.SetActive(false);

            if (resultText != null)
            {
                resultText.gameObject.SetActive(true);
                resultText.text =
                    "RESULTS\n\n" +
                    "P1: " + player1Lives + " LIVES\n" +
                    "P2: " + player2Lives + " LIVES\n\n" +
                    result;

                if (resultTextRect != null)
                {
                    resultTextRect.anchoredPosition = resultTextPosition;
                    resultTextRect.sizeDelta = resultTextSize;
                }
            }

            if (startButton != null)
            {
                startButton.gameObject.SetActive(true);
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(Rematch);
                SetButtonLabel(startButton, "Rematch");
            }

            if (backToLobbyIntroButton != null)
            {
                backToLobbyIntroButton.gameObject.SetActive(true);
                backToLobbyIntroButton.onClick.RemoveAllListeners();
                backToLobbyIntroButton.onClick.AddListener(ReturnToLobby);
                SetButtonLabel(backToLobbyIntroButton, "Back to Lobby");
            }

            if (startButtonRect != null)
                startButtonRect.anchoredPosition = resultStartButtonPosition;

            if (backToLobbyIntroButtonRect != null)
                backToLobbyIntroButtonRect.anchoredPosition = resultBackButtonPosition;

            if (rematchButton != null && rematchButton != startButton)
                rematchButton.gameObject.SetActive(false);

            if (backToLobbyEndButton != null && backToLobbyEndButton != backToLobbyIntroButton)
                backToLobbyEndButton.gameObject.SetActive(false);
            
            FocusButton(startButton);
        }

        private void UpdateLivesUI()
        {
            if (p1LivesText != null)
                p1LivesText.text = "P1 Lives: " + player1Lives;

            if (p2LivesText != null)
                p2LivesText.text = "P2 Lives: " + player2Lives;
        }

        private void SetButtonLabel(Button button, string label)
        {
            if (button == null)
                return;

            TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();

            if (tmp != null)
            {
                tmp.text = label;
                return;
            }

            Text legacy = button.GetComponentInChildren<Text>();

            if (legacy != null)
                legacy.text = label;
        }

        public void ReturnToLobby()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Lobby");
        }

        public void Rematch()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("TronMinigame");
        }
    }
}