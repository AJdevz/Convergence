using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody enemyRB;
    public float moveSpeed;

    private float originalSpeed;
    private float slowTimer = 0f;

    private PlayerController thePlayer;

    [Header("Repel Settings")]
    public float repelDistance = 2.5f;
    public float repelTime = 0.15f;

    private bool isRepelling = false;
    private Vector3 repelVelocity;
    private float repelTimer;

    void Start()
    {
        enemyRB = GetComponent<Rigidbody>();
        thePlayer = Object.FindFirstObjectByType<PlayerController>();
        originalSpeed = moveSpeed;
    }

    void Update()
    {
        if (thePlayer == null) return;

        transform.LookAt(thePlayer.transform.position);

        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0)
                moveSpeed = originalSpeed;
        }

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

        // NORMAL CHASE
        Vector3 moveDir = (thePlayer.transform.position - transform.position).normalized;
        enemyRB.linearVelocity = moveDir * moveSpeed;
    }

    // 💥 SIMPLE REPEL FROM PLAYER (MAIN FIX)
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

    public void ApplySlow(float slowPercent, float duration)
    {
        moveSpeed = originalSpeed * (1f - slowPercent);
        slowTimer = duration;
    }
}