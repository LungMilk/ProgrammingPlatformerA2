using System.Collections;
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
        Walking,Jumping,Idle,Dead, WallJumping,Dashing
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

    public float turnSpeed = 0f;

    public float dashVelocity;
    public float dashTime;

    private Vector2 dashDirection;
    private bool isDashing;
    private bool isWallJumping = false;
    private bool canDash = true;


    //headers are cool
    [Header("vertical movement")]
    private float accelRate;
    private float decelRate;

    private Vector2 velocity;
    Vector2 lastPlayerInput;

    public float apexHeight =3f;
    public float apexTime = 0.5f;

    private float gravity;
    private float jumpVelocity;

    //flushing out bool states
    private bool isGrounded = false;
    private bool isWalking = false;
    private bool isDead = false;
    private bool isTouchingRightWall = false;
    private bool isTouchingLeftWall = false;

    [Header("Ground check")]
    public float groundCheckOffset = 0.5f;
    public Vector3 groundCheckSize = new(0.4f, 0.1f);
    public LayerMask groundCheckMask;

    public float wallCheckOffset = 0.5f;
    public Vector3 wallCheckSize = new(0.4f, 0.1f);
    public LayerMask wallCheckMask;


    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        //disables gravity so it can be manipulated for gameplay feel
        rb.gravityScale = 0;
        //setting base physics values for movement and player physics
        accelRate = maxSpeed / accelTime;
        decelRate = maxSpeed / decelTime;
        //setting gravity of the player and their jump velocity so it can be changed and tweaked
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
        //probs want to check if dead before doing anything
        //starts each frame with the detection of walls and ground to determine state
        CheckForGround();
        CheckForWalls();
        UpdateCharacterState();

        //player input is saved so it changes everyframe and applied each frame
        //digital values instead of simulated joystick movement
        Vector2 playerInput = new Vector2();
        playerInput.x = Input.GetAxisRaw("Horizontal");
        //movement update is all physics placed on the player
        MovementUpdate(playerInput);
        //records the last placer input used for turning
        if (playerInput != Vector2.zero)
        {
            lastPlayerInput = playerInput;
        }
        //sets the player state to dashing if they are elligable to dash
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isGrounded)
        {
            //print("dashing");
            isDashing = true;
            canDash = false;
            //dash direction records all player input for direciton to be applied over time 
            dashDirection = new Vector2(playerInput.x, Input.GetAxisRaw("Vertical"));
            StartCoroutine(StopDashing());
        }
        //jumpUpdate updates the players vertical position after all horizontal, physics and state logic that might impact it
        JumpUpdate();
        //AirDash(playerInput);
        //fallspeed clamps and falling physics
        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;
        else
            velocity.y = 0;
            canDash = true ;
        //landing state
        if (velocity.y == 0)
        {
            print("landed");
            isWallJumping = false;
        }
        //every frame updates the players physics
        rb.velocity = velocity;
        print(currentState);
    }

    private void UpdateCharacterState()
    {
        //state machine managing the players state and what states flow into one another
        switch (currentState)
        {
            case CharacterState.Idle:
                if (!isGrounded)
                    currentState = CharacterState.Jumping;
                else if (!isGrounded && (!isGrounded && isWallJumping))
                    currentState = CharacterState.WallJumping;
                else if (isWalking)
                    currentState = CharacterState.Walking;
                break;
            case CharacterState.Walking:
                if (!isGrounded)
                    currentState = CharacterState.Jumping;
                else if (!isWalking)
                    currentState = CharacterState.Idle;
                else if (isDashing)
                    currentState = CharacterState.Dashing;
                break;
            case CharacterState.Jumping:
                if (!isGrounded && isWallJumping)
                    currentState = CharacterState.WallJumping;
                else if (isDashing)
                {
                    currentState = CharacterState.Dashing;
                    print("dashing");
                }
                else if (isGrounded)
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
            case CharacterState.WallJumping:
                if (isGrounded)
                {
                    currentState = CharacterState.Idle;
                }
                else if (isDashing)
                {
                    currentState = CharacterState.Dashing;
                    print("dashing");
                }
                break;
            case CharacterState.Dashing:
                if (isGrounded)
                {
                    currentState = CharacterState.Idle;
                }
                break;
        }
    }

    void CheckForWalls()
    {
        //physics boxcasts that determine collisions with other colliders
        //right is default
        isTouchingRightWall = Physics2D.OverlapBox(transform.position + Vector3.right * wallCheckOffset,wallCheckSize,0,wallCheckMask);
        isTouchingLeftWall = Physics2D.OverlapBox(transform.position + Vector3.right * -wallCheckOffset, wallCheckSize, 0, wallCheckMask);
    }

    void CheckForGround()
    {
        //same as walls but for ground only and underneath player
        //had to invert the jumping animations to change properly when on gorund or not
        isGrounded = Physics2D.OverlapBox(transform.position + Vector3.down * groundCheckOffset,groundCheckSize,0,groundCheckMask);

    }
    public void OnDrawGizmos()
    {
        //draws the cubes in scene view so I can see what they are and are not touching
        Gizmos.DrawWireCube(transform.position + Vector3.down * groundCheckOffset, groundCheckSize);
        Gizmos.DrawWireCube(transform.position + Vector3.right * wallCheckOffset, wallCheckSize);
        Gizmos.DrawWireCube(transform.position + Vector3.right * -wallCheckOffset, wallCheckSize);
    }
    private void MovementUpdate(Vector2 playerInput)
    {
        //TurningAccel(playerInput);
        //player sprite direciton is updated to move with the input.
        UpdateFacingDirection(playerInput);

        //for quick turn maybe conserve speed value and simply apply it in teh opposing direction

        //detects if the player should be moving and applies acceleration to a vector which is then added to the players velocity
        if (playerInput.x != 0)
        {
            //turnspeed 
            velocity.x += accelRate * Time.deltaTime * playerInput.x * turnSpeed;
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

        if (isDashing)
        {
            //print("dashed");
            //so the problem is with the velocity being modified
            velocity = dashDirection.normalized * dashVelocity;
        }
    }

    void JumpUpdate()
    {
        if ((isGrounded || (isTouchingRightWall || isTouchingLeftWall)) && Input.GetButton("Jump"))
        {
            //determines wall jumping state if jumping when touching a wall
            if ((isTouchingLeftWall || isTouchingRightWall))
            {
                isWallJumping = true;
            }
            //wall jump is the same as normal jump just different conditionals
            velocity.y = jumpVelocity;
            isGrounded = false;
            
            //everytime im running this update the player is touching the wall and 
        }
    }
    IEnumerator StopDashing()
    {
        //co routine is determineing the length of time in which the player is able to dash
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
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
