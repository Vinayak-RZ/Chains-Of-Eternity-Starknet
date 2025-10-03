using UnityEngine;
public class AttackState : BaseEnemyState
{
    private float attackCooldown;
    private float attackTimer;

    public AttackState(Enemy owner, StateMachine<Enemy> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        StopMoving();
        attackTimer = 0f;
        attackCooldown = owner.AttackCooldown;
    }

    public override void LogicUpdate()
    {
        if (!owner.CanSeePlayer())
        {
            stateMachine.ChangeState(owner.FreeRoamingState);
            return;
        }

        float distanceToPlayer = Vector2.Distance(owner.transform.position, Player.position);
        if (distanceToPlayer > owner.AttackRange)
        {
            stateMachine.ChangeState(owner.FollowState);
            return;
        }

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackCooldown)
        {
            // Attack the player
            attackTimer = 0f;
        }
    }
}
