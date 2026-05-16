using System.Collections.Generic;
using UnityEngine;

namespace Runner
{
    public class BallSpawner : MonoBehaviour
    {
        [Header("Obstacle Prefabs")]
        public GameObject ballPrefab;
        public GameObject crystalPrefab;
        public GameObject doubleLaneBlockPrefab;

        [Header("Spawn Settings")]
        public float spawnInterval = 1.5f;
        public float spawnZ = 35f;
        public float spawnY = 1f;

        [Header("Difficulty")]
        [Range(0f, 1f)]
        public float doubleBlockChance = 0.35f;

        [Range(0f, 1f)]
        public float twoSingleObstaclesChance = 0.25f;

        [Range(0f, 1f)]
        public float crystalChance = 0.4f;

        private float timer;

        private readonly float[] player1Lanes = { -5f, -3f, -1f };
        private readonly float[] player2Lanes = { 1f, 3f, 5f };

        private readonly float[] player1DoubleBlockCenters = { -4f, -2f };
        private readonly float[] player2DoubleBlockCenters = { 2f, 4f };

        private void OnEnable()
        {
            timer = 0f;
        }

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                SpawnWave();
                timer = 0f;
            }
        }

        private void SpawnWave()
        {
            SpawnForPlayerSide(player1Lanes, player1DoubleBlockCenters);
            SpawnForPlayerSide(player2Lanes, player2DoubleBlockCenters);
        }

        private void SpawnForPlayerSide(float[] lanes, float[] doubleBlockCenters)
        {
            bool spawnDoubleBlock =
                doubleLaneBlockPrefab != null &&
                Random.value < doubleBlockChance;

            if (spawnDoubleBlock)
            {
                int randomIndex = Random.Range(0, doubleBlockCenters.Length);
                float xPos = doubleBlockCenters[randomIndex];

                SpawnObstacle(doubleLaneBlockPrefab, xPos);
                return;
            }

            int obstacleAmount = Random.value < twoSingleObstaclesChance ? 2 : 1;
            SpawnSingleObstacles(lanes, obstacleAmount);
        }

        private void SpawnSingleObstacles(float[] lanes, int amount)
        {
            List<float> availableLanes = new List<float>(lanes);

            for (int i = 0; i < amount; i++)
            {
                if (availableLanes.Count == 0)
                    return;

                int randomIndex = Random.Range(0, availableLanes.Count);
                float xPos = availableLanes[randomIndex];

                availableLanes.RemoveAt(randomIndex);

                GameObject prefab = GetRandomSingleObstaclePrefab();
                SpawnObstacle(prefab, xPos);
            }
        }

        private GameObject GetRandomSingleObstaclePrefab()
        {
            if (crystalPrefab != null && Random.value < crystalChance)
                return crystalPrefab;

            return ballPrefab;
        }

        private void SpawnObstacle(GameObject prefab, float xPos)
        {
            if (prefab == null)
                return;

            Vector3 spawnPosition = new Vector3(xPos, spawnY, spawnZ);
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }

        public void ApplyEasyDifficulty()
        {
            spawnInterval = 1.9f;
            doubleBlockChance = 0.15f;
            twoSingleObstaclesChance = 0.15f;
            crystalChance = 0.25f;
        }

        public void ApplyNormalDifficulty()
        {
            spawnInterval = 1.5f;
            doubleBlockChance = 0.35f;
            twoSingleObstaclesChance = 0.25f;
            crystalChance = 0.4f;
        }

        public void ApplyHardDifficulty()
        {
            spawnInterval = 1.1f;
            doubleBlockChance = 0.55f;
            twoSingleObstaclesChance = 0.45f;
            crystalChance = 0.55f;
        }
    }
}