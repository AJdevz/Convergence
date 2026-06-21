using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    private PlayerHealth playerHealth;
    public TMP_Text hpText;

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth == null) return;

        slider.maxValue = playerHealth.maxHealth;
        slider.value = playerHealth.currentHealth;

        if (hpText != null)
            hpText.text = $"{playerHealth.currentHealth} / {playerHealth.maxHealth}";
    }
}