using UnityEngine;

public class ProjectileSpellRuntime : MonoBehaviour
{
    private ProjectileData data;
    private Vector2 direction;
    private Transform caster;
    private Rigidbody2D rb;

    public void Initialize(ProjectileData data, Vector2 direction, Transform caster)
    {
        this.data = data;
        this.direction = direction.normalized;
        this.caster = caster;
        rb = GetComponent<Rigidbody2D>();

        // Launch the projectile
        rb.linearVelocity = this.direction * data.projectileSpeed;

        // Auto-destroy after 5 seconds
        Destroy(gameObject, 5f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Apply damage
            Enemy enemy = other.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(10,caster.position); // Call the base method; actual method from derived class is executed
            }

            Destroy(gameObject); // destroy projectile
        }
    }
}
