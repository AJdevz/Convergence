using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private float shakeTime;
    private float shakeMagnitude;

    private Vector3 shakeOffset;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (shakeTime > 0)
        {
            shakeTime -= Time.deltaTime;

            shakeOffset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            ) * shakeMagnitude;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }
    }

    public void Shake(float duration, float magnitude)
    {
        shakeTime = Mathf.Max(shakeTime, duration);
        shakeMagnitude = Mathf.Clamp(shakeMagnitude + magnitude, 0f, 1.2f);
    }

    public Vector3 GetOffset()
    {
        return shakeOffset;
    }
}