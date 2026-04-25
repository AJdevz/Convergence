using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private float shakeTime;
    private float shakeDuration;
    private float shakeMagnitude;

    private Vector3 offset;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;

            float strength = shakeMagnitude * (shakeTime / shakeDuration);

            offset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            ) * strength;
        }
        else
        {
            offset = Vector3.zero;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        shakeDuration = duration;
        shakeTime = duration;
        shakeMagnitude = magnitude;
    }

    public Vector3 GetOffset()
    {
        return offset;
    }
}