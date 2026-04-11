using UnityEngine;

namespace Runner
{
    public class BallSpawner : MonoBehaviour
    {
        public GameObject ballPrefab;

        public float spawnInterval = 1.5f;
        public float spawnZ = 20f;
        public float spawnY = 1f;

        private float timer;

        private float[] lanePositions = { -6f, -4f, -2f, 2f, 4f, 6f };

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                SpawnBall();
                timer = 0f;
            }
        }

        private void SpawnBall()
        {
            int randomLane = Random.Range(0, lanePositions.Length);
            float xPos = lanePositions[randomLane];

            Vector3 spawnPosition = new Vector3(xPos, spawnY, spawnZ);
            Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        }
    }
}