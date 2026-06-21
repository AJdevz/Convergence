using UnityEngine;
using TMPro;

public class CoinsManager : MonoBehaviour
{
    public static CoinsManager Instance;

    [Header("UI")]
    public TMP_Text coinsText; // THIS = run coins HUD (in-game)

    [Header("Coins")]
    public int runCoins = 0;     // coins earned THIS RUN ONLY
    public int totalCoins = 0;   // permanent coins (saved)

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (GameManager.Instance != null)
        {
            totalCoins = GameManager.Instance.playerData.totalCoins;
        }

        UpdateUI();
    }



    // =========================
    //  ADD COINS (DURING RUN)
    // =========================
    public void AddRunCoins(int amount)
    {
        runCoins += amount;
        UpdateUI();
    }

    public void AddToTotal()
    {
        totalCoins += runCoins;   // move run → total
        runCoins = 0;

        UpdateUI();
    }

    // =========================
    //  RESET RUN (NEW GAME)
    // =========================
    public void ResetRunCoins()
    {
        runCoins = 0;
        UpdateUI();
    }

    public enum CoinDisplayType
    {
        RunCoins,
        TotalCoins
    }

    public CoinDisplayType displayType;

    // =========================
    //  UPDATE HUD (RUN ONLY)
    // =========================
    public void UpdateUI()
    {
        if (coinsText == null) return;

        if (displayType == CoinDisplayType.RunCoins)
        {
            coinsText.text = "Coins: " + runCoins;
        }
        else if (displayType == CoinDisplayType.TotalCoins)
        {
            coinsText.text = "Coins: " + totalCoins;
        }
    }
}