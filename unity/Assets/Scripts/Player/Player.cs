using System;
using System.Collections;
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
    public PlayerStunState stunState { get; private set; }
    // public PlayerAttackState attackState { get; private set; } // for future

    [Header("Components")]
    public Rigidbody2D RB { get; private set; }
    public Animator Animator { get; private set; }

    public Vector2 CurrentVelocity => RB.linearVelocity;
    public bool IsFacingRight = true;
    private float RegenTimer = 0f;
    public bool isInvincible = false; // For future use, e.g., after taking damage

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
        stunState = new PlayerStunState(this, stateMachine);
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

    public void Invincibilty(bool isInvincibleee)
    {
        isInvincible = isInvincibleee;
    }
    public void TakeDamage(int damage, Vector3 sourcePos, bool applyKnockback = true, bool applyStun = true, string damageType = "Physical")
    {
        if (isInvincible) return; // Ignore damage if invincible
        //TODO : Handle different damage types (e.g., Physical, Magical)
        int effectiveDamage = Mathf.Max(0, damage - heroData.defensiveStats.defense);
        currentHealth = Mathf.Max(0, currentHealth - effectiveDamage);
        if (applyKnockback)
            ApplyKnockback((transform.position - sourcePos).normalized, 15f,applyStun);
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
    public void Enable_DisableInput(bool check)
    {
        // This method can be used to disable player input during certain states (e.g., stunned, dashing)
        if (check)
        {
            InputManager.Instance.EnableInput();
        }
        else
        {
            InputManager.Instance.DisableInput();
        }
    }
    public void RegenerateResources()
    {
        currentHealth = Mathf.Min(heroData.defensiveStats.maxHealth, currentHealth + heroData.defensiveStats.healthRegeneration);
        currentMana = Mathf.Min(heroData.specialStats.maxMana, currentMana + heroData.specialStats.manaRegeneration);
        currentEnergy = Mathf.Min(heroData.specialStats.maxEnergy, currentEnergy + heroData.specialStats.energyRegeneration);
    }
    public void ApplyKnockback(Vector2 direction, float force, bool applyStun, float duration = 0.2f)
    {   
        stateMachine.ChangeState(idleState);
        StartCoroutine(KnockbackRoutine(direction, force, duration, applyStun));
    }
    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration, bool applyStun)
    {
        // Freeze current velocity and apply impulse
        RB.linearVelocity = Vector2.zero;
        Enable_DisableInput(false); // Disable input during knockback
        RB.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        // Wait for knockback duration
        yield return new WaitForSeconds(duration);
        Enable_DisableInput(true); // Re-enable input after knockback
        if (applyStun)
        {
            stateMachine.ChangeState(stunState);
        }
        else
        {
            // Resume movement or idle based on input
            if (InputManager.Instance.MoveDirection != Vector2.zero)
                stateMachine.ChangeState(moveState);
            else
                stateMachine.ChangeState(idleState);
        }
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
