using System;
using Game.Controllers.Other_Controllers;
using UnityEngine;
using Util;

namespace Game.Controllers.NPC_Controllers
{
    /**
     * Problem: Synchronize NPC animations with state changes.
     * Goal: Trigger appropriate animator states and UI cues.
     * Approach: Map NpcState to animator triggers and pop-up controls.
     * Time: O(n) to reset triggers (n = animator states).
     * Space: O(1).
     */
    public class PlayerAnimationStateController : MonoBehaviour
    {
        private InfoPopUpController _infoPopUpController;
        private GameObject _tryItem;
        private Animator _animator;

        public void Start()
        {
            var character = gameObject.transform.Find(Settings.NpcCharacter).gameObject;
            _animator = character.GetComponent<Animator>();
            //On top Info popup
            var infoPopUpGameobject = transform.Find(Settings.TopPopUpObject).gameObject;
            _infoPopUpController = infoPopUpGameobject.GetComponent<InfoPopUpController>();
            // Left hand try object
            _tryItem = gameObject.transform.Find(Settings.TryObject).gameObject;
            _tryItem.SetActive(false);

            if (_animator == null)
            {
                GameLog.LogWarning("PlayerAnimationStateController/Animator null");
            }
        }

        public void SetInfoPopUItem(ItemType item)
        {
            _infoPopUpController.SetInfoPopUItem(item);
        }

        public void SetState(NpcState state)
        {
            if (!_animator)
            {
                return;
            }

            ResetAllTriggers();

            if (state == NpcState.WalkingToTable)
            {
                _animator.SetTrigger(NpcAnimatorState.WalkingToTable.ToString());
                _tryItem.SetActive(true);
                _infoPopUpController.Disable();
            }
            else if (state == NpcState.Walking)
            {
                _animator.SetTrigger(NpcAnimatorState.Walking.ToString());
                _tryItem.SetActive(false);
                _infoPopUpController.Disable();
            }
            else if (state == NpcState.WaitingToBeAttended)
            {
                _animator.SetTrigger(NpcAnimatorState.WaitingAtTable.ToString());
                _tryItem.SetActive(false);
                _infoPopUpController.EnableWithoutAnimation();
            }
            else if (state == NpcState.TakingOrder)
            {
                _animator.SetTrigger(NpcAnimatorState.IdleTry.ToString());
                _tryItem.SetActive(true);
                _infoPopUpController.Disable();
            }
            else if (state == NpcState.EatingFood)
            {
                _animator.SetTrigger(NpcAnimatorState.EatingAtTable.ToString());
            }
            else
            {
                _animator.SetTrigger(NpcAnimatorState.Idle.ToString());
                _tryItem.SetActive(false);
                _infoPopUpController.Disable();
            }
        }

        private void ResetAllTriggers()
        {
            foreach (NpcAnimatorState state in Enum.GetValues(typeof(NpcAnimatorState)))
            {
                _animator.ResetTrigger(state.ToString());
            }
        }
    }
}
