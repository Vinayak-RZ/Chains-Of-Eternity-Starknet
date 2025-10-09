using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("State Machine")]
    public StateMachine<Player> stateMachine { get; private set; }

    // Player States (Add more later)
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    // public PlayerAttackState attackState { get; private set; } // for future

    [Header("Components")]
    public Rigidbody2D RB { get; private set; }
    public Animator Animator { get; private set; }

    public Vector2 CurrentVelocity => RB.linearVelocity;
    public bool IsFacingRight = true;

    private void Awake()
    {
        RB = transform.GetComponent<Rigidbody2D>();
        Animator = transform.GetComponent<Animator>();

        stateMachine = new StateMachine<Player>();

        idleState = new PlayerIdleState(this, stateMachine);
        moveState = new PlayerMoveState(this, stateMachine);
        // attackState = new PlayerAttackState(this, stateMachine); // for future
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.HandleInput();
        stateMachine.LogicUpdate();
    }

    private void FixedUpdate()
    {
        stateMachine.PhysicsUpdate();
    }

    #region Utility Methods — Called by States

    public void SetVelocity(Vector2 velocity)
    {
        RB.linearVelocity = velocity;
    }

    public void SetXVelocity(float x)
    {
        RB.linearVelocity = new Vector2(x, RB.linearVelocity.y);
    }

    public void FlipIfNeeded(float xInput)
    {
        if (xInput != 0)
        {
            bool shouldFlip = (xInput > 0 && !IsFacingRight) || (xInput < 0 && IsFacingRight);
            if (shouldFlip)
            {
                IsFacingRight = !IsFacingRight;
                transform.Rotate(0f, 180f, 0f);
            }
        }
    }

    public void PlayAnimation(string animationName)
    {
        if (Animator != null)
        {
            Animator.Play(animationName);
        }
    }

    internal void Move(float inputX)
    {
        throw new NotImplementedException();
    }

    #endregion
}
