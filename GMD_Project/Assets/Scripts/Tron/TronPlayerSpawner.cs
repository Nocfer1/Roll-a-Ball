using UnityEngine;
using Player;
using Runner;
using Managers;

namespace Tron
{
    public class TronPlayerSpawner : MonoBehaviour
    {
        public GameObject alicePrefab;
        public GameObject billyPrefab;
        public GameObject charliePrefab;

        public Transform player1Spawn;
        public Transform player2Spawn;

        private void Start()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("No existe GameManager en la escena.");
                return;
            }

            SpawnCharacter(GameManager.Instance.player1Character, player1Spawn, 1);
            SpawnCharacter(GameManager.Instance.player2Character, player2Spawn, 2);
        }

        private void SpawnCharacter(CharacterType characterType, Transform spawnPoint, int playerID)
        {
            GameObject prefab = GetPrefab(characterType);

            if (prefab == null || spawnPoint == null) return;

            GameObject spawnedPlayer = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

            LobbyPlayerController lobbyController = spawnedPlayer.GetComponent<LobbyPlayerController>();
            if (lobbyController != null) lobbyController.enabled = false;

            RunnerPlayerController runnerController = spawnedPlayer.GetComponent<RunnerPlayerController>();
            if (runnerController != null) runnerController.enabled = false;

            RunnerPlayerHealth runnerHealth = spawnedPlayer.GetComponent<RunnerPlayerHealth>();
            if (runnerHealth != null) runnerHealth.enabled = false;

            TronPlayerController tronController = spawnedPlayer.GetComponent<TronPlayerController>();
            if (tronController != null)
            {
                tronController.playerID = playerID;
                tronController.enabled = true;
            }

            TronTrailSpawner trailSpawner = spawnedPlayer.GetComponent<TronTrailSpawner>();
            if (trailSpawner != null)
            {
                trailSpawner.enabled = true;
            }
        }

        private GameObject GetPrefab(CharacterType characterType)
        {
            switch (characterType)
            {
                case CharacterType.Alice: return alicePrefab;
                case CharacterType.Billy: return billyPrefab;
                case CharacterType.Charlie: return charliePrefab;
                default: return null;
            }
        }
    }
}