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

        if (state == NpcState.WALKING)
        {
            animator.ResetTrigger(NPCAnimatorState.WaitingAtTable);
            animator.SetTrigger(NPCAnimatorState.Walking);
            animator.ResetTrigger(NPCAnimatorState.Idle);
        }
        else if (state != NpcState.WAITING_TO_BE_ATTENDED)
        {
            animator.ResetTrigger(NPCAnimatorState.Walking);
            animator.ResetTrigger(NPCAnimatorState.WaitingAtTable);
            animator.SetTrigger(NPCAnimatorState.Idle);
        }
        else if (state == NpcState.WAITING_TO_BE_ATTENDED)
        {
            animator.ResetTrigger(NPCAnimatorState.Idle);
            animator.ResetTrigger(NPCAnimatorState.Walking);
            animator.SetTrigger(NPCAnimatorState.WaitingAtTable);
        }
    }
}