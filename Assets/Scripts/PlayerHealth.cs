using System.Collections;
using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Base Health")]
    public int baseHealth = 100;

    [Header("Runtime Health")]
    public int maxHealth;
    public int currentHealth;
    public TMP_Text hpText;

    [Header("Flash Settings")]
    public float flashLength = 0.1f;

    private float flashCounter;
    private Renderer rend;

    private Color normalColor;
    private Color damageColor = Color.red;
    private Color healColor = Color.green;

    private bool isHealing = false;
    private bool isDead = false;

    [Header("UI References")]
    public GameObject redFlashImage;
    public GameObject greenFlashImage;
    public GameObject endGamePanel;

    void Start()
    {
        rend = GetComponent<Renderer>();

        if (rend != null)
            normalColor = rend.material.GetColor("_Color");

        ApplyHealthStats();

        currentHealth = maxHealth;

        UpdateHealthText();

        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        if (redFlashImage != null)
            redFlashImage.SetActive(false);

        if (greenFlashImage != null)
            greenFlashImage.SetActive(false);
    }

    // =========================
    // APPLY SKILL TREE HP
    // =========================
    public void ApplyHealthStats()
    {
        float bonusHP = 0f;

        if (GameManager.Instance != null)
            bonusHP = GameManager.Instance.playerData.maxHPBonus;

        maxHealth = baseHealth + Mathf.RoundToInt(bonusHP);

        currentHealth = maxHealth;

        Debug.Log("MAX HP UPDATED: " + maxHealth);
    }

    void Update()
    {
        // =========================
        // DEATH
        // =========================
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;

            if (endGamePanel != null)
                endGamePanel.SetActive(true);

            CoinsManager.Instance.AddToTotal();

            if (GameManager.Instance != null)
                GameManager.Instance.SaveGame();

            gameObject.SetActive(false);
        }

        // =========================
        // DAMAGE/HEAL FLASH
        // =========================
        if (flashCounter > 0)
        {
            flashCounter -= Time.deltaTime;

            if (rend != null)
                rend.material.color = isHealing ? healColor : damageColor;

            if (isHealing && greenFlashImage != null)
                greenFlashImage.SetActive(true);
            else if (!isHealing && redFlashImage != null)
                redFlashImage.SetActive(true);

            if (flashCounter <= 0)
            {
                if (rend != null)
                    rend.material.color = normalColor;

                if (redFlashImage != null)
                    redFlashImage.SetActive(false);

                if (greenFlashImage != null)
                    greenFlashImage.SetActive(false);
            }
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth < 0)
            currentHealth = 0;

        flashCounter = flashLength;
        isHealing = false;

        UpdateHealthText();
    }

    public void Heal(float healAmount)
    {
        currentHealth += Mathf.RoundToInt(healAmount);

        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        flashCounter = flashLength;
        isHealing = true;

        UpdateHealthText();
    }

    // =========================
    // FIXED HEALTH TEXT SYSTEM
    // =========================
    public void UpdateHealthText()
    {
        if (hpText != null)
            hpText.text = currentHealth + " / " + maxHealth;
    }
}