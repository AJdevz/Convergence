using UnityEngine;

public enum SkillEffectType
{
    Damage,
    FireRate,
    CritChance,
    CritMultiplier,

    MaxHP,
    DamageReduction,
    LifeSteal,

    XPMultiplier,
    XPRange,
    CoinMultiplier,
    Luck,
    SecretChance,

    MoveSpeedOnKill
}

[CreateAssetMenu(menuName = "Skill")]
public class SkillData : ScriptableObject
{
    public string skillID;
    public string displayName;
    public int cost;

    public SkillEffectType effectType;

    public float value;
}