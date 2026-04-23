using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Base Stats")]
    public float speed;
    public float lifeTime;
    public int GiveDamage;

    [Header("Upgrade Flags")]
    public bool explosiveShots;
    public bool chainLightning;
    public bool piercing;
    public bool freezeEffect;

    public float lifestealPercent;

    [Header("Explosion")]
    [SerializeField] LayerMask enemyLayer;
    public float explosionRadius = 3f;
    public int explosionDamage = 20;

    [Header("Chain Lightning")]
    public int chainCount = 3;
    public float chainRange = 9f;

    [Header("Piercing")]
    public int pierceCount = 2;
    private int currentPierce;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    [Header("Freeze Effect")]
    public float freezeStrength = 0.5f;
    public float freezeDuration = 2f;

    public LineRenderer lightningLinePrefab;
    public GameObject explosionPrefab;
    public GameObject explosionRadiusVisual;

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

            // 🩸 VFX
            if (VFXManager.Instance != null)
                VFXManager.Instance.PlayVFX(VFXManager.Instance.bloodEffect, transform.position);

            // 💥 Explosion
            if (explosiveShots)
            {
                Explode();
            }

            // ⚡ Chain Lightning
            if (chainLightning)
            {
                StartCoroutine(ChainLightning(enemy.transform));
            }

            // ❄️ Freeze (simple version placeholder)
            if (freezeEffect)
            {
                EnemyController enemyMove = other.gameObject.GetComponent<EnemyController>();

                if (enemyMove != null)
                {
                    enemyMove.ApplySlow(freezeStrength, freezeDuration);
                }
            }
        }

        // 🧩 Piercing logic
        // 🧩 Piercing logic
        // 🧩 Piercing logic (FIXED)
        if (hitEnemies.Contains(other.gameObject))
        {
            return; // already hit this enemy, ignore
        }

        hitEnemies.Add(other.gameObject);

        if (piercing && currentPierce > 0)
        {
            currentPierce--;
            return; // keep flying
        }

        // no pierce left → destroy
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
                enemy.TakeDamage(explosionDamage); // 💥 FULL DAMAGE ALWAYS
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // ⚡ Chain Lightning Function
    IEnumerator ChainLightning(Transform firstTarget)
    {
        Transform currentTarget = firstTarget;

        // 🧠 Track already hit enemies (VERY IMPORTANT)
        HashSet<Transform> hitTargets = new HashSet<Transform>();
        hitTargets.Add(firstTarget);

        for (int i = 0; i < chainCount; i++)
        {
            if (currentTarget == null) yield break;

            Collider[] hits = Physics.OverlapSphere(currentTarget.position, chainRange);

            Transform nextTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;

                Transform t = hit.transform;

                // 🚫 skip already hit targets
                if (hitTargets.Contains(t)) continue;

                float dist = Vector3.Distance(currentTarget.position, t.position);

                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    nextTarget = t;
                }
            }

            if (nextTarget == null) yield break;

            // ⚡ LIGHTNING VFX
            if (lightningLinePrefab != null)
            {
                LineRenderer line = Instantiate(lightningLinePrefab);

                int segments = 7;
                line.positionCount = segments;

                Vector3 start = currentTarget.position;
                Vector3 end = nextTarget.position;

                for (int j = 0; j < segments; j++)
                {
                    float t = j / (float)(segments - 1);
                    Vector3 point = Vector3.Lerp(start, end, t);

                    // ⚡ Stronger zig-zag
                    if (j != 0 && j != segments - 1)
                    {
                        point += new Vector3(
                            Random.Range(-0.7f, 0.7f),
                            Random.Range(-0.3f, 0.3f),
                            Random.Range(-0.7f, 0.7f)
                        );
                    }

                    line.SetPosition(j, point);
                }

                Destroy(line.gameObject, 0.08f);
            }

            // 💥 DAMAGE (better scaling)
            EnemyHealth enemy = nextTarget.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                int chainDamage = Mathf.RoundToInt(GiveDamage * (1f + (chainCount * 0.1f)));
                enemy.TakeDamage(chainDamage);
            }

            // ✅ mark as hit
            hitTargets.Add(nextTarget);

            currentTarget = nextTarget;

            yield return new WaitForSeconds(0.04f);
        }
    }
}