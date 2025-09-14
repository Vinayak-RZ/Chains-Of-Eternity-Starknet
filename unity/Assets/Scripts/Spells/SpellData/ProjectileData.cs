using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProjectileData
{
    public enum ProjectilePath { Straight, ZigZag, Random }

    [Header("Projectile Settings")]
    public int numberOfProjectiles = 1;
    public float projectileSpeed = 15f;
    public float damage = 25f;
    public List<Vector2> directions = new List<Vector2> { Vector2.right }; // default forward
    public ProjectilePath movementPath = ProjectilePath.Straight;
}