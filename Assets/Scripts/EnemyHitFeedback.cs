using UnityEngine;
using System.Collections;

public class EnemyHitFeedback : MonoBehaviour
{
    [Header("Flash Settings")]
    public Color hitEmissionColor = Color.white * 10f;
    public float flashDuration = 0.08f;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;

    [Header("Blood Effect")]
    public bool useBlood = true;
    public float bloodIncreasePerHit = 0.15f;

    private Renderer rend;
    private Material mat;

    private Color originalEmission;
    private Rigidbody rb;

    void Start()
    {
        Debug.Log("FEEDBACK CALLED");

        rend = GetComponentInChildren<Renderer>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
            rb = GetComponentInParent<Rigidbody>();

        if (rb == null)
            Debug.LogError("NO RIGIDBODY FOUND ON ENEMY " + gameObject.name);

        if (rend != null)
        {
            mat = rend.material;

            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                originalEmission = mat.GetColor("_EmissionColor");
            }
        }
    }

    // 🔥 CALL THIS WHEN ENEMY GETS HIT
    public void PlayHitFeedback(Vector3 hitDirection)
    {
        StopAllCoroutines();
        StartCoroutine(Flash());

        CameraShake.Instance.Shake(0.1f, 0.2f);

        SoundManager.Instance?.PlayZombieHurt();
    }

    IEnumerator Flash()
    {
        if (mat != null && mat.HasProperty("_EmissionColor"))
        {
            mat.SetColor("_EmissionColor", hitEmissionColor);
        }

        yield return new WaitForSeconds(flashDuration);

        if (mat != null && mat.HasProperty("_EmissionColor"))
        {
            mat.SetColor("_EmissionColor", originalEmission);
        }
    }
}