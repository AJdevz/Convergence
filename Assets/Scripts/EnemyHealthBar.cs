using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    private Transform target;

    public void Setup(Transform enemy, int maxHealth)
    {
        target = enemy;

        if (slider == null)
            slider = GetComponentInChildren<Slider>();

        slider.maxValue = maxHealth;
        slider.value = maxHealth;
    }

    public void UpdateHealth(int currentHealth)
    {
        if (slider != null)
            slider.value = currentHealth;
    }

    void LateUpdate()
    {
        if (target == null) return;

        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}