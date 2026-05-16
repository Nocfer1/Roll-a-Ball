using UnityEngine;
using UnityEngine.SceneManagement;

namespace Audio
{
    public class GameAudioManager : MonoBehaviour
    {
        public static GameAudioManager Instance;

        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource uiSource;

        [Header("Music")]
        public AudioClip characterSelectMusic;
        public AudioClip lobbyMusic;
        public AudioClip runnerMusic;
        public AudioClip tronMusic;

        [Header("UI SFX")]
        public AudioClip buttonSelectSfx;
        public AudioClip buttonConfirmSfx;
        public AudioClip buttonBackSfx;

        [Header("Portal SFX")]
        public AudioClip portalEnterSfx;

        [Range(0f, 1f)]
        public float portalEnterVolume = 0.35f;

        [Header("Runner SFX")]
        public AudioClip runnerHitSfx;

        [Header("Tron SFX")]
        public AudioClip tronHitSfx;

        [Header("Gameplay SFX")]
        public AudioClip countdownTickSfx;
        public AudioClip countdownGoSfx;
        public AudioClip winSfx;
        public AudioClip drawSfx;

        private string currentMusicName = "";

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "CharacterSelect")
            {
                PlayCharacterSelectMusic();
            }
            else if (scene.name == "Lobby")
            {
                PlayLobbyMusic();
            }
            else if (scene.name == "RunnerMinigame")
            {
                PlayRunnerMusic();
            }
            else if (scene.name == "TronMinigame")
            {
                PlayTronMusic();
            }
        }

        public void PlayMusic(AudioClip clip, string musicName)
        {
            if (clip == null || musicSource == null)
                return;

            if (currentMusicName == musicName && musicSource.isPlaying)
                return;

            currentMusicName = musicName;

            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void PlayCharacterSelectMusic()
        {
            PlayMusic(characterSelectMusic, "CharacterSelect");
        }

        public void PlayLobbyMusic()
        {
            PlayMusic(lobbyMusic, "Lobby");
        }

        public void PlayRunnerMusic()
        {
            PlayMusic(runnerMusic, "Runner");
        }

        public void PlayTronMusic()
        {
            PlayMusic(tronMusic, "Tron");
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip == null || sfxSource == null)
                return;

            sfxSource.PlayOneShot(clip);
        }

        public void PlayUi(AudioClip clip)
        {
            if (clip == null || uiSource == null)
                return;

            uiSource.PlayOneShot(clip);
        }

        public void PlayButtonSelect()
        {
            PlayUi(buttonSelectSfx);
        }

        public void PlayButtonConfirm()
        {
            PlayUi(buttonConfirmSfx);
        }

        public void PlayButtonBack()
        {
            PlayUi(buttonBackSfx);
        }

        public void PlayPortalEnter()
        {
            if (portalEnterSfx == null || sfxSource == null)
                return;

            sfxSource.PlayOneShot(portalEnterSfx, portalEnterVolume);
        }

        public void PlayCountdownTick()
        {
            PlaySfx(countdownTickSfx);
        }

        public void PlayCountdownGo()
        {
            PlaySfx(countdownGoSfx);
        }

        public void PlayWin()
        {
            PlaySfx(winSfx);
        }

        public void PlayDraw()
        {
            PlaySfx(drawSfx);
        }

        public void PlayRunnerHit()
        {
            PlaySfx(runnerHitSfx);
        }

        public void PlayTronHit()
        {
            PlaySfx(tronHitSfx);
        }
    }
}