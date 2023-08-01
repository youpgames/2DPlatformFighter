using UnityEngine;
using UnityEngine.InputSystem;


enum EMovementStates
{
    StateWalking,
    StateFalling,
    StateLanded,
    StateWallSliding,
    StateDodging,
    StateDashing
}

[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInputActions inputActions;

    private Rigidbody2D rb;

    [Header("Ground")]
    [SerializeField, Tooltip("Distance between the two ground checking colliders")]
    private float groundOffset = 0.4f;
    [SerializeField, Tooltip("How far on the sides to check for walls")]
    private float wallLength = 0.75f;
    [SerializeField,Tooltip("Length of the ground checking collider")]
    private float groundLength = 0.75f;
    [SerializeField, Tooltip("Items having this layer will be considered ground")] 
    private LayerMask groundLayer;
    
    [Space]
    [Header("Jump")]
    [SerializeField, Tooltip("How much the player can jump")] 
    private float jumpHeight = 10f;
    [SerializeField] 
    private float wallJumpForce = 10f;
    [SerializeField, Tooltip("How many times the player can jump before having to land")] 
    private int maxJumpCount = 3;

    [Space]
    [Header("Gravity")]
    [SerializeField, Tooltip("Movement control while in the air")]
    private float airControl = 1f;
    [SerializeField, Tooltip("Gravity applied normally during ascent")] 
    private float defaultGravity = 2f;
    [SerializeField, Tooltip("Gravity applied while the player is falling")] 
    private float fallGravity = 10f;
    [SerializeField, Tooltip("Gravity applied while the player is sliding on walls")] 
    private float wallGravity = 2f;
    [SerializeField, Tooltip("Max falling velocity")] 
    private float terminalVel = 100f;
    
    [Space]
    [Header("Movement")]
    [SerializeField,Tooltip("Movement speed of the player")]
    private float maxWalkSpeed = 5f;


    // How many times the player has currently jumped
    private int curJumpCount;
    // Is the player on the ground or not
    private bool landed;
    // Is the player currently wall jumping
    private bool wallJump;
    // Stores the normal of the wall the player is on
    private Vector2 wallNormal;

    // current movement state of the player
    [SerializeField]
    private EMovementStates curMovState;


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

        rb = GetComponent<Rigidbody2D>();
    }
    
    private void Update()
    {
        // Check if the character was previously not on the ground and now is on the ground
        if (GroundCheck())
        {
            if (!landed)
            {
                // Set movement state to landed
                SetMovementState(EMovementStates.StateLanded);
            }
        }
        else
        {
            // Set movement state to wall sliding if on a wall and falling if not
            SetMovementState(WallCheck() ? EMovementStates.StateWallSliding : EMovementStates.StateFalling);
        }
        
    }

    private void FixedUpdate()
    {
        Move(inputActions.Gameplay.Move.ReadValue<float>());
        HandleGravity();
    }

    private EMovementStates GetMovementState()
    {
        return curMovState;
    }
    
    private void SetMovementState(EMovementStates newState)
    {
        if (curMovState == newState) return;
        switch (newState)
        {
            case EMovementStates.StateWalking:
                break;
            case EMovementStates.StateFalling:
                landed = false;
                break;
            case EMovementStates.StateWallSliding:
                curJumpCount = 0;
                break;
            case EMovementStates.StateDodging:
                break;
            case EMovementStates.StateDashing:
                break;
            case EMovementStates.StateLanded:
                Landed();
                break;
        }
        curMovState = newState;
    }

    private void Move(float scale)
    {
        var control = IsFalling() ? airControl : IsOnWall() ? 0f : 1f;
        // Handles sideways movement
        if (wallJump)
        {
            if (!IsOnWall() && scale != 0f)
            {
                TurnOffWallJump();
            }
        }
        else
        {
            rb.velocity = new Vector2(scale * maxWalkSpeed * control, rb.velocity.y);
        }
    }

    private bool IsFalling()
    {
        return GetMovementState() == EMovementStates.StateFalling;
    }

    private bool IsOnWall()
    {
        return GetMovementState() == EMovementStates.StateWallSliding;
    }
    
    private void Landed()
    {
        // Reset jump count on landing
        curJumpCount = 0;
        landed = true;
        TurnOffWallJump();
        SetMovementState(EMovementStates.StateWalking);
    }

    private bool GroundCheck()
    {
        Vector2 pos = transform.position;
        // Fire two raycasts for both "feet" locations to check for the ground
        return Physics2D.Raycast(pos + (Vector2.right * groundOffset), Vector2.down, groundLength, groundLayer) ||
               Physics2D.Raycast(pos - (Vector2.right * groundOffset), Vector2.down, groundLength, groundLayer);
    }

    private RaycastHit2D WallCheck()
    {
        Vector2 pos = transform.position;
        var leftHit = Physics2D.Raycast(pos, Vector2.right, wallLength, groundLayer);
        var rightHit = Physics2D.Raycast(pos, -Vector2.right, wallLength, groundLayer);
        wallNormal = leftHit ? leftHit.normal : rightHit.normal;
        return leftHit ? leftHit : rightHit;
    }

    private void HandleGravity()
    {
        if (!IsOnWall())
        {
            // Cap falling velocity to the terminal velocity
            if (rb.velocity.y > terminalVel)
            {
                rb.velocity = new Vector2(rb.velocity.x,terminalVel);
            }
            // Change gravity scale based on if the player is falling or ascending
            rb.gravityScale = rb.velocity.y < -1f ? fallGravity : defaultGravity;
        }
        else
        {
            rb.gravityScale = wallGravity;
        }
    }
    
    private void Jump(InputAction.CallbackContext obj)
    {
        // Check if player has not jump more than allowed to
        if (curJumpCount >= maxJumpCount) return;
        curJumpCount++;
        float sideJump;
        if (IsOnWall())
        {
            sideJump = wallNormal.x * wallJumpForce;
            wallJump = true;
        }
        else
        {
            sideJump = 0f;
        }
        // Launch the character with the desired jump height cancelling out any falling velocity
        var jumpForce = new Vector2(sideJump,jumpHeight - rb.velocity.y);
        rb.AddForce(jumpForce,ForceMode2D.Impulse);
    }

    private void TurnOffWallJump()
    {
        wallJump = false;
    }
    
    
}
