using UnityEngine;

public class ZombieMerge : MonoBehaviour
{
    [Header("Merge Settings")]
    public float mergeRadius = 1.5f;
    [Tooltip("Time in seconds to reach the other zombie when merging")]
    public float mergeDuration = 8f;
    public float checkInterval = 1f;

    public GameObject bossPrefab;

    [Header("Merge Ring")]
    public GameObject mergeRing;
    public Color defaultRingColor = Color.green;

    [Header("Audio")]
    public AudioClip spawnSound;
    public AudioClip mergeSound;
    [Range(0f, 1f)] public float spawnVolume = 1f;
    [Range(0f, 1f)] public float mergeVolume = 1f;

    private AudioSource spawnAudioSource;
    private Renderer ringRenderer;
    private bool isMerging = false;
    private ZombieMerge targetZombie = null;
    private float mergeProgress = 0f;
    private Vector3 startPosition;

    void Start()
    {
        spawnAudioSource = GetComponent<AudioSource>();
        if (spawnAudioSource == null)
            spawnAudioSource = gameObject.AddComponent<AudioSource>();

        spawnAudioSource.clip = spawnSound;
        spawnAudioSource.spatialBlend = 1f;
        spawnAudioSource.volume = spawnVolume;
        spawnAudioSource.Play();

        if (mergeRing != null)
            ringRenderer = mergeRing.GetComponent<Renderer>();

        InvokeRepeating(nameof(CheckForMerge), checkInterval, checkInterval);
    }

    void Update()
    {
        if (isMerging && targetZombie != null)
        {
            mergeProgress += Time.deltaTime / mergeDuration;
            mergeProgress = Mathf.Clamp01(mergeProgress);

            transform.position = Vector3.Lerp(
                startPosition,
                targetZombie.transform.position,
                mergeProgress
            );

            if (ringRenderer != null)
            {
                float distance = Vector3.Distance(transform.position, targetZombie.transform.position);
                float progress = Mathf.Clamp01(1f - (distance / mergeRadius));
                ringRenderer.material.color = Color.Lerp(defaultRingColor, Color.red, progress);
            }

            if (Vector3.Distance(transform.position, targetZombie.transform.position) < 0.05f || mergeProgress >= 1f)
            {
                FinishMerge();
            }
        }
    }

    void CheckForMerge()
    {
        if (isMerging) return;

        Collider[] nearby = Physics.OverlapSphere(transform.position, mergeRadius);

        foreach (Collider col in nearby)
        {
            if (col.gameObject == gameObject) continue;

            ZombieMerge other = col.GetComponent<ZombieMerge>();

            if (other != null && !other.isMerging)
            {
                StartMerge(other);
                break;
            }
        }
    }

    void StartMerge(ZombieMerge other)
    {
        isMerging = true;
        targetZombie = other;
        other.isMerging = true;
        startPosition = transform.position;
        mergeProgress = 0f;
    }

    void FinishMerge()
    {
        if (targetZombie == null) return;

        Vector3 mergePosition = targetZombie.transform.position;

        // Play merge sound
        if (mergeSound != null)
        {
            GameObject tempAudio = new GameObject("MergeSound");
            tempAudio.transform.position = mergePosition;

            AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
            audioSource.clip = mergeSound;
            audioSource.volume = mergeVolume;
            audioSource.spatialBlend = 1f;
            audioSource.Play();

            Destroy(tempAudio, mergeSound.length);
        }

        SpawnEnemies spawnManager = FindFirstObjectByType<SpawnEnemies>();

        if (spawnManager != null)
        {
            spawnManager.EnemyDied();
            spawnManager.EnemyDied();
        }

        Destroy(targetZombie.gameObject);

        // Spawn boss (health scaling handled automatically by EnemyHealth)
        if (bossPrefab != null)
        {
            GameObject boss = Instantiate(bossPrefab, mergePosition, Quaternion.identity);

            if (spawnManager != null)
                spawnManager.RegisterEnemy(boss);
        }

        if (mergeRing != null)
            Destroy(mergeRing);

        Destroy(gameObject);

        targetZombie = null;
        isMerging = false;
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, mergeRadius);
    }
}
