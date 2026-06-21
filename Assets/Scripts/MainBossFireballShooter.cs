using UnityEngine;

public class MainBossFireballShooter : MonoBehaviour
{
    public EnemyFireball fireballPrefab;

    public float fireRate = 5f;
    public float fireballSpeed = 8f;
    public float fireballLifeTime = 5f;

    public Transform firePoint;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= fireRate)
        {
            timer = 0f;
            ShootCircle();
        }
    }

    void ShootCircle()
    {
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f;

            Vector3 dir = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            EnemyFireball fb = Instantiate(
                fireballPrefab,
                firePoint.position,
                Quaternion.identity
            );

            fb.speed = fireballSpeed;
            fb.lifeTime = fireballLifeTime;

            fb.SetDirection(dir);
        }
    }
}