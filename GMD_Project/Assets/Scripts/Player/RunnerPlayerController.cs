using UnityEngine;
using Runner;

namespace Player
{
    public class RunnerPlayerController : MonoBehaviour
    {
        public int playerID = 1;

        public float laneOffset = 2f;
        public float laneChangeSpeed = 10f;

        private int currentLane = 1;
        private float baseX;
        private float baseZ;

        private RunnerPlayerHealth health;

        private void Start()
        {
            baseX = transform.position.x;
            baseZ = transform.position.z;
            health = GetComponent<RunnerPlayerHealth>();
        }

        private void Update()
        {
            if (health is not null && health.isDead) return;

            HandleInput();
            MoveToLane();
        }

        private void HandleInput()
        {
            if (playerID == 1)
            {
                if (Input.GetKeyDown(KeyCode.A))
                    currentLane = Mathf.Max(0, currentLane - 1);

                if (Input.GetKeyDown(KeyCode.D))
                    currentLane = Mathf.Min(2, currentLane + 1);
            }
            else if (playerID == 2)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    currentLane = Mathf.Max(0, currentLane - 1);

                if (Input.GetKeyDown(KeyCode.RightArrow))
                    currentLane = Mathf.Min(2, currentLane + 1);
            }
        }

        private void MoveToLane()
        {
            float targetX = baseX + ((currentLane - 1) * laneOffset);
            Vector3 targetPosition = new Vector3(targetX, transform.position.y, baseZ);

            transform.position = Vector3.Lerp(transform.position, targetPosition, laneChangeSpeed * Time.deltaTime);
        }
    }
}