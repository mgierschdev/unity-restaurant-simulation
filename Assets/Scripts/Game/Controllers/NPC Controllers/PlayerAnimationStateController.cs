using UnityEngine;

public class PlayerAnimationStateController : MonoBehaviour
{
    private InfoPopUpController infoPopUpController;
    private GameObject tryItem;
    private Animator animator;
    public void Start()
    {
        GameObject character = gameObject.transform.Find(Settings.NpcCharacter).gameObject;
        animator = character.GetComponent<Animator>();
        //On top Info popup
        GameObject infoPopUpGameobject = transform.Find(Settings.TopPopUpObject).gameObject;
        infoPopUpController = infoPopUpGameobject.GetComponent<InfoPopUpController>();
        // Left hand try object
        tryItem = gameObject.transform.Find(Settings.TryObject).gameObject;
        tryItem.SetActive(false);

        if (animator == null)
        {
            GameLog.LogWarning("PlayerAnimationStateController/Animator null");
        }
    }

    public void SetInfoPopUItem(ItemType item)
    {
        infoPopUpController.SetInfoPopUItem(item);
    }

    public void SetState(NpcState state)
    {
        if (!animator)
        {
            return;
        }

        if (state == NpcState.WALKING_TO_TABLE)
        {
            animator.ResetTrigger(NPCAnimatorState.WaitingAtTable);
            animator.ResetTrigger(NPCAnimatorState.Walking);
            animator.ResetTrigger(NPCAnimatorState.Idle);
            animator.SetTrigger(NPCAnimatorState.WalkingToTable);
            tryItem.SetActive(true);
            infoPopUpController.Disable();
        }
        else if (state == NpcState.WALKING)
        {
            animator.ResetTrigger(NPCAnimatorState.WaitingAtTable);
            animator.ResetTrigger(NPCAnimatorState.WalkingToTable);
            animator.ResetTrigger(NPCAnimatorState.Idle);
            animator.SetTrigger(NPCAnimatorState.Walking);
            tryItem.SetActive(false);
            infoPopUpController.Disable();
        }
        else if (state == NpcState.WAITING_TO_BE_ATTENDED)
        {
            animator.ResetTrigger(NPCAnimatorState.Idle);
            animator.ResetTrigger(NPCAnimatorState.Walking);
            animator.ResetTrigger(NPCAnimatorState.WalkingToTable);
            animator.SetTrigger(NPCAnimatorState.WaitingAtTable);
            tryItem.SetActive(false);
            infoPopUpController.EnableWithoutAnimation();
        }
        else if (state != NpcState.WAITING_TO_BE_ATTENDED)
        {
            animator.ResetTrigger(NPCAnimatorState.Walking);
            animator.ResetTrigger(NPCAnimatorState.WaitingAtTable);
            animator.ResetTrigger(NPCAnimatorState.WalkingToTable);
            animator.SetTrigger(NPCAnimatorState.Idle);
            tryItem.SetActive(false);
            infoPopUpController.Disable();
        }

    }
}