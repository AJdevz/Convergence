using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;          // Player reference
    public Vector3 offset;            // Distance from player
    public float smoothSpeed = 5f;    // Camera smoothing speed

    void LateUpdate()
    {
        if (target == null)
            return;

        // Desired position
        Vector3 desiredPosition = target.position + offset;

        // Smooth movement
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}
