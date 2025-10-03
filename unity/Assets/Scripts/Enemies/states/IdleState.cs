using System;
using UnityEngine;

public class IdleState : BaseEnemyState
{
    private float idleDuration = 2f;
    private float idleTimer;

    public IdleState(Enemy owner, StateMachine<Enemy> stateMachine, float idleDuration = 2f)
        : base(owner, stateMachine)
    {
        this.idleDuration = idleDuration;
    }

    public override void Enter()
    {
        StopMoving();
        idleTimer = 0f;
        //Debug.Log("Entering idle state");
        owner.animator.SetBool("isWalking", false);
    }

    public override void LogicUpdate()
    {
        idleTimer += Time.deltaTime;
        if (idleTimer >= idleDuration)
        {
            stateMachine.ChangeState(owner.FreeRoamingState);
        }
    }
}
