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

        if (state == NpcState.IDLE_0 || state == NpcState.AT_COUNTER_4 || state == NpcState.TAKING_ORDER_6 || state == NpcState.REGISTERING_CASH_10)
        {
            animator.ResetTrigger(AnimatorState.Walking);
            animator.ResetTrigger(AnimatorState.WaitingAtTable);
            animator.SetTrigger(AnimatorState.Idle);
        }
        else if (state == NpcState.WALKING_TO_TABLE_1 || state == NpcState.WALKING_TO_COUNTER_3 || state == NpcState.WALKING_UNRESPAWN_8 || state == NpcState.WALKING_TO_COUNTER_AFTER_ORDER_9 || state == NpcState.WALKING_WANDER_5)
        {
            animator.ResetTrigger(AnimatorState.WaitingAtTable);
            animator.ResetTrigger(AnimatorState.Idle);
            animator.SetTrigger(AnimatorState.Walking);
        }
        else if (state == NpcState.WAITING_TO_BE_ATTENDED_7)
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