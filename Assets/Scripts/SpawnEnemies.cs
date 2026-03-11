using System.Collections;
using UnityEngine;

public class SpawnEnemies : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject EnemyZombies;
    public GameObject ZombieMerge;
    public GameObject bossEnemy;

    [Header("Wave Settings")]
    public int enemiesPerWave = 0;
    public int waveNumber = 1;
    public float minSpawnDistance = 20f;
    public float spawnHeight = 1f;

    [Header("References")]
    public GameObject player;
    public GameObject floor;

    private WaveTextDisplay waveTextDisplay;
    private bool firstWaveMessageShown = false;
    private Bounds floorBounds;
    private int enemiesAlive = 0;

    void Start()
    {
        waveTextDisplay = FindFirstObjectByType<WaveTextDisplay>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (floor == null)
            floor = GameObject.Find("Floor");

        Renderer floorRenderer = floor.GetComponent<Renderer>();
        floorBounds = floorRenderer.bounds;

        StartCoroutine(EnemySpawnLoop());
    }

    void Update()
    {
        int actualEnemies = FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None).Length;

        if (actualEnemies != enemiesAlive)
        {
            Debug.LogWarning("DESYNC! Tracked: " + enemiesAlive + " | Actual: " + actualEnemies);
        }
    }


    IEnumerator EnemySpawnLoop()
    {
        if (!firstWaveMessageShown)
        {
            waveTextDisplay?.DisplayWaveText("Good Luck!");
            yield return new WaitForSeconds(2f);
            firstWaveMessageShown = true;
        }

        while (true)
        {
            waveTextDisplay?.DisplayWaveText("Wave: " + waveNumber);
            yield return new WaitForSeconds(2f);

            enemiesAlive = 0;

            if (GameManager.Instance.SelectedMode == GameMode.Escalation)   
                yield return StartCoroutine(SpawnWave(false));
            else
                yield return StartCoroutine(SpawnWave(true));

            yield return new WaitUntil(() => enemiesAlive <= 0);

            waveNumber++;
        }
    }

    IEnumerator SpawnWave(bool useMerge)
    {
        int enemiesInWave = enemiesPerWave + (waveNumber * 5);

        for (int i = 0; i < enemiesInWave; i++)
        {
            SpawnEnemy(useMerge);
            yield return new WaitForSeconds(0.2f);
        }

        if (waveNumber % 5 == 0)
            SpawnBoss();
    }

    void SpawnEnemy(bool useMerge)
    {
        GameObject prefab = useMerge ? ZombieMerge : EnemyZombies;
        if (prefab == null) return;

        Vector3 spawnPos = GetValidSpawnPosition();
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        RegisterEnemy(enemy);
    }

    void SpawnBoss()
    {
        if (bossEnemy == null) return;

        Vector3 spawnPos = GetValidSpawnPosition();
        GameObject boss = Instantiate(bossEnemy, spawnPos, Quaternion.identity);

        RegisterEnemy(boss);
    }

    public void RegisterEnemy(GameObject enemy)
    {
        enemiesAlive++;
        EnemyHealth health = enemy.GetComponent<EnemyHealth>();
        if (health != null)
            health.OnEnemyDeath += () => enemiesAlive--;
    }

    // Called manually by ZombieMerge before destroying merged zombies
    public void EnemyDied()
    {
        enemiesAlive--;
    }

    Vector3 GetValidSpawnPosition()
    {
        Vector3 spawnPosition;
        int attempts = 0;

        do
        {
            float randomX = Random.Range(floorBounds.min.x, floorBounds.max.x);
            float randomZ = Random.Range(floorBounds.min.z, floorBounds.max.z);
            spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

            attempts++;
            if (attempts > 50) break;

        } while (Vector3.Distance(spawnPosition, player.transform.position) < minSpawnDistance);

        return spawnPosition;
    }
}
