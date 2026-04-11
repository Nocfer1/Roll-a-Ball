using UnityEngine;

namespace Tron
{
    public class TronPlayerController : MonoBehaviour
    {
        public int playerID = 1;
        public float moveSpeed = 8f;
        public float startInvulnerability = 0.4f;
        public float ownerTrailArmDelay = 0.08f;
        public float headOnKillDistance = 0.45f;

        private bool isDead = false;
        private float aliveTimer = 0f;

        private float ownerTrailTimer = 0f;
        private bool waitingToArmOwnerTrail = false;

        public LayerMask trailLayer;
        public float trailCheckRadius = 0.14f;
        public Vector3 trailCheckOffset = new Vector3(0f, -0.12f, 0f);
        
        private float ignoreOwnTrailAfterTurnTimer = 0f;
        public float ignoreOwnTrailAfterTurnDuration = 0.1f;
        
        private Vector3 currentDirection = Vector3.right;

        private TronTrailSpawner trailSpawner;

        private void Start()
        {
            trailSpawner = GetComponent<TronTrailSpawner>();

            Vector3 pos = transform.position;
            pos.y = 0.65f;
            transform.position = pos;

            currentDirection = GetCardinalDirection(transform.forward);
            transform.forward = currentDirection;

            if (trailSpawner != null)
            {
                trailSpawner.SetOwner(playerID);
                trailSpawner.BeginTrail(transform.position, currentDirection);
            }
        }

        private void Update()
        {
            if (isDead) return;

            aliveTimer += Time.deltaTime;

            HandleInputImmediate();
            MoveForward();

            if (trailSpawner != null)
            {
                trailSpawner.UpdateTrail(transform.position, currentDirection);
            }
            
            if (ignoreOwnTrailAfterTurnTimer > 0f)
            {
                ignoreOwnTrailAfterTurnTimer -= Time.deltaTime;
            }
            
            UpdateOwnerTrailArming();
            CheckHeadOnCollision();
            CheckTrailCollision();
        }

        private void HandleInputImmediate()
        {
            Vector3 desiredDirection = currentDirection;

            if (playerID == 1)
            {
                if (Input.GetKeyDown(KeyCode.W)) desiredDirection = Vector3.forward;
                if (Input.GetKeyDown(KeyCode.S)) desiredDirection = Vector3.back;
                if (Input.GetKeyDown(KeyCode.A)) desiredDirection = Vector3.left;
                if (Input.GetKeyDown(KeyCode.D)) desiredDirection = Vector3.right;
            }
            else if (playerID == 2)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow)) desiredDirection = Vector3.forward;
                if (Input.GetKeyDown(KeyCode.DownArrow)) desiredDirection = Vector3.back;
                if (Input.GetKeyDown(KeyCode.LeftArrow)) desiredDirection = Vector3.left;
                if (Input.GetKeyDown(KeyCode.RightArrow)) desiredDirection = Vector3.right;
            }

            if (desiredDirection != currentDirection && desiredDirection != -currentDirection)
            {
                currentDirection = desiredDirection;
                transform.forward = currentDirection;

                waitingToArmOwnerTrail = true;
                ownerTrailTimer = 0f;

                ignoreOwnTrailAfterTurnTimer = ignoreOwnTrailAfterTurnDuration;
            }
        }

        private void UpdateOwnerTrailArming()
        {
            if (!waitingToArmOwnerTrail || trailSpawner == null) return;

            ownerTrailTimer += Time.deltaTime;

            if (ownerTrailTimer >= ownerTrailArmDelay)
            {
                trailSpawner.ArmCurrentSegment();
                waitingToArmOwnerTrail = false;
            }
        }

        private void MoveForward()
        {
            transform.position += currentDirection * (moveSpeed * Time.deltaTime);
        }

        private void CheckHeadOnCollision()
        {
            if (aliveTimer < startInvulnerability) return;

            TronPlayerController[] players = FindObjectsByType<TronPlayerController>(FindObjectsSortMode.None);

            foreach (TronPlayerController otherPlayer in players)
            {
                if (otherPlayer == this) continue;
                if (otherPlayer.playerID == playerID) continue;
                if (otherPlayer.IsDead()) continue;

                if (DistanceXZ(transform.position, otherPlayer.transform.position) <= headOnKillDistance)
                {
                    Debug.Log("Head-on collision");
                    Die();
                    otherPlayer.Die();
                    return;
                }
            }
        }

        private void CheckTrailCollision()
        {
            if (aliveTimer < startInvulnerability) return;

            Vector3 checkPosition =
                transform.position +
                trailCheckOffset +
                (currentDirection * 0.1f);

            Collider[] hits = Physics.OverlapSphere(checkPosition, trailCheckRadius, trailLayer);

            foreach (Collider hit in hits)
            {
                TronTrailSegment trailSegment = hit.GetComponent<TronTrailSegment>();
                if (trailSegment == null) continue;

                if (trailSegment.ownerPlayerID == playerID)
                {
                    // ignorar el segmento activo actual
                    if (trailSpawner != null && trailSegment == trailSpawner.GetCurrentSegment())
                    {
                        continue;
                    }

                    // ignorar por un instante tu propio trail después de girar
                    if (ignoreOwnTrailAfterTurnTimer > 0f)
                    {
                        continue;
                    }

                    if (trailSegment.armedForOwner)
                    {
                        Die();
                        return;
                    }
                }
                else
                {
                    Die();
                    return;
                }
            }
        }
        
        private float DistanceXZ(Vector3 a, Vector3 b)
        {
            a.y = 0f;
            b.y = 0f;
            return Vector3.Distance(a, b);
        }

        private Vector3 GetCardinalDirection(Vector3 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
                return direction.x >= 0 ? Vector3.right : Vector3.left;

            return direction.z >= 0 ? Vector3.forward : Vector3.back;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isDead) return;
            if (aliveTimer < startInvulnerability) return;

            if (other.CompareTag("Obstacle"))
            {
                if (other.GetComponent<TronTrailSegment>() == null)
                {
                    Die();
                }
            }
        }

        public void Die()
        {
            if (isDead) return;

            isDead = true;

            if (trailSpawner != null)
            {
                trailSpawner.FinalizeCurrentTrail(transform.position);
            }

            TronGameManager gameManager = FindFirstObjectByType<TronGameManager>();
            if (gameManager != null)
            {
                gameManager.ReportPlayerCrash(playerID);
            }
        }

        public bool IsDead()
        {
            return isDead;
        }
    }
}