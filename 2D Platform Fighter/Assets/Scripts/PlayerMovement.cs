using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed; //speed float + editable in Unity editor due to SerializeField
    [SerializeField] private float jumpheight; // jump height float + editable in Unity editor
    [SerializeField] private int jumpcount; // amount of jumps a player can do before needing to touch the ground
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
    private void Update()
    {
        body.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, body.velocity.y); //Horizontal player movement, the velocity gets influenced by the input axis (value between -1 and 1)
        if (Input.GetKey(KeyCode.Space)) // check for player input (can be improved by not checking on every frame)
            if (jumpcount > 0) //check jump count, if above 0 the player jumps
               {
                    body.velocity = new Vector2(body.velocity.x, jumpheight); // Jump implementation
                    jumpcount = jumpcount - 1; // Decreases the jump by 1 (currently bugged due to the pressed/not pressed on update)
               }
    }
}
