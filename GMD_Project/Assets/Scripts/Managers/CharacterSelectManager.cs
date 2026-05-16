using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class CharacterSelectManager : MonoBehaviour
    {
        [Header("Intro UI")]
        public GameObject introPanel;
        public Button playButton;
        public Button exitButton;

        [Header("Preview Spots")]
        public Transform player1PreviewSpot;
        public Transform player2PreviewSpot;

        [Header("Character Prefabs")]
        public GameObject alicePrefab;
        public GameObject billyPrefab;
        public GameObject charliePrefab;

        [Header("UI Text")]
        public TextMeshProUGUI player1Text;
        public TextMeshProUGUI player2Text;
        public TextMeshProUGUI player1ReadyText;
        public TextMeshProUGUI player2ReadyText;

        [Header("Input")]
        public float stickDeadZone = 0.55f;
        public float stickResetZone = 0.25f;
        public float inputLockAfterIntro = 0.25f;

        private int player1Index = 0;
        private int player2Index = 1;

        private GameObject player1Preview;
        private GameObject player2Preview;

        private bool introOpen = true;
        private int introSelectedIndex = 0;

        private bool introStickReady = true;
        private bool player1StickReady = true;
        private bool player2StickReady = true;

        private float inputLockTimer = 0f;

        private readonly CharacterType[] characters =
        {
            CharacterType.Alice,
            CharacterType.Billy,
            CharacterType.Charlie
        };

        private void Start()
        {
            GameAudioManager.Instance?.PlayCharacterSelectMusic();
            
            if (GameManager.Instance != null)
                GameManager.Instance.ResetReadyState();

            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(StartCharacterSelection);
            }

            if (exitButton != null)
            {
                exitButton.onClick.RemoveAllListeners();
                exitButton.onClick.AddListener(ExitGame);
            }

            UpdateSelections();
            UpdatePreviews();
            ShowIntro();
        }

        private void ReturnToIntroPanel()
        {
            GameAudioManager.Instance?.PlayButtonBack();

            if (GameManager.Instance != null)
                GameManager.Instance.ResetReadyState();

            introOpen = true;

            if (introPanel != null)
                introPanel.SetActive(true);

            SetCharacterSelectionVisible(false);
            UpdateSelections();
            SelectIntroButton();
        }
        
        private void Update()
        {
            if (inputLockTimer > 0f)
                inputLockTimer -= Time.deltaTime;

            if (introOpen)
            {
                HandleIntroInput();
                return;
            }

            if (inputLockTimer > 0f)
                return;

            HandleCharacterSelectionInput();

            if (GameManager.Instance != null &&
                GameManager.Instance.player1Ready &&
                GameManager.Instance.player2Ready)
            {
                SceneManager.LoadScene("Lobby");
            }
        }

        private void ShowIntro()
        {
            introOpen = true;
            introSelectedIndex = 0;

            if (introPanel != null)
                introPanel.SetActive(true);

            SetCharacterSelectionVisible(false);
            SelectIntroButton();
        }

        public void StartCharacterSelection()
        {
            GameAudioManager.Instance?.PlayButtonConfirm();

            if (GameManager.Instance != null)
                GameManager.Instance.ResetReadyState();

            introOpen = false;
            inputLockTimer = inputLockAfterIntro;

            if (introPanel != null)
                introPanel.SetActive(false);

            SetCharacterSelectionVisible(true);
            UpdateSelections();
            UpdatePreviews();
        }

        public void ExitGame()
        {
            GameAudioManager.Instance?.PlayButtonBack();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void HandleIntroInput()
        {
            float vertical = 0f;
            bool confirmPressed = false;
            bool backPressed = false;

            if (Keyboard.current != null)
            {
                if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame)
                    vertical = 1f;

                if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame)
                    vertical = -1f;

                if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
                    confirmPressed = true;

                if (Keyboard.current.escapeKey.wasPressedThisFrame)
                    backPressed = true;
            }

            foreach (Gamepad pad in Gamepad.all)
            {
                float stickY = pad.leftStick.y.ReadValue();
                float dpadY = pad.dpad.y.ReadValue();
                float inputY = Mathf.Abs(dpadY) > Mathf.Abs(stickY) ? dpadY : stickY;

                if (Mathf.Abs(inputY) > Mathf.Abs(vertical))
                    vertical = inputY;

                if (pad.buttonSouth.wasPressedThisFrame || pad.startButton.wasPressedThisFrame)
                    confirmPressed = true;

                if (pad.buttonEast.wasPressedThisFrame)
                    backPressed = true;
            }

            if (!introStickReady)
            {
                if (Mathf.Abs(vertical) <= stickResetZone)
                    introStickReady = true;
            }
            else
            {
                if (vertical >= stickDeadZone)
                {
                    introSelectedIndex = 0;
                    SelectIntroButton();
                    GameAudioManager.Instance?.PlayButtonSelect();
                    introStickReady = false;
                }
                else if (vertical <= -stickDeadZone)
                {
                    introSelectedIndex = 1;
                    SelectIntroButton();
                    GameAudioManager.Instance?.PlayButtonSelect();
                    introStickReady = false;
                }
            }

            if (confirmPressed)
            {
                if (introSelectedIndex == 0)
                    StartCharacterSelection();
                else
                    ExitGame();
            }

            if (backPressed)
                ExitGame();
        }

        private void SelectIntroButton()
        {
            if (introSelectedIndex == 0 && playButton != null)
                playButton.Select();

            if (introSelectedIndex == 1 && exitButton != null)
                exitButton.Select();
        }

        private void HandleCharacterSelectionInput()
        {
            foreach (Gamepad pad in Gamepad.all)
            {
                if (pad.buttonEast.wasPressedThisFrame)
                {
                    ReturnToIntroPanel();
                    return;
                }
            }

            HandleKeyboardInput();

            if (Gamepad.all.Count == 1)
            {
                Gamepad pad = Gamepad.all[0];

                if (GameManager.Instance != null && !GameManager.Instance.player1Ready)
                    HandlePlayerGamepad(pad, 1);
                else
                    HandlePlayerGamepad(pad, 2);
            }
            else if (Gamepad.all.Count >= 2)
            {
                HandlePlayerGamepad(Gamepad.all[0], 1);
                HandlePlayerGamepad(Gamepad.all[1], 2);
            }
        }
        private void HandleKeyboardInput()
        {
            if (Keyboard.current == null)
                return;

            if (Keyboard.current.aKey.wasPressedThisFrame)
                MovePlayer1(-1);

            if (Keyboard.current.dKey.wasPressedThisFrame)
                MovePlayer1(1);

            if (Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.spaceKey.wasPressedThisFrame)
                ReadyPlayer1();

            if (Keyboard.current.sKey.wasPressedThisFrame)
                UnreadyPlayer1();

            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
                MovePlayer2(-1);

            if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
                MovePlayer2(1);

            if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
                ReadyPlayer2();

            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
                UnreadyPlayer2();
        }

        private void HandlePlayerGamepad(Gamepad pad, int playerID)
        {
            float stickX = pad.leftStick.x.ReadValue();
            float dpadX = pad.dpad.x.ReadValue();
            float horizontal = Mathf.Abs(dpadX) > Mathf.Abs(stickX) ? dpadX : stickX;

            bool stickReady = playerID == 1 ? player1StickReady : player2StickReady;

            if (!stickReady)
            {
                if (Mathf.Abs(horizontal) <= stickResetZone)
                {
                    if (playerID == 1)
                        player1StickReady = true;
                    else
                        player2StickReady = true;
                }
            }
            else
            {
                if (horizontal >= stickDeadZone)
                {
                    if (playerID == 1)
                    {
                        MovePlayer1(1);
                        player1StickReady = false;
                    }
                    else
                    {
                        MovePlayer2(1);
                        player2StickReady = false;
                    }
                }
                else if (horizontal <= -stickDeadZone)
                {
                    if (playerID == 1)
                    {
                        MovePlayer1(-1);
                        player1StickReady = false;
                    }
                    else
                    {
                        MovePlayer2(-1);
                        player2StickReady = false;
                    }
                }
            }

            if (pad.buttonSouth.wasPressedThisFrame)
            {
                if (playerID == 1)
                    ReadyPlayer1();
                else
                    ReadyPlayer2();
            }

            if (pad.buttonEast.wasPressedThisFrame)
            {
                if (playerID == 1)
                    UnreadyPlayer1();
                else
                    UnreadyPlayer2();
            }
        }

        private void MovePlayer1(int direction)
        {
            if (GameManager.Instance != null && GameManager.Instance.player1Ready)
                return;

            player1Index = (player1Index + direction + characters.Length) % characters.Length;

            GameAudioManager.Instance?.PlayButtonSelect();

            UpdateSelections();
            UpdatePreviews();
        }

        private void MovePlayer2(int direction)
        {
            if (GameManager.Instance != null && GameManager.Instance.player2Ready)
                return;

            player2Index = (player2Index + direction + characters.Length) % characters.Length;

            GameAudioManager.Instance?.PlayButtonSelect();

            UpdateSelections();
            UpdatePreviews();
        }

        private void ReadyPlayer1()
        {
            if (GameManager.Instance == null || GameManager.Instance.player1Ready)
                return;

            GameManager.Instance.player1Ready = true;
            GameAudioManager.Instance?.PlayButtonConfirm();
            UpdateSelections();
        }

        private void ReadyPlayer2()
        {
            if (GameManager.Instance == null || GameManager.Instance.player2Ready)
                return;

            GameManager.Instance.player2Ready = true;
            GameAudioManager.Instance?.PlayButtonConfirm();
            UpdateSelections();
        }

        private void UnreadyPlayer1()
        {
            if (GameManager.Instance == null || !GameManager.Instance.player1Ready)
                return;

            GameManager.Instance.player1Ready = false;
            GameAudioManager.Instance?.PlayButtonBack();
            UpdateSelections();
        }

        private void UnreadyPlayer2()
        {
            if (GameManager.Instance == null || !GameManager.Instance.player2Ready)
                return;

            GameManager.Instance.player2Ready = false;
            GameAudioManager.Instance?.PlayButtonBack();
            UpdateSelections();
        }

        private void SetCharacterSelectionVisible(bool visible)
        {
            if (player1Text != null)
                player1Text.gameObject.SetActive(visible);

            if (player2Text != null)
                player2Text.gameObject.SetActive(visible);

            if (player1ReadyText != null)
                player1ReadyText.gameObject.SetActive(visible);

            if (player2ReadyText != null)
                player2ReadyText.gameObject.SetActive(visible);

            if (player1Preview != null)
                player1Preview.SetActive(visible);

            if (player2Preview != null)
                player2Preview.SetActive(visible);
        }

        private void UpdateSelections()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.player1Character = characters[player1Index];
                GameManager.Instance.player2Character = characters[player2Index];
            }

            if (player1Text != null)
                player1Text.text = "P1: " + characters[player1Index];

            if (player2Text != null)
                player2Text.text = "P2: " + characters[player2Index];

            if (GameManager.Instance != null)
            {
                if (player1ReadyText != null)
                    player1ReadyText.text = "P1 Ready: " + (GameManager.Instance.player1Ready ? "YES" : "NO");

                if (player2ReadyText != null)
                    player2ReadyText.text = "P2 Ready: " + (GameManager.Instance.player2Ready ? "YES" : "NO");
            }
        }

        private void UpdatePreviews()
        {
            if (player1Preview != null)
                Destroy(player1Preview);

            if (player2Preview != null)
                Destroy(player2Preview);

            GameObject p1Prefab = GetPrefab(characters[player1Index]);
            GameObject p2Prefab = GetPrefab(characters[player2Index]);

            if (p1Prefab != null && player1PreviewSpot != null)
            {
                player1Preview = Instantiate(p1Prefab, player1PreviewSpot.position, player1PreviewSpot.rotation);
                PreparePreview(player1Preview);
            }

            if (p2Prefab != null && player2PreviewSpot != null)
            {
                player2Preview = Instantiate(p2Prefab, player2PreviewSpot.position, player2PreviewSpot.rotation);
                PreparePreview(player2Preview);
            }

            if (introOpen)
                SetCharacterSelectionVisible(false);
        }

        private void PreparePreview(GameObject preview)
        {
            if (preview == null)
                return;

            preview.transform.localScale = Vector3.one;

            MonoBehaviour[] scripts = preview.GetComponentsInChildren<MonoBehaviour>();

            foreach (MonoBehaviour script in scripts)
                script.enabled = false;
        }

        private GameObject GetPrefab(CharacterType character)
        {
            switch (character)
            {
                case CharacterType.Alice:
                    return alicePrefab;

                case CharacterType.Billy:
                    return billyPrefab;

                case CharacterType.Charlie:
                    return charliePrefab;

                default:
                    return null;
            }
        }
    }
}