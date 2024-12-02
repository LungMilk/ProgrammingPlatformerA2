using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public enum FacingDirection
    {
        left, right
    }
    public enum CharacterState
    {
        Walking,Jumping,Idle,Dead
    }

    public CharacterState currentState = CharacterState.Idle;
    public CharacterState lastState = CharacterState.Idle;

    private FacingDirection currentDirection;
    //going off of the logic explained in class for base movement.
    [SerializeField] private Rigidbody2D rb;

    [Header("Horizontal Movement")]
    public float maxSpeed = 5f;
    public float accelTime = 1f;
    public float decelTime = 0.75f;

    //headers are cool
    [Header("vertical movement")]
    private float accelRate;
    private float decelRate;

    private Vector2 velocity;

    public float apexHeight =3f;
    public float apexTime = 0.5f;

    private float gravity;
    private float jumpVelocity;

    //flushing out bool states
    private bool isGrounded = false;
    private bool isWalking = false;
    private bool isDead = false;

    [Header("Ground check")]
    public float groundCheckOffset = 0.5f;
    public Vector3 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();

        rb.gravityScale = 0;
        accelRate = maxSpeed / accelTime;
        decelRate = maxSpeed / decelTime;

        gravity = -2 * apexHeight / (apexTime * apexTime);
        jumpVelocity = 2 * apexHeight / apexTime;

    }

    // Update is called once per frame
    void Update()
    {
        // The input from the player needs to be determined and
        // then passed in the to the MovementUpdate which should
        // manage the actual movement of the character.

        lastState = currentState;
        //probs want to check if dad before doing anything
        CheckForGround();

        //digital values instead of simulated joystick movement
        Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxisRaw("Horizontal");

        UpdateCharacterState();

        MovementUpdate(playerInput);
        JumpUpdate();
        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y = 0;

        //getting an error with player assignment
        rb.velocity = velocity;

    }

    private void UpdateCharacterState()
    {
        switch (currentState)
        {
            case CharacterState.Idle:
                if (!isGrounded)
                    currentState = CharacterState.Jumping;
                else if (isWalking)
                    currentState = CharacterState.Walking;
                break;
            case CharacterState.Walking:
                if (!isGrounded)
                    currentState = CharacterState.Jumping;
                else if (!isWalking)
                    currentState = CharacterState.Idle;
                break;
            case CharacterState.Jumping:
                if (isGrounded)
                {
                    if (isWalking)
                        currentState = CharacterState.Walking;
                    else
                        currentState = CharacterState.Idle;
                }
                break;

            case CharacterState.Dead:
                //do nothing
                break;
        }
    }

    void CheckForGround()
    {
        //had to invert teh jumping animations to change properly when on gorund or not
        isGrounded = Physics2D.OverlapBox(transform.position + Vector3.down * groundCheckOffset,groundCheckSize,0,groundCheckMask);

    }
    public void OnDrawGizmos()
    {
        //just want to see the cube spawn;
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundCheckOffset, groundCheckSize);
    }
    private void MovementUpdate(Vector2 playerInput)
    {
        UpdateFacingDirection(playerInput);
        //for quick turn maybe conserve speed value and simply apply it in teh opposing direction
        if (playerInput.x != 0)
        {
            velocity.x += accelRate * Time.deltaTime * playerInput.x;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeed, maxSpeed);
        }
        else
        {
            if (velocity.x > 0)
            {
                velocity.x -= decelRate * Time.deltaTime;
                velocity.x = Mathf.Max(velocity.x, 0);
            }
            if (velocity.x < 0)
            {
                velocity.x += decelRate * Time.deltaTime;
                velocity.x = Mathf.Min(velocity.x, 0);
            }
        }
        isWalking = velocity.x != 0;
    }

    void JumpUpdate()
    {
        if (isGrounded && Input.GetButton("Jump"))
        {
            velocity.y = jumpVelocity;
            isGrounded = false;
        }
    }

    public void UpdateFacingDirection(Vector2 playerInput)
    {
        if (playerInput.x < 0f)
        {
            currentDirection = FacingDirection.left;
        }
        else if (playerInput.x > 0f)
        {
            currentDirection = FacingDirection.right;
        }
    }

    public bool IsWalking()
    {
        return velocity.x != 0;
    }
    public bool IsGrounded()
    {
        return isGrounded;

    }
    public FacingDirection GetFacingDirection()
    {
        return currentDirection;
    }
}
