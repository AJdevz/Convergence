using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int health; // Max health
    public int currentHealth;

    [Header("Flash Settings")]
    public float flashLength = 0.1f; // Duration of flash
    private float flashCounter;
    private Renderer rend;
    private Color normalColor;
    private Color damageColor = Color.red;
    private Color healColor = Color.green;
    private bool isHealing = false; // Track if currently healing for color flash

    [Header("UI References")]
    public GameObject redFlashImage;   // UI flash for damage
    public GameObject greenFlashImage; // UI flash for healing
    public GameObject endGamePanel;

    void Start()
    {
        currentHealth = health;
        rend = GetComponent<Renderer>();
        normalColor = rend.material.GetColor("_Color");

        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        if (redFlashImage != null)
            redFlashImage.SetActive(false);

        if (greenFlashImage != null)
            greenFlashImage.SetActive(false);
    }

    void Update()
    {
        // Check for death
        if (currentHealth <= 0)
        {
            if (endGamePanel != null)
                endGamePanel.SetActive(true); // Show the end game panel

            gameObject.SetActive(false); // Disable the player object
        }

        // Handle flash timer
        if (flashCounter > 0)
        {
            flashCounter -= Time.deltaTime;

            // Update renderer color based on flash type
            if (isHealing)
                rend.material.color = healColor;
            else
                rend.material.color = damageColor;

            // Activate the correct flash UI
            if (isHealing)
            {
                if (greenFlashImage != null)
                    greenFlashImage.SetActive(true);
            }
            else
            {
                if (redFlashImage != null)
                    redFlashImage.SetActive(true);
            }

            // Reset after flash ends
            if (flashCounter <= 0)
            {
                rend.material.color = normalColor;

                if (redFlashImage != null)
                    redFlashImage.SetActive(false);

                if (greenFlashImage != null)
                    greenFlashImage.SetActive(false);
            }
        }
    }

    // Called when player takes damage
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth < 0)
            currentHealth = 0;

        // Start flash
        flashCounter = flashLength;
        isHealing = false;
    }

    // Called when player heals
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > health)
            currentHealth = health;

        // Start flash
        flashCounter = flashLength;
        isHealing = true;
    }
}
