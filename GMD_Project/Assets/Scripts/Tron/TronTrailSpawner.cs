using UnityEngine;

namespace Tron
{
    public class TronTrailSpawner : MonoBehaviour
    {
        public GameObject trailPrefab;
        public Material playerMaterial;
        public float trailThickness = 0.4f;

        private TronTrailSegment currentSegment;
        private Vector3 lastDirection;
        private int ownerPlayerID;

        private Transform trailContainer;

        public void SetOwner(int playerID)
        {
            ownerPlayerID = playerID;
        }

        public void SetTrailContainer(Transform container)
        {
            trailContainer = container;
            Debug.Log("TrailContainer asignado a: " + (trailContainer != null ? trailContainer.name : "NULL"));
        }

        public void BeginTrail(Vector3 startPosition, Vector3 direction)
        {
            lastDirection = direction;
            CreateNewSegment(startPosition, direction);
        }

        public void UpdateTrail(Vector3 currentPosition, Vector3 currentDirection)
        {
            if (currentSegment == null)
            {
                BeginTrail(currentPosition, currentDirection);
                return;
            }

            if (currentDirection != lastDirection)
            {
                currentSegment.UpdateEndPoint(currentPosition);
                currentSegment.ArmForOwner();

                lastDirection = currentDirection;
                CreateNewSegment(currentPosition, currentDirection);
            }
            else
            {
                currentSegment.UpdateEndPoint(currentPosition);
            }
        }

        public void ArmCurrentSegment()
        {
            if (currentSegment != null)
            {
                currentSegment.ArmForOwner();
            }
        }

        public void FinalizeCurrentTrail(Vector3 currentPosition)
        {
            if (currentSegment == null) return;

            currentSegment.UpdateEndPoint(currentPosition);
            currentSegment.ArmForOwner();
        }

        public TronTrailSegment GetCurrentSegment()
        {
            return currentSegment;
        }

        private void CreateNewSegment(Vector3 startPosition, Vector3 direction)
        {
            GameObject segmentObj = Instantiate(trailPrefab);

            if (trailContainer != null)
            {
                segmentObj.transform.SetParent(trailContainer, true);
            }

            currentSegment = segmentObj.GetComponent<TronTrailSegment>();

            if (currentSegment != null)
            {
                currentSegment.Initialize(
                    startPosition,
                    direction,
                    playerMaterial,
                    trailThickness,
                    ownerPlayerID
                );
                Debug.Log("Creando trail. Parent: " + (trailContainer != null ? trailContainer.name : "NULL"));
            }
        }
    }
}