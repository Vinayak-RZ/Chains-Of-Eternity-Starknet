using Org.BouncyCastle.Security;
using System.Collections.Generic;
using UnityEngine;

public class attackHandler : MonoBehaviour
{
    [SerializeField] private BoxCollider2D attackHitbox; // Reference to the attack hitbox GameObject
    [SerializeField] private LayerMask enemyLayer; // Layer mask for enemies
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>(); // Track hit enemies to avoid multiple hits
    [SerializeField] private HeroData heroData;
    private Enemy enemy;
    [SerializeField] bool dealsKnockback = true;
    [SerializeField] bool dealsStun = false;
    [SerializeField] string weaponType = "Physical";
    [SerializeField] private Player player;

    private void Awake()
    {
        attackHitbox = GetComponent<BoxCollider2D>();
        player = GetComponentInParent<Player>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!attackHitbox.enabled) return;

        if (enemyLayer == (enemyLayer | (1 << other.gameObject.layer)) &&
            !hitEnemies.Contains(other.gameObject))
        {
            hitEnemies.Add(other.gameObject);
            //Debug.Log($"Hit enemy: {other.gameObject.name}");

            enemy = other.GetComponentInParent<Enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage(heroData.offensiveStats.damage, player.transform.position, dealsKnockback, dealsStun, weaponType);
            }


            //other.GetComponent<EnemyHealth>()?.TakeDamage(heroData.offensiveStats.attackPower);
        }
    }

    public void ClearHitEnemies()
    {
        hitEnemies.Clear();
    }
}
