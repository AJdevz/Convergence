using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int totalCoins;

    public List<string> unlockedSkills = new List<string>();

    // DAMAGE
    public float damageMultiplier = 1f;
    public float fireRateMultiplier = 1f;

    public float critChance = 0f;
    public float critMultiplier = 2f;

    // DEFENSE
    public float maxHPBonus = 0f;
    public float damageReduction = 0f;
    public float lifesteal = 0f; // ✅ FIXED (lowercase)

    // UTILITY
    public float xpMultiplier = 1f;
    public int magnetLevel = 0;  // ✅ ADD THIS

    public float coinMultiplier = 1f;

    public float luck = 0f;
    public float secretChance = 0f;

    public float moveSpeedBonus = 0f;
}