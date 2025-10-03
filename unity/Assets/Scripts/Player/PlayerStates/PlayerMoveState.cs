using System;
using UnityEngine;

public class PlayerMoveState : PlayerState
{
    private Vector2 moveInput;
    public PlayerMoveState(Player player, StateMachine<Player> stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        //player.PlayAnimation("Run");
    }

    public override void LogicUpdate()
    {
        moveInput = InputManager.Instance.MoveDirection;

        if (moveInput == Vector2.zero)
        {
            stateMachine.ChangeState(player.idleState);
        }

        player.FlipIfNeeded(moveInput.x);
    }

    public override void PhysicsUpdate()
    {
        player.SetVelocity(moveInput * player.moveSpeed);
    }
}
