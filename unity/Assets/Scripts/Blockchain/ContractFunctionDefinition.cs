using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using System;
using System.Collections.Generic;
using System.Numerics;

// Mint Hero Function
[Function("mintHero")]
public class MintHeroFunction : FunctionMessage
{
    [Parameter("string", "_playerName", 1)]
    public string PlayerName { get; set; }

    [Parameter("address", "_owner", 2)]
    public string Owner { get; set; }  // ADD THIS

    [Parameter("string", "_playerID", 3)]
    public string PlayerID { get; set; }

    [Parameter("string", "_raceName", 4)]
    public string RaceName { get; set; }

    [Parameter("uint32[]", "_resistances", 5)]
    public List<UInt32> Resistance { get; set; }  // Consider changing to List<uint32>

    [Parameter("string", "_tokenURI", 6)]
    public string TokenURI { get; set; }
}

// Get Hero Data Function
[Function("heroData", "tuple")]
public class HeroDataFunction : FunctionMessage
{
    [Parameter("uint256", "tokenId", 1)]
    public BigInteger TokenId { get; set; }
}

// Hero Data Output Structure
[FunctionOutput]
public class HeroDataOutput : IFunctionOutputDTO
{
    [Parameter("string", "playername", 1)]
    public string PlayerName { get; set; }

    [Parameter("string", "playerID", 2)]
    public string PlayerID { get; set; }

    [Parameter("uint32", "level", 3)]
    public uint Level { get; set; }

    [Parameter("string[]", "equippeditem", 4)]
    public List<string> EquippedItems { get; set; }

    [Parameter("tuple", "statstable", 5)]
    public StatsOutput StatsTable { get; set; }

    [Parameter("tuple", "racestable", 6)]
    public RaceOutput RaceTable { get; set; }

    [Parameter("bool", "isbanned", 7)]
    public bool IsBanned { get; set; }
}

[FunctionOutput]
public class StatsOutput : IFunctionOutputDTO
{
    [Parameter("tuple", "offstats", 1)]
    public OffensiveStatsOutput OffStats { get; set; }

    [Parameter("tuple", "defstats", 2)]
    public DefensiveStatsOutput DefStats { get; set; }

    [Parameter("tuple", "specstats", 3)]
    public SpecialStatsOutput SpecStats { get; set; }
}

[FunctionOutput]
public class OffensiveStatsOutput : IFunctionOutputDTO
{
    [Parameter("uint32", "damage", 1)]
    public uint Damage { get; set; }

    [Parameter("uint32", "attackspeed", 2)]
    public uint AttackSpeed { get; set; }

    [Parameter("uint32", "criticalrate", 3)]
    public uint CriticalRate { get; set; }

    [Parameter("uint32", "criticaldamage", 4)]
    public uint CriticalDamage { get; set; }
}

[FunctionOutput]
public class DefensiveStatsOutput : IFunctionOutputDTO
{
    [Parameter("uint32", "maxhealth", 1)]
    public uint MaxHealth { get; set; }

    [Parameter("uint32", "defense", 2)]
    public uint Defense { get; set; }

    [Parameter("uint32", "healthregeneration", 3)]
    public uint HealthRegeneration { get; set; }

    [Parameter("uint32[]", "resistances", 4)]
    public List<uint> Resistances { get; set; }
}

[FunctionOutput]
public class SpecialStatsOutput : IFunctionOutputDTO
{
    [Parameter("uint32", "maxenergy", 1)]
    public uint MaxEnergy { get; set; }

    [Parameter("uint32", "energyregeneration", 2)]
    public uint EnergyRegeneration { get; set; }

    [Parameter("uint32", "maxmana", 3)]
    public uint MaxMana { get; set; }

    [Parameter("uint32", "manaregeneration", 4)]
    public uint ManaRegeneration { get; set; }
}

[FunctionOutput]
public class RaceOutput : IFunctionOutputDTO
{
    [Parameter("string", "name", 1)]
    public string Name { get; set; }
}