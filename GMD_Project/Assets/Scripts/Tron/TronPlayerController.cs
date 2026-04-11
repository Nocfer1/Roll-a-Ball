using UnityEngine;

namespace Tron
{
    public class TronPlayerController : MonoBehaviour
    {
        public int playerID = 1;
        public float cellSize = 1.2f;
        public float stepTime = 0.15f;
        private int stepsTaken = 0;
        public float startInvulnerability = 0.5f;

        private bool isDead = false;
        private float stepTimer = 0f;
        private float aliveTimer = 0f;

        private Vector3 currentDirection = Vector3.right;
        private Vector3 nextDirection = Vector3.right;

        private TronTrailSpawner trailSpawner;
        private bool hasMovedOnce = false;

        private void Start()
        {
            trailSpawner = GetComponent<TronTrailSpawner>();

            Vector3 pos = SnapToGrid(transform.position);
            pos.y = 1f;
            transform.position = pos;

            Vector3 forward = transform.forward;
            currentDirection = GetCardinalDirection(forward);
            nextDirection = currentDirection;
        }

        private void Update()
        {
            if (isDead) return;

            aliveTimer += Time.deltaTime;

            HandleInput();

            stepTimer += Time.deltaTime;

            if (stepTimer >= stepTime)
            {
                stepTimer = 0f;
                StepForward();
            }
        }

        private void HandleInput()
        {
            if (playerID == 1)
            {
                if (Input.GetKeyDown(KeyCode.W)) TrySetDirection(Vector3.forward);
                if (Input.GetKeyDown(KeyCode.S)) TrySetDirection(Vector3.back);
                if (Input.GetKeyDown(KeyCode.A)) TrySetDirection(Vector3.left);
                if (Input.GetKeyDown(KeyCode.D)) TrySetDirection(Vector3.right);
            }
            else if (playerID == 2)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) TrySetDirection(Vector3.forward);
                if (Input.GetKeyDown(KeyCode.DownArrow)) TrySetDirection(Vector3.back);
                if (Input.GetKeyDown(KeyCode.LeftArrow)) TrySetDirection(Vector3.left);
                if (Input.GetKeyDown(KeyCode.RightArrow)) TrySetDirection(Vector3.right);
            }
        }

        private void TrySetDirection(Vector3 newDirection)
        {
            if (newDirection == -currentDirection) return;
            nextDirection = newDirection;
        }

        private void StepForward()
        {
            currentDirection = nextDirection;

            Vector3 oldPosition = transform.position;
            Vector3 newPosition = transform.position + currentDirection * cellSize;

            transform.position = SnapToGrid(newPosition);
            transform.forward = currentDirection;

            stepsTaken++;

            if (stepsTaken > 2 && trailSpawner != null)
            {
                trailSpawner.SpawnTrailAtPosition(oldPosition);
            }
        }

        private Vector3 SnapToGrid(Vector3 position)
        {
            return new Vector3(
                Mathf.Round(position.x / cellSize) * cellSize,
                position.y,
                Mathf.Round(position.z / cellSize) * cellSize
            );
        }

        private Vector3 GetCardinalDirection(Vector3 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                return direction.x >= 0 ? Vector3.right : Vector3.left;
            else
                return direction.z >= 0 ? Vector3.forward : Vector3.back;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("Hit: " + other.name);

            if (isDead) return;
            if (aliveTimer < startInvulnerability) return;

            if (other.CompareTag("Obstacle"))
            {
                Die();
            }
        }

        public void Die()
        {
            isDead = true;

            TronGameManager gameManager = FindFirstObjectByType<TronGameManager>();
            if (gameManager != null)
            {
                gameManager.RegisterDeath(playerID);
            }
        }

        public bool IsDead()
        {
            return isDead;
        }
    }
}