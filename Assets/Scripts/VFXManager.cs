using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    public GameObject bloodEffect;
    public GameObject levelUpEffect;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // ✅ WORLD POSITION (for bullets, explosions)
    public void PlayVFX(GameObject vfx, Vector3 position)
    {
        if (vfx == null) return;

        GameObject effect = Instantiate(vfx, position, Quaternion.identity);
        Destroy(effect, 2f);
    }

    // ✅ ATTACHED (for player effects like level up)
    public void PlayVFX(GameObject vfx, Transform parent, Vector3 offset)
    {
        if (vfx == null || parent == null) return;

        GameObject effect = Instantiate(vfx, parent);
        effect.transform.localPosition = offset;

        Destroy(effect, 2f);
    }
}