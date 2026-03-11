using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody enemyRB;
    public float moveSpeed;

    private PlayerController thePlayer; // Changed to private to avoid accidental reassignments

    void Start()
    {
        enemyRB = GetComponent<Rigidbody>();
        thePlayer = Object.FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (thePlayer == null)
        {
            // Player is dead or missing, stop movement
            enemyRB.linearVelocity = Vector3.zero;
            return;
        }

        transform.LookAt(thePlayer.transform.position);
    }

    void FixedUpdate()
    {
        if (thePlayer != null) // Check if player exists before moving
        {
            enemyRB.linearVelocity = transform.forward * moveSpeed;
        }
        else
        {
            enemyRB.linearVelocity = Vector3.zero; // Stop movement if player is gone
        }
    }
}
