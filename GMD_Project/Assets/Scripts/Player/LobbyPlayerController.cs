using UnityEngine;

namespace Player
{
    public class LobbyPlayerController : MonoBehaviour
    {
        public int playerID = 1;
        public float moveSpeed = 5f;

        private CharacterController controller;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            float moveX = 0f;
            float moveZ = 0f;

            if (playerID == 1)
            {
                if (Input.GetKey(KeyCode.A)) moveX = -1f;
                if (Input.GetKey(KeyCode.D)) moveX = 1f;
                if (Input.GetKey(KeyCode.W)) moveZ = 1f;
                if (Input.GetKey(KeyCode.S)) moveZ = -1f;
            }
            else if (playerID == 2)
            {
                if (Input.GetKey(KeyCode.LeftArrow)) moveX = -1f;
                if (Input.GetKey(KeyCode.RightArrow)) moveX = 1f;
                if (Input.GetKey(KeyCode.UpArrow)) moveZ = 1f;
                if (Input.GetKey(KeyCode.DownArrow)) moveZ = -1f;
            }

            Vector3 move = new Vector3(moveX, 0f, moveZ).normalized;

            controller.Move(move * (moveSpeed * Time.deltaTime));
        }
    }
}