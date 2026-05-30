using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameMode { Escalation, Evolution }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game Settings")]
    public GameMode SelectedMode = GameMode.Escalation;

    public GunController.GunType SelectedGun = GunController.GunType.AssaultRifle;

    public PlayerData playerData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            playerData = SaveSystem.Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (CoinsManager.Instance != null)
            CoinsManager.Instance.totalCoins = playerData.totalCoins;

        // ❌ OLD:
        // ApplyAllSkills();

        // ✅ FIXED:
        StartCoroutine(ApplySkillsDelayed());
    }

    // =========================
    // 🔥 FIXED SKILL LOADING
    // =========================
    IEnumerator ApplySkillsDelayed()
    {
        yield return new WaitForSeconds(0.1f);

        if (SkillTreeManager.Instance == null)
        {
            Debug.LogError("SkillTreeManager missing!");
            yield break;
        }

        // 🔥 IMPORTANT
        playerData.ResetRuntimeStats();

        foreach (string id in playerData.unlockedSkills)
        {
            SkillData skill = FindSkillByID(id);

            if (skill != null)
            {
                SkillTreeManager.Instance.ApplySkillEffect(skill);
            }
        }

        ApplyStatsToScene();

        Debug.Log("Skills rebuilt successfully.");
    }

    public void ApplyStatsToScene()
    {
        GunController gun = FindFirstObjectByType<GunController>();
        XPManager xp = FindFirstObjectByType<XPManager>();
        PlayerHealth player = FindFirstObjectByType<PlayerHealth>();
        XPMagnet magnet = FindFirstObjectByType<XPMagnet>();

        var data = playerData;

        if (gun != null)
        {
            gun.damageMultiplier = data.damageMultiplier;
            gun.fireRateMultiplier = data.fireRateMultiplier;
            gun.lifestealPercent = data.lifesteal;

            gun.RecalculateStats();
        }

        if (xp != null)
        {
            xp.xpMultiplier = data.xpMultiplier;
        }

        if (magnet != null)
        {
            magnet.magnetLevel = data.magnetLevel;
        }

        if (player != null)
        {
            player.ApplyHealthStats();
        }
    }

    // =========================
    // 🔍 FIND SKILL BY ID
    // =========================
    SkillData FindSkillByID(string id)
    {
        SkillData[] all = Resources.FindObjectsOfTypeAll<SkillData>();

        foreach (var s in all)
        {
            if (s.skillID == id)
                return s;
        }

        return null;
    }

    // =========================
    // 💾 SAVE GAME
    // =========================
    public void SaveGame()
    {
        if (CoinsManager.Instance != null)
        {
            playerData.totalCoins = CoinsManager.Instance.totalCoins;
        }

        SaveSystem.Save(playerData);
    }

    // =========================
    // 🎮 MODE SELECTION
    // =========================
    public void SetModeEscalation()
    {
        SelectedMode = GameMode.Escalation;
        PlayerPrefs.SetInt("SelectedMode", 0);
        SceneManager.LoadScene("PlayScene");
    }

    public void SetModeEvolution()
    {
        SelectedMode = GameMode.Evolution;
        PlayerPrefs.SetInt("SelectedMode", 1);
        SceneManager.LoadScene("PlayScene");
    }
}