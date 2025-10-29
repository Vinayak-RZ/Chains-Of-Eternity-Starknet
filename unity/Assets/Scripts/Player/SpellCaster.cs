using Unity.VisualScripting;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

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
        // await DojoActions.SpawnPlayer();
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

        Debug.Log($"Casting spell from slot {index + 1}: {spell.name}, Mana cost: {spell.manaCost}, Remaining Mana: {playerstats.currentMana}");

        // 2. Call blockchain function in background (non-blocking)
        _ = CallFireSpell(spell.attackData.projectileData.damage);
    }

    private async Task CallFireSpell(float damage)
    {
        try
        {
            // This runs in the background without blocking gameplay
            var result = await DojoActions.TakeDamage((ushort)damage);
            Debug.Log($"Player took {damage} damage. Tx: {result.Inner}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error in CallFireSpell: {ex}");
        }
    }
}