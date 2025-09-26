using UnityEngine;
public class PlayerIdleState : PlayerState
{
    private Vector2 moveInput;

    public PlayerIdleState(Player player, StateMachine<Player> stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        player.SetVelocity(Vector2.zero);
        player.PlayAnimation("Idle");
    }

    public override void HandleInput()
    {
        moveInput = InputManager.Instance.MoveDirection;

        if (moveInput != Vector2.zero)
        {
            stateMachine.ChangeState(player.moveState);
        }
    }
}
