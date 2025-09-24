using UnityEngine;

public enum ElementType { Fire, Water, Earth, Wind }
public enum SpellCategory { Attack, Buff }
public enum AttackSubtype { AoE, Projectile, ShortRange }

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell System/Spell")]
public class SpellObject : ScriptableObject
{
    [Header("Core Info")]
    public string spellName;
    public ElementType element;
    public SpellCategory category;
    public GameObject visualPrefab;
    public float manaCost;
    public float cooldown;

    [Header("Attack Type")]
    public AttackSubtype attackSubtype;

    [Header("Nested Data")]
    public AttackData attackData;
    public BuffData buffData;
}
