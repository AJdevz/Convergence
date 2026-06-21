using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class XPBar : MonoBehaviour
{
    public Slider xpSlider;
    public TextMeshProUGUI xpText;

    void Update()
    {
        if (XPManager.Instance == null) return;

        int current = XPManager.Instance.playerXP;
        int max = XPManager.Instance.xpToNextLevel;

        xpSlider.maxValue = max;
        xpSlider.value = current;

        if (xpText != null)
            xpText.text = $"XP: {current} / {max}";
    }
}