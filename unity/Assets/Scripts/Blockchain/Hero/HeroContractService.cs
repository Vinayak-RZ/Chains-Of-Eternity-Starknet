// HeroContractService.cs
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using UnityEngine;

public class HeroContractService : MonoBehaviour
{
    public static HeroContractService Instance { get; private set; }

    private Web3 web3;
    private Contract heroContract;
    private Account account;

    private string contractAddress = "0xbD1f8F2Fa5B33baB19604fCDC3315d070711DfeD";
    private string contractABI;
    private string rpcUrl = "https://api.avax-test.network/ext/bc/C/rpc";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        LoadContractABI();
        InitializeContract();
    }

    private void LoadContractABI()
    {
        try
        {
            string contractJson = Resources.Load("ABI/HeroABI")?.ToString();
            if (string.IsNullOrEmpty(contractJson))
            {
                Debug.LogError("Contract JSON file not found");
                return;
            }
            JObject contractData = JObject.Parse(contractJson);
            contractABI = contractData["abi"]?.ToString() ?? contractJson;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load ABI: {e.Message}");
        }
    }

    public void InitializeContract()
    {
        //try
        //{
        //    string privateKey = Web3AuthManager.Instance.GetPrivateKey();
        //    if (string.IsNullOrEmpty(privateKey))
        //    {
        //        Debug.LogError("Private key missing.");
        //        return;
        //    }

        //    account = new Account(privateKey);
        //    web3 = new Web3(account, rpcUrl);
        //    heroContract = web3.Eth.GetContract(contractABI, contractAddress);
        //}
        //catch (Exception e)
        //{
        //    Debug.LogError($"Initialization failed: {e.Message}");
        //}

        //Debug.Log("HeroContractService initialized successfully.");
    }

    public async Task<string> MintHero(string playerName, string raceName, List<uint> resistances, string tokenURI)
    {
        var mintFn = heroContract.GetFunction("mintHero");
        var receipt = await mintFn.SendTransactionAndWaitForReceiptAsync(
            from: account.Address,
            gas: new HexBigInteger(1000000),
            value: null,
            functionInput: new object[] {
                playerName, account.Address, account.Address.ToString(), raceName, resistances.ToArray(), tokenURI
            });
        return receipt.TransactionHash;
    }

    public async Task<HeroDataRaw> GetFullHeroData(BigInteger tokenId)
    {
        var fn = heroContract.GetFunction("getHeroData");
        return await fn.CallDeserializingToObjectAsync<HeroDataRaw>(tokenId);
    }

    public HeroData ConvertToUnityHeroData(HeroDataRaw raw)
    {
        var hero = ScriptableObject.CreateInstance<HeroData>();

        hero.playerName = raw.PlayerName;
        hero.playerID = raw.PlayerID;
        hero.level = (int)raw.Level;
        hero.isBanned = raw.IsBanned;
        hero.raceName = raw.RaceName;
        hero.equippedItems = new List<string>(raw.EquippedItems ?? new List<string>());

        var stats = raw.Stats;
        if (stats != null)
        {
            var off = stats.OffensiveStats;
            var def = stats.DefensiveStats;
            var spec = stats.SpecialStats;
            var main = stats.StatPointsAssigned;

            hero.offensiveStats = new OffensiveStats
            {
                damage = (int)off.Damage,
                attackSpeed = (int)off.AttackSpeed,
                criticalRate = (int)off.CriticalRate,
                criticalDamage = (int)off.CriticalDamage
            };

            hero.defensiveStats = new DefensiveStats
            {
                maxHealth = (int)def.MaxHealth,
                defense = (int)def.Defense,
                healthRegeneration = (int)def.HealthRegeneration,
                resistances = def.Resistances.ConvertAll(r => (int)r)
            };

            hero.specialStats = new SpecialStats
            {
                maxEnergy = (int)spec.MaxEnergy,
                energyRegeneration = (int)spec.EnergyRegeneration,
                maxMana = (int)spec.MaxMana,
                manaRegeneration = (int)spec.ManaRegeneration
            };

            hero.statPointsAssigned = new StatPointsAssigned
            {
                constitution = (int)main.Constitution,
                strength = (int)main.Strength,
                dexterity = (int)main.Dexterity,
                intelligence = (int)main.Intelligence,
                stamina = (int)main.Stamina,
                agility = (int)main.Agility,
                remainingPoints = (int)main.RemainingPoints
            };
        }

        return hero;
    }


    public async Task<HeroBasicInfoOutput> GetHeroBasicInfo(BigInteger tokenId)
    {
        var fn = heroContract.GetFunction("getHeroBasicInfo");
        return await fn.CallDeserializingToObjectAsync<HeroBasicInfoOutput>(tokenId);
    }
    public async Task<OffensiveStatsOutput> GetHeroOffensiveStats(BigInteger tokenId)
    {
        var fn = heroContract.GetFunction("getHeroOffensiveStats");
        return await fn.CallDeserializingToObjectAsync<OffensiveStatsOutput>(tokenId);
    }

    [FunctionOutput]
    public class OffensiveStatsOutput : IFunctionOutputDTO
    {
        [Parameter("uint256", "damage", 1)]
        public uint Damage { get; set; }

        [Parameter("uint256", "attackSpeed", 2)]
        public uint AttackSpeed { get; set; }

        [Parameter("uint256", "criticalRate", 3)]
        public uint CriticalRate { get; set; }

        [Parameter("uint256", "criticalDamage", 4)]
        public uint CriticalDamage { get; set; }
    }


    public async Task<DefensiveStatsOutput> GetHeroDefensiveStats(BigInteger tokenId)
    {
        var fn = heroContract.GetFunction("getHeroDefensiveStats");
        return await fn.CallDeserializingToObjectAsync<DefensiveStatsOutput>(tokenId);
    }

    [FunctionOutput]
    public class DefensiveStatsOutput : IFunctionOutputDTO
    {
        [Parameter("uint256", "maxHealth", 1)]
        public uint MaxHealth { get; set; }

        [Parameter("uint256", "defense", 2)]
        public uint Defense { get; set; }

        [Parameter("uint256", "healthRegeneration", 3)]
        public uint HealthRegeneration { get; set; }

        [Parameter("uint256[]", "resistances", 4)]
        public List<uint> Resistances { get; set; }
    }


    public async Task<SpecialStatsOutput> GetHeroSpecialStats(BigInteger tokenId)
    {
        var fn = heroContract.GetFunction("getHeroSpecialStats");
        return await fn.CallDeserializingToObjectAsync<SpecialStatsOutput>(tokenId);
    }

    [FunctionOutput]
    public class SpecialStatsOutput : IFunctionOutputDTO
    {
        [Parameter("uint256", "maxEnergy", 1)]
        public uint MaxEnergy { get; set; }

        [Parameter("uint256", "energyRegeneration", 2)]
        public uint EnergyRegeneration { get; set; }

        [Parameter("uint256", "maxMana", 3)]
        public uint MaxMana { get; set; }

        [Parameter("uint256", "manaRegeneration", 4)]
        public uint ManaRegeneration { get; set; }
    }


    public async Task<StatPointsAssignedOutput> GetHeroStatPoints(BigInteger tokenId)
    {
        var fn = heroContract.GetFunction("getHeroStatPoints");
        return await fn.CallDeserializingToObjectAsync<StatPointsAssignedOutput>(tokenId);
    }

    [FunctionOutput]
    public class StatPointsAssignedOutput : IFunctionOutputDTO
    {
        [Parameter("uint256", "constitution", 1)]
        public uint Constitution { get; set; }

        [Parameter("uint256", "strength", 2)]
        public uint Strength { get; set; }

        [Parameter("uint256", "dexterity", 3)]
        public uint Dexterity { get; set; }

        [Parameter("uint256", "intelligence", 4)]
        public uint Intelligence { get; set; }

        [Parameter("uint256", "stamina", 5)]
        public uint Stamina { get; set; }

        [Parameter("uint256", "agility", 6)]
        public uint Agility { get; set; }

        [Parameter("uint256", "remainingPoints", 7)]
        public uint RemainingPoints { get; set; }
    }


    public async Task<List<string>> GetHeroEquippedItems(BigInteger tokenId)
    {
        var fn = heroContract.GetFunction("getHeroEquippedItems");
        return await fn.CallAsync<List<string>>(tokenId);
    }

    public async Task<HeroDataRaw> GetHeroData(BigInteger tokenId)
    {
        var fn = heroContract.GetFunction("getHeroData");
        return await fn.CallDeserializingToObjectAsync<HeroDataRaw>(tokenId);
    }

    public async Task<List<BigInteger>> GetHeroesByOwner(string owner)
    {
        var fn = heroContract.GetFunction("getHeroesByOwner");
        return await fn.CallAsync<List<BigInteger>>(owner);
    }

    public async Task<List<BigInteger>> GetActiveHeroesByOwner(string owner)
    {
        var fn = heroContract.GetFunction("getActiveHeroesByOwner");
        return await fn.CallAsync<List<BigInteger>>(owner);
    }

    public async Task<uint> GetHeroCountByOwner(string owner)
    {
        var fn = heroContract.GetFunction("getHeroCountByOwner");
        return await fn.CallAsync<uint>(owner);
    }

    public async Task<(string[], uint[], bool[], string[])> GetMultipleHeroesBasicInfo(BigInteger[] tokenIds)
    {
        var fn = heroContract.GetFunction("getMultipleHeroesBasicInfo");
        return await fn.CallAsync<(string[], uint[], bool[], string[])>(tokenIds);
    }

    public async Task<bool> HeroExists(BigInteger tokenId)
    {
        var fn = heroContract.GetFunction("heroExists");
        return await fn.CallAsync<bool>(tokenId);
    }

    public async Task<string> BanHero(BigInteger tokenId)
    {
        var fn = heroContract.GetFunction("banHero");
        var receipt = await fn.SendTransactionAndWaitForReceiptAsync(
            from: account.Address,
            gas: new HexBigInteger(200000),
            value: null,
            functionInput: new object[] { tokenId });
        return receipt.TransactionHash;
    }

    public async Task<string> UnbanHero(BigInteger tokenId)
    {
        var fn = heroContract.GetFunction("unbanHero");
        var receipt = await fn.SendTransactionAndWaitForReceiptAsync(
            from: account.Address,
            gas: new HexBigInteger(200000),
            value: null,
            functionInput: new object[] { tokenId });
        return receipt.TransactionHash;
    }

    public async Task<string> UpdateFullHeroStats(BigInteger tokenId, HeroData heroData)
    {
        string statsTx = await UpdateHeroStats(
            tokenId,
            (uint)heroData.level,
            (uint)heroData.offensiveStats.damage,
            (uint)heroData.offensiveStats.attackSpeed,
            (uint)heroData.offensiveStats.criticalRate,
            (uint)heroData.offensiveStats.criticalDamage,
            (uint)heroData.defensiveStats.maxHealth,
            (uint)heroData.defensiveStats.defense,
            (uint)heroData.defensiveStats.healthRegeneration,
            heroData.defensiveStats.resistances.ConvertAll(x => (uint)x),
            (uint)heroData.specialStats.maxEnergy,
            (uint)heroData.specialStats.energyRegeneration,
            (uint)heroData.specialStats.maxMana,
            (uint)heroData.specialStats.manaRegeneration
        );

        string mainStatsTx = await UpdateHeroMainStats(
            tokenId,
            (uint)heroData.statPointsAssigned.constitution,
            (uint)heroData.statPointsAssigned.strength,
            (uint)heroData.statPointsAssigned.dexterity,
            (uint)heroData.statPointsAssigned.intelligence,
            (uint)heroData.statPointsAssigned.stamina,
            (uint)heroData.statPointsAssigned.agility,
            (uint)heroData.statPointsAssigned.remainingPoints
        );

        string itemTx = await UpdateHeroItems(tokenId, heroData.equippedItems);

        return $"Stats: {statsTx}, Main: {mainStatsTx}, Items: {itemTx}";
    }



    public async Task<string> UpdateHeroStats(BigInteger tokenId, uint level, uint dmg, uint atkSpd, uint critRate, uint critDmg,
        uint maxHp, uint def, uint regen, List<uint> resistances, uint maxEnergy, uint enRegen, uint maxMana, uint manaRegen)
    {
        var fn = heroContract.GetFunction("updateHeroStats");
        var receipt = await fn.SendTransactionAndWaitForReceiptAsync(
            from: account.Address,
            gas: new HexBigInteger(400000),
            value: null,
            functionInput: new object[] {
                tokenId, level, dmg, atkSpd, critRate, critDmg, maxHp, def, regen, resistances.ToArray(), maxEnergy, enRegen, maxMana, manaRegen
            });
        return receipt.TransactionHash;
    }

    public async Task<string> UpdateHeroMainStats(BigInteger tokenId, uint con, uint str, uint dex, uint intl, uint stam, uint agi, uint rem)
    {
        var fn = heroContract.GetFunction("updateHeroMainStats");
        var receipt = await fn.SendTransactionAndWaitForReceiptAsync(
            from: account.Address,
            gas: new HexBigInteger(300000),
            value: null,
            functionInput: new object[] {
                tokenId, con, str, dex, intl, stam, agi, rem
            });
        return receipt.TransactionHash;
    }

    public async Task<string> UpdateHeroItems(BigInteger tokenId, List<string> newItems)
    {
        var fn = heroContract.GetFunction("updateHeroItems");
        var receipt = await fn.SendTransactionAndWaitForReceiptAsync(
            from: account.Address,
            gas: new HexBigInteger(300000),
            value: null,
            functionInput: new object[] {
                tokenId, newItems.ToArray()
            });
        return receipt.TransactionHash;
    }
}

[FunctionOutput]
public class HeroDataRaw : IFunctionOutputDTO
{
    [Parameter("string", "playerName", 1)]
    public string PlayerName { get; set; }

    [Parameter("string", "playerID", 2)]
    public string PlayerID { get; set; }

    [Parameter("uint256", "level", 3)]
    public BigInteger Level { get; set; }

    [Parameter("bool", "isBanned", 4)]
    public bool IsBanned { get; set; }

    [Parameter("string", "raceName", 5)]
    public string RaceName { get; set; }

    [Parameter("string[]", "equippedItems", 6)]
    public List<string> EquippedItems { get; set; }

    [Parameter("tuple", "stats", 7)]
    public StatsRaw Stats { get; set; }
}

[FunctionOutput]
public class StatsRaw : IFunctionOutputDTO
{
    [Parameter("tuple", "offensiveStats", 1)]
    public OffensiveStatsRaw OffensiveStats { get; set; }

    [Parameter("tuple", "defensiveStats", 2)]
    public DefensiveStatsRaw DefensiveStats { get; set; }

    [Parameter("tuple", "specialStats", 3)]
    public SpecialStatsRaw SpecialStats { get; set; }

    [Parameter("tuple", "statPointsAssigned", 4)]
    public StatPointsAssignedRaw StatPointsAssigned { get; set; }
}

[FunctionOutput]
public class OffensiveStatsRaw : IFunctionOutputDTO
{
    [Parameter("uint32", "damage", 1)]
    public uint Damage { get; set; }

    [Parameter("uint32", "attackSpeed", 2)]
    public uint AttackSpeed { get; set; }

    [Parameter("uint32", "criticalRate", 3)]
    public uint CriticalRate { get; set; }

    [Parameter("uint32", "criticalDamage", 4)]
    public uint CriticalDamage { get; set; }
}

[FunctionOutput]
public class DefensiveStatsRaw : IFunctionOutputDTO
{
    [Parameter("uint32", "maxHealth", 1)]
    public uint MaxHealth { get; set; }

    [Parameter("uint32", "defense", 2)]
    public uint Defense { get; set; }

    [Parameter("uint32", "healthRegeneration", 3)]
    public uint HealthRegeneration { get; set; }

    [Parameter("uint32[]", "resistances", 4)]
    public List<uint> Resistances { get; set; }
}

[FunctionOutput]
public class SpecialStatsRaw : IFunctionOutputDTO
{
    [Parameter("uint32", "maxEnergy", 1)]
    public uint MaxEnergy { get; set; }

    [Parameter("uint32", "energyRegeneration", 2)]
    public uint EnergyRegeneration { get; set; }

    [Parameter("uint32", "maxMana", 3)]
    public uint MaxMana { get; set; }

    [Parameter("uint32", "manaRegeneration", 4)]
    public uint ManaRegeneration { get; set; }
}

[FunctionOutput]
public class StatPointsAssignedRaw : IFunctionOutputDTO
{
    [Parameter("uint32", "constitution", 1)]
    public uint Constitution { get; set; }

    [Parameter("uint32", "strength", 2)]
    public uint Strength { get; set; }

    [Parameter("uint32", "dexterity", 3)]
    public uint Dexterity { get; set; }

    [Parameter("uint32", "intelligence", 4)]
    public uint Intelligence { get; set; }

    [Parameter("uint32", "stamina", 5)]
    public uint Stamina { get; set; }

    [Parameter("uint32", "agility", 6)]
    public uint Agility { get; set; }

    [Parameter("uint32", "remainingPoints", 7)]
    public uint RemainingPoints { get; set; }
}

[FunctionOutput]
public class HeroBasicInfoOutput : IFunctionOutputDTO
{
    [Parameter("string", "playerName", 1)]
    public string PlayerName { get; set; }

    [Parameter("uint256", "level", 2)]
    public uint Level { get; set; }

    [Parameter("bool", "isBanned", 3)]
    public bool IsBanned { get; set; }

    [Parameter("string", "raceName", 4)]
    public string RaceName { get; set; }
}
