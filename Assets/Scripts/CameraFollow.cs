using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 basePos = target.position + offset;

        if (CameraShake.Instance != null)
        {
            basePos += CameraShake.Instance.GetOffset();
        }

        transform.position = Vector3.Lerp(
            transform.position,
            basePos,
            smoothSpeed * Time.deltaTime
        );
    }
}