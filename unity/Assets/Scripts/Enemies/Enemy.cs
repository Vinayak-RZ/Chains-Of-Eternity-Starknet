using System;
using System.Collections;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float moveSpeed = 2f;
    [Header("Enemy Stats")]
    public int maxHealth = 100;
    public int currentHealth;
    public int damage = 10;
    public float attackSpeed = 1f; // Attacks per second
    public float AttackRange = 1.5f;
    public float AttackCooldown = 1f;
    public float knockbackForce = 5f; // Force applied when knocked back
    public String Name = "Enemy";

    public NavMeshAgent agent;



    // fields for player detection
    [SerializeField] private float visionRange = 5f;
    [SerializeField] private float visionRadius = 0.5f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;


    // fields for random roaming positions
    [SerializeField] private float roamRadius = 5f;
    [SerializeField] private LayerMask groundLayer;

    // Animator for the enemy reference
    public Animator animator;

    public Rigidbody2D RB { get; private set; }

    public Transform PlayerTransform => playerTransform;
    public float MoveSpeed => moveSpeed;

    public StateMachine<Enemy> StateMachine { get; private set; }

    public IdleState IdleState { get; private set; }
    public FreeRoamingState FreeRoamingState { get; private set; }
    public FollowState FollowState { get; private set; }
    public AttackState AttackState { get; private set; }
    public StunState StunState { get; private set; }

    public bool IsFacingRight = true;

    private void Awake()
    {
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        RB = transform.GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        RB.freezeRotation = true;
        currentHealth = maxHealth; // Initialize current health to max health
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Disable automatic rotation to control it manually
        agent.updateUpAxis = false; // Disable automatic up-axis adjustment if not needed

        StateMachine = new StateMachine<Enemy>();

        IdleState = new IdleState(this, StateMachine);
        FreeRoamingState = new FreeRoamingState(this, StateMachine);
        FollowState = new FollowState(this, StateMachine);
        AttackState = new AttackState(this, StateMachine);
        StunState = new StunState(this, StateMachine);
        StateMachine.Initialize(IdleState);

    }

    private void Update()
    {
        StateMachine?.LogicUpdate();
    }

    private void FixedUpdate()
    {
        StateMachine?.PhysicsUpdate();
        FlipIfNeeded();
    }

    public bool CanSeePlayer()
    {
        if (playerTransform == null)
            return false;

        Vector2 direction = (playerTransform.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // Don't bother if player too far
        if (distance > visionRange)
            return false;

        RaycastHit2D hit = Physics2D.CircleCast(
            origin: transform.position,
            radius: visionRadius,
            direction: direction,
            distance: distance,
            layerMask: playerLayer | obstacleLayer // Combine masks
        );

        if (hit)
        {
            // If the hit is the player, return true; if it's an obstacle, return false.
            if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
                return true;
        }

        return false;
    }

    public Vector2 GetRandomRoamPosition()
    {
        Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * roamRadius;
        Vector2 targetPos = (Vector2)transform.position + randomOffset;
        for (int i = 0; i < 100; i++) // Retry a few times in case of invalid points
        {
            randomOffset = UnityEngine.Random.insideUnitCircle * roamRadius;
            targetPos = (Vector2)transform.position + randomOffset;
            //Debug.Log(randomOffset);
            //Debug.Log(targetPos);

            // Check ground layer
            RaycastHit2D hit = Physics2D.Raycast(
                origin: transform.position,
                direction: (targetPos - (Vector2)transform.position).normalized,
                distance: (targetPos - (Vector2)transform.position).magnitude,
                layerMask: obstacleLayer
            );
            if (hit == false)
            {
                return targetPos;
            }

        }
        return targetPos;
    }
    public void TakeDamage(int damage, Vector3 sourcePos, bool applyKnockback = true, bool applyStun = true, string damageType = "Physical")
    {
        //TODO : Handle different damage types (e.g., Physical, Magical)
        int effectiveDamage = Mathf.Max(0, damage);
        currentHealth = Mathf.Max(0, currentHealth - effectiveDamage);
        if (applyKnockback)
            ApplyKnockback((transform.position - sourcePos).normalized, applyStun,15f);
        if (currentHealth == 0)
            Die();
    }
    public void ApplyKnockback(Vector2 direction, bool applyStun,float force= 10f,  float duration = 0.2f)
    {
        StateMachine.ChangeState(StunState);
        StartCoroutine(KnockbackRoutine(direction, force, duration, applyStun));
    }
    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration, bool applyStun)
    {
        RB.linearVelocity = Vector2.zero;
        RB.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        if (applyStun)
        {
            StateMachine.ChangeState(StunState);
        }
        else
        {
            StateMachine.ChangeState(FreeRoamingState); // Return to idle state if not stunned
        }
    }
    public void FlipIfNeeded()
    {
        if (Mathf.Abs(agent.velocity.x) < 0.5f)
        {
            bool shouldFlip = (agent.velocity.x > 0 && !IsFacingRight) || (agent.velocity.x < 0 && IsFacingRight);
            if (shouldFlip)
            {
                Flip();
            }
        }
    }
    public void Flip()
    {
        Debug.Log("Flipping" + Name);
        IsFacingRight = !IsFacingRight;
        RB.transform.Rotate(0f, 180f, 0f);
    }
    
    public void Die()
    {
        Debug.Log(Name + " has died.");
        //TODO: Implement death logic, like playing a death animation, dropping loot, etc.
        onDeath();
        Destroy(gameObject); // For now, just destroy the enemy
    }
    public virtual void PerformAttack()
    {
        Debug.Log("Base Enemy Attack");
    }
    public virtual void onDeath()
    {
        // Override this method to implement custom death behavior
        Debug.Log(Name + " has been defeated!");
    }
}
