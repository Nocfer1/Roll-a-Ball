using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerFolder
{
    public class LobbyPlayerController : MonoBehaviour
    {
        public int playerID = 1;
        public float moveSpeed = 5f;

        [Header("Input")]
        public float inputDeadZone = 0.15f;

        private CharacterController controller;
        private Animator animator;

        private void Start()
        {
            controller = GetComponent<CharacterController>();
            animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            Vector2 input = GetMovementInput();

            if (input.magnitude < inputDeadZone)
                input = Vector2.zero;

            Vector3 move = new Vector3(input.x, 0f, input.y);

            if (move.magnitude > 1f)
                move.Normalize();

            if (controller != null)
            {
                controller.Move(move * (moveSpeed * Time.deltaTime));
            }
            else
            {
                transform.Translate(move * (moveSpeed * Time.deltaTime), Space.World);
            }

            if (animator != null)
            {
                bool isRunning = move.magnitude > 0.1f;
                animator.SetBool("IsRunning", isRunning);
            }

            if (move.magnitude > 0.1f)
            {
                transform.forward = move;
            }
        }

        private Vector2 GetMovementInput()
        {
            Gamepad pad = GetGamepadForPlayer(playerID);

            if (pad != null)
                return pad.leftStick.ReadValue();

            return GetKeyboardInput();
        }

        private Vector2 GetKeyboardInput()
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

            return new Vector2(moveX, moveZ);
        }

        private Gamepad GetGamepadForPlayer(int id)
        {
            if (Gamepad.all.Count == 0)
                return null;

            if (Gamepad.all.Count >= 2)
            {
                int index = id - 1;

                if (index >= 0 && index < Gamepad.all.Count)
                    return Gamepad.all[index];

                return null;
            }

            return Gamepad.all[0];
        }
    }
}