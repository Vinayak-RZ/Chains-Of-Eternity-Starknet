using UnityEngine;
using Dojo.Starknet;
using Dojo;
using System.Threading.Tasks;

public static class DojoActions
{
    private static Account Account => DojoManager.Instance.Account;
    private static Actions Actions => DojoManager.Instance.Actions;

    private static bool IsReady => DojoManager.Instance != null && DojoManager.Instance.IsInitialized;

    private static void EnsureReady()
    {
        if (!IsReady)
            throw new System.Exception("❌ DojoManager not initialized. Wait for initialization before calling actions.");
    }

    public static async Task<FieldElement> SpawnPlayer()
    {
        EnsureReady();
        Debug.Log("🧍 Spawning player...");
        var tx = await Actions.spawn_player(Account);
        Debug.Log($"✅ Player spawned. Tx: {tx.Inner}");
        return tx;
    }

    public static async Task<FieldElement> UpdatePlayerState(
        PlayerFSMState state,
        ushort pos_x,
        ushort pos_y,
        byte facing_dir,
        short velocity
    )
    {
        EnsureReady();
        Debug.Log("🧭 Updating player state...");
        var tx = await Actions.update_player_state(
            Account,
            state,
            pos_x: pos_x,
            pos_y: pos_y,
            facing_dir: facing_dir,
            velocity: velocity
        );
        Debug.Log($"✅ Player state updated. Tx: {tx.Inner}");
        return tx;
    }

    public static async Task<FieldElement> CreateSpell(
        FieldElement spell_id,
        SpellElement element,
        AttackSubtype attackSubtype,
        ushort damage,
        ushort knockback,
        ushort projectile_speed,
        ushort projectile_size,
        byte number_of_projectiles,
        short staggered_angle,
        ushort zigzag_amplitude,
        ushort zigzag_frequency,
        ushort homing_radius,
        ushort arc_gravity,
        ushort random_offset,
        ushort circular_speed,
        ushort circular_radius,
        ushort mana_cost,
        ushort cooldown
    )
    {
        EnsureReady();
        Debug.Log("🧙 Creating spell...");

        var tx = await Actions.create_spell(
            Account,
            spell_id,
            element,
            attackSubtype,
            damage,
            knockback,
            projectile_speed,
            projectile_size,
            number_of_projectiles,
            staggered_angle,
            zigzag_amplitude,
            zigzag_frequency,
            homing_radius,
            arc_gravity,
            random_offset,
            circular_speed,
            circular_radius,
            mana_cost,
            cooldown
        );

        Debug.Log($"✅ Spell created successfully. Tx: {tx.Inner}");
        return tx;
    }

    public static async Task<FieldElement> EquipSpell(FieldElement spell_id)
    {
        EnsureReady();
        Debug.Log("🪄 Equipping spell...");
        var tx = await Actions.equip_spell(Account, spell_id);
        Debug.Log($"✅ Spell equipped. Tx: {tx.Inner}");
        return tx;
    }

    public static async Task<FieldElement> TakeDamage(ushort damage)
    {
        EnsureReady();
        Debug.Log("💥 Applying damage...");
        var tx = await Actions.take_damage(Account, damage);
        Debug.Log($"✅ Damage applied. Tx: {tx.Inner}");
        return tx;
    }
}