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
    public float explosionRadius = 3f;
    public int explosionDamage = 20;

    [Header("Chain Lightning")]
    public int chainCount = 3;
    public float chainRange = 500f;

    [Header("Piercing")]
    public int pierceCount = 2;

    private int currentPierce;

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
                    enemyMove.ApplySlow(0.5f, 2f); // 50% slow for 2 seconds
                }
            }
        }

        // 🧩 Piercing logic
        if (piercing && currentPierce > 0)
        {
            currentPierce--;
            return; // DON'T destroy bullet yet
        }

        Destroy(gameObject);
    }

    // 💥 Explosion Function
    void Explode()
    {
        float radius = explosionRadius;

        // 💥 VFX
        if (explosionPrefab != null)
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // 🌐 VISUAL RING
        if (explosionRadiusVisual != null)
        {
            GameObject ring = Instantiate(explosionRadiusVisual, transform.position, Quaternion.identity);
            float scale = radius * 2f;
            ring.transform.localScale = new Vector3(scale, 0.1f, scale);
            Destroy(ring, 0.25f);
        }

        // 💥 DEBUG (IMPORTANT FOR TESTING)
        Debug.Log("Explosion radius: " + radius);

        // 💣 DAMAGE
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(explosionDamage);
            }
        }
    }

    // ⚡ Chain Lightning Function
    IEnumerator ChainLightning(Transform firstTarget)
    {
        Transform currentTarget = firstTarget;

        for (int i = 0; i < chainCount; i++)
        {
            // 🛑 STOP if target is destroyed
            if (currentTarget == null) yield break;

            Collider[] hits = Physics.OverlapSphere(currentTarget.position, chainRange);

            Transform nextTarget = null;
            float closestDistance = Mathf.Infinity;

            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Enemy") && hit.transform != currentTarget)
                {
                    float dist = Vector3.Distance(currentTarget.position, hit.transform.position);

                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        nextTarget = hit.transform;
                    }
                }
            }

            // 🛑 STOP if no valid target
            if (nextTarget == null) yield break;

            // 🛑 EXTRA safety check
            if (nextTarget == null || currentTarget == null) yield break;

            // ⚡ VFX (safe)
            if (lightningLinePrefab != null)
            {
                LineRenderer line = Instantiate(lightningLinePrefab);

                // number of zigzag points
                int segments = 6;
                line.positionCount = segments;

                Vector3 start = currentTarget.position;
                Vector3 end = nextTarget.position;

                for (int j = 0; j < segments; j++)
                {
                    float t = j / (float)(segments - 1);

                    Vector3 point = Vector3.Lerp(start, end, t);

                    // ⚡ ADD ZIGZAG OFFSET
                    if (j != 0 && j != segments - 1)
                    {
                        point += new Vector3(
                            Random.Range(-0.5f, 0.5f),
                            Random.Range(-0.2f, 0.2f),
                            Random.Range(-0.5f, 0.5f)
                        );
                    }

                    line.SetPosition(j, point);
                }

                Destroy(line.gameObject, 0.1f);
            }

            // 💥 Damage
            EnemyHealth enemy = nextTarget.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                float chainDamageMultiplier = 0.75f + (chainCount * 0.05f);
                enemy.TakeDamage(Mathf.RoundToInt(GiveDamage * chainDamageMultiplier));
            }

            currentTarget = nextTarget;

            yield return new WaitForSeconds(0.05f);
        }
    }
}