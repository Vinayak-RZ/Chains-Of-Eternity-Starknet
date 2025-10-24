using System.Collections.Generic;
using UnityEngine;

public class attackHandler : MonoBehaviour
{
    [SerializeField] private BoxCollider2D attackHitbox; // Reference to the attack hitbox GameObject
    [SerializeField] private LayerMask enemyLayer; // Layer mask for enemies
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // Track hit enemies to avoid multiple hits

    private void Awake()
    {
        attackHitbox = GetComponent<BoxCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!attackHitbox.enabled) return;

        if (enemyLayer == (enemyLayer | (1 << other.gameObject.layer)) &&
            !hitEnemies.Contains(other.gameObject))
        {
            hitEnemies.Add(other.gameObject);
            Debug.Log($"Hit enemy: {other.gameObject.name}");
            //other.GetComponent<EnemyHealth>()?.TakeDamage(heroData.offensiveStats.attackPower);
        }
    }

    public void ClearHitEnemies()
    {
        hitEnemies.Clear();
    }
}
