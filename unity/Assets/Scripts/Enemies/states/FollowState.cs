using UnityEngine;
public class FollowState : BaseEnemyState
{
    public FollowState(Enemy owner, StateMachine<Enemy> stateMachine)
        : base(owner, stateMachine) { }

    public override void LogicUpdate()
    {
        if (!owner.CanSeePlayer())
        {
            stateMachine.ChangeState(owner.FreeRoamingState);
            return;
        }

        float distanceToPlayer = Vector2.Distance(owner.transform.position, Player.position);
        if (distanceToPlayer <= owner.AttackRange)
        {
            stateMachine.ChangeState(owner.AttackState);
            return;
        }

        MoveTowards(Player.position);
    }
}
