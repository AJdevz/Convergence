using UnityEngine;

public class EnemyProjectileShooter : MonoBehaviour
{
    public GameObject fireballPrefab;

    public float shootInterval = 5f;

    public Transform firePoint;

    private Transform player;

    private float timer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null)
            return;

        timer += Time.deltaTime;

        if (timer >= shootInterval)
        {
            timer = 0f;
            Shoot();
        }
    }

    void Shoot()
    {
        GameObject obj =
            Instantiate(fireballPrefab,
                        firePoint.position,
                        Quaternion.identity);

        EnemyFireball fireball =
            obj.GetComponent<EnemyFireball>();

        fireball.Initialize(player.position);
    }
}