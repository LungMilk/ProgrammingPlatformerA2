using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;

    public float apexHeight, apexTime;
    public float jumpVelocity;
    float gravity;
    float timeInAir;

    public float terminalSpeed;

    public LayerMask myLayerMask;
    public float coyoteTime = 0.2f;
    float coyoteTimeCounter;

    bool jumping = false;
    Vector2 playerInput = new Vector2();
    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        coyoteTimeCounter = coyoteTime;
    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.
        playerInput.x = Input.GetAxis("Horizontal");
        //playerInput.y = Input.GetAxis("Vertical");
        print(coyoteTimeCounter);
        if (!IsGrounded())
        {
            timeInAir++;
            coyoteTimeCounter -= Time.deltaTime;

        }
        else if (IsGrounded()) { timeInAir = 0; coyoteTimeCounter = coyoteTime; jumping = false; }

        if (Input.GetKeyDown(KeyCode.Space) && coyoteTimeCounter > 0)
        {
            jumping = true;
        }
        MovementUpdate(playerInput);
        GetFacingDirection();
        print(IsGrounded());
        

    }

    private void MovementUpdate(Vector2 playerInput)
    {
        //removed && IsGrounded() from addForce to have the player still be able to move in the air.
        //maybe addforce isnt the way to move the player.
        if (IsWalking())
        {
            rb.AddForce(playerInput * speed);
        }
        if (jumping)
        {
            gravity = -2 * apexHeight / (Mathf.Pow(apexTime, 2));
            jumpVelocity = 2 * apexHeight / apexTime;
            rb.velocity = new Vector3(rb.velocity.x, jumpVelocity);
            
            
        }
        //print(rb.velocity.y);
        if (rb.velocity.y < 0) 
        {
            rb.velocity = new Vector2 (rb.velocity.x, Mathf.Max(rb.velocity.y, -terminalSpeed));
        }

    }

    public bool IsWalking()
    {
        if (playerInput.x != 0) { return true; }else
        return false;
    }
    public bool IsGrounded()
    {
        //it could be because of fixed update
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down,1f,myLayerMask); 
        //print(hit.point + hit.collider.gameObject.name);
        //Debug.DrawRay(transform.position, Vector2.down,Color.red,1);
        if (hit.collider != null)
        {
            return true;
        }
        else 
        { 
            
            return false;
        }
        
    }

    public FacingDirection GetFacingDirection()
    {
        if (rb.velocity.x < 0.1)
        {
            return FacingDirection.left;
        }
        else { return FacingDirection.right; }
    }
}
