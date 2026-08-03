using System.Collections;
using UnityEngine;

public class TelemetryLogger : MonoBehaviour
{
    public SpawnEnemies spawnEnemies;
    public PlayerHealth playerHealth;

    private int mergedEnemies;
    private int mergeEvents;

    // FPS tracking
    private int frameCount;
    private int currentFPS;
    private float fpsTimer;


    private void Start()
    {
        AnalyticsManager.Initialise();

        Debug.Log("Telemetry Started");
        Debug.Log("CSV Location: " + AnalyticsManager.GetFilePath());

        StartCoroutine(LogRoutine());
    }


    private void Update()
    {
        // Count frames
        frameCount++;
        fpsTimer += Time.unscaledDeltaTime;

        // Update FPS every second
        if (fpsTimer >= 1f)
        {
            currentFPS = frameCount;

            frameCount = 0;
            fpsTimer = 0f;
        }
    }


    public void AddMergedEnemy()
    {
        mergedEnemies++;
    }


    public void AddMergeEvent()
    {
        mergeEvents++;
    }


    IEnumerator LogRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (spawnEnemies == null)
            {
                Debug.LogError("TelemetryLogger: SpawnEnemies reference missing!");
                continue;
            }

            if (playerHealth == null)
            {
                Debug.LogError("TelemetryLogger: PlayerHealth reference missing!");
                continue;
            }


            AnalyticsManager.LogTelemetry(
                currentFPS,
                spawnEnemies.CurrentWave,
                spawnEnemies.ActiveEnemies,
                mergedEnemies,
                mergeEvents,
                playerHealth.currentHealth
            );
        }
    }
}