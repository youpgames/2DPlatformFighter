using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] 
    private float speed; // Speed float + editable in Unity editor due to SerializeField
    private float MoveHorizontal;

    [SerializeField] public float jump;
    [SerializeField] public int jumpcount; // Amount of jumps a player can do before needing to touch the ground

    [SerializeField] public float WalljumpHorizontalVelocity;
    [SerializeField] public float WalljumpVerticalVelocity;

    private bool canDash = true; // Checks if the player is allowed to dash
    [SerializeField] public float DashVelocity; //How fast the player dashes
    [SerializeField] public float DashTime; //How long the dash lasts
    [SerializeField] public float DashCooldown; // How long the player waits until the player can dash again

    private bool FacingRight = true;

    private bool Grounded;
    private bool onWall;
    private bool Dashing;
    

    public Rigidbody2D Rigidbody; // Rigidbody2D is now referenced to as rb

    private void Awake()
    {
        
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

        if (Input.GetButtonDown("Jump") && jumpcount > 0)
        {
            if (Grounded)
            {
                EndDashIfDashing();
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0); // Sets y velocity to 0 before jumping to make jump height consistent
                Rigidbody.AddForce(new Vector2(Rigidbody.velocity.x, jump));
                jumpcount--;
            }
            else if (onWall)
            {
                Rigidbody.velocity = new Vector2(0, 0);
                Rigidbody.AddForce(new Vector2(-MoveHorizontal * WalljumpHorizontalVelocity, WalljumpVerticalVelocity)); // Change this for a raycast collision that tells the script on what side the wall is, add a slight input delay when jumping off the wall
                jumpcount--;
            }
            else
            {
                Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 0); // Sets y velocity to 0 before jumping to make jump height consistent
                Rigidbody.AddForce(new Vector2(Rigidbody.velocity.x, jump));
                jumpcount--;
            }

        }


        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && Grounded) // if player is grounded, can dash and pressed left shift the dash enumeration starts
        {
            StartCoroutine(Dash());
        }
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
        else if (other.gameObject.CompareTag("Wall"))
        {
            onWall=true;
            jumpcount = 2;
        }
    }

    private void OnCollisionExit2D(Collision2D other) // Sets grounded false when stops colliding
    {
        if (other.gameObject.CompareTag("Floor"))
        {
            Grounded = false;
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            onWall = false;
        }
    }

    private IEnumerator Dash()
    {
        if (canDash)
        {
            canDash = false;
            Dashing = true;
            Rigidbody.velocity = Vector2.zero;
            Rigidbody.AddForce(new Vector2(transform.localScale.x * DashVelocity, 0f), ForceMode2D.Impulse);

            yield return new WaitForSeconds(DashTime);

            // Check if the player is still dashing (hasn't jumped) before ending the dash.
            if (Dashing)
            {
                Rigidbody.velocity = Vector2.zero;
                Dashing = false;
                yield return new WaitForSeconds(DashCooldown);
                canDash = true;
            }
        }
    }
    
    // Call this function to end the dash prematurely.
    private void EndDashIfDashing()
    {
        if (Dashing)
        {
            StopCoroutine(Dash());
            Rigidbody.velocity = Vector2.zero;
            Dashing = false;
            canDash = true;
        }
    }
}
