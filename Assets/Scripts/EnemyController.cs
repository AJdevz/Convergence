using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody enemyRB;

    [Header("Base Movement")]
    public float moveSpeed;

    private PlayerController thePlayer;

    // =========================
    // 🧊 STATUS SYSTEM (NEW)
    // =========================
    private float slowMultiplier = 1f;
    private float freezeMultiplier = 1f;

    private float slowTimer;
    private bool isFrozen;

    [Header("Repel Settings")]
    public float repelDistance = 2.5f;
    public float repelTime = 0.15f;

    private bool isRepelling = false;
    private Vector3 repelVelocity;
    private float repelTimer;

    private float growlTimer;

    void Start()
    {
        enemyRB = GetComponent<Rigidbody>();
        thePlayer = Object.FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (thePlayer == null) return;

        transform.LookAt(thePlayer.transform.position);

        // 🎧 Growl system
        growlTimer -= Time.deltaTime;
        if (growlTimer <= 0f)
        {
            SoundManager.Instance?.PlayZombieGrowl();
            growlTimer = Random.Range(3f, 6f);
        }

        // ⏳ Slow timer
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;

            if (slowTimer <= 0)
                slowMultiplier = 1f;
        }

        // ⏳ Repel timer
        if (isRepelling)
        {
            repelTimer -= Time.deltaTime;

            if (repelTimer <= 0f)
                isRepelling = false;
        }
    }

    void FixedUpdate()
    {
        if (thePlayer == null)
        {
            enemyRB.linearVelocity = Vector3.zero;
            return;
        }

        // 🔥 REPULSION OVERRIDE
        if (isRepelling)
        {
            enemyRB.linearVelocity = repelVelocity;
            return;
        }

        // ❄️ FREEZE OVERRIDE
        if (isFrozen)
        {
            enemyRB.linearVelocity = Vector3.zero;
            return;
        }

        // 🧠 NORMAL CHASE
        Vector3 moveDir = (thePlayer.transform.position - transform.position).normalized;

        float finalSpeed = moveSpeed * slowMultiplier * freezeMultiplier;

        enemyRB.linearVelocity = moveDir * finalSpeed;
    }

    // =========================
    // 💥 REPULSION
    // =========================
    public void ApplyRepelFromPlayer()
    {
        if (thePlayer == null) return;

        Vector3 dir = transform.position - thePlayer.transform.position;
        dir.y = 0f;
        dir = dir.normalized;

        repelVelocity = dir * repelDistance / repelTime;

        isRepelling = true;
        repelTimer = repelTime;
    }

    // =========================
    // 🧊 SLOW SYSTEM (FIXED)
    // =========================
    public void ApplySlow(float slowPercent, float duration)
    {
        slowMultiplier = 1f - slowPercent;
        slowTimer = duration;
    }

    // =========================
    // ❄️ FREEZE SYSTEM (FIXED)
    // =========================
    public void Freeze(float duration)
    {
        StopCoroutine("FreezeRoutine");
        StartCoroutine(FreezeRoutine(duration));
    }

    IEnumerator FreezeRoutine(float duration)
    {
        isFrozen = true;
        freezeMultiplier = 0f;

        yield return new WaitForSeconds(duration);

        freezeMultiplier = 1f;
        isFrozen = false;
    }
}