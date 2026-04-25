using UnityEngine;
using TMPro;

public class CoinsManager : MonoBehaviour
{
    public static CoinsManager Instance;

    public TMP_Text coinsText;

    public int coins = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateUI();
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (coinsText != null)
        {
            coinsText.text = "Coins: " + coins;
        }
    }
}