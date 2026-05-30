using TMPro;
using UnityEngine;

public class DebugStatsUI : MonoBehaviour
{
    public TMP_Text text;

    private PlayerData data;

    void Start()
    {
        Invoke(nameof(Initialize), 1f);
    }

    void Initialize()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager missing");
            enabled = false;
            return;
        }

        data = GameManager.Instance.playerData;
    }

    void Update()
    {
        if (data == null || text == null)
            return;

        GunController gun = FindFirstObjectByType<GunController>();
        PlayerHealth hp = FindFirstObjectByType<PlayerHealth>();
        XPMagnet magnet = FindFirstObjectByType<XPMagnet>();

        string stats = "";

        stats += "======== PLAYER STATS ========\n\n";

        // ================= DAMAGE =================
        stats += "=== DAMAGE ===\n";

        stats += "Damage Multiplier: x" + data.damageMultiplier.ToString("F2") + "\n";

        if (gun != null)
        {
            stats += "Final Gun Damage: " + gun.damage + "\n";
            stats += "Fire Rate Mult: " + data.fireRateMultiplier.ToString("F2") + "\n";
        }

        stats += "Crit Chance: " + (data.critChance * 100f).ToString("F0") + "%\n";
        stats += "Crit Multiplier: x" + data.critMultiplier.ToString("F2") + "\n";

        // ================= DEFENSE =================
        stats += "\n=== DEFENSE ===\n";

        if (hp != null)
        {
            stats += "Base HP: " + hp.baseHealth + "\n";
            stats += "Total HP: " + hp.maxHealth + "\n";
            stats += "Current HP: " + hp.currentHealth + "\n";
        }

        stats += "HP Bonus: " + data.maxHPBonus + "\n";
        stats += "Damage Reduction: " + (data.damageReduction * 100f).ToString("F0") + "%\n";
        stats += "Lifesteal: " + (data.lifesteal * 100f).ToString("F0") + "%\n";

        // ================= UTILITY =================
        stats += "\n=== UTILITY ===\n";

        stats += "XP Multiplier: x" + data.xpMultiplier.ToString("F2") + "\n";

        if (magnet != null)
            stats += "Magnet Level: " + magnet.magnetLevel + "\n";
        else
            stats += "Magnet Level: " + data.magnetLevel + "\n";

        stats += "Coin Multiplier: x" + data.coinMultiplier.ToString("F2") + "\n";

        stats += "Luck: " + data.luck.ToString("F2") + "\n";

        stats += "Secret Chance: " + (data.secretChance * 100f).ToString("F0") + "%\n";

        text.text = stats;
    }
}