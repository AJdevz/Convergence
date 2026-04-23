using UnityEngine;
using TMPro;

public class DamageNumber : MonoBehaviour
{
    public float lifeTime = 1f;
    public float floatSpeed = 1f;

    private TextMeshProUGUI text;
    private Transform cam;
    private Vector3 worldPos;

    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();

        if (text == null)
        {
            Debug.LogError("No TextMeshProUGUI found on DamageNumber!");
            return;
        }

        cam = Camera.main.transform;
    }

    public void Setup(int damage)
    {
        if (text == null) return;

        text.text = damage.ToString();
        worldPos = transform.position;
    }

    void Update()
    {
        if (text == null) return;

        transform.position = worldPos + Vector3.up * (1f - lifeTime);
        transform.forward = cam.forward;

        lifeTime -= Time.deltaTime;

        float alpha = lifeTime;
        text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }
}