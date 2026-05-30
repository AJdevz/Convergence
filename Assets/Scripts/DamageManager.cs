using System.Collections;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    [Header("Base Damage")]
    public int baseDamage = 5;

    [Header("Scaling")]
    public float damageIncreasePerWave = 0.12f;

    [Header("Boss Settings")]
    public bool isBoss = false;
    public float bossDamageMultiplier = 3f;

    [Header("Attack Speed")]
    public float damageInterval = 1f;

    private bool isTouchingPlayer = false;
    private Coroutine damageCoroutine;

    private SpawnEnemies waveManager;

    void Start()
    {
        waveManager = FindFirstObjectByType<SpawnEnemies>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTouchingPlayer = true;
            damageCoroutine = StartCoroutine(DealDamageOverTime(other.gameObject));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isTouchingPlayer = false;

            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DealDamageOverTime(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        while (isTouchingPlayer && playerHealth != null)
        {
            int currentWave = waveManager.waveNumber;

            // Wave scaling
            float scaledDamage =
                baseDamage * (1 + (currentWave * damageIncreasePerWave));

            // Boss multiplier
            if (isBoss)
                scaledDamage *= bossDamageMultiplier;

            playerHealth.TakeDamage(Mathf.RoundToInt(scaledDamage));

            yield return new WaitForSeconds(damageInterval);
        }
    }
}