using UnityEngine;

public class PlayerAnimationStateController : MonoBehaviour
{
    private Animator animator;
    public void Start()
    {
        GameObject character = gameObject.transform.Find(Settings.NPC_CHARACTER).gameObject;
        animator = character.GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogWarning("PlayerAnimationStateController/Animator null");
        }
    }

    public void SetState(NPCState state)
    {
        if (animator == null)
        {
            return;
        }
        
        if (state == NPCState.IDLE || state == NPCState.AT_COUNTER || state == NPCState.TAKING_ORDER || state == NPCState.REGISTERING_CASH)
        {
            animator.SetTrigger(AnimatorState.IDLE);
        }
        else if (state == NPCState.WALKING_TO_TABLE || state == NPCState.WALKING_TO_COUNTER || state == NPCState.WALKING_UNRESPAWN || state == NPCState.WALKING_TO_COUNTER_AFTER_ORDER || state == NPCState.WALKING_WANDER)
        {
            animator.ResetTrigger(AnimatorState.IDLE);
            animator.SetTrigger(AnimatorState.WALKING);
        }
        else
        {
            animator.SetTrigger(AnimatorState.IDLE);
        }
    }
}