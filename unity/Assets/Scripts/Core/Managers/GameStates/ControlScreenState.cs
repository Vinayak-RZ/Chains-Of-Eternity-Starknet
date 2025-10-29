using UnityEngine;

public class ControlScreenState : State<GameManagerUnity>
{
    public ControlScreenState(GameManagerUnity owner, StateMachine<GameManagerUnity> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        Time.timeScale = 0f;
        owner.ShowControlsCanvas();
    }

    public override void Exit()
    {
        //Debug.Log("Exiting stats ");
        owner.HideControlsCanvas();
    }

    public override void HandleInput()
    {
        if (InputManager.Instance.StatsPressed)
        {
            stateMachine.ChangeState(new InGameState(owner, stateMachine));
        }
    }
}
