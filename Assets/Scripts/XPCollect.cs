using UnityEngine;

public class XPCollect : MonoBehaviour
{
    private int xpAmount;

    private void Start()
    {
        // Get XP value from the prefab itself
        xpAmount = PlayerPrefs.GetInt(gameObject.name + "_XPAmount", 10); // Default to 10 if no value is set
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            XPManager.Instance.AddXP(xpAmount);
            Destroy(gameObject);
        }
    }

    public void SetXP(int amount)
    {
        xpAmount = amount;
        PlayerPrefs.SetInt(gameObject.name + "_XPAmount", amount); // Save XP value dynamically
    }
}
