using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Base")]
    public float speed;
    public float lifeTime;
    public int GiveDamage;

    [Header("Flags")]
    public bool explosiveShots;
    public bool chainLightning;
    public bool piercing;
    public bool freezeEffect;

    [Header("Stats")]
    public float explosionRadius;
    public int chainCount;
    public float chainRange = 9f;
    public int pierceCount;
    public float freezeStrength;
    public float freezeDuration;
    public float lifestealPercent;

    [Header("Scaling")]
    public float explosionMultiplier;
    public float chainMultiplier;

    [Header("Refs")]
    [SerializeField] LayerMask enemyLayer;
    public LineRenderer lightningLinePrefab;
    public GameObject explosionPrefab;
    public GameObject explosionRadiusVisual;

    private int currentPierce;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    private bool hasChained = false;

    void Start()
    {
        currentPierce = pierceCount;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.CompareTag("Enemy")) return;

        EnemyHealth enemy = other.gameObject.GetComponent<EnemyHealth>();

        if (enemy != null)
        {
            // 💥 Base Damage
            enemy.TakeDamage(GiveDamage);

            // 💥 Explosion
            if (explosiveShots)
                Explode();

            // ⚡ Chain Lightning (ALWAYS works now)
            if (chainLightning && !hasChained)
            {
                hasChained = true;
                DoChain(enemy.transform);
            }

            // ❄️ Freeze
            if (freezeEffect)
            {
                EnemyController enemyMove = other.gameObject.GetComponent<EnemyController>();
                if (enemyMove != null)
                    enemyMove.ApplySlow(freezeStrength, freezeDuration);
            }
        }

        // 🧠 TRACK HIT AFTER abilities
        if (hitEnemies.Contains(other.gameObject))
            return;

        hitEnemies.Add(other.gameObject);

        // 🧩 Piercing logic
        if (piercing && currentPierce > 0)
        {
            currentPierce--;
            return;
        }

        Destroy(gameObject);
    }

    // 💥 Explosion Function
    // 💥 Explosion Function
    void Explode()
    {
        float radius = explosionRadius;

        // 💥 VFX (scaled properly)
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            float scale = radius * 2f; // match radius
            explosion.transform.localScale = new Vector3(scale, scale, scale);

            Destroy(explosion, 1f);
        }

        // 🌐 VISUAL RING (matches EXACT radius)
        if (explosionRadiusVisual != null)
        {
            GameObject ring = Instantiate(explosionRadiusVisual, transform.position, Quaternion.identity);

            float scale = radius * 2f;
            ring.transform.localScale = new Vector3(scale, 0.1f, scale);

            Destroy(ring, 0.3f);
        }

        Debug.Log("Explosion radius: " + radius);

        // 💣 DAMAGE
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                int finalDamage = Mathf.RoundToInt(GiveDamage * explosionMultiplier);
                enemy.TakeDamage(finalDamage);
            }
        }
    }

    // ⚡ Chain Lightning Function
    void DoChain(Transform startTarget)
    {
        Transform currentTarget = startTarget;

        HashSet<GameObject> hitTargets = new HashSet<GameObject>();
        hitTargets.Add(currentTarget.gameObject);

        int maxJumps = chainCount;

        for (int i = 0; i < maxJumps; i++)
        {
            if (currentTarget == null) return;

            Collider[] hits = Physics.OverlapSphere(currentTarget.position, chainRange);

            Transform nextTarget = null;
            float closest = Mathf.Infinity;

            // 🔥 STRICT FILTERING
            for (int h = 0; h < hits.Length; h++)
            {
                Collider hit = hits[h];

                if (!hit.CompareTag("Enemy")) continue;

                GameObject enemyObj = hit.gameObject;

                if (hitTargets.Contains(enemyObj)) continue;

                float dist = Vector3.Distance(currentTarget.position, hit.transform.position);

                if (dist < closest)
                {
                    closest = dist;
                    nextTarget = hit.transform;
                }
            }

            // ❌ STOP IF NO VALID TARGET
            if (nextTarget == null) return;

            hitTargets.Add(nextTarget.gameObject);

            // 💥 DAMAGE
            EnemyHealth hp = nextTarget.GetComponent<EnemyHealth>();
            if (hp != null)
            {
                int dmg = Mathf.RoundToInt(GiveDamage * chainMultiplier);
                hp.TakeDamage(dmg);
            }

            // ⚡ ZIG ZAG LIGHTNING
            if (lightningLinePrefab != null)
            {
                LineRenderer line = Instantiate(lightningLinePrefab);

                int segments = 8;
                line.positionCount = segments;

                Vector3 start = currentTarget.position;
                Vector3 end = nextTarget.position;

                Vector3 dir = (end - start).normalized;
                Vector3 right = Vector3.Cross(dir, Vector3.up);

                for (int j = 0; j < segments; j++)
                {
                    float t = j / (float)(segments - 1);

                    Vector3 point = Vector3.Lerp(start, end, t);

                    if (j != 0 && j != segments - 1)
                    {
                        float zigzag = Mathf.Sin(t * 10f) * 0.35f;
                        point += right * zigzag;

                        point += new Vector3(
                            Random.Range(-0.1f, 0.1f),
                            Random.Range(-0.05f, 0.05f),
                            Random.Range(-0.1f, 0.1f)
                        );
                    }

                    line.SetPosition(j, point);
                }

                Destroy(line.gameObject, 0.15f);
            }

            currentTarget = nextTarget;
        }
    }
}