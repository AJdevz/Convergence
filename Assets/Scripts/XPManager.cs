using UnityEngine;
using TMPro;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    public int playerXP = 0;
    public int playerLevel = 1;
    public int xpToNextLevel = 100;

    public UpgradeMenu upgradeMenuScript; // Reference to UpgradeMenu

    public TextMeshProUGUI levelText;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (upgradeMenuScript == null)
        {
            Debug.LogError("XPManager: upgradeMenuScript is NULL. Make sure the UpgradeMenu GameObject is set in the Inspector.");
        }

        UpdateLevelUI();
    }

    public void AddXP(int amount)
    {
        playerXP += amount;
        if (playerXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    void LevelUp()
    {
        playerXP = 0; // Reset XP after leveling up
        playerLevel++;
        xpToNextLevel += 50; // Increase XP needed for the next level

        // Update XP UI
        XPBar xpBar = Object.FindFirstObjectByType<XPBar>();
        if (xpBar != null)
        {
            xpBar.SetMaxXP(xpToNextLevel); // Set new max XP when leveling up
            xpBar.UpdateXP(playerXP); // Reset the XP bar to 0
        }

        UpdateLevelUI();

        if (upgradeMenuScript != null)
        {
            upgradeMenuScript.OpenUpgradeMenu();
        }
        else
        {
            Debug.LogError("XPManager: upgradeMenuScript is NULL. Assign it in the Inspector.");
        }
    }


    void UpdateLevelUI()
    {
        if (levelText != null)
        {
            levelText.text = "Level: " + playerLevel;
        }
    }
}
