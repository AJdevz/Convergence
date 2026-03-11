using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed; // Speed at which the bullet moves
    public float lifeTime; // How long the bullet exists before being destroyed
    public int GiveDamage; // Damage dealt by the bullet

    void Update()
    {
        // Move the bullet forward based on its speed
        transform.Translate(Vector3.forward * speed * Time.deltaTime);

        // Reduce the bullet's lifetime over time
        lifeTime -= Time.deltaTime;

        // Destroy the bullet once its lifetime expires
        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Check if the bullet collides with an enemy
        if (other.gameObject.CompareTag("Enemy"))
        {
            // Apply damage to the enemy
            other.gameObject.GetComponent<EnemyHealth>().TakeDamage(GiveDamage);

            // Destroy the bullet on impact
            Destroy(gameObject);
        }
    }
}
