using Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class MinigamePortal : MonoBehaviour
    {
        public string sceneToLoad;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerFolder.LobbyPlayerController>() != null)
            {
                GameAudioManager.Instance?.PlayPortalEnter();
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}