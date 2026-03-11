using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBar : MonoBehaviour
{
    public Slider xpSlider; 
    public TextMeshProUGUI xpText; 

    void Start()
    {
        if (xpSlider == null || xpText == null)
        {
            Debug.LogError("XPBar: XP Slider or XP Text is not assigned in the Inspector.");
            return;
        }

        // Initialize the XP bar with current XP data
        SetMaxXP(XPManager.Instance.xpToNextLevel);
        UpdateXP(XPManager.Instance.playerXP);
    }

    void Update()
    {
        if (XPManager.Instance != null)
        {
            UpdateXP(XPManager.Instance.playerXP);
            xpText.text = "XP: " + XPManager.Instance.playerXP + "/" + XPManager.Instance.xpToNextLevel;
        }
    }

    // Call this when the player levels up to set a new max XP requirement
    public void SetMaxXP(int maxXP)
    {
        xpSlider.maxValue = maxXP;
        xpSlider.value = 0; // Reset XP bar when leveling up
    }

    public void UpdateXP(int xp)
    {
        xpSlider.value = xp;
    }
}
