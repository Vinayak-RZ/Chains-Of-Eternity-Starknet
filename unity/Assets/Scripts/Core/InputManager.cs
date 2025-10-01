using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public Vector2 MoveDirection { get; private set; }
    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool IsAttackHeld { get; private set; }

    private PlayerInputActions inputActions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        inputActions = new PlayerInputActions();
        inputActions.Enable();
    }

    private void Update()
    {
        MoveDirection = inputActions.Player.Move.ReadValue<Vector2>();
        DashPressed = inputActions.Player.Dash.triggered;
        AttackPressed = inputActions.Player.Attack.triggered;
        IsAttackHeld = inputActions.Player.Attack.ReadValue<float>() > 0.1f;
    }
}
