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

    void Start()
    {
        enemyRB = GetComponent<Rigidbody>();
        thePlayer = Object.FindFirstObjectByType<PlayerController>();

        originalSpeed = moveSpeed;
    }

    void Update()
    {
        if (thePlayer == null)
        {
            enemyRB.linearVelocity = Vector3.zero;
            return;
        }

        transform.LookAt(thePlayer.transform.position);

        // ❄️ Handle slow timer
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;

            if (slowTimer <= 0)
            {
                moveSpeed = originalSpeed; // reset speed
            }
        }
    }

    void FixedUpdate()
    {
        if (thePlayer != null)
        {
            enemyRB.linearVelocity = transform.forward * moveSpeed;
        }
        else
        {
            enemyRB.linearVelocity = Vector3.zero;
        }
    }

    // ❄️ THIS IS CALLED FROM BULLET
    public void ApplySlow(float slowPercent, float duration)
    {
        moveSpeed = originalSpeed * (1f - slowPercent);
        slowTimer = duration;
    }
}