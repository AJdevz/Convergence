using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class UpgradeManager : MonoBehaviour
{
    public TMP_Text xpText;

    [Header("Upgrade Buttons")]
    public Button button1;
    public Button button2;
    public Button button3;

    [Header("Button Backgrounds")]
    public Image button1Image;
    public Image button2Image;
    public Image button3Image;

    public TMP_Text button1Text;
    public TMP_Text button2Text;
    public TMP_Text button3Text;

    [Header("Base Stats (DO NOT MODIFY)")]
    public int baseDamage = 20;
    public float baseTimeBetweenShots = 0.2f;

    [Header("Rarity Settings")]
    public float secretChance = 0.01f; // 1% chance for secret tier
    public int waveForSecretUnlock = 10; // Wave at which secret becomes available

    public enum UpgradeType
    {
        Damage,
        FireRate,
        Explosion,
        ChainLightning,
        Piercing,
        Health,
        Lifesteal,
        XPMagnet,
    }

    public enum Rarity
    {
        Common,
        Rare,
        Epic,
        Legendary,
        Secret
    }

    bool IsMaxed(UpgradeType type, GunController gun)
    {
        switch (type)
        {
            case UpgradeType.Lifesteal:
                return gun.lifestealPercent >= 0.10f;

            case UpgradeType.Health:
                {
                    PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
                    return player != null && player.maxHealth >= 5000;
                }

            case UpgradeType.Explosion:
                return gun.explosionRadius >= 12f;

            case UpgradeType.ChainLightning:
                return gun.chainCount >= 12;

            case UpgradeType.Piercing:
                return gun.pierceCount >= 10;

            case UpgradeType.XPMagnet:
                XPMagnet magnet = FindFirstObjectByType<XPMagnet>();
                return magnet != null && magnet.magnetLevel >= 40;

            default:
                return false;
        }
    }

    int GetCoinReward(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return 5;
            case Rarity.Rare: return 15;
            case Rarity.Epic: return 40;
            case Rarity.Legendary: return 100;
            case Rarity.Secret: return 250;
            default: return 0;
        }
    }

    [System.Serializable]
    public class UpgradeInstance
    {
        public UpgradeType type;
        public Rarity rarity;
        public int timesObtained = 0;
    }

    private List<UpgradeInstance> obtainedUpgrades = new List<UpgradeInstance>();
    private int upgradeCount = 0;

    void Start()
    {
        // Initialize tracking
    }

    void Update()
    {
        if (xpText != null && XPManager.Instance != null)
        {
            xpText.text = "XP: " + XPManager.Instance.playerXP + "/" + XPManager.Instance.xpToNextLevel;
        }
    }

    // GENERATE UPGRADES

    public void GenerateUpgradeChoices()
    {
        List<UpgradeInstance> choices = new List<UpgradeInstance>();
        GunController gun = FindFirstObjectByType<GunController>();

        for (int i = 0; i < 3; i++)
        {
            UpgradeInstance upgrade;
            int attempts = 0;

            do
            {
                upgrade = GenerateRandomUpgrade();
                attempts++;
            }
            while (gun != null && !IsUpgradeUseful(upgrade.type, gun) && attempts < 10);

            choices.Add(upgrade);
        }

        SetupButton(button1, button1Text, button1Image, choices[0]);
        SetupButton(button2, button2Text, button2Image, choices[1]);
        SetupButton(button3, button3Text, button3Image, choices[2]);
    }

    UpgradeInstance GenerateRandomUpgrade()
    {
        UpgradeType type = (UpgradeType)Random.Range(0, System.Enum.GetNames(typeof(UpgradeType)).Length);
        Rarity rarity = DetermineRarity();

        return new UpgradeInstance
        {
            type = type,
            rarity = rarity,
            timesObtained = 0
        };
    }

    Rarity DetermineRarity()
    {
        float roll = Random.value;

        // Secret tier (1% chance or special conditions)
        if (roll < secretChance && IsSecretUnlocked())
            return Rarity.Secret;

        // Weighted distribution
        if (roll < 0.50f) return Rarity.Common;      // 50%
        if (roll < 0.75f) return Rarity.Rare;        // 25%
        if (roll < 0.85f) return Rarity.Epic;        // 15%
        return Rarity.Legendary;                      // 10%
    }

    bool IsSecretUnlocked()
    {
        // Check if player has progressed far enough
        return upgradeCount >= 1; 
    }

    //  BUTTON SETUP

    void SetupButton(Button button, TMP_Text text, Image backgroundImage, UpgradeInstance upgrade)
    {
        button.gameObject.SetActive(true);
        button.onClick.RemoveAllListeners();

        // Format text
        text.text = FormatUI(upgrade);

        // Set colors based on rarity
        backgroundImage.color = GetRarityColor(upgrade.rarity);

        button.interactable = true;

        button.onClick.AddListener(() =>
        {
            ApplyUpgrade(upgrade);
        });
    }


    //  RARITY COLORS

    Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return new Color(0.6f, 0.6f, 0.6f);           // Gray
            case Rarity.Rare: return new Color(0.2f, 0.5f, 1f);               // Blue
            case Rarity.Epic: return new Color(0.8f, 0.2f, 0.9f);             // Purple
            case Rarity.Legendary: return new Color(1f, 0.8f, 0.2f);          // Gold
            case Rarity.Secret: return new Color(1f, 0.2f, 0.2f);             // Red
            default: return Color.white;
        }
    }

    string GetRarityLabel(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return "COMMON";
            case Rarity.Rare: return "RARE";
            case Rarity.Epic: return "EPIC";
            case Rarity.Legendary: return "LEGENDARY";
            case Rarity.Secret: return "SECRET";
            default: return "";
        }
    }


    // UI FORMAT


    string FormatUI(UpgradeInstance upgrade)
    {
        GunController gun = FindFirstObjectByType<GunController>();
        string color = GetRarityHexColor(upgrade.rarity);

        bool isMaxed = gun != null && IsMaxed(upgrade.type, gun);

        string valueLine = GetUpgradeValueLine(upgrade);

        string statLine;

        if (isMaxed)
        {
            int coinsReward = GetCoinReward(upgrade.rarity);

            statLine = $"<color={color}><b>MAX</b></color>\n" +
                       $"<color=#FFD700>+{coinsReward} Coins</color>";
        }
        else
        {
            statLine =
                $"<color=#FF5555>{GetCurrentStat(upgrade)}</color> " +
                $"→ " +
                $"<color=#55FF55>{GetNextStat(upgrade)}</color>";
        }

        return
    $@"<align=center>

<size=110%><b><color={color}>{GetRarityLabel(upgrade.rarity)}</color></b></size>

<size=100%><b>{upgrade.type.ToString().ToUpper()}</b></size>

<size=75%><color=#555555>────────────</color></size>

<size=90%><color={color}>{valueLine}</color></size>

<size=85%>{statLine}</size>

</align>";
    }

    string GetMaxStatLine(UpgradeInstance upgrade)
{
    switch (upgrade.type)
    {
        case UpgradeType.Damage:
            return "<color=#AAAAAA>Max: INF</color>";

        case UpgradeType.Lifesteal:
            return "<color=#AAAAAA>Max: 10%</color>";

        case UpgradeType.Health:
            return "<color=#AAAAAA>Max Health: 5000</color>";

        default:
            return "<color=#AAAAAA>Max: -</color>";
    }
}

    string GetUpgradeValueLine(UpgradeInstance upgrade)
    {
        string color = GetRarityHexColor(upgrade.rarity);

        switch (upgrade.type)
        {
            case UpgradeType.Damage:
                return $"<color={color}>+{(int)(GetDamageBonus(upgrade.rarity) * 100)}% Damage</color>";

            case UpgradeType.FireRate:
                return $"<color={color}>+{(int)(GetFireRateBonus(upgrade.rarity) * 100)}% Fire Rate</color>";

            case UpgradeType.Explosion:
                return $"<color={color}>+{(int)(GetExplosionBonus(upgrade.rarity) * 100)}% Explosion Damage</color>";

            case UpgradeType.ChainLightning:
                return $"<color={color}>Chains +{GetChainCount(upgrade.rarity)}</color>";

            case UpgradeType.Piercing:
                return $"<color={color}>Pierce +{GetPierceCount(upgrade.rarity)}</color>";

            case UpgradeType.Health:
                {
                    return $"<color={color}>+{GetHealthBonus(upgrade.rarity)} Max HP</color>";
                }

            case UpgradeType.Lifesteal:
                return $"<color={color}>+{Mathf.RoundToInt(GetLifestealPercent(upgrade.rarity) * 100)}% Lifesteal</color>";

            case UpgradeType.XPMagnet:
                return $"<color={color}>+{GetMagnetBonus(upgrade.rarity)} Range</color>";

            default:
                return "<color=#FFFFFF>Upgrade</color>";
        }
    }

    string GetCurrentStat(UpgradeInstance upgrade)
    {
        GunController gun = FindFirstObjectByType<GunController>();

        switch (upgrade.type)
        {
            case UpgradeType.Damage:
                return gun.GetCurrentDamage().ToString();

            case UpgradeType.FireRate:
                return (1f / gun.timeBetweenShots).ToString("F1") + " shots/s";

            case UpgradeType.Explosion:
                return $"{Mathf.RoundToInt(gun.explosionMultiplier * 100)}%";

            case UpgradeType.ChainLightning:
                return gun.chainCount.ToString();

            case UpgradeType.Piercing:
                return gun.pierceCount.ToString();

            case UpgradeType.Health:
                {
                    PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
                    return player != null ? player.maxHealth.ToString() : "-";
                }

            case UpgradeType.Lifesteal:
                return Mathf.RoundToInt(gun.lifestealPercent * 100) + "%";

            case UpgradeType.XPMagnet:
                XPMagnet magnet = FindFirstObjectByType<XPMagnet>();
                return magnet != null ? magnet.magnetLevel.ToString() : "-";

            default:
                return "-";
        }
    }

    string GetNextStat(UpgradeInstance upgrade)
    {
        GunController gun = FindFirstObjectByType<GunController>();

        switch (upgrade.type)
        {
            case UpgradeType.Damage:
                float dmg = GetDamageBonus(upgrade.rarity);
                return Mathf.RoundToInt(gun.GetCurrentDamage() * (1f + dmg)).ToString();

            case UpgradeType.FireRate:
                float fr = GetFireRateBonus(upgrade.rarity);
                float newDelay = gun.timeBetweenShots * (1f - fr);
                return (1f / newDelay).ToString("F1") + " shots/s";

            case UpgradeType.Explosion:
                {
                    float next = gun.explosionMultiplier + GetExplosionBonus(upgrade.rarity);
                    return $"{Mathf.RoundToInt(next * 100)}%";
                }

            case UpgradeType.ChainLightning:
                return (gun.chainCount + GetChainCount(upgrade.rarity)).ToString();

            case UpgradeType.Piercing:
                return (gun.pierceCount + GetPierceCount(upgrade.rarity)).ToString();

            case UpgradeType.Health:
                {
                    PlayerHealth player = FindFirstObjectByType<PlayerHealth>();

                    if (player == null)
                        return "-";

                    return (player.maxHealth + GetHealthBonus(upgrade.rarity)).ToString();
                }

            case UpgradeType.Lifesteal:
                float ls = Mathf.Clamp(
                    gun.lifestealPercent + GetLifestealPercent(upgrade.rarity),
                    0f,
                    0.10f
                );
                return Mathf.RoundToInt(ls * 100) + "%";

            case UpgradeType.XPMagnet:
                XPMagnet magnet = FindFirstObjectByType<XPMagnet>();
                if (magnet == null) return "-";
                return (magnet.magnetLevel + GetMagnetBonus(upgrade.rarity)).ToString();

            default:
                return "-";
        }
    }

    string Separator()
    {
        return "<color=#555555>────────────</color>";
    }

    string GetRarityHexColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return "#999999";
            case Rarity.Rare: return "#3399FF";
            case Rarity.Epic: return "#DD33FF";
            case Rarity.Legendary: return "#FFDD00";
            case Rarity.Secret: return "#FF3333";
            default: return "#FFFFFF";
        }
    }

    string GetUpgradeIcon(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Damage: return "[DMG]";
            case UpgradeType.FireRate: return "[SPD]";
            case UpgradeType.Explosion: return "[AOE]";
            case UpgradeType.ChainLightning: return "[CHAIN]";
            case UpgradeType.Piercing: return "[PEN]";
            case UpgradeType.Health: return "[HP]";
            case UpgradeType.Lifesteal: return "[VAMP]";
            case UpgradeType.XPMagnet: return "[MAG]";
            default: return "";
        }
    }

    // DESCRIPTION (RARITY-BASED)

    string GetUpgradeDescription(UpgradeType type, Rarity rarity)
    {
        string rarityColor = GetRarityHexColor(rarity);

        switch (type)
        {
            case UpgradeType.Damage:
                {
                    float damageBonus = GetDamageBonus(rarity);
                    int nextDamage = Mathf.RoundToInt(baseDamage * (1f + damageBonus));
                    return $"<color={rarityColor}>+{(int)(damageBonus * 100)}% DAMAGE</color>\n" +
                           $"→ {nextDamage} damage";
                }

            case UpgradeType.FireRate:
                {
                    float fireRateBonus = GetFireRateBonus(rarity);
                    float newDelay = baseTimeBetweenShots * (1f - fireRateBonus);
                    float shotsPerSecond = 1f / newDelay;
                    return $"<color={rarityColor}>+{(int)(fireRateBonus * 100)}% FIRE RATE</color>\n" +
                           $"→ {shotsPerSecond:F1} shots/sec";
                }

            case UpgradeType.Explosion:
                {
                    float explosionBonus = GetExplosionBonus(rarity);
                    return $"<color={rarityColor}>+{(int)(explosionBonus * 100)}% EXPLOSION</color>\n" +
                           $"Shots explode on impact";
                }

            case UpgradeType.ChainLightning:
                {
                    int chainCount = GetChainCount(rarity);
                    return $"<color={rarityColor}>CHAIN TO {chainCount} ENEMIES</color>\n" +
                           $"Lightning chains between targets";
                }

            case UpgradeType.Piercing:
                {
                    int pierceCount = GetPierceCount(rarity);
                    return $"<color={rarityColor}>PIERCE {pierceCount} ENEMIES</color>\n" +
                           $"Shots go through enemies";
                }

            case UpgradeType.Health:
                {
                    int hp = GetHealthBonus(rarity);

                    return
                        $"<color={rarityColor}>+{hp} MAX HP</color>\n" +
                        $"Become harder to kill";
                }

            case UpgradeType.Lifesteal:
                {
                    float lifesteal = GetLifestealPercent(rarity);
                    return $"<color={rarityColor}>+{Mathf.RoundToInt(lifesteal * 100f)}% LIFESTEAL</color>\n" +
                         $"Heal on hit";
                }

            case UpgradeType.XPMagnet:
                {
                    int magnetBonus = GetMagnetBonus(rarity);
                    return $"<color={rarityColor}>MAGNET RANGE +{magnetBonus}</color>\n" +
                           $"Pull XP from further away";
                }

            default:
                return "";
        }
    }

    // RARITY-BASED STAT CALCULATIONS

    float GetDamageBonus(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return 0.10f;
            case Rarity.Rare: return 0.20f;
            case Rarity.Epic: return 0.35f;
            case Rarity.Legendary: return 0.60f;
            case Rarity.Secret: return 1.00f;
            default: return 0f;
        }
    }

    float GetFireRateBonus(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return 0.05f;
            case Rarity.Rare: return 0.10f;
            case Rarity.Epic: return 0.20f;
            case Rarity.Legendary: return 0.35f;
            case Rarity.Secret: return 0.50f;
            default: return 0f;
        }
    }

    float GetExplosionBonus(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return 0.10f;
            case Rarity.Rare: return 0.20f;
            case Rarity.Epic: return 0.40f;
            case Rarity.Legendary: return 0.70f;
            case Rarity.Secret: return 1.20f;
            default: return 0f;
        }
    }

    int GetChainCount(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return 1;
            case Rarity.Rare: return 2;
            case Rarity.Epic: return 3;
            case Rarity.Legendary: return 5;
            case Rarity.Secret: return 8;
            default: return 0;
        }
    }

    int GetPierceCount(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return 1;
            case Rarity.Rare: return 2;
            case Rarity.Epic: return 3;
            case Rarity.Legendary: return 5;
            case Rarity.Secret: return 8;
            default: return 0;
        }
    }

    int GetHealthBonus(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return 10;
            case Rarity.Rare: return 20;
            case Rarity.Epic: return 35;
            case Rarity.Legendary: return 60;
            case Rarity.Secret: return 100;
            default: return 0;
        }
    }

    float GetLifestealPercent(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return 0.001f;
            case Rarity.Rare: return 0.002f;
            case Rarity.Epic: return 0.003f;
            case Rarity.Legendary: return 0.005f;
            case Rarity.Secret: return 0.01f;
            default: return 0f;
        }
    }

    int GetMagnetBonus(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return 2;
            case Rarity.Rare: return 5;
            case Rarity.Epic: return 10;
            case Rarity.Legendary: return 25;
            case Rarity.Secret: return 50;
            default: return 0;
        }
    }

    bool IsUpgradeUseful(UpgradeType type, GunController gun)
    {
        switch (type)
        {
            case UpgradeType.FireRate:
                return gun.timeBetweenShots > gun.GetFireRateClamp();

            case UpgradeType.Lifesteal:
                return gun.lifestealPercent < 0.10f;

            case UpgradeType.Explosion:
                return gun.explosionRadius < 12f;

            case UpgradeType.XPMagnet:
                XPMagnet magnet = FindFirstObjectByType<XPMagnet>();
                return magnet != null && magnet.magnetLevel < 40;

            case UpgradeType.Health:
                {
                    PlayerHealth player = FindFirstObjectByType<PlayerHealth>();

                    return player != null && player.maxHealth < 5000;
                }

            case UpgradeType.ChainLightning:
                return gun.chainCount < 12;

            case UpgradeType.Piercing:
                return gun.pierceCount < 10;

            default:
                return true;
        }
    }

    //  APPLY UPGRADES

    void ApplyUpgrade(UpgradeInstance upgrade)
    {
        GunController gun = FindFirstObjectByType<GunController>();

        if (gun == null)
        {
            Debug.LogError("GunController not found!");
            return;
        }

        if (IsMaxed(upgrade.type, gun))
        {
            int coinsReward = GetCoinReward(upgrade.rarity);
            CoinsManager.Instance.AddRunCoins(coinsReward);

            Debug.Log($"[MAXED] +{coinsReward} Coins");

            CloseUpgradeMenu();
            GenerateUpgradeChoices();
            return;
        }

        obtainedUpgrades.Add(upgrade);
        upgradeCount++;

        switch (upgrade.type)
        {

            // DAMAGE (MULTIPLICATIVE)
            case UpgradeType.Damage:
                {
                    float bonus = GetDamageBonus(upgrade.rarity);

                    gun.damageMultiplier *= (1f + bonus);

                    gun.RecalculateStats();

                    Debug.Log($"[STACK] DAMAGE x{1f + bonus} → Total Mult: {gun.damageMultiplier}");
                    break;
                }

            // FIRE RATE (MULTIPLICATIVE)

            case UpgradeType.FireRate:
                {
                    float bonus = GetFireRateBonus(upgrade.rarity);

                    gun.fireRateMultiplier *= (1f - bonus);

                    gun.RecalculateStats();

                    Debug.Log($"[STACK] FIRE RATE x{1f - bonus} → Total Mult: {gun.fireRateMultiplier}");
                    break;
                }

            //  EXPLOSION (HYBRID)
            // damage scales, radius capped

            case UpgradeType.Explosion:
                {
                    gun.explosiveShots = true;

                    float bonus = GetExplosionBonus(upgrade.rarity);

                    gun.explosionMultiplier += bonus;

                    gun.explosionRadius = Mathf.Clamp(
                        gun.explosionRadius + (bonus * 1.2f),
                        2f,
                        12f
                    );

                    Debug.Log($"[STACK] EXPLOSION +{bonus * 100}% → Radius: {gun.explosionRadius}");
                    break;
                }

            // CHAIN (ADD + SOFT CAP)

            case UpgradeType.ChainLightning:
                {
                    gun.chainLightning = true;

                    int bonus = GetChainCount(upgrade.rarity);

                    gun.chainCount += bonus;

                    gun.chainCount = Mathf.Min(gun.chainCount, 12); // soft cap

                    Debug.Log($"[STACK] CHAIN +{bonus} → Total: {gun.chainCount}");
                    break;
                }

            //  PIERCING (ADD + CAP)

            case UpgradeType.Piercing:
                {
                    gun.piercing = true;

                    int bonus = GetPierceCount(upgrade.rarity);

                    gun.pierceCount += bonus;

                    gun.pierceCount = Mathf.Min(gun.pierceCount, 10);

                    Debug.Log($"[STACK] PIERCE +{bonus} → Total: {gun.pierceCount}");
                    break;
                }


            // Health (ADD + HARD CAP)

            case UpgradeType.Health:
                {
                    PlayerHealth player = FindFirstObjectByType<PlayerHealth>();

                    if (player == null)
                        break;

                    int bonus = GetHealthBonus(upgrade.rarity);

                    player.maxHealth += bonus;
                    player.currentHealth += bonus;

                    if (player.currentHealth > player.maxHealth)
                        player.currentHealth = player.maxHealth;

                    Debug.Log($"[STACK] HEALTH +{bonus} HP → Total: {player.maxHealth}");

                    break;
                }


            // LIFESTEAL (HARD CAP)

            case UpgradeType.Lifesteal:
                {
                    float bonus = GetLifestealPercent(upgrade.rarity);

                    gun.lifestealPercent += bonus;

                    // HARD CAP
                    gun.lifestealPercent = Mathf.Clamp(gun.lifestealPercent, 0f, 0.01f);

                    Debug.Log($"[STACK] LIFESTEAL → {gun.lifestealPercent * 100f}% (CAPPED 10%)");
                    break;
                }

            //  XP MAGNET (ADDITIVE)

            case UpgradeType.XPMagnet:
                {
                    XPMagnet magnet = FindFirstObjectByType<XPMagnet>();
                    if (magnet == null) break;

                    int bonus = GetMagnetBonus(upgrade.rarity);
                    int max = 40;

                    if (magnet.magnetLevel >= max)
                    {
                        XPManager.Instance.AddXP(bonus * 5);
                        Debug.Log($"[CONVERT] MAGNET MAXED → XP ONLY +{bonus}");
                        break;
                    }

                    int spaceLeft = max - magnet.magnetLevel;

                    if (bonus <= spaceLeft)
                    {
                        magnet.magnetLevel += bonus;
                        Debug.Log($"[STACK] XP MAGNET +{bonus} → {magnet.magnetLevel}");
                    }
                    else
                    {
                        magnet.magnetLevel = max;
                        int overflow = bonus - spaceLeft;

                        XPManager.Instance.AddXP(overflow * 5);
                        Debug.Log($"[CONVERT] XP MAGNET MAXED → overflow XP +{overflow}");
                    }

                    break;
                }
        }

        CloseUpgradeMenu();
        GenerateUpgradeChoices();
    }

    void CloseUpgradeMenu()
    {
        if (XPManager.Instance != null && XPManager.Instance.upgradeMenuScript != null)
        {
            XPManager.Instance.upgradeMenuScript.CloseUpgradeMenu();
        }
    }
}