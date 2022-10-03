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

    public void SetState(NpcState state)
    {
        if (!animator)
        {
            return;
        }

        if (state == NpcState.IDLE || state == NpcState.AT_COUNTER || state == NpcState.TAKING_ORDER || state == NpcState.REGISTERING_CASH)
        {
            animator.ResetTrigger(AnimatorState.Walking);
            animator.ResetTrigger(AnimatorState.WaitingAtTable);
            animator.SetTrigger(AnimatorState.Idle);
        }
        else if (state == NpcState.WALKING_TO_TABLE || state == NpcState.WALKING_TO_COUNTER || state == NpcState.WALKING_UNRESPAWN || state == NpcState.WALKING_TO_COUNTER_AFTER_ORDER || state == NpcState.WALKING_WANDER)
        {
            animator.ResetTrigger(AnimatorState.WaitingAtTable);
            animator.ResetTrigger(AnimatorState.Idle);
            animator.SetTrigger(AnimatorState.Walking);
        }
        else if (state == NpcState.WAITING_TO_BE_ATTENDED)
        {
            animator.ResetTrigger(AnimatorState.Idle);
            animator.ResetTrigger(AnimatorState.Walking);
            animator.SetTrigger(AnimatorState.WaitingAtTable);
        }
        else
        {
            animator.ResetTrigger(AnimatorState.WaitingAtTable);
            animator.ResetTrigger(AnimatorState.Walking);
            animator.SetTrigger(AnimatorState.Idle);
        }
    }
}