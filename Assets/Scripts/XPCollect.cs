using UnityEngine;

public class XPCollect : MonoBehaviour
{
    private int xpAmount;

    private Transform player;
    private XPMagnet magnet;

    void Start()
    {
        xpAmount = PlayerPrefs.GetInt(gameObject.name + "_XPAmount", 10);

        InvokeRepeating(nameof(FindPlayer), 0f, 0.5f);
    }

    void FindPlayer()
    {
        if (player != null) return;

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
        {
            player = p.transform;
            magnet = p.GetComponent<XPMagnet>();

            if (magnet != null)
            {
                CancelInvoke(nameof(FindPlayer));
            }
        }
    }

    void Update()
    {
        if (player == null || magnet == null) return;

        float range = magnet.GetRange();
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= range)
        {
            float speed = magnet.pullSpeed;

            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SoundManager.Instance?.PlayXP();
            XPManager.Instance.AddXP(xpAmount);
            Destroy(gameObject);
        }
    }

    public void SetXP(int amount)
    {
        xpAmount = amount;
        PlayerPrefs.SetInt(gameObject.name + "_XPAmount", amount);
    }
}