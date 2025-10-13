using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;

public static class SpellFactory
{
    public static void CastSpell(SpellObject spell, Transform caster)
    {
        if (spell.category == SpellCategory.Attack)
        {
            switch (spell.attackSubtype)
            {
                case AttackSubtype.Projectile:
                    CreateProjectileSpell(spell.attackData.projectileData, spell.visualPrefab, caster);
                    break;

                case AttackSubtype.AoE:
                    CreateAoESpell(spell.attackData.aoeData, spell.visualPrefab, caster);
                    break;

                case AttackSubtype.ShortRange:
                    CreateShortRangeSpell(spell.attackData.shortRangeData, spell.visualPrefab, caster);
                    break;
            }
        }
        else if (spell.category == SpellCategory.Buff)
        {
            ApplyBuff(spell.buffData, caster.gameObject);
        }
    }

    public static void CreateProjectileSpell(ProjectileData data, GameObject prefab, Transform caster)
    {
        if (data.delayBetweenProjectiles > 0f)
        {
            caster.GetComponent<MonoBehaviour>().StartCoroutine(SpawnProjectilesWithDelay(data, prefab, caster));
        }
        else
        {
            for (int i = 0; i < data.directions.Count; i++)
            {           
                bool isFacingRight = caster.localScale.x > 0;
                bool flip =false;
                Vector2 baseDir = data.directions[i];
                if ((!isFacingRight && baseDir.x > 0) || (isFacingRight && baseDir.x < 0))
                    flip = true; // Flip direction 
                SpawnOneProjectile(data, prefab, caster, baseDir, i,flip);
            }

        }
    }
    private static IEnumerator SpawnProjectilesWithDelay(ProjectileData data, GameObject prefab, Transform caster)
    {
        bool isFacingRight = caster.localScale.x > 0;

        for (int i = 0; i < data.directions.Count; i++)
        {
            bool flip =false;
            Vector2 baseDir = data.directions[i];
            if ((!isFacingRight && baseDir.x > 0) || (isFacingRight && baseDir.x < 0))
                flip = true; // Flip direction 
            SpawnOneProjectile(data, prefab, caster, baseDir, i,flip);   
            yield return new WaitForSeconds(data.delayBetweenProjectiles);
        }

    }
    private  static void SpawnOneProjectile(ProjectileData data, GameObject prefab, Transform caster, Vector2 direction, int index,bool flip = false)
    {   
        float Angle = 0f;
        Vector2 offset = Vector2.zero;
        if (index < data.spawnOffsets.Count)
            offset = data.spawnOffsets[index];

        Vector3 spawnPos = caster.position + (Vector3)offset;
        if (flip)
        {
            Angle = 180; // Flip the direction if needed
        }
        GameObject projectile = GameObject.Instantiate(prefab, spawnPos, Quaternion.Euler(0, Angle,0));
        var runtime = projectile.AddComponent<ProjectileSpellRuntime>();
        runtime.Initialize(data, direction, caster);
    }


    private static void CreateAoESpell(AoEData data, GameObject prefab, Transform caster)
    {
        // To be implemented
    }

    private static void CreateShortRangeSpell(ShortRangeData data, GameObject prefab, Transform caster)
    {
        // To be implemented
    }

    private static void ApplyBuff(BuffData data, GameObject target)
    {
        // To be implemented
    }
}
