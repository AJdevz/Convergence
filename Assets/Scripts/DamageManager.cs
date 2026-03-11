using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public int GiveDamage = 10; // Amount of damage dealt to the player
    public float damageInterval = 0.2f; // Time between each damage tick

    private bool isTouchingPlayer = false; // Tracks whether the enemy is touching the player
    private Coroutine damageCoroutine; // Stores the damage coroutine instance

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is the player
        if (other.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true; // Set flag to true
            damageCoroutine = StartCoroutine(DealDamageOverTime(other.gameObject)); // Start damaging the player over time
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the player exits the damage zone
        if (other.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false; // Set flag to false
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine); // Stop the ongoing damage coroutine
                damageCoroutine = null;
            }
        }
    }

    private IEnumerator DealDamageOverTime(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>(); // Get the player's health component

        // Continue dealing damage as long as the player is in contact and has a health component
        while (isTouchingPlayer && playerHealth != null)
        {
            playerHealth.TakeDamage(GiveDamage); // Apply damage to the player
            yield return new WaitForSeconds(damageInterval); // Wait for the next damage tick
        }
    }
}
