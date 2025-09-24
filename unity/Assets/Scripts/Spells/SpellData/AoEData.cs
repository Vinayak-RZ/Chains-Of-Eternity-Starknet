using System;
using UnityEngine;

[Serializable]
public class AoEData
{
    public enum AoEShape { Circle, Cone, Rectangle }

    [Header("AoE Settings")]
    public AoEShape shape;
    public float range = 5f;
    public float damagePerTick = 10f;
    public float tickInterval = 1f;
    public float duration = 5f;
}
