using UnityEngine;

public class ProjectileSpellRuntime : MonoBehaviour
{
    private ProjectileData data;
    private Vector2 direction;
    private Transform caster;
    private Rigidbody2D rb;

    // ZigZag
    private float zigzagAmplitude = 6f;
    private float zigzagFrequency = 8f;
    private float zigzagTime;
    //Circular
    private float circularAngle;
    private float circularRadius = 0.5f;
    private float circularSpeed = 4f;
    private float radialSpeed = 0.8f;
    private Vector3 casterStartPosition;


    // Random
    private Vector2 randomOffset;

    public void Initialize(ProjectileData data, Vector2 direction, Transform caster)
    {
        this.data = data;
        this.caster = caster;
        Debug.Assert(data != null, "ProjectileData cannot be null");
        // Face-aware direction
        bool isFacingRight = caster.localScale.x > 0;
        Vector2 baseDir = direction.normalized;
        if (!isFacingRight) baseDir.x *= -1;
        this.direction = baseDir;

        rb = GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        ApplyMovement();

        Destroy(gameObject, data.destroyTime); // fallback lifetime
    }

    private void ApplyMovement()
    {
        switch (data.movementPath)
        {
            case ProjectileData.ProjectilePath.Straight:
                rb.linearVelocity = direction * data.projectileSpeed;
                break;

            case ProjectileData.ProjectilePath.Arc:
                rb.gravityScale = 1f;
                rb.linearVelocity = new Vector2(direction.x * data.projectileSpeed, data.projectileSpeed);
                break;

            case ProjectileData.ProjectilePath.Homing:
                rb.linearVelocity = direction * data.projectileSpeed; // Launch forward
                Invoke(nameof(StartHoming), 0f); // Delay before homing begins
                break;


            case ProjectileData.ProjectilePath.ZigZag:
                rb.linearVelocity = direction * data.projectileSpeed;
                zigzagTime = 0f;
                break;

            case ProjectileData.ProjectilePath.Random:
                randomOffset = new Vector2(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f));
                rb.linearVelocity = (direction + randomOffset).normalized * data.projectileSpeed;
                break;
            case ProjectileData.ProjectilePath.Circular:
                casterStartPosition = caster.position;
                circularAngle = 0f;
                rb.bodyType = RigidbodyType2D.Kinematic; // We control movement manually
                break;

        }
    }

    private void Update()
    {
        if (data.movementPath == ProjectileData.ProjectilePath.ZigZag)
        {
            zigzagTime += Time.deltaTime;
            float offset = Mathf.Sin(zigzagTime * zigzagFrequency) * zigzagAmplitude;
            Vector2 side = Vector2.Perpendicular(direction).normalized;
            Vector2 zigzagVelocity = (direction * data.projectileSpeed) + (side * offset);
            rb.linearVelocity = zigzagVelocity;
        }
        if (data.movementPath == ProjectileData.ProjectilePath.Circular)
        {
            circularAngle += circularSpeed * Time.deltaTime;        // Rotate
            circularRadius += radialSpeed * Time.deltaTime;         // Move outward

            float x = Mathf.Cos(circularAngle) * circularRadius;
            float y = Mathf.Sin(circularAngle) * circularRadius;

            transform.position = casterStartPosition + new Vector3(x, y, 0f);
        }

    }
    private void StartHoming()
    {
        transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        InvokeRepeating(nameof(UpdateHoming), 0f, 0.1f); // Start tracking target
    }

    private void UpdateHoming()
    {
        GameObject target = GetClosestEnemyInRadius(6f);
        if (target == null) return;

        Vector2 toTarget = (target.transform.position - transform.position).normalized;
        rb.linearVelocity = toTarget * data.projectileSpeed;

        float angle = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //transform.localScale = new Vector3(Mathf.Sign(toTarget.x)*transform.localScale.x, transform.localScale.y, transform.localScale.z); // Flip based on direction

    }


    private GameObject GetClosestEnemyInRadius(float radius)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Enemy"));
        GameObject closest = null;
        float closestDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(transform.position, hit.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = hit.gameObject;
            }
        }

        return closest;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(data.damage, caster.position);
            }

            Destroy(gameObject);
        }
    }
}
