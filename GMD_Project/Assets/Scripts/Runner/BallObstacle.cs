using UnityEngine;

namespace Runner
{
    public class BallObstacle : MonoBehaviour
    {
        public float moveSpeed = 10f;
        public float destroyZ = -10f;
        public int damage = 1;

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
            RunnerPlayerHealth playerHealth = other.GetComponent<RunnerPlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}