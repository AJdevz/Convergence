using UnityEngine;
using TMPro;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance { get; private set; }

    public int playerXP = 0;
    public int playerLevel = 1;
    public int xpToNextLevel = 100;

    public float xpMultiplier = 1.5f;
    public UpgradeMenu upgradeMenuScript;
    public TextMeshProUGUI levelText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddXP(int amount)
    {
        amount = Mathf.RoundToInt(amount * GameManager.Instance.playerData.xpMultiplier);
        playerXP += amount;

        while (playerXP >= xpToNextLevel)
        {
            playerXP -= xpToNextLevel;
            LevelUp();
        }

        UpdateXPUI();
    }

    void UpdateXPUI()
    {
        XPBar bar = FindFirstObjectByType<XPBar>();
        if (bar == null) return;
    }

    void LevelUp()
    {
        playerLevel++;
        xpToNextLevel =
            Mathf.RoundToInt(xpToNextLevel * 1.25f);

        UpdateXPUI();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            VFXManager.Instance.PlayVFX(
                VFXManager.Instance.levelUpEffect,
                player.transform,
                new Vector3(0, 1.5f, 0)
            );
        }

        if (upgradeMenuScript != null)
            upgradeMenuScript.OpenUpgradeMenu();

        UpdateLevelUI();
    }

    void UpdateLevelUI()
    {
        if (levelText != null)
            levelText.text = $"Level: {playerLevel}";
    }
}