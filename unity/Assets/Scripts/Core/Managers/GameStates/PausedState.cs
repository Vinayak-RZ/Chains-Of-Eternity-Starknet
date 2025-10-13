using UnityEngine;

public class PausedState : State<GameManagerUnity>
{
    public PausedState(GameManagerUnity owner, StateMachine<GameManagerUnity> stateMachine)
        : base(owner, stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entered Paused State");
        Time.timeScale = 0f;
        owner.ShowPauseMenu();
    }

    public override void Exit()
    {
        Debug.Log("Exiting Paused State");
        owner.HidePauseMenu();
    }

    public override void HandleInput()
    {
        if (InputManager.Instance.PausePressed)
        {
            stateMachine.ChangeState(new InGameState(owner, stateMachine));
        }
    }
}
