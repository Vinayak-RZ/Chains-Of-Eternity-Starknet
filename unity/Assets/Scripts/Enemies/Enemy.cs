using System;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float moveSpaaaaaaaaaaaeed = 2f;
    public String Name = "Enemy";

    public NavMeshAgent agent;

    public float AttackRange = 1.5f;
    public float AttackCooldown = 1f;

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

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Disable automatic rotation to control it manually
        agent.updateUpAxis = false; // Disable automatic up-axis adjustment if not needed

        StateMachine = new StateMachine<Enemy>();

        IdleState = new IdleState(this, StateMachine);
        FreeRoamingState = new FreeRoamingState(this, StateMachine);
        FollowState = new FollowState(this, StateMachine);
        AttackState = new AttackState(this, StateMachine);

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

    public void FlipIfNeeded()
    {
        if (agent.velocity.x != 0)
        {
            bool shouldFlip = (agent.velocity.x > 0 && !IsFacingRight) || (agent.velocity.x < 0 && IsFacingRight);
            if (shouldFlip)
            {
                Debug.Log("Flipping" + Name);
                IsFacingRight = !IsFacingRight;
                RB.transform.Rotate(0f, 180f, 0f);
            }
        }
    }
    public virtual void PerformAttack()
    {
        Debug.Log("Base Enemy Attack");
    }
}
    