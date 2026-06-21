using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int totalCoins;

    public List<string> unlockedSkills = new List<string>();

    // RUNTIME STATS ONLY

    [HideInInspector] public float damageMultiplier;
    [HideInInspector] public float fireRateMultiplier;

    [HideInInspector] public float critChance;
    [HideInInspector] public float critMultiplier;

    [HideInInspector] public float maxHPBonus;
    [HideInInspector] public float damageReduction;
    [HideInInspector] public float lifesteal;

    [HideInInspector] public float xpMultiplier;
    [HideInInspector] public int magnetLevel;

    [HideInInspector] public float coinMultiplier;

    [HideInInspector] public float luck;
    [HideInInspector] public float secretChance;

    [HideInInspector] public float moveSpeedBonus;


    // RESET STATS

    public void ResetRuntimeStats()
    {
        damageMultiplier = 1f;
        fireRateMultiplier = 1f;

        critChance = 0f;
        critMultiplier = 2f;

        maxHPBonus = 0f;
        damageReduction = 0f;
        lifesteal = 0f;

        xpMultiplier = 1f;
        magnetLevel = 0;

        coinMultiplier = 1f;

        luck = 0f;
        secretChance = 0f;

        moveSpeedBonus = 0f;
    }
}