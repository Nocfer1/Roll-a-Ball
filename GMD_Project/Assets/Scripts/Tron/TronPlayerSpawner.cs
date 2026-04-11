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

        private GameObject currentPlayer1;
        private GameObject currentPlayer2;

        public Transform trailContainer;
        
        public void SpawnPlayersForRound()
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("No existe GameManager en la escena.");
                return;
            }

            currentPlayer1 = SpawnCharacter(GameManager.Instance.player1Character, player1Spawn, 1);
            currentPlayer2 = SpawnCharacter(GameManager.Instance.player2Character, player2Spawn, 2);
        }

        public void ClearSpawnedPlayers()
        {
            if (currentPlayer1 != null) Destroy(currentPlayer1);
            if (currentPlayer2 != null) Destroy(currentPlayer2);

            if (trailContainer != null)
            {
                for (int i = trailContainer.childCount - 1; i >= 0; i--)
                {
                    Destroy(trailContainer.GetChild(i).gameObject);
                }
            }

            TronTrailSegment[] allTrails = FindObjectsByType<TronTrailSegment>(FindObjectsSortMode.None);
            foreach (TronTrailSegment trail in allTrails)
            {
                Destroy(trail.gameObject);
            }
        }

        private GameObject SpawnCharacter(CharacterType characterType, Transform spawnPoint, int playerID)
        {
            GameObject prefab = GetPrefab(characterType);

            if (prefab == null || spawnPoint == null) return null;

            GameObject spawnedPlayer = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

            spawnedPlayer.transform.localScale = new Vector3(0.45f, 0.7f, 0.45f);

            Renderer playerRenderer = spawnedPlayer.GetComponentInChildren<Renderer>();
            TronTrailSpawner trailSpawner = spawnedPlayer.GetComponent<TronTrailSpawner>();

            if (playerRenderer != null && trailSpawner != null)
            {
                trailSpawner.playerMaterial = playerRenderer.sharedMaterial;
            }

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

            if (trailSpawner != null)
            {
                trailSpawner.enabled = true;
                trailSpawner.SetOwner(playerID);
            }

            CapsuleCollider capsule = spawnedPlayer.GetComponent<CapsuleCollider>();
            if (capsule != null)
            {
                capsule.radius = 0.16f;
                capsule.height = 0.75f;
                capsule.center = new Vector3(0f, 0.38f, 0f);
            }

            CharacterController characterController = spawnedPlayer.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.radius = 0.16f;
                characterController.height = 0.75f;
                characterController.center = new Vector3(0f, 0.38f, 0f);
            }

            return spawnedPlayer;
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