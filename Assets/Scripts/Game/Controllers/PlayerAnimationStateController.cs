using UnityEngine;

public class PlayerAnimationStateController : MonoBehaviour
{
    private Animator animator;
    public void Start()
    {
        GameObject character = gameObject.transform.Find(Settings.NpcCharacter).gameObject;
        animator = character.GetComponent<Animator>();

        if (animator == null)
        {
            GameLog.LogWarning("PlayerAnimationStateController/Animator null");
        }
    }

    public void SetState(NpcState state, float walkingSpeed)
    {
        if (!animator)
        {
            return;
        }

        if (walkingSpeed == 0f)
        {
            animator.ResetTrigger(AnimatorState.Walking);
            animator.ResetTrigger(AnimatorState.WaitingAtTable);
            animator.SetTrigger(AnimatorState.Idle);
        }
        else if (walkingSpeed > 0f)
        {
            animator.ResetTrigger(AnimatorState.WaitingAtTable);
            animator.SetTrigger(AnimatorState.Walking);
            animator.ResetTrigger(AnimatorState.Idle);
        }
        else if (state == NpcState.WAITING_TO_BE_ATTENDED)
        {
            animator.ResetTrigger(AnimatorState.Idle);
            animator.ResetTrigger(AnimatorState.Walking);
            animator.SetTrigger(AnimatorState.WaitingAtTable);
        }

    }
}