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

        EnemyHealth enemy = other.gameObject.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(GiveDamage);

            // 🎯 TRUE IMPACT DIRECTION (bullet → enemy)
            Vector3 dir = enemy.transform.position - transform.position;

            // keep horizontal push, but allow slight lift
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
                dir.Normalize();

            // 🔥 FEEDBACK
            EnemyHitFeedback feedback = enemy.GetComponentInParent<EnemyHitFeedback>();

            if (feedback != null)
            {
                feedback.PlayHitFeedback(dir);

                if (CameraShake.Instance != null)
                    CameraShake.Instance.Shake(0.12f, 0.35f);
            }

            // 💥 KNOCKBACK (STRONG + CONSISTENT)
            EnemyController controller = enemy.GetComponentInParent<EnemyController>();

            if (controller != null)
            {
                controller.ApplyRepelFromPlayer();
            }

            // 💣 EXPLOSIVE
            if (explosiveShots)
                Explode();

            // ⚡ CHAIN
            if (chainLightning && !hasChained)
            {
                hasChained = true;
                DoChain(enemy.transform);
            }

            // ❄️ SLOW
            if (freezeEffect)
            {
                EnemyController enemyMove = other.gameObject.GetComponentInParent<EnemyController>();
                if (enemyMove != null)
                    enemyMove.ApplySlow(freezeStrength, freezeDuration);
            }
        }

        // 🧠 TRACK HIT
        if (hitEnemies.Contains(other.gameObject))
            return;

        hitEnemies.Add(other.gameObject);

        // 🧩 PIERCE
        if (piercing && currentPierce > 0)
        {
            currentPierce--;
            return;
        }

        Destroy(gameObject);
    }

    // 💥 Explosion Function
    void Explode()
    {
        float radius = explosionRadius;

        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            float scale = radius * 2f;
            explosion.transform.localScale = new Vector3(scale, scale, scale);
            Destroy(explosion, 1f);
        }

        if (explosionRadiusVisual != null)
        {
            GameObject ring = Instantiate(explosionRadiusVisual, transform.position, Quaternion.identity);
            float scale = radius * 2f;
            ring.transform.localScale = new Vector3(scale, 0.1f, scale);
            Destroy(ring, 0.3f);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                int finalDamage = Mathf.RoundToInt(GiveDamage * explosionMultiplier);
                enemy.TakeDamage(finalDamage);

                EnemyHitFeedback feedback = hit.GetComponentInParent<EnemyHitFeedback>();
                if (feedback != null)
                {
                    Vector3 dir = (hit.transform.position - transform.position).normalized;
                    feedback.PlayHitFeedback(dir);
                }
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

            for (int h = 0; h < hits.Length; h++)
            {
                Collider hit = hits[h];

                EnemyHealth eh = hit.GetComponentInParent<EnemyHealth>();
                if (eh == null) continue;

                GameObject enemyObj = eh.gameObject;

                if (hitTargets.Contains(enemyObj)) continue;

                float dist = Vector3.Distance(currentTarget.position, enemyObj.transform.position);

                if (dist < closest)
                {
                    closest = dist;
                    nextTarget = enemyObj.transform;
                }
            }

            if (nextTarget == null) return;

            hitTargets.Add(nextTarget.gameObject);

            EnemyHealth hp = nextTarget.GetComponentInParent<EnemyHealth>();
            if (hp != null)
            {
                int dmg = Mathf.RoundToInt(GiveDamage * chainMultiplier);
                hp.TakeDamage(dmg);

                EnemyHitFeedback feedback = nextTarget.GetComponentInParent<EnemyHitFeedback>();
                if (feedback != null)
                {
                    Vector3 dir = (nextTarget.position - currentTarget.position).normalized;
                    feedback.PlayHitFeedback(dir);
                }
            }

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
                            Random.Range(-3f, 3f),
                            Random.Range(-3f, 3f),
                            Random.Range(-3f, 3f)
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