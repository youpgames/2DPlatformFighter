using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed; //speed float + editable in Unity editor due to SerializeField
    [SerializeField] private float jumpheight; // jump height float + editable in Unity editor
    [SerializeField] private int jumpcount; // amount of jumps a player can do before needing to touch the ground
    private int originaljumpcount;
    private bool grounded;
    private Rigidbody2D body;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }


    // Start is called before the first frame update
    void Start()
    {
        originaljumpcount = jumpcount;
    }

    // Update is called once per frame
    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y); //Horizontal player movement, the velocity gets influenced by the input axis (value between -1 and 1)
        if (Input.GetKey(KeyCode.Space)) // check for player input (can be improved by not checking on every frame)
            if (jumpcount > 0) //check jump count, if above 0 the player jumps
                if (grounded)
                   {
                     body.velocity = new Vector2(body.velocity.x, jumpheight); // Jump implementation
                     grounded = false;
                     jumpcount = jumpcount - 1; // Decreases the jump by 1 (currently bugged due to the pressed/not pressed on update) 
                   }
    }
    private void OnCollisionEnter2D(Collision2D collision) // Check player collision with ground
        {
        if (collision.gameObject.tag == "Main Stage") // Checks if the player is colliding with the main stage
            grounded = true;
            jumpcount = originaljumpcount;
        }
}



// use keydown events and make booleans to check if the key is pressed or not, use them to determine whether the character can jump again