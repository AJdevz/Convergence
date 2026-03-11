using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    public GameObject upgradePanel; // Assign in Inspector

    void Start()
    {
        // Make sure the panel is initially hidden
        upgradePanel.SetActive(false);
    }

    public void OpenUpgradeMenu()
    {
        upgradePanel.SetActive(true); // Show the upgrade menu
        Time.timeScale = 0; // Pause the game when the upgrade menu opens
    }

    public void CloseUpgradeMenu()
    {
        upgradePanel.SetActive(false); // Hide the upgrade menu
        Time.timeScale = 1; // Unpause the game when the upgrade menu closes
    }
}
