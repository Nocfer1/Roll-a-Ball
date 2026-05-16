using UnityEngine;
using UnityEngine.InputSystem;
using Runner;

namespace PlayerFolder
{
    public class RunnerPlayerController : MonoBehaviour
    {
        public int playerID = 1;

        [Header("Lane Movement")]
        public float laneOffset = 2f;
        public float laneChangeSpeed = 10f;

        [Header("Arcade / Gamepad Input")]
        public bool singleGamepadTestMode = false;
        public float inputDeadZone = 0.5f;
        public float resetDeadZone = 0.2f;

        private int currentLane = 1;
        private float baseX;
        private float baseZ;

        private bool stickReady = true;

        private RunnerPlayerHealth health;

        private void Start()
        {
            baseX = transform.position.x;
            baseZ = transform.position.z;
            health = GetComponent<RunnerPlayerHealth>();
        }

        private void Update()
        {
            if (health != null && health.isDead)
                return;

            HandleInput();
            MoveToLane();
        }

        private void HandleInput()
        {
            HandleKeyboardInput();
            HandleGamepadInput();
        }

        private void HandleKeyboardInput()
        {
            if (playerID == 1)
            {
                if (Input.GetKeyDown(KeyCode.A))
                    MoveLeft();

                if (Input.GetKeyDown(KeyCode.D))
                    MoveRight();
            }
            else if (playerID == 2)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                    MoveLeft();

                if (Input.GetKeyDown(KeyCode.RightArrow))
                    MoveRight();
            }
        }

        private void HandleGamepadInput()
        {
            float horizontal = GetGamepadHorizontal();

            if (stickReady)
            {
                if (horizontal <= -inputDeadZone)
                {
                    MoveLeft();
                    stickReady = false;
                }
                else if (horizontal >= inputDeadZone)
                {
                    MoveRight();
                    stickReady = false;
                }
            }

            if (Mathf.Abs(horizontal) <= resetDeadZone)
            {
                stickReady = true;
            }
        }

        private float GetGamepadHorizontal()
        {
            if (Gamepad.all.Count == 0)
                return 0f;

            if (singleGamepadTestMode && Gamepad.current != null)
            {
                if (playerID == 1)
                    return Gamepad.current.leftStick.x.ReadValue();

                if (playerID == 2)
                    return Gamepad.current.rightStick.x.ReadValue();
            }

            int gamepadIndex = playerID - 1;

            if (Gamepad.all.Count > gamepadIndex)
            {
                return Gamepad.all[gamepadIndex].leftStick.x.ReadValue();
            }

            if (playerID == 1 && Gamepad.current != null)
            {
                return Gamepad.current.leftStick.x.ReadValue();
            }

            return 0f;
        }

        private void MoveLeft()
        {
            currentLane = Mathf.Max(0, currentLane - 1);
        }

        private void MoveRight()
        {
            currentLane = Mathf.Min(2, currentLane + 1);
        }

        private void MoveToLane()
        {
            float targetX = baseX + ((currentLane - 1) * laneOffset);
            Vector3 targetPosition = new Vector3(targetX, transform.position.y, baseZ);

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                laneChangeSpeed * Time.deltaTime
            );
        }
    }
}