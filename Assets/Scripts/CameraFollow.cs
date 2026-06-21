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

        transform.position = Vector3.Lerp(
            transform.position,
            basePos,
            smoothSpeed * Time.deltaTime
        );
    }
}