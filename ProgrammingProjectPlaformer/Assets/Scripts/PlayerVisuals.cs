using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer bodyRenderer;
    //player controller to get the players states and values to then assign it animaitons
    public PlayerController playerController;
    //sets the animation clips in the animator to something that can be modified by a script
    private readonly int idleStateHash = Animator.StringToHash("Idle");
    private readonly int walkingStateHash = Animator.StringToHash("Walking");
    private readonly int jumpingStateHash = Animator.StringToHash("Jumping");
    private readonly int wallJumpingStateHash = Animator.StringToHash("WallJumping");
    private readonly int dashingStateHash = Animator.StringToHash("Dashing");

    public void Update()
    {
        //every frame update visuals
        UpdateAnimatorState();
        UpdateSpriteDirection();
    }

    private void UpdateAnimatorState()
    {
        //refactored code 
        //change visuals based on the players state and play said animations
        if(playerController.lastState != playerController.currentState)
        {
            switch (playerController.currentState)
            {
                case PlayerController.CharacterState.Idle:
                    //calls the animator to play the different animations
                    animator.CrossFade(idleStateHash, 0);
                    break;
                case PlayerController.CharacterState.Walking:
                    animator.CrossFade(walkingStateHash, 0);
                    break;
                case PlayerController.CharacterState.Jumping:
                    animator.CrossFade(jumpingStateHash, 0);
                    break;
                case PlayerController.CharacterState.Dead:
                    break;
                case PlayerController.CharacterState.WallJumping:
                    animator.CrossFade(wallJumpingStateHash, 0);
                    break;
                case PlayerController.CharacterState.Dashing:
                    animator.CrossFade(dashingStateHash,0);
                    break;
            }
        }
    }

    private void UpdateSpriteDirection()
    {
        //runs methods from the playerController to get the return values and modifies the sprite
        //facing direction just flips the player sprite.
        switch (playerController.GetFacingDirection())
        {
            case PlayerController.FacingDirection.left:
                bodyRenderer.flipX = true;
                break;
            case PlayerController.FacingDirection.right:
                bodyRenderer.flipX = false;
                break;
        }
    }
}
