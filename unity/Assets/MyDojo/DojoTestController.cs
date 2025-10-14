using UnityEngine;
using Dojo.Starknet;
using Dojo;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

public class DojoTestController : MonoBehaviour
{
    [Header("Starknet Settings")]
    public string rpcUrl = "https://starknet-sepolia.g.alchemy.com/starknet/version/rpc/v0_9/WiJmx2NG7dz1PEyksSPP2";
    public string accountAddress = "<YOUR_ACCOUNT_ADDRESS>";
    public string privateKey = "<YOUR_PRIVATE_KEY>";
    public string actionsContract = "<ACTIONS_CONTRACT_ADDRESS>";

    [Header("Test Options")]
    public TestFunction functionToTest = TestFunction.CreateSpell;

    private Account account;
    private Actions actions;

    public enum TestFunction
    {
        SpawnPlayer,
        UpdatePlayerState,
        CreateSpell,
        EquipSpell,
        TakeDamage
    }

    async void Start()
    {
        try
        {
            await InitializeAsync();

            switch (functionToTest)
            {
                case TestFunction.SpawnPlayer:
                    await TestSpawnPlayer();
                    break;

                case TestFunction.UpdatePlayerState:
                    await TestUpdatePlayerState();
                    break;

                case TestFunction.CreateSpell:
                    await TestCreateSpell();
                    break;

                case TestFunction.EquipSpell:
                    await TestEquipSpell();
                    break;

                case TestFunction.TakeDamage:
                    await TestTakeDamage();
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Initialization or test failed: {ex.Message}");
        }
    }

    async Task InitializeAsync()
    {
        Debug.Log("🔗 Connecting to Starknet...");
        var provider = new JsonRpcClient(rpcUrl);
        var signer = new SigningKey(privateKey);
        account = new Account(provider, signer, new FieldElement(accountAddress));

        actions = gameObject.AddComponent<Actions>();
        actions.contractAddress = actionsContract;

        Debug.Log("✅ Connected to Starknet & Actions contract initialized!");
    }

    // -----------------------------------------------------------------------
    // 🧍 spawn_player
    // -----------------------------------------------------------------------
    async Task TestSpawnPlayer()
    {
        Debug.Log("🧍 Testing spawn_player()...");
        var txHash = await actions.spawn_player(account);
        Debug.Log($"✅ Player spawned. Tx: {txHash.Inner}");
    }

    // -----------------------------------------------------------------------
    // 🧭 update_player_state
    // -----------------------------------------------------------------------
    async Task TestUpdatePlayerState()
    {
        Debug.Log("🧭 Testing update_player_state()...");
        var txHash = await actions.update_player_state(
            account,
            new PlayerFSMState.Idle(), // Example enum
            pos_x: 80,
            pos_y: 200,
            facing_dir: 0,
            velocity: 10
        );  
        Debug.Log($"✅ Player state updated. Tx: {txHash.Inner}");
    }

    // -----------------------------------------------------------------------
    // 🧙 create_spell
    // -----------------------------------------------------------------------
    async Task TestCreateSpell()
    {
        Debug.Log("🧙 Testing create_spell()...");

        FieldElement spell_id = new FieldElement(2);
        SpellElement element = new SpellElement.Water();
        AttackSubtype attackSubtype = new AttackSubtype.Projectile();

        ushort damage = 30;
        ushort knockback = 20;
        ushort projectile_speed = 100;
        ushort projectile_size = 5;
        byte number_of_projectiles = 3;
        short staggered_angle = 15;
        ushort zigzag_amplitude = 2;
        ushort zigzag_frequency = 3;
        ushort homing_radius = 10;
        ushort arc_gravity = 1;
        ushort random_offset = 0;
        ushort circular_speed = 0;
        ushort circular_radius = 0;
        ushort mana_cost = 25;
        ushort cooldown = 5;

        var txHash = await actions.create_spell(
            account,
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

        Debug.Log($"✅ Spell created successfully. Tx: {txHash.Inner}");
    }

    // -----------------------------------------------------------------------
    // 🪄 equip_spell
    // -----------------------------------------------------------------------
    async Task TestEquipSpell()
    {
        Debug.Log("🪄 Testing equip_spell()...");
        FieldElement spell_id = new FieldElement(1);
        var txHash = await actions.equip_spell(account, spell_id);
        Debug.Log($"✅ Spell equipped. Tx: {txHash.Inner}");
    }

    // -----------------------------------------------------------------------
    // 💥 take_damage
    // -----------------------------------------------------------------------
    async Task TestTakeDamage()
    {
        Debug.Log("💥 Testing take_damage()...");
        ushort damage = 25;
        var txHash = await actions.take_damage(account, damage);
        Debug.Log($"✅ Damage applied. Tx: {txHash.Inner}");
    }
}
