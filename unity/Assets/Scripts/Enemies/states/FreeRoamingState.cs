using UnityEngine;

public class FreeRoamingState : BaseEnemyState
{
    private Vector2 roamTarget;

    public FreeRoamingState(Enemy owner, StateMachine<Enemy> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        roamTarget = owner.GetRandomRoamPosition();
        owner.animator.SetBool("freeRoam", true);
        owner.animator.SetBool("followPlayer", false);
        owner.animator.SetBool("isAttacking", false);
    }

    public override void LogicUpdate()
    {
        //Debug.Log("Free roaming state active");
        
        MoveTowards(roamTarget, owner.MoveSpeed);

        if (owner.CanSeePlayer())
        {
            //Debug.Log("Player detected, switching to follow state");
            stateMachine.ChangeState(owner.FollowState);
        }
        else if (Vector2.Distance(owner.transform.position, roamTarget) < 0.2f)
        {
            //Debug.Log("Reached roam target, switching to idle state");
            stateMachine.ChangeState(owner.IdleState);
        }
    }

}