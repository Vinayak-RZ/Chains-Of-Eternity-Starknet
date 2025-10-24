using UnityEngine;

public class PlayerStunState : PlayerState
{
    private float stunDuration = 0.5f;
    private float startTime;

    public PlayerStunState(Player player, StateMachine<Player> stateMachine)
        : base(player, stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        player.Enable_DisableInput(false); // Disable input
        startTime = Time.time;
        player.SetVelocity(Vector2.zero); // Optional freeze
    }
    public override void Exit()
    {
        base.Exit();
        player.Enable_DisableInput(true); // Re-enable input
        player.PlayAnimation("Idle"); // Reset animation to idle after stun
    }
    public override void LogicUpdate()
    {
        if (Time.time >= startTime + stunDuration)
        {
            player.PlayAnimation("Idle"); // Reset animation to idle after stun
            stateMachine.ChangeState(player.idleState); // or moveState if input exists
        }
    }
 // Disable input while stunned
}
