using UnityEngine;

public class PlayerNoAttackState : PlayerState
{
    
    private bool attackComplete = false;

    public PlayerNoAttackState(Player player, StateMachine<Player> stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        player.Animator.SetBool("attacking", false);
    }

    public override void LogicUpdate()
    {
        //base.LogicUpdate();
        if (InputManager.Instance.AttackPressed)
        {
            stateMachine.ChangeState(player.attackState);
            return;
        }

    }

    public void OnAttackAnimationComplete() => attackComplete = true;

    public override void Exit()
    {
        player.ClearHitEnemies();
    }
}
