using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth == null)
            return;

        // Update max HP dynamically
        slider.maxValue = playerHealth.maxHealth;

        // Update current HP
        slider.value = playerHealth.currentHealth;
    }
}