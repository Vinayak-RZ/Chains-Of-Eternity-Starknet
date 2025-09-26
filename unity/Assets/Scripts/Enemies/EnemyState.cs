using UnityEngine;

public abstract class BaseEnemyState : State<Enemy>
{
    protected BaseEnemyState(Enemy owner, StateMachine<Enemy> stateMachine)
        : base(owner, stateMachine) { }

    protected Transform Player => owner.PlayerTransform;
    protected Rigidbody2D Rb => owner.RB;
    protected float MoveSpeed => owner.MoveSpeed;

    protected void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)owner.transform.position).normalized;
        Rb.linearVelocity = direction * MoveSpeed;
    }

    protected void StopMoving()
    {
        Rb.linearVelocity = Vector2.zero;
    }
}
