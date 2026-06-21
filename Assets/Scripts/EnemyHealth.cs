using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour
{
    [Header("Base Stats")]
    public int baseHealth = 100;

    [Header("Enemy Type")]
    public bool isTankBoss = false;   // merged boss
    public bool isMainBoss = false;   // wave boss
    private int maxHealth;

    [Header("Scaling")]
    public int waveHealthIncrease = 20;
    public int levelHealthIncrease = 10;

    [Header("Health Bar")]
    public GameObject healthBarPrefab;
    private EnemyHealthBar healthBarInstance;

    [Header("Boss UI")]
    public bool showBossUI = false;

    [Header("Drops")]
    public GameObject xpDropPrefab;
    public GameObject bossXpDropPrefab;
    public GameObject healthDropPrefab;

    public int normalXpAmount = 10;
    public int bossXpAmount = 100;

    [Range(0f, 1f)]
    public float healthDropChance = 0.4f;

    private int currentHealth;
    private int currentWave;
    private bool isDead = false;

    private int pendingDamage = 0;
    private float damageTimer = 0f;
    public float damageCombineWindow = 0.15f;

    public event Action OnEnemyDeath;

    [SerializeField] int coinReward = 2;

    void Start()
    {
        SpawnEnemies spawnManager = FindFirstObjectByType<SpawnEnemies>();
        currentWave = spawnManager != null ? spawnManager.waveNumber : 1;

        ApplyScaling();

        if (healthBarPrefab != null)
        {
            GameObject bar = Instantiate(healthBarPrefab, transform);
            bar.transform.localPosition = new Vector3(0, 2.5f, 0);

            healthBarInstance = bar.GetComponent<EnemyHealthBar>();

            if (healthBarInstance != null)
                healthBarInstance.Setup(transform, currentHealth);
        }

        if (isMainBoss)
        {
            BossUIManager.Instance?.SetBoss(this);
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
        float levelScale = 1f + (level - 1) * 0.04f;
        float burstProtection = 1f + Mathf.Log10(wave) * 0.5f;

        float scaled = baseHp * linear * exponential * levelScale * burstProtection;

        if (isMainBoss)
            scaled *= 5f;

        if (isTankBoss)
            scaled *= 2f;

        maxHealth = Mathf.RoundToInt(scaled);
        currentHealth = maxHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        pendingDamage += damage;

        if (currentHealth <= 0)
        {
            ShowDamageNumber();
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
                ShowDamageNumber();
                pendingDamage = 0;
            }
        }
    }

    void ShowDamageNumber()
    {
        if (DamageNumberManager.Instance == null) return;
        if (pendingDamage <= 0) return;

        Vector3 spawnPos = transform.position;

        Collider col = GetComponentInChildren<Collider>();
        if (col != null)
            spawnPos = col.bounds.center + Vector3.up * col.bounds.extents.y;

        DamageNumberManager.Instance.ShowDamage(pendingDamage, spawnPos);
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        int reward = coinReward;

        if (isMainBoss)
            reward *= 10;
        else if (isTankBoss)
            reward *= 5;

        CoinsManager.Instance.AddRunCoins(reward);

        DropLoot();
        OnEnemyDeath?.Invoke();

        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);

        Destroy(gameObject);
    }

    void DropLoot()
    {
        GameObject xpPrefab =
            isMainBoss ? bossXpDropPrefab :
            isTankBoss ? bossXpDropPrefab :
            xpDropPrefab;

        int xpValue =
            isMainBoss ? bossXpAmount * 3 :
            isTankBoss ? bossXpAmount * 2 :
            normalXpAmount;

        if (xpPrefab != null)
        {
            GameObject drop = Instantiate(xpPrefab, transform.position, Quaternion.identity);

            XPCollect xp = drop.GetComponent<XPCollect>();
            if (xp != null)
                xp.SetXP(xpValue);
        }

        if (healthDropPrefab != null && UnityEngine.Random.value <= healthDropChance)
        {
            Instantiate(healthDropPrefab, transform.position, Quaternion.identity);
        }
    }
}