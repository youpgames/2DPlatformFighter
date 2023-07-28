using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInputActions inputActions;

    private Rigidbody2D rb;
    
    [SerializeField, Tooltip("Distance between the two ground checking colliders")]
    private Vector3 colliderOffset;
    [SerializeField,Tooltip("Length of the ground checking collider")]
    private float groundLength = 0.95f;
    [SerializeField, Tooltip("Items having this layer will be considered ground")] 
    private LayerMask groundLayer;
    
    [SerializeField, Tooltip("How much the player can jump")] 
    private float jumpHeight = 500f;
    [SerializeField, Tooltip("How many times the player can jump before having to land")] 
    private int maxJumpCount = 3;
    
    [SerializeField, Tooltip("Gravity applied normally during ascent")] 
    private float defaultGravity = 1f;
    [SerializeField, Tooltip("Gravity applied while the player is falling")] 
    private float fallGravity = 2f;
    [SerializeField, Tooltip("Max falling velocity")] 
    private float terminalVel = 100f;
    

    [SerializeField,Tooltip("Movement speed of the player")]
    private float moveSpeed = 5f;


    // How many times the player has currently jumped
    private int curJumpCount;
    // Is the player on the ground or not
    [SerializeField]
    private bool onGround;
    // If the player was on the ground the previous frame
    private bool wasOnGround;

    private void Awake()
    {
        // Reference to the player input component
        playerInput = GetComponent<PlayerInput>();
        // Switch to the gameplay action map
        playerInput.SwitchCurrentActionMap("Gameplay");
        
        // Create a new set of input actions and enable it
        inputActions = new PlayerInputActions();
        inputActions.Gameplay.Enable();
        
        // Assign functions to the input bindings
        inputActions.Gameplay.Jump.performed += Jump;

        // Reference to the rigidbody component
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        // Fire two raycasts for both "feet" locations to check for the ground
        onGround = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer) ||
                   Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer);
        
        // Check if the character was previously not on the ground and now is on the ground
        if (!wasOnGround && onGround)
        {
            // Call the Land function 
            Landed();
        }

        // Update the previous ground state for the next frame
        wasOnGround = onGround;
    }

    private void FixedUpdate()
    {
        // Handles sideways movement
        rb.velocity = new Vector2(inputActions.Gameplay.Move.ReadValue<float>() * moveSpeed,rb.velocity.y);
        if (rb.velocity.y > terminalVel)
        {
            rb.velocity = new Vector2(rb.velocity.x,terminalVel);
        }
        // Change gravity scale based on if the player is falling or ascending
        rb.gravityScale = rb.velocity.y < -1f ? fallGravity : defaultGravity;
    }
    
    private void Jump(InputAction.CallbackContext obj)
    {
        // Check if player has not jump more than allowed to
        if (curJumpCount < maxJumpCount)
        {
            curJumpCount++;
            var jumpForce = new Vector2(0f,jumpHeight - rb.velocity.y);
            rb.AddForce(jumpForce,ForceMode2D.Impulse);
        }
    }

    private bool IsFalling()
    {
        return !onGround;
    }
    
    private void Landed()
    {
        // Reset jump count on landing
        curJumpCount = 0;
    }
}
