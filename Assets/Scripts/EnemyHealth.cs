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

    [Header("Damage Popup Buffer")]
    private int pendingDamage = 0;
    private float damageTimer = 0f;
    public float damageCombineWindow = 0.15f;

    public event Action OnEnemyDeath;

    [SerializeField] int coinReward = 2;

    void Start()
    {
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

        float wave = currentWave;
        float level = playerLevel;

        float baseHp = baseHealth;

        float linear = 1f + (wave - 1) * 0.12f;
        float exponential = Mathf.Pow(1.15f, wave - 1);
        float levelScale = 1f + (level - 1) * 0.05f;

        float burstProtection = 1f + Mathf.Log10(wave) * 0.6f;

        float scaled = baseHp * linear * exponential * levelScale * burstProtection;

        if (isBoss)
        {
            scaled *= (6f + Mathf.Pow(wave, 1.04f) * 0.4f);
        }

        currentHealth = Mathf.RoundToInt(scaled);
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Enemy took damage: " + damage);
        if (isDead) return;

        currentHealth -= damage;

        pendingDamage += damage;

        // 💀 IF THIS HIT KILLS → SHOW ONCE
        if (currentHealth <= 0)
        {
            ShowDamageNumber(); // 👈 just call once
            pendingDamage = 0;

            Die();
            return;
        }

        damageTimer = damageCombineWindow;

        if (healthBarInstance != null)
            healthBarInstance.UpdateHealth(currentHealth);
    }

    void Update()
    {
        if (pendingDamage > 0)
        {
            damageTimer -= Time.deltaTime;

            if (damageTimer <= 0f)
            {
                ShowDamageNumber(); // ✅ FIXED CALL
                pendingDamage = 0;
            }
        }
    }

    void ShowDamageNumber()
    {
        if (DamageNumberManager.Instance == null) return;

        if (pendingDamage <= 0) return; // 🚫 prevents 0

        Vector3 spawnPos = transform.position;

        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
            spawnPos = col.bounds.center + Vector3.up * col.bounds.extents.y;

        // optional offset tweak (top-down feel)
        spawnPos += new Vector3(-0.3f, 0.5f, 0f);

        DamageNumberManager.Instance.ShowDamage(pendingDamage, spawnPos);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        int reward = coinReward;

        if (isBoss)
            reward *= 5;

        CoinsManager.Instance.AddCoins(reward);

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
