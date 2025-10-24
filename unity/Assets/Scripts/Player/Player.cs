using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class Player : MonoBehaviour
{
    [Header("Hero Data")]
    public HeroData heroData; // Drag in via Inspector

    // Runtime values
    [SerializeField] public int currentHealth;
    [SerializeField] private int currentMana;
    [SerializeField] public int currentEnergy;
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    [Header("Dash Settings")]
    public float dashSpeed = 15f;
    public float dashDuration = 1f;
    public float dashCooldown = 0.5f;

    [HideInInspector]
    public bool canDash = true;

    [Header("Energy Settings")]
    [SerializeField] public int RegenerateResourcesRate = 1; // How often to regenerate resources (in seconds)
    public int dashEnergyCost = 40;

    [Header("State Machine")]
    public StateMachine<Player> stateMachine { get; private set; }

    // Player States (Add more later)
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    // public PlayerAttackState attackState { get; private set; } // for future

    [Header("Components")]
    public Rigidbody2D RB { get; private set; }
    public Animator Animator { get; private set; }

    public Vector2 CurrentVelocity => RB.linearVelocity;
    public bool IsFacingRight = true;
    private float RegenTimer = 0f;

    private void Awake()
    {
        RB = transform.GetComponent<Rigidbody2D>();
        Animator = transform.GetComponent<Animator>();

        currentHealth = heroData.defensiveStats.maxHealth;
        currentMana = heroData.specialStats.maxMana;
        currentEnergy = heroData.specialStats.maxEnergy;

        stateMachine = new StateMachine<Player>();

        idleState = new PlayerIdleState(this, stateMachine);
        moveState = new PlayerMoveState(this, stateMachine);
        dashState = new PlayerDashState(this, stateMachine);
        // attackState = new PlayerAttackState(this, stateMachine); // for future
    }

    private void Start()
    {
        RB.freezeRotation = true;
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        stateMachine.HandleInput();
        stateMachine.LogicUpdate();
    }

    private void FixedUpdate()
    {   
        RegenTimer += Time.fixedDeltaTime;
        if (RegenTimer >= RegenerateResourcesRate)
        {
            RegenerateResources();
            RegenTimer = 0f;
        }
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

    public void TakeDamage(int damage, string damageType = "Physical")
    {
        //TODO : Handle different damage types (e.g., Physical, Magical)
        int effectiveDamage = Mathf.Max(0, damage - heroData.defensiveStats.defense);
        currentHealth = Mathf.Max(0, currentHealth - effectiveDamage);

        if (currentHealth == 0)
            Die();
    }

    public void UseMana(int amount)
    {
        currentMana = Mathf.Max(0, currentMana - amount);
    }

    public void UseEnergy(int amount)
    {
        currentEnergy = Mathf.Max(0, currentEnergy - amount);
    }

    public void RegenerateResources()
    {
        currentHealth = Mathf.Min(heroData.defensiveStats.maxHealth, currentHealth + heroData.defensiveStats.healthRegeneration);
        currentMana = Mathf.Min(heroData.specialStats.maxMana, currentMana + heroData.specialStats.manaRegeneration);
        currentEnergy = Mathf.Min(heroData.specialStats.maxEnergy, currentEnergy + heroData.specialStats.energyRegeneration);
    }

    private void Die()
    {
        // Add animation, disable movement, etc.
        Debug.Log($"{heroData.playerName} has died.");
    }

    internal void Move(float inputX)
    {
        throw new NotImplementedException();
    }

    #endregion
}
