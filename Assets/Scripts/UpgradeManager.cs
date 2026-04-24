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

    [System.Serializable]
    public class UpgradeData
    {
        public UpgradeType type;
        public int level = 0;
    }

    public int maxLevel = 10;

    private Dictionary<UpgradeType, UpgradeData> upgradeLevels = new Dictionary<UpgradeType, UpgradeData>();

    public enum UpgradeType
    {
        Damage,
        FireRate,
        Explosion,
        ChainLightning,
        Piercing,
        Freeze,
        Lifesteal,
        XPMagnet,
    }

    void Start()
    {
        foreach (UpgradeType type in System.Enum.GetValues(typeof(UpgradeType)))
        {
            upgradeLevels[type] = new UpgradeData
            {
                type = type,
                level = 0
            };
        }
    }

    void Update()
    {
        if (xpText != null && XPManager.Instance != null)
        {
            xpText.text = "XP: " + XPManager.Instance.playerXP + "/" + XPManager.Instance.xpToNextLevel;
        }
    }

    // =========================
    // 🎲 GENERATE UPGRADES
    // =========================

    public void GenerateUpgradeChoices()
    {
        List<UpgradeType> available = GetAvailableUpgrades();

        if (available.Count == 0)
        {
            Debug.Log("All upgrades maxed!");
            CloseUpgradeMenu();
            return;
        }

        List<UpgradeType> choices = new List<UpgradeType>();

        for (int i = 0; i < 3; i++)
        {
            if (available.Count == 0) break;

            int index = Random.Range(0, available.Count);
            choices.Add(available[index]);
            available.RemoveAt(index);
        }

        SetupButton(button1, button1Text, choices[0]);

        if (choices.Count > 1)
            SetupButton(button2, button2Text, choices[1]);
        else
            button2.gameObject.SetActive(false);

        if (choices.Count > 2)
            SetupButton(button3, button3Text, choices[2]);
        else
            button3.gameObject.SetActive(false);
    }

    List<UpgradeType> GetAvailableUpgrades()
    {
        return upgradeLevels
            .Where(x => x.Value.level < maxLevel)
            .Select(x => x.Key)
            .ToList();
    }

    // =========================
    // 🔘 BUTTON SETUP
    // =========================

    void SetupButton(Button button, TMP_Text text, UpgradeType type)
    {
        button.gameObject.SetActive(true);
        button.onClick.RemoveAllListeners();

        UpgradeData data = upgradeLevels[type];

        text.text = FormatUI(type, data.level);

        Image img = button.GetComponent<Image>();
        if (img != null)
            img.color = GetUpgradeColor(type);

        button.interactable = true;

        button.onClick.AddListener(() =>
        {
            ApplyUpgrade(type);
        });
    }

    // =========================
    // 🎨 UI COLORS
    // =========================

    Color GetUpgradeColor(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Damage: return new Color(0.8f, 0.2f, 0.2f);
            case UpgradeType.FireRate: return new Color(1f, 0.6f, 0.1f);
            case UpgradeType.Explosion: return new Color(1f, 0.4f, 0.1f);
            case UpgradeType.ChainLightning: return new Color(0.2f, 1f, 1f);
            case UpgradeType.Piercing: return new Color(0.7f, 0.7f, 0.7f);
            case UpgradeType.Freeze: return new Color(0.4f, 0.7f, 1f);
            case UpgradeType.Lifesteal: return new Color(0.6f, 0.1f, 0.6f);
            case UpgradeType.XPMagnet: return new Color(0.9f, 0.9f, 0.2f);
            default: return Color.white;
        }
    }

    // ⚠️ FIXED ICONS (no emojis → avoids missing font boxes)
    string GetUpgradeIcon(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Damage: return "[DMG]";
            case UpgradeType.FireRate: return "[SPD]";
            case UpgradeType.Explosion: return "[AOE]";
            case UpgradeType.ChainLightning: return "[CHAIN]";
            case UpgradeType.Piercing: return "[PEN]";
            case UpgradeType.Freeze: return "[ICE]";
            case UpgradeType.Lifesteal: return "[VAMP]";
            case UpgradeType.XPMagnet: return "[MAG]";
            default: return "";
        }
    }

    string GetUpgradeLabel(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Damage: return "Damage Bonus";
            case UpgradeType.FireRate: return "Fire Rate";
            case UpgradeType.Explosion: return "Explosion Power";
            case UpgradeType.ChainLightning: return "Chain Count";
            case UpgradeType.Piercing: return "Pierce Count";
            case UpgradeType.Freeze: return "Freeze Strength";
            case UpgradeType.Lifesteal: return "Lifesteal";
            case UpgradeType.XPMagnet: return "XP Magnet Range";
            default: return "";
        }
    }

    // =========================
    // 🎯 UI FORMAT (FIXED DUPES + ALIGNMENT)
    // =========================

    string FormatUI(UpgradeType type, int level)
    {
        return
            $"<size=120%><b>{GetUpgradeIcon(type)} {type.ToString().ToUpper()}</b></size>\n" +
            $"Level {level}/{maxLevel}\n" +
            $"────────────\n\n" +

            $"{GetUpgradeLabel(type)}\n" +
            $"{GetUpgradeDescription(type, level)}\n\n" +

            $"────────────\n" +
            $"{GetLevelDots(level)}";
    }

    // =========================
    // 📊 DOTS (CENTER FIXED)
    // =========================

    string GetLevelDots(int level)
    {
        string dots = "<size=85%><align=center>\n";

        for (int i = 0; i < maxLevel; i++)
        {
            dots += (i < level)
                ? "<color=#ffffff>●</color> "
                : "<color=#444444>○</color> ";
        }

        dots += "\n</align></size>";
        return dots;
    }

    // =========================
    // 📈 DESCRIPTION (UNCHANGED LOGIC, CLEANED DUPES REMOVED)
    // =========================

    string GetUpgradeDescription(UpgradeType type, int level)
    {
        switch (type)
        {
            case UpgradeType.Damage:
                {
                    float currentPercent = 0.30f * level;
                    float nextPercent = 0.30f * (level + 1);

                    int currentDamage = Mathf.RoundToInt(baseDamage * (1f + currentPercent));
                    int nextDamage = Mathf.RoundToInt(baseDamage * (1f + nextPercent));

                    return
                        $"<color=#ff5555>{(int)(currentPercent * 100)}%</color> → <color=#55ff55>{(int)(nextPercent * 100)}%</color>\n" +
                        $"{currentDamage} → {nextDamage}";
                }

            case UpgradeType.FireRate:
                {
                    float currentPercent = Mathf.Min(0.05f * level, 0.5f);
                    float nextPercent = Mathf.Min(0.05f * (level + 1), 0.5f);

                    float currentDelay = baseTimeBetweenShots * (1f - currentPercent);
                    float nextDelay = baseTimeBetweenShots * (1f - nextPercent);

                    float currentShots = 1f / currentDelay;
                    float nextShots = 1f / nextDelay;

                    return
                        $"<color=#ff5555>{(int)(currentPercent * 100)}%</color> → <color=#55ff55>{(int)(nextPercent * 100)}%</color>\n" +
                        $"{currentShots:F1}/s → {nextShots:F1}/s";
                }

            case UpgradeType.Explosion:
                {
                    float current = level * 0.05f;
                    float next = (level + 1) * 0.05f;

                    return $"<color=#ff5555>{(int)(current * 100)}%</color> → <color=#55ff55>{(int)(next * 100)}%</color>";
                }

            case UpgradeType.ChainLightning:
                {
                    return $"{level} → {level + 1}";
                }

            case UpgradeType.Piercing:
                {
                    return $"{level} → {level + 1}";
                }

            case UpgradeType.Freeze:
                {
                    float current = level * 0.1f;
                    float next = (level + 1) * 0.1f;

                    return $"<color=#ff5555>{(int)(current * 100)}%</color> → <color=#55ff55>{(int)(next * 100)}%</color>";
                }

            case UpgradeType.Lifesteal:
                {
                    float current = level * 0.02f;
                    float next = (level + 1) * 0.02f;

                    return $"<color=#ff5555>{(int)(current * 100)}%</color> → <color=#55ff55>{(int)(next * 100)}%</color>";
                }

            case UpgradeType.XPMagnet:
                {
                    return $"Range +10 per level";
                }

            default:
                return "";
        }
    }

    // =========================
    // 🟢 APPLY UPGRADES (UNCHANGED)
    // =========================

    void ApplyUpgrade(UpgradeType type)
    {
        GunController gun = FindFirstObjectByType<GunController>();
        UpgradeData data = upgradeLevels[type];

        if (data.level >= maxLevel) return;

        data.level++;

        switch (type)
        {
            case UpgradeType.Damage:
                {
                    float percent = 0.30f * data.level;
                    gun.damage = Mathf.RoundToInt(gun.baseDamage * (1f + percent));
                    break;
                }

            case UpgradeType.FireRate:
                {
                    float percent = Mathf.Min(0.05f * data.level, 0.5f);
                    gun.timeBetweenShots = gun.baseTimeBetweenShots * (1f - percent);
                    break;
                }

            case UpgradeType.Explosion:
                {
                    gun.explosiveShots = true;

                    gun.explosionMultiplier = Mathf.Min(0.05f * data.level, 1f);
                    gun.explosionRadius = 3f + data.level * 0.5f;

                    break;
                }

            case UpgradeType.ChainLightning:
                {
                    gun.chainLightning = true;

                    // FIXED: absolute value, NOT additive
                    gun.chainCount = data.level; // or Mathf.Max(1, data.level)

                    gun.chainMultiplier = data.level * 0.05f;

                    break;
                }

            case UpgradeType.Piercing:
                gun.piercing = true;
                gun.pierceCount += 1;
                break;

            case UpgradeType.Freeze:
                gun.freezeEffect = true;
                gun.freezeStrength += 0.1f;
                break;

            case UpgradeType.Lifesteal:
                gun.lifestealPercent += 0.02f;
                break;

            case UpgradeType.XPMagnet:
                {
                    XPMagnet magnet = FindFirstObjectByType<XPMagnet>();

                    if (magnet != null)
                    {
                        magnet.magnetLevel++;
                        Debug.Log("Magnet Level UP → " + magnet.magnetLevel);
                    }
                    else
                    {
                        Debug.LogError("NO XPMagnet FOUND");
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