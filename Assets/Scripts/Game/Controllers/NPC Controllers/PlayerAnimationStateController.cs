using System;
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

        ResetAllTriggers();

        if (state == NpcState.WALKING_TO_TABLE)
        {
            animator.SetTrigger(NPCAnimatorState.WalkingToTable.ToString());
            tryItem.SetActive(true);
            infoPopUpController.Disable();
        }
        else if (state == NpcState.WALKING)
        {
            animator.SetTrigger(NPCAnimatorState.Walking.ToString());
            tryItem.SetActive(false);
            infoPopUpController.Disable();
        }
        else if (state == NpcState.WAITING_TO_BE_ATTENDED)
        {
            animator.SetTrigger(NPCAnimatorState.WaitingAtTable.ToString());
            tryItem.SetActive(false);
            infoPopUpController.EnableWithoutAnimation();
        }
        else if (state == NpcState.TAKING_ORDER)
        {
            animator.SetTrigger(NPCAnimatorState.IdleTry.ToString());
            tryItem.SetActive(true);
            infoPopUpController.Disable();
        }
        else if (state == NpcState.EATING_FOOD)
        {
            animator.SetTrigger(NPCAnimatorState.EatingAtTable.ToString());
        }
        else if (state != NpcState.WAITING_TO_BE_ATTENDED)
        {

            animator.SetTrigger(NPCAnimatorState.Idle.ToString());
            tryItem.SetActive(false);
            infoPopUpController.Disable();
        }
    }

    private void ResetAllTriggers()
    {
        foreach (NPCAnimatorState state in Enum.GetValues(typeof(NPCAnimatorState)))
        {
            animator.ResetTrigger(state.ToString());
        }
    }
}