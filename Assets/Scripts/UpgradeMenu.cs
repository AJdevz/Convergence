using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    public GameObject upgradePanel;
    public UpgradeManager upgradeManager;

    void Start()
    {
        upgradePanel.SetActive(false);
    }

    public void OpenUpgradeMenu()
    {
        upgradePanel.SetActive(true);
        Time.timeScale = 0;

        // 🔥 Generate new upgrades when opened
        if (upgradeManager != null)
        {
            upgradeManager.GenerateUpgradeChoices();
        }
    }

    public void CloseUpgradeMenu()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1;
    }
}