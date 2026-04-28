using UnityEngine;

public class SkillTreeManager : MonoBehaviour
{
    public static SkillTreeManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public bool CanUnlock(SkillData skill, SkillNodeUI node)
    {
        bool isFirstNode = node.previousNode == null;

        if (GameManager.Instance.playerData.unlockedSkills.Contains(skill.skillID))
            return false;

        if (CoinsManager.Instance.totalCoins < skill.cost)
            return false;

        if (!isFirstNode)
        {
            if (node.previousNode != null && !node.previousNode.IsUnlocked())
                return false;
        }

        return true;
    }

    public void UnlockSkill(SkillData skill)
    {
        Debug.Log("Trying to unlock: " + skill.skillID);

        if (GameManager.Instance.playerData.unlockedSkills.Contains(skill.skillID))
            return;

        if (CoinsManager.Instance.totalCoins < skill.cost)
        {
            Debug.Log("Not enough coins!");
            return;
        }

        CoinsManager.Instance.totalCoins -= skill.cost;

        Debug.Log("Coins deducted. Remaining: " + CoinsManager.Instance.totalCoins);

        GameManager.Instance.playerData.unlockedSkills.Add(skill.skillID);

        // ✅ APPLY TO PLAYER DATA
        ApplySkillEffect(skill);

        // 🔥 APPLY TO SCENE IMMEDIATELY (THIS IS WHAT YOU WERE MISSING)
        GameManager.Instance.ApplyStatsToScene();

        // 💾 SAVE PROGRESS
        GameManager.Instance.SaveGame();

        // 💰 UPDATE UI
        if (CoinsManager.Instance != null)
            CoinsManager.Instance.UpdateUI();

        // 🔄 refresh UI nodes
        RefreshAllNodes();
    }

    public void RefreshAllNodes()
    {
        var nodes = FindObjectsByType<SkillNodeUI>(FindObjectsSortMode.None);

        foreach (var node in nodes)
            node.UpdateVisual();
    }

    public void ApplySkillEffect(SkillData skill)
    {
        if (GameManager.Instance == null) return;

        PlayerData data = GameManager.Instance.playerData;

        switch (skill.effectType)
        {
            // ================= DAMAGE =================
            case SkillEffectType.Damage:
                data.damageMultiplier += skill.value;
                break;

            case SkillEffectType.FireRate:
                data.fireRateMultiplier *= (1f - skill.value);
                break;

            case SkillEffectType.CritChance:
                data.critChance += skill.value;
                break;

            case SkillEffectType.CritMultiplier:
                data.critMultiplier += skill.value;
                break;

            // ================= UTILITY =================
            case SkillEffectType.XPMultiplier:
                data.xpMultiplier += skill.value;
                break;

            case SkillEffectType.XPRange:
                data.magnetLevel += Mathf.RoundToInt(skill.value);
                break;

            case SkillEffectType.CoinMultiplier:
                data.coinMultiplier += skill.value;
                break;

            case SkillEffectType.Luck:
                data.luck += skill.value;
                break;

            case SkillEffectType.SecretChance:
                data.secretChance += skill.value;
                break;

            // ================= DEFENSE =================
            case SkillEffectType.MaxHP:
                data.maxHPBonus += skill.value;
                break;

            case SkillEffectType.DamageReduction:
                data.damageReduction += skill.value;
                break;

            case SkillEffectType.LifeSteal:
                data.lifesteal += skill.value;
                break;
        }
    }
}