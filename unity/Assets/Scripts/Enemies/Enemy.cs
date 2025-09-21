using System;
using System.Collections;
using System.Threading.Tasks;
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
    public float knockbackForce = 7f; // Force applied when knocked back
    public float attackanimationtime = 1f; // Duration of stun effect
    public UInt32 maxExpAward;
    public UInt32 minExpAward;
    private int expAward;
    public String Name = "Enemy";

    private bool hasDied = false;
    private bool fetchedExp = false;

    private float timer;

    private TaskCompletionSource<UInt32> expTCS;

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
    public CooldownState CooldownState { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }

    private SimpleRandomContract simpleRandomContract;

    public bool IsFacingRight = true;

    private void Awake()
    {   
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;

        RB = transform.GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        if (VRFResultRouter.Instance != null)
            VRFResultRouter.Instance.OnVRFResult += OnVRFResult;
    }

    void OnDisable()
    {
        if (VRFResultRouter.Instance != null)
            VRFResultRouter.Instance.OnVRFResult -= OnVRFResult;
    }
    void Start()
    {
        RB.freezeRotation = true;
        currentHealth = maxHealth; // Initialize current health to max health
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Disable automatic rotation to control it manually
        agent.updateUpAxis = false; // Disable automatic up-axis adjustment if not needed

        StateMachine = new StateMachine<Enemy>();
        simpleRandomContract = new SimpleRandomContract();

        IdleState = new IdleState(this, StateMachine);
        FreeRoamingState = new FreeRoamingState(this, StateMachine);
        FollowState = new FollowState(this, StateMachine);
        AttackState = new AttackState(this, StateMachine);
        StunState = new StunState(this, StateMachine);
        CooldownState = new CooldownState(this, StateMachine, AttackCooldown);
        StateMachine.Initialize(IdleState);

        expAward = (int) UnityEngine.Random.Range(minExpAward, maxExpAward);
        StartCoroutine(enumerator());
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

        Debug.Log("Performed raycast for player detection");

        if (hit)
        {
            // If the hit is the player, return true; if it's an obstacle, return false.
            if (((1 << hit.collider.gameObject.layer) & playerLayer) != 0)
                return true;
        }
        Debug.Log("Player not detected within vision range or blocked by an obstacle.");
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
    public void TakeDamage(int damage, Vector3 sourcePos, bool applyKnockback = true, bool applyStun = true, bool flash = true, string damageType = "Physical")
    {
        //TODO : Handle different damage types (e.g., Physical, Magical)
        int effectiveDamage = Mathf.Max(0, damage);

        currentHealth = Mathf.Max(0, currentHealth - effectiveDamage);
        animator.SetBool("isHit", true);
        //Debug.Log(sourcePos);
        //Debug.Log(transform.position);
        //Debug.Log(transform.position - sourcePos);
        Vector2 knockbackDirection = ((Vector2)transform.position - (Vector2)sourcePos).normalized;
        //Debug.Log(knockbackDirection);
        //knockbackDirection = new Vector2(knockbackDirection.x, -knockbackDirection.y);
        //Debug.Log(knockbackDirection);
        if (applyKnockback)
            ApplyKnockback(knockbackDirection, applyStun, 5f);
        if (flash)
            StartCoroutine(FlashOnHit());

        if (currentHealth == 0 && !hasDied)
            Die();
    }
    private IEnumerator FlashOnHit()
    {
        
        if (spriteRenderer == null) yield break; // No sprite renderer to flash

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.red*2; // Flash color
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor; // Reset to original color
    }
    public void ApplyKnockback(Vector2 direction, bool applyStun, float force = 0.1f, float duration = 0.2f)
    {
        StartCoroutine(KnockbackRoutine(direction, force, duration, applyStun));
    }
    private IEnumerator KnockbackRoutine(Vector2 direction, float force, float duration, bool applyStun)
    {
        agent.enabled = false;
        RB.AddForce(direction.normalized * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        RB.linearVelocity = Vector2.zero;
        agent.enabled = true;
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
        //Debug.Log("Flipping" + Name);
        IsFacingRight = !IsFacingRight;
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
    
    public void Die()
    {
        hasDied = true;
        
        animator.SetBool("isDead", true);
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

    public void hitAnimComplete()
    {
        animator.SetBool("isHit", false);
    }

    void OnVRFResult(uint exp)
    {
        Debug.Log($"?? Awarding {exp} EXP to player");

        expAward = (int)exp;
    }

    void HandleVRFResult(uint result)
    {
        string myAddress = Web3AuthManager.Instance.GetWalletAddress();

        // Optional: if using `requestId` later, you can match here
        // For now, just resolve the Task with the exp
        if (expTCS != null && !expTCS.Task.IsCompleted)
        {
            expTCS.SetResult(result);
        }
    }

    private IEnumerator enumerator()
    {
        yield return new WaitForSeconds(1f);

        fetchExpFromContract();
    }

    private async void fetchExpFromContract()
    {
        expTCS = new TaskCompletionSource<UInt32>();
        VRFResultRouter.Instance.OnVRFResult += HandleVRFResult;
        Debug.Log("Requesting random experience award...");
        string txnHash = await simpleRandomContract.RequestRandom(minExpAward, maxExpAward);
        Debug.Log($"VRF request sent with transaction hash: {txnHash}");
        Debug.Log("Waiting for VRF result...");
        UInt32 exp = await expTCS.Task;
        Debug.Log($"Received EXP: {exp} from VRF request {txnHash}");
        ProgressManager.Instance.AddExperience((int)exp);
    }
}
