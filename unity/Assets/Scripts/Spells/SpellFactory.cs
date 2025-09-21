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

    private static void CreateProjectileSpell(ProjectileData data, GameObject prefab, Transform caster)
    {
        foreach (Vector2 direction in data.directions)
        {
            GameObject projectile = GameObject.Instantiate(prefab, caster.position, Quaternion.identity);
            var runtime = projectile.AddComponent<ProjectileSpellRuntime>();
            runtime.Initialize(data, direction, caster);
        }
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
