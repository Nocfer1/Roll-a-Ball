using UnityEngine;

namespace Tron
{
    public class TronTrailSpawner : MonoBehaviour
    {
        public GameObject trailPrefab;
        public Material playerMaterial;

        // ReSharper disable Unity.PerformanceAnalysis
        public void SpawnTrailAtPosition(Vector3 worldPosition)
        {
            Vector3 spawnPosition = new Vector3(worldPosition.x, 0.5f, worldPosition.z);

            GameObject segment = Instantiate(trailPrefab, spawnPosition, Quaternion.identity);

            Renderer rend = segment.GetComponent<Renderer>();
            if (rend is not null && playerMaterial != null)
            {
                rend.material = playerMaterial;
            }
        }
    }
}