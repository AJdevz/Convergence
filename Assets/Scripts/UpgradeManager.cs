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

    public TMP_Text button1Text;
    public TMP_Text button2Text;
    public TMP_Text button3Text;

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
        Lifesteal
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

        button.interactable = true;

        button.onClick.AddListener(() =>
        {
            ApplyUpgrade(type);
        });
    }

    // =========================
    // 🎨 UI FORMAT
    // =========================

    string FormatUI(UpgradeType type, int level)
    {
        return
            "----------------\n" +
            "LEVEL " + (level + 1) + "/" + maxLevel + "\n" +
            "----------------\n" +
            "<b>" + type.ToString().ToUpper() + "</b>\n" +
            GetUpgradeDescription(type, level) + "\n" +
            GetLevelDots(level) + "\n" +
            "----------------";
    }

    string GetLevelDots(int level)
    {
        string dots = "";

        for (int i = 0; i < maxLevel; i++)
        {
            dots += (i < level) ? "●" : "○";
        }

        return dots;
    }

    // =========================
    // 📈 DESCRIPTIONS
    // =========================

    string GetUpgradeDescription(UpgradeType type, int level)
    {
        switch (type)
        {
            case UpgradeType.Damage:
                return "+ " + (10 + level * 5) + "% Damage";

            case UpgradeType.FireRate:
                return "- " + (5 + level * 2) + "% Delay";

            case UpgradeType.Explosion:
                return "+ " + (20 + level * 10) + "% Radius / Damage";

            case UpgradeType.ChainLightning:
                return "+ " + (1 + level) + " Chain Bounces";

            case UpgradeType.Piercing:
                return "+ " + (1 + level) + " Pierces";

            case UpgradeType.Freeze:
                return "+ " + (10 + level * 5) + "% Slow Strength";

            case UpgradeType.Lifesteal:
                return "+ " + (2 + level) + "% Lifesteal";

            default:
                return "";
        }
    }

    // =========================
    // 🟢 APPLY UPGRADES
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
                gun.damage = Mathf.RoundToInt(gun.damage * 1.1f);
                break;

            case UpgradeType.FireRate:
                gun.timeBetweenShots *= 0.95f;
                break;

            case UpgradeType.Explosion:
                gun.explosiveShots = true;
                gun.explosionRadius += 0.5f;
                gun.explosionDamage += 5;
                break;

            case UpgradeType.ChainLightning:
                gun.chainLightning = true;
                gun.chainCount += 1;
                break;

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