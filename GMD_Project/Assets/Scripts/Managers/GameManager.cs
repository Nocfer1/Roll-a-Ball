using UnityEngine;

namespace Managers
{
    public enum CharacterType
    {
        Alice,
        Billy,
        Charlie
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public CharacterType player1Character = CharacterType.Alice;
        public CharacterType player2Character = CharacterType.Billy;

        public bool player1Ready = false;
        public bool player2Ready = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ResetReadyState()
        {
            player1Ready = false;
            player2Ready = false;
        }
    }
}