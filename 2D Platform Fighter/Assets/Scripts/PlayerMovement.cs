 using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed; // Speed float + editable in Unity editor due to SerializeField
    [SerializeField] private float jumpheight; // Jump height float + editable in Unity editor
    [SerializeField] private int jumpcount; // Amount of jumps a player can do before needing to touch the ground
    private bool FacingRight = true;
    private bool grounded = true;
    private float horizontal;
    private Rigidbody2D body;

    private void Awake() 
    {
        body = GetComponent<Rigidbody2D>();
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal"); // Gets a value which is either -1, 0 or 1. can be used to determine the characters movement and which way it should face

        FlipCharacter(); // Check if the character needs to be flipped
    }

    private void FixedUpdate()
    {
        body.velocity = new Vector2(Input.GetAxisRaw("Horizontal") * speed, body.velocity.y); // Horizontal player movement, the velocity gets influenced by the input axis (value between -1 and 1)
    }

    private void FlipCharacter() // Event to flip the player when it's moving opposing to the way it's facing
    {
        if (FacingRight && horizontal < 0f || !FacingRight && horizontal > 0f) 
        {
            FacingRight = !FacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f; // Turns the player around
            transform.localScale = localScale;
        }
    }
}