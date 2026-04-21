using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    public GameObject bloodEffect;
    public GameObject levelUpEffect;

    void Awake()
    {
        Instance = this;
    }

    public void PlayVFX(GameObject vfx, Vector3 position)
    {
        if (vfx == null) return;

        GameObject effect = Instantiate(vfx, position, Quaternion.identity);
        Destroy(effect, 2f);
    }
}