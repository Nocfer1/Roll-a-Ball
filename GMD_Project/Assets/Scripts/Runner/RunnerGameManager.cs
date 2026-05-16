using System.Collections;
using Audio;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayerFolder;
using UnityEngine.InputSystem;

namespace Runner
{
    public class RunnerGameManager : MonoBehaviour
    {
        public enum RunnerDifficulty
        {
            Easy,
            Normal,
            Hard
        }
        [Header("Difficulty UI")]
        public RunnerDifficulty selectedDifficulty = RunnerDifficulty.Normal;
        public BallSpawner ballSpawner;

        [Header("Pause Menu")]
        public bool allowPauseMenu = true;
        private bool gamePausedByStart = false;
        
        public static RunnerGameManager Instance;

        [Header("Intro UI")]
        public GameObject introPanel;
        public Button startButton;
        public Button backToLobbyIntroButton;

        [Header("Menu Layout")]
        public RectTransform introPanelRect;
        public RectTransform startButtonRect;
        public RectTransform backToLobbyIntroButtonRect;
        public RectTransform difficultyButtonRect;
        public RectTransform resultTextRect;

        public Button difficultyButton;

        public GameObject titleText;
        public GameObject p1PreviewText;
        public GameObject p2PreviewText;

        public Vector2 introPanelSize = new Vector2(700f, 400f);
        public Vector2 resultPanelSize = new Vector2(620f, 600f);

        public Vector2 introStartButtonPosition = new Vector2(-120f, -120f);
        public Vector2 introBackButtonPosition = new Vector2(120f, -120f);
        public Vector2 introDifficultyButtonPosition = new Vector2(0f, -65f);

        public Vector2 resultStartButtonPosition = new Vector2(-120f, -180f);
        public Vector2 resultBackButtonPosition = new Vector2(120f, -180f);

        public Vector2 resultTextPosition = new Vector2(0f, 45f);
        public Vector2 resultTextSize = new Vector2(600f, 280f);

        [Header("Runner Animation")]
        public RuntimeAnimatorController runnerAnimatorController;
        

        [Header("Countdown UI")]
        public TextMeshProUGUI countdownText;
        public Vector2 countdownTextSize = new Vector2(900f, 250f);
        public float countdownFontSize = 100f;

        [Header("Players")]
        public RunnerPlayerHealth player1Health;
        public RunnerPlayerHealth player2Health;

        public RunnerPlayerController player1Controller;
        public RunnerPlayerController player2Controller;

        public Animator player1Animator;
        public Animator player2Animator;

        [Header("Animation Settings")]
        public string runningBoolName = "IsRunning";

        [Header("Gameplay UI")]
        public TextMeshProUGUI player1LivesText;
        public TextMeshProUGUI player2LivesText;
        public TextMeshProUGUI progressText;
        public TextMeshProUGUI resultText;

        [Header("Legacy End UI Optional")]
        public Button backToLobbyButton;
        public Button rematchButton;

        [Header("Objects Disabled Until Start")]
        public GameObject[] objectsToDisableUntilStart;

        [Header("Game Settings")]
        public float runDuration = 45f;

        [Header("Optional Font")]
        public TMP_FontAsset menuFont;

        private float elapsedTime = 0f;
        private bool gameStarted = false;
        private bool gameEnded = false;
        private bool startSequenceRunning = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Time.timeScale = 0f;
        }

        private void Start()
        {
            CacheMissingRects();
            ApplyFonts();
            ConfigureCountdownText();
            ShowIntroMenu();
            
            if (difficultyButton != null)
            {
                difficultyButton.onClick.RemoveAllListeners();
                difficultyButton.onClick.AddListener(CycleDifficulty);
            }

            UpdateDifficultyButtonText();
            ApplySelectedDifficulty();
        }

        private void Update()
        {
            HandleArcadeMenuInput();

            if (!gameStarted || gameEnded || gamePausedByStart)
                return;

            elapsedTime += Time.deltaTime;
            UpdateUI();
            CheckEndConditions();
        }
        
        private void HandleArcadeMenuInput()
        {
            if (startSequenceRunning)
                return;

            bool confirmPressed = false;
            bool backPressed = false;
            bool startPressed = false;
            bool changeDifficultyPressed = false;

            foreach (Gamepad gamepad in Gamepad.all)
            {
                if (gamepad.buttonSouth.wasPressedThisFrame)
                    confirmPressed = true;

                if (gamepad.buttonEast.wasPressedThisFrame)
                    backPressed = true;

                if (gamepad.startButton.wasPressedThisFrame)
                    startPressed = true;

                if (gamepad.buttonWest.wasPressedThisFrame)
                    changeDifficultyPressed = true;
            }

            if (!gameStarted && !gameEnded)
            {
                if (changeDifficultyPressed)
                    CycleDifficulty();

                if (confirmPressed)
                    StartGame();

                if (backPressed)
                    ReturnToLobby();

                return;
            }

            if (gameStarted && !gameEnded)
            {
                if (startPressed)
                {
                    if (gamePausedByStart)
                        ClosePauseMenu();
                    else
                        OpenPauseMenu();
                }

                return;
            }

            if (gameEnded)
            {
                if (confirmPressed || startPressed)
                    Rematch();

                if (backPressed)
                    ReturnToLobby();
            }
        }
        
        public void CycleDifficulty()
        {
            if (gameStarted)
                return;

            if (selectedDifficulty == RunnerDifficulty.Easy)
                selectedDifficulty = RunnerDifficulty.Normal;
            else if (selectedDifficulty == RunnerDifficulty.Normal)
                selectedDifficulty = RunnerDifficulty.Hard;
            else
                selectedDifficulty = RunnerDifficulty.Easy;

            UpdateDifficultyButtonText();
            ApplySelectedDifficulty();
        }
        
        private void OpenPauseMenu()
        {
            if (!allowPauseMenu || gameEnded || !gameStarted)
                return;

            gamePausedByStart = true;

            Time.timeScale = 0f;

            SetPlayerControls(false);
            SetPlayersRunning(false);

            if (introPanel != null)
                introPanel.SetActive(true);

            ApplyResultLayout();

            if (resultText != null)
            {
                resultText.gameObject.SetActive(true);
            }

            if (difficultyButton != null)
                difficultyButton.gameObject.SetActive(false);

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

            SelectButton(startButton);
        }

        private void ClosePauseMenu()
        {
            if (!gamePausedByStart)
                return;

            gamePausedByStart = false;

            if (introPanel != null)
                introPanel.SetActive(false);

            if (resultText != null)
                resultText.gameObject.SetActive(false);

            SetPlayerControls(true);
            SetPlayersRunning(true);

            Time.timeScale = 1f;
        }

        private void UpdateDifficultyButtonText()
        {
            if (difficultyButton == null)
                return;

            SetButtonLabel(difficultyButton, selectedDifficulty.ToString());
        }

        private void ApplySelectedDifficulty()
        {
            if (ballSpawner == null)
                return;

            if (selectedDifficulty == RunnerDifficulty.Easy)
                ballSpawner.ApplyEasyDifficulty();
            else if (selectedDifficulty == RunnerDifficulty.Normal)
                ballSpawner.ApplyNormalDifficulty();
            else if (selectedDifficulty == RunnerDifficulty.Hard)
                ballSpawner.ApplyHardDifficulty();
        }

        private void CacheMissingRects()
        {
            if (introPanelRect == null && introPanel != null)
                introPanelRect = introPanel.GetComponent<RectTransform>();

            if (startButtonRect == null && startButton != null)
                startButtonRect = startButton.GetComponent<RectTransform>();

            if (backToLobbyIntroButtonRect == null && backToLobbyIntroButton != null)
                backToLobbyIntroButtonRect = backToLobbyIntroButton.GetComponent<RectTransform>();

            if (difficultyButtonRect == null && difficultyButton != null)
                difficultyButtonRect = difficultyButton.GetComponent<RectTransform>();
            
            if (resultTextRect == null && resultText != null)
                resultTextRect = resultText.GetComponent<RectTransform>();
        }

        private void ShowIntroMenu()
        {
            Time.timeScale = 0f;

            gameStarted = false;
            gameEnded = false;
            startSequenceRunning = false;
            elapsedTime = 0f;

            if (introPanel != null)
                introPanel.SetActive(true);

            ApplyIntroLayout();

            if (countdownText != null)
                countdownText.gameObject.SetActive(false);

            if (player1LivesText != null)
                player1LivesText.gameObject.SetActive(false);

            if (player2LivesText != null)
                player2LivesText.gameObject.SetActive(false);

            if (progressText != null)
                progressText.gameObject.SetActive(false);

            if (resultText != null)
                resultText.gameObject.SetActive(false);

            if (backToLobbyButton != null)
                backToLobbyButton.gameObject.SetActive(false);

            if (rematchButton != null)
                rematchButton.gameObject.SetActive(false);

            if (startButton != null)
            {
                startButton.gameObject.SetActive(true);
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(StartGame);
                SetButtonLabel(startButton, "Start");
            }

            if (backToLobbyIntroButton != null)
            {
                backToLobbyIntroButton.gameObject.SetActive(true);
                backToLobbyIntroButton.onClick.RemoveAllListeners();
                backToLobbyIntroButton.onClick.AddListener(ReturnToLobby);
                SetButtonLabel(backToLobbyIntroButton, "Back to Lobby");
            }

            if (difficultyButton != null)
            {
                difficultyButton.gameObject.SetActive(true);
                difficultyButton.onClick.RemoveAllListeners();
                difficultyButton.onClick.AddListener(CycleDifficulty);
                UpdateDifficultyButtonText();
            }

            ApplySelectedDifficulty();
            SetPlayerControls(false);
            SetPlayersRunning(false);

            foreach (GameObject obj in objectsToDisableUntilStart)
            {
                if (obj != null)
                    obj.SetActive(false);
            }

            SelectButton(startButton);
            
            gamePausedByStart = false;

            if (difficultyButton != null)
            {
                difficultyButton.gameObject.SetActive(true);
                difficultyButton.onClick.RemoveAllListeners();
                difficultyButton.onClick.AddListener(CycleDifficulty);
                UpdateDifficultyButtonText();
            }

            ApplySelectedDifficulty();
        }

        public void StartGame()
        {
            if (startSequenceRunning || gameStarted)
                return;

            StartCoroutine(StartGameSequence());
        }

        private IEnumerator StartGameSequence()
        {
            startSequenceRunning = true;

            if (introPanel != null)
                introPanel.SetActive(false);

            if (resultText != null)
                resultText.gameObject.SetActive(false);

            if (countdownText != null)
            {
                countdownText.gameObject.SetActive(true);

                countdownText.text = "3";
                GameAudioManager.Instance?.PlayCountdownTick();
                yield return new WaitForSecondsRealtime(0.7f);

                countdownText.text = "2";
                GameAudioManager.Instance?.PlayCountdownTick();
                yield return new WaitForSecondsRealtime(0.7f);

                countdownText.text = "1";
                GameAudioManager.Instance?.PlayCountdownTick();
                yield return new WaitForSecondsRealtime(0.7f);

                countdownText.text = "DODGE IT!";
                GameAudioManager.Instance?.PlayCountdownGo();
                yield return new WaitForSecondsRealtime(0.8f);

                countdownText.gameObject.SetActive(false);
            }

            foreach (GameObject obj in objectsToDisableUntilStart)
            {
                if (obj != null)
                    obj.SetActive(true);
            }

            if (player1LivesText != null)
                player1LivesText.gameObject.SetActive(true);

            if (player2LivesText != null)
                player2LivesText.gameObject.SetActive(true);

            if (progressText != null)
                progressText.gameObject.SetActive(true);

            elapsedTime = 0f;
            gameEnded = false;
            gameStarted = true;
            startSequenceRunning = false;

            SetPlayerControls(true);
            SetPlayersRunning(true);

            gamePausedByStart = false;

            if (difficultyButton != null)
                difficultyButton.gameObject.SetActive(false);
            
            Time.timeScale = 1f;

            UpdateUI();
        }

        private void SetPlayerControls(bool enabled)
        {
            if (player1Controller != null)
                player1Controller.enabled = enabled;

            if (player2Controller != null)
                player2Controller.enabled = enabled;
        }

        private void SetPlayersRunning(bool isRunning)
        {
            if (player1Animator != null)
                player1Animator.SetBool(runningBoolName, isRunning);

            if (player2Animator != null)
                player2Animator.SetBool(runningBoolName, isRunning);
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
            if (player1Health == null || player2Health == null)
                return;

            bool bothDead = player1Health.isDead && player2Health.isDead;
            bool reachedEnd = elapsedTime >= runDuration;

            if (bothDead || reachedEnd)
                EndGame();
        }

        public float GetCurrentProgressPercent()
        {
            return Mathf.Clamp01(elapsedTime / runDuration) * 100f;
        }

        private void EndGame()
        {
            if (gameEnded)
                return;

            if (player1Health == null || player2Health == null)
                return;

            gameEnded = true;
            gameStarted = false;

            Time.timeScale = 0f;

            SetPlayerControls(false);
            SetPlayersRunning(false);

            if (player1LivesText != null)
                player1LivesText.gameObject.SetActive(false);

            if (player2LivesText != null)
                player2LivesText.gameObject.SetActive(false);

            if (progressText != null)
                progressText.gameObject.SetActive(false);

            if (introPanel != null)
                introPanel.SetActive(true);

            ApplyResultLayout();

            float finalProgressPercent = GetCurrentProgressPercent();

            float p1Progress = player1Health.isDead ? player1Health.deathProgressPercent : finalProgressPercent;
            float p2Progress = player2Health.isDead ? player2Health.deathProgressPercent : finalProgressPercent;

            int p1Score = Mathf.RoundToInt(p1Progress * 10f)
                          + (player1Health.currentLives * 100)
                          - (player1Health.hitsReceived * 25);

            int p2Score = Mathf.RoundToInt(p2Progress * 10f)
                          + (player2Health.currentLives * 100)
                          - (player2Health.hitsReceived * 25);

            string winnerText;

            if (p1Score > p2Score)
                winnerText = "Winner: Player 1";
            else if (p2Score > p1Score)
                winnerText = "Winner: Player 2";
            else
                winnerText = "Winner: Draw";

            if (winnerText == "Winner: Draw")
                GameAudioManager.Instance?.PlayDraw();
            else
                GameAudioManager.Instance?.PlayWin();
            
            if (resultText != null)
            {
                resultText.gameObject.SetActive(true);
                resultText.text =
                    "RESULTS\n\n" +
                    "P1 SCORE: " + p1Score + "\n" +
                    "P2 SCORE: " + p2Score + "\n\n" +
                    winnerText.ToUpper();
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

            if (backToLobbyButton != null)
                backToLobbyButton.gameObject.SetActive(false);

            if (rematchButton != null)
                rematchButton.gameObject.SetActive(false);

            SelectButton(startButton);
        }

        private void ApplyIntroLayout()
        {
            ResizeIntroPanel(introPanelSize);

            if (startButtonRect != null)
                startButtonRect.anchoredPosition = introStartButtonPosition;

            if (backToLobbyIntroButtonRect != null)
                backToLobbyIntroButtonRect.anchoredPosition = introBackButtonPosition;

            if (difficultyButtonRect != null)
                difficultyButtonRect.anchoredPosition = introDifficultyButtonPosition;

            if (difficultyButton != null)
                difficultyButton.gameObject.SetActive(true);
            
            if (resultText != null)
                resultText.gameObject.SetActive(false);

            if (titleText != null)
                titleText.SetActive(true);

            if (p1PreviewText != null)
                p1PreviewText.SetActive(true);

            if (p2PreviewText != null)
                p2PreviewText.SetActive(true);
        }

        private void ApplyResultLayout()
        {
            ResizeIntroPanel(resultPanelSize);

            if (startButtonRect != null)
                startButtonRect.anchoredPosition = resultStartButtonPosition;

            if (backToLobbyIntroButtonRect != null)
                backToLobbyIntroButtonRect.anchoredPosition = resultBackButtonPosition;

            if (difficultyButton != null)
                difficultyButton.gameObject.SetActive(false);
            
            if (resultTextRect != null)
            {
                resultTextRect.anchoredPosition = resultTextPosition;
                resultTextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, resultTextSize.x);
                resultTextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, resultTextSize.y);
            }

            if (resultText != null)
            {
                resultText.enableWordWrapping = false;
                resultText.overflowMode = TextOverflowModes.Overflow;
                resultText.alignment = TextAlignmentOptions.Center;
                resultText.enableAutoSizing = true;
                resultText.fontSizeMin = 16f;
                resultText.fontSizeMax = 34f;
            }

            if (titleText != null)
                titleText.SetActive(false);

            if (p1PreviewText != null)
                p1PreviewText.SetActive(false);

            if (p2PreviewText != null)
                p2PreviewText.SetActive(false);
        }

        private void ResizeIntroPanel(Vector2 size)
        {
            if (introPanelRect == null && introPanel != null)
                introPanelRect = introPanel.GetComponent<RectTransform>();

            if (introPanelRect == null)
                return;

            introPanelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            introPanelRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }

        private void ConfigureCountdownText()
        {
            if (countdownText == null)
                return;

            countdownText.enableWordWrapping = false;
            countdownText.overflowMode = TextOverflowModes.Overflow;
            countdownText.alignment = TextAlignmentOptions.Center;
            countdownText.enableAutoSizing = false;
            countdownText.fontSize = countdownFontSize;

            RectTransform rect = countdownText.GetComponent<RectTransform>();

            if (rect != null)
            {
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, countdownTextSize.x);
                rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, countdownTextSize.y);
            }
        }

        private void SetButtonLabel(Button button, string label)
        {
            if (button == null)
                return;

            TextMeshProUGUI tmpText = button.GetComponentInChildren<TextMeshProUGUI>();

            if (tmpText != null)
            {
                tmpText.text = label;
                return;
            }

            Text legacyText = button.GetComponentInChildren<Text>();

            if (legacyText != null)
                legacyText.text = label;
        }

        private void SelectButton(Button button)
        {
            if (button == null || EventSystem.current == null)
                return;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

        private void ApplyFonts()
        {
            if (menuFont == null)
                return;

            ApplyFontToText(countdownText);
            ApplyFontToText(player1LivesText);
            ApplyFontToText(player2LivesText);
            ApplyFontToText(progressText);
            ApplyFontToText(resultText);

            ApplyFontToButton(startButton);
            ApplyFontToButton(backToLobbyIntroButton);
            ApplyFontToButton(backToLobbyButton);
            ApplyFontToButton(rematchButton);
        }

        private void ApplyFontToText(TextMeshProUGUI text)
        {
            if (text != null)
                text.font = menuFont;
        }

        private void ApplyFontToButton(Button button)
        {
            if (button == null)
                return;

            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.font = menuFont;
        }

        public void ReturnToLobby()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("Lobby");
        }

        public void Rematch()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("RunnerMinigame");
        }
    }
}