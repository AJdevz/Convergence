using UnityEngine;

public class HealthCollect : MonoBehaviour
{
    public int healthAmount = 10; // Amount healed

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.Heal(healthAmount);

            Destroy(gameObject); // Remove the health pack after pickup
        }
    }
}
