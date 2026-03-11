using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider; // Reference to the UI Slider
    private PlayerHealth playerHealth; // Reference to PlayerHealth

    void Start()
    {
        // Find the PlayerHealth script in the scene
        playerHealth = Object.FindFirstObjectByType<PlayerHealth>();

        if (playerHealth != null)
        {
            // Set the max health when the game starts
            SetMaxHealth(playerHealth.health);
        }
    }

    void Update()
    {
        if (playerHealth != null)
        {
            // Update the slider value based on current health
            SetHealth(playerHealth.currentHealth);
        }
    }

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
