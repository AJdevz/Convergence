using UnityEngine;
using TMPro; 

public class UpgradeManager : MonoBehaviour
{
    public TMP_Text xpText; 

    void Update()
    {
        if (xpText != null)
        {
            // Update the XP text to match the current XP
            xpText.text = "XP: " + XPManager.Instance.playerXP + "/" + XPManager.Instance.xpToNextLevel;
        }
        else
        {
            Debug.LogError("XP Text is not assigned in the Inspector.");
        }
    }

    public void UpgradeDamage()
    {
        GunController gun = FindFirstObjectByType<GunController>();
        gun.damage = Mathf.RoundToInt(gun.damage * 1.15f); // 15% increase
        CloseUpgradeMenu();
    }


    public void UpgradeFireRate()
    {
        FindFirstObjectByType<GunController>().timeBetweenShots *= 0.95f; // each 0.99 is 1% faster fire rate
        CloseUpgradeMenu(); // Close the upgrade menu after the upgrade
    }

    // Close the upgrade menu
    void CloseUpgradeMenu()
    {
        if (XPManager.Instance.upgradeMenuScript != null)
        {
            XPManager.Instance.upgradeMenuScript.CloseUpgradeMenu(); // Close the upgrade menu
        }
        else
        {
            Debug.LogError("UpgradeMenuScript is not assigned in the Inspector of XPManager.");
        }
    }
}
