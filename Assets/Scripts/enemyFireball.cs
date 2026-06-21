using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyFireball : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 10;

    [Header("Lifetime")]
    public float lifeTime = 5f;

    private Vector3 direction;
    Vector3 moveDirection;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Initialize(Vector3 targetPosition)
    {
        targetPosition.y = transform.position.y;
        direction = (targetPosition - transform.position).normalized;
        transform.forward = direction;
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        transform.forward = direction;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();

            if (player != null)
                player.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}