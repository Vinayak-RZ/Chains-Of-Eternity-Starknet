using UnityEngine;
using Dojo.Starknet;
using Dojo;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

public class DojoGameController : MonoBehaviour
{
    public string rpcUrl = "https://starknet-sepolia.g.alchemy.com/starknet/version/rpc/v0_9/WiJmx2NG7dz1PEyksSPP2";
    public string accountAddress = "<YOUR_ACCOUNT_ADDRESS>";
    public string privateKey = "<YOUR_PRIVATE_KEY>";
    public string actionsContract = "<ACTIONS_CONTRACT_ADDRESS>";

    private Account account;
    private Actions actions;

    async void Start()
    {
        // Initialize RPC + Account
        var provider = new JsonRpcClient(rpcUrl);
        var signer = new SigningKey(privateKey);
        account = new Account(provider, signer, new FieldElement(accountAddress));

        // Initialize actions contract
        actions = gameObject.AddComponent<Actions>();
        actions.contractAddress = actionsContract;

        Debug.Log("✅ Connected to Starknet & Dojo system!");

        // Test spell creation
        await TestCreateSpell();
    }

    async Task TestCreateSpell()
    {
        Debug.Log("🧙‍♂️ Testing create_spell() with dummy values...");

        // Dummy data
        FieldElement spell_id = new FieldElement(1);
        SpellElement element = new SpellElement.Fire(); // Example enum
        AttackSubtype attackSubtype = new AttackSubtype.Projectile(); // Example enum

        ushort damage = 50;
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

        try
        {
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

            Debug.Log($"✨ Spell creation tx: {txHash.Inner} ::: {txHash.ToString()}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ Spell creation failed: {ex.Message}");
        }
    }
}
