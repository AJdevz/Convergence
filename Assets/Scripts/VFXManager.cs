using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    [Header("VFX")]
    public GameObject bloodEffect;
    public GameObject levelUpEffect;
    public GameObject hitFlashEffect;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlayVFX(GameObject vfx, Vector3 position)
    {
        if (vfx == null) return;

        GameObject effect = Instantiate(vfx, position, Quaternion.identity);
        Destroy(effect, 2f);
    }

    public void PlayVFX(GameObject vfx, Transform parent, Vector3 offset)
    {
        if (vfx == null || parent == null) return;

        GameObject effect = Instantiate(vfx, parent);
        effect.transform.localPosition = offset;

        Destroy(effect, 2f);
    }
}