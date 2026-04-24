using UnityEngine;

public class XPMagnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    public float baseRange = 5f;
    public float rangePerLevel = 10f; // BIG increase

    public float pullSpeed = 12f;

    public int magnetLevel = 0;

    public float GetRange()
    {
        return baseRange + (magnetLevel * rangePerLevel);
    }
}