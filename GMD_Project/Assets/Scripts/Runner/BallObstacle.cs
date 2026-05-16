using UnityEngine;

namespace Runner
{
    public class BallObstacle : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public float destroyZ = -10f;
        public int damage = 1;

        private bool alreadyHit = false;

        private void Update()
        {
            transform.Translate(Vector3.back * (moveSpeed * Time.deltaTime), Space.World);

            if (transform.position.z < destroyZ)
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (alreadyHit) return;

            RunnerPlayerHealth playerHealth = other.GetComponentInParent<RunnerPlayerHealth>();

            if (playerHealth != null)
            {
                alreadyHit = true;

                playerHealth.TakeDamage(damage);

                Collider[] colliders = GetComponentsInChildren<Collider>();
                foreach (Collider col in colliders)
                {
                    col.enabled = false;
                }

                Destroy(gameObject);
            }
        }
    }
}