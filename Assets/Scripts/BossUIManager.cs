using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossUIManager : MonoBehaviour
{
    public static BossUIManager Instance;

    public GameObject bossUI;
    public Slider bossSlider;
    public TextMeshProUGUI bossHPText;

    private EnemyHealth currentBoss;

    void Awake()
    {
        Instance = this;
        bossUI.SetActive(false);
    }

    public void SetBoss(EnemyHealth boss)
    {
        Debug.Log("SetBoss called for: " + boss.name);

        currentBoss = boss;
        bossUI.SetActive(true);

        UpdateUI(boss);
    }

    void Update()
    {
        if (currentBoss == null)
        {
            bossUI.SetActive(false);
            return;
        }

        UpdateUI(currentBoss);

        if (currentBoss == null || currentBoss.gameObject == null)
        {
            bossUI.SetActive(false);
        }
    }

    void UpdateUI(EnemyHealth boss)
    {
        if (boss == null) return;

        bossSlider.maxValue = bossMaxHP(boss);
        bossSlider.value = bossCurrentHP(boss);

        bossHPText.text =
            $"{bossCurrentHP(boss)} / {bossMaxHP(boss)}";
    }

    int bossMaxHP(EnemyHealth boss)
    {
        return boss.GetMaxHealth();
    }

    int bossCurrentHP(EnemyHealth boss)
    {
        return boss.GetCurrentHealth();
    }
}