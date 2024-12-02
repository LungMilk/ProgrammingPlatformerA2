using UnityEngine;

public class PlayerVisuals : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer bodyRenderer;
    public PlayerController playerController;

    private readonly int idleStateHash = Animator.StringToHash("Idle");
    private readonly int walkingStateHash = Animator.StringToHash("Walking");
    private readonly int jumpingStateHash = Animator.StringToHash("Jumping");


    

    public void Update()
    {
        UpdateAnimatorState();
        UpdateSpriteDirection();
    }

    private void UpdateAnimatorState()
    {
        //refactored code 
        if(playerController.lastState != playerController.currentState)
        {
            switch (playerController.currentState)
            {
                case PlayerController.CharacterState.Idle:
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
            }
        }
    }

    private void UpdateSpriteDirection()
    {
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
