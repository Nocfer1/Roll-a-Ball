using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // Rigidbody of the player.
    private Rigidbody rb; 
    private Collider playerCollider;
    private int count;

    // Movement along X and Y axes.
    private float movementX;
    private float movementY;
    private bool isJumpHeld;
    private float jumpHoldTimer;
    private bool jumpQueued;

    // Speed at which the player moves.
    public float speed = 0; 
    public float minJumpForce = 5f;
    public float maxJumpForce = 12f;
    public float maxJumpHoldTime = 0.6f;
    public float jumpDirectionBoost = 4f;
    [Header("Jump UI")]
    public Slider jumpBar;
    public bool showOnlyWhileCharging = true;
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    // Start is called before the first frame update.
    void Start()
    {
        // Get and store the Rigidbody component attached to the player.
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        count = 0;
        SetCountText();
        winTextObject.SetActive(false);
    }
 
    // This function is called when a move input is detected.
    void OnMove(InputValue movementValue)
    {
        // Convert the input value into a Vector2 for movement.
        Vector2 movementVector = movementValue.Get<Vector2>();

        // Store the X and Y components of the movement.
        movementX = movementVector.x; 
        movementY = movementVector.y; 
    }

    void OnJump(InputValue v)
    {
        Debug.Log($"OnJump called. isPressed={v.isPressed}");
        if (v.isPressed)
        {
            if (IsGrounded())
            {
                isJumpHeld = true;
                jumpHoldTimer = 0f;
            }
        }
        else
        {
            if (isJumpHeld)
                jumpQueued = true;

            isJumpHeld = false;
        }
    }

    private void Update()
    {
        if (isJumpHeld)
        {
            jumpHoldTimer += Time.deltaTime;
            jumpHoldTimer = Mathf.Min(jumpHoldTimer, maxJumpHoldTime);
        }

        if (jumpBar is not null)
        {
            jumpBar.value = Mathf.Clamp01(jumpHoldTimer / maxJumpHoldTime);

            if (showOnlyWhileCharging)
                jumpBar.gameObject.SetActive(isJumpHeld && IsGrounded());
        }
    }
    
    // FixedUpdate is called once per fixed frame-rate frame.
    private void FixedUpdate() 
    {
        // Create a 3D movement vector using the X and Y inputs.
        Vector3 movement = new Vector3 (movementX, 0.0f, movementY);

        if (jumpQueued)
        {
            PerformChargedJump();
            jumpQueued = false;
        }

        // Apply force to the Rigidbody to move the player.
        rb.AddForce(movement * speed); 
    }

    private void PerformChargedJump()
    {
        float charge01 = Mathf.Clamp01(jumpHoldTimer / maxJumpHoldTime);
        float jumpForce = Mathf.Lerp(minJumpForce, maxJumpForce, charge01);

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        Vector3 moveDirection = new Vector3(movementX, 0f, movementY);
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            moveDirection.Normalize();
        }
        else
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            moveDirection = horizontalVelocity.sqrMagnitude > 0.001f ? horizontalVelocity.normalized : Vector3.zero;
        }

        if (moveDirection != Vector3.zero)
        {
            rb.AddForce(moveDirection * (jumpDirectionBoost * (1f + charge01)), ForceMode.Impulse);
        }

        jumpHoldTimer = 0f;
        if (jumpBar is not null)
        {
            jumpBar.value = 0f;
            if (showOnlyWhileCharging) jumpBar.gameObject.SetActive(false);
        }
    }

    private bool IsGrounded()
    {
        float radius = 0.25f;
        float extra = 0.15f;

        float dist = (playerCollider is not null ? playerCollider.bounds.extents.y : 0.5f) + extra;
        return Physics.SphereCast(transform.position, radius, Vector3.down, out _, dist);
    }
    
    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("PickUp")) 
        {
            other.gameObject.SetActive(false);
            count = count + 1;
            SetCountText();
        }
    }
    
    void SetCountText() 
    {
        countText.text =  "Count: " + count.ToString();
        if (count >= 8)
        {
            winTextObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));
        }
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Destroy the current object
            Destroy(gameObject); 
            // Update the winText to display "You Lose!"
            winTextObject.gameObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        }
    }
}