using UnityEngine;

public class PlayerController : GameObjectMovementBase
{
    [SerializeField]
    NpcState localState;
    PlayerAnimationStateController animationController;
    private void Start()
    {
        animationController = GetComponent<PlayerAnimationStateController>();
        localState = NpcState.IDLE_0;
        // Click controller
        GameObject cController = GameObject.FindGameObjectWithTag(Settings.ConstParentGameObject);
        ClickController clickController = cController.GetComponent<ClickController>();
        SetClickController(clickController);
    }
    // For Handling non-physics related objects
    private void Update()
    {
        // Player Movement on click
        if (Settings.PlayerWalkOnClick)
        {
            MouseOnClick();
        }
        // Player Moving on long click/touch
        MovingOnLongTouch();
        animationController.SetState(localState);
    }

    // Called every physics step, Update called every frame
    private void FixedUpdate()
    {
        UpdateTargetMovement();
        UpdatePosition();
    }
}