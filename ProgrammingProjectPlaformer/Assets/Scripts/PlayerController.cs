using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed;

    public LayerMask myLayerMask;

    Vector2 playerInput = new Vector2();
    public enum FacingDirection
    {
        left, right
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.
        playerInput.x = Input.GetAxis("Horizontal");
        //playerInput.y = Input.GetAxis("Vertical");
        MovementUpdate(playerInput);
        GetFacingDirection();

        IsGrounded();

    }

    private void MovementUpdate(Vector2 playerInput)
    {
        if (IsWalking()) 
        { 
            rb.AddForce(playerInput * speed);
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
        print(hit.point + hit.collider.gameObject.name);
        Debug.DrawRay(transform.position, Vector2.down,Color.red,1);
        if (hit.collider != null)
        {
            print("on gorund");
            return true;
        }
        else 
        { 
            print("not on ground"); 
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
