using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed; //speed float + editable in Unity editor
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
    }
}
