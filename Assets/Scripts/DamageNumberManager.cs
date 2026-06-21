using UnityEngine;

public class DamageNumberManager : MonoBehaviour
{
    public static DamageNumberManager Instance;

    public GameObject damageTextPrefab;
    public Transform worldCanvas;

    void Awake()
    {
        Instance = this;
    }

    public void ShowDamage(int damage, Vector3 worldPosition)
    {
        if (damage <= 0)
        {
            Debug.Log("Blocked 0 damage");
            return;
        }

        GameObject obj = Instantiate(damageTextPrefab, worldCanvas);
        obj.transform.position = worldPosition;

        DamageNumber dmg = obj.GetComponent<DamageNumber>();
        if (dmg != null)
            dmg.Setup(damage);
    }
}