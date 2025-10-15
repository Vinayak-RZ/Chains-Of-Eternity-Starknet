using Unity.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using Dojo.Starknet;

public class SpellCaster : MonoBehaviour
{
    public SpellObject[] spellbook; // Assign in Inspector (size = 8)
    public PlayerStats playerstats; // Reference to the Player script
    private float Cooldown = 0f;

    async void Start()
    {
        // Wait until the manager is initialized
        while (!DojoManager.Instance.IsInitialized)
            await Task.Yield();

        // await DojoActions.CreateSpell();
        await DojoActions.SpawnPlayer();
    }

    void Update()
    {
        Cooldown -= Time.deltaTime;

        // Loop through 8 spell slots
        for (int i = 0; i < spellbook.Length && i < 8; i++)
        {
            if (spellbook[i] == null) continue; // Skip empty slots

            // Map keys 1-8 -> Alpha1 to Alpha8
            if (Cooldown < 0f)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    TryCastSpell(i);
                }
            }
        }
    }

    private async void TryCastSpell(int index)
    {
        SpellObject spell = spellbook[index];

        if (playerstats.currentMana < spell.manaCost)
        {
            Debug.LogWarning($"Not enough mana to cast spell in slot {index + 1}.");
            return;
        }

        // 1. Cast spell instantly (gameplay effect)
        SpellFactory.CastSpell(spell, transform);
        playerstats.currentMana.value -= (int)spell.manaCost;
        Cooldown = spell.cooldown;
        Vector2 baseDir = GetMouseDirection(transform).normalized;
        float angle = Mathf.Atan2(transform.position.x, transform.position.x) * Mathf.Rad2Deg;
        Debug.Log($"Casting spell from slot {index + 1}: {spell.name}, Mana cost: {spell.manaCost}, Remaining Mana: {playerstats.currentMana}");

        // 2. Call blockchain function in background (non-blocking)
        _ = CallFireSpell(spell.spell_id, angle);
    }

    private async Task CallFireSpell(FieldElement spell_id,float angle)
    {
        try
        {
            // This runs in the background without blocking gameplay
            var result = await DojoActions.FireSpell(spell_id,(int)transform.position.x,(int)transform.position.y,(short)angle);
            Debug.Log($"Fired Spell {spell_id} . Tx: {result.Inner}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in CallFireSpell: {ex}");
        }
    }
        private static Vector2 GetMouseDirection(Transform caster)
    {
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0f;
        return ((Vector2)(mouseWorld - caster.position)).normalized;
    }
}