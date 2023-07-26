 using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public float speed; // Speed float + editable in Unity editor due to SerializeField

    [SerializeField] public float jump;
    [SerializeField] public int jumpcount; // Amount of jumps a player can do before needing to touch the ground

    public bool FacingRight = true;

    private bool Grounded;
    private float MoveHorizontal;

    public Rigidbody2D Rigidbody; // Rigidbody2D is now referenced to as rb

    private void Awake() 
    {
        Application.targetFrameRate = 60; // Put in the game start-up script once it gets created
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        MoveHorizontal = Input.GetAxis("Horizontal"); // Gets a value which is either -1, 0 or 1. can be used to determine the characters movement and which way the character should face

        FlipCharacter(); // Check if the character needs to be flipped

        Rigidbody.velocity = new Vector2(MoveHorizontal * speed, Rigidbody.velocity.y); // Horizontal player movement, the velocity gets influenced by the input axis (value between -1 and 1)

        if (Input.GetButtonDown("Jump") && jumpcount >0)
        { 
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0); // Sets y velocity to 0 before jumping to make jumpheight consistent
            Rigidbody.AddForce(new Vector2(Rigidbody.velocity.x, jump));
            jumpcount --;
        }
    }

    private void FixedUpdate()
    {
    }

    private void FlipCharacter() // Event to flip the player when it's moving opposing to the way it's facing 
    {
        if (FacingRight && MoveHorizontal < 0f || !FacingRight && MoveHorizontal > 0f) 
        {
            FacingRight = !FacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f; // Turns the player around
            transform.localScale = localScale;
        }
    }


    // The floor collision detection method below should be changed by a Raycast collision detection
    // The game detects if the player collides with the Floor (object with "Floor" Tag) if so; the game sets Grounded = true;
    private void OnCollisionEnter2D(Collision2D other) // Sets grounded true when starts colliding
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            Grounded = true;
            jumpcount = 3;
        }
    }

    private void OnCollisionExit2D(Collision2D other) // Sets grounded false when stops colliding
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            Grounded = false;
        }
    }
} 
