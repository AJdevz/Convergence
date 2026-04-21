using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Base Stats")]
    public int baseHealth = 100;
    public bool isBoss = false;

    [Header("Scaling")]
    public int waveHealthIncrease = 20;
    public int levelHealthIncrease = 10;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;

    private EnemyHealthBar healthBarInstance;

    [Header("Drops")]
    public GameObject xpDropPrefab;
    public GameObject bossXpDropPrefab;
    public GameObject healthDropPrefab;

    public int normalXpAmount = 10;
    public int bossXpAmount = 5000;

    [Range(0f, 1f)]
    public float healthDropChance = 0.4f;

    private int currentHealth;
    private int currentWave;
    private bool isDead = false; // ✅ Death protection

    public event Action OnEnemyDeath;

    void Start()
    {
            Debug.Log("=== ENEMY HEALTH DEBUG ===");
            Debug.Log("Enemy: " + gameObject.name);
            Debug.Log("healthBarPrefab reference = " + healthBarPrefab);

            SpawnEnemies spawnManager = FindFirstObjectByType<SpawnEnemies>();

        if (spawnManager != null)
            currentWave = spawnManager.waveNumber;
        else
            currentWave = 1;

        ApplyScaling();

        if (healthBarPrefab != null)
        {
            GameObject bar = Instantiate(healthBarPrefab, transform);
            bar.transform.localPosition = new Vector3(0, 2.5f, 0);
            bar.transform.localRotation = Quaternion.identity; 

            healthBarInstance = bar.GetComponent<EnemyHealthBar>();

            if (healthBarInstance != null)
            {
                healthBarInstance.Setup(transform, currentHealth);
            }
            else
            {
                Debug.LogError("EnemyHealthBar script missing on prefab!");
            }
        }
        else
        {
            Debug.LogError("HealthBarPrefab is NULL");
        }
    }

    void ApplyScaling()
    {
        int playerLevel = XPManager.Instance != null ? XPManager.Instance.playerLevel : 1;

        // Wave scaling (Wave 1 = base health)
        int waveBonus = (currentWave - 1) * waveHealthIncrease;

        // Player level scaling (Level 1 = no bonus)
        int levelBonus = (playerLevel - 1) * levelHealthIncrease;

        int scaledHealth = baseHealth + waveBonus + levelBonus;

        if (isBoss)
            scaledHealth *= 3;

        currentHealth = scaledHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (healthBarInstance != null)
            healthBarInstance.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return; // 🔥 Double safety
        isDead = true;

        DropLoot();

        OnEnemyDeath?.Invoke();

        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);

        Destroy(gameObject);

    }

    void DropLoot()
    {
        // XP Drop
        GameObject xpPrefab = isBoss ? bossXpDropPrefab : xpDropPrefab;

        if (xpPrefab != null)
        {
            GameObject droppedXP = Instantiate(xpPrefab, transform.position, Quaternion.identity);

            XPCollect xpScript = droppedXP.GetComponent<XPCollect>();
            if (xpScript != null)
                xpScript.SetXP(isBoss ? bossXpAmount : normalXpAmount);
        }

        // Health Drop (chance-based)
        if (healthDropPrefab != null && UnityEngine.Random.value <= healthDropChance)
        {
            Instantiate(healthDropPrefab, transform.position, Quaternion.identity);
        }
    }
}
