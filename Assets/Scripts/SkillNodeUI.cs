using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SkillNodeUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SkillData skill;

    [Header("UI")]
    public Button button;
    public Image background;
    public TMP_Text costText;
    public TMP_Text nameText;

    public SkillNodeUI previousNode;

    private Vector3 originalScale;
    public float hoverScale = 1.1f;
    public float tweenSpeed = 10f;

    private bool isHovering = false;

    void Start()
    {
        if (skill == null)
        {
            Debug.LogError("Skill missing on node: " + gameObject.name);
            return;
        }

        if (SkillTreeManager.Instance == null)
        {
            Debug.LogError("SkillTreeManager is NULL in scene!");
            return;
        }

        button.onClick.AddListener(OnClick);

        costText.text = skill.cost.ToString();
        nameText.text = skill.displayName;

        originalScale = transform.localScale;

        UpdateVisual();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
            isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }

    void Update()
    {
        Vector3 targetScale = isHovering ? originalScale * hoverScale : originalScale;

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            Time.deltaTime * tweenSpeed
        );
    }

    void OnClick()
    {
        Debug.Log("Clicked: " + skill.skillID);

        if (SkillTreeManager.Instance == null)
        {
            Debug.LogError("No SkillTreeManager in scene!");
            return;
        }

        if (SkillTreeManager.Instance.CanUnlock(skill, this))
        {
            SkillTreeManager.Instance.UnlockSkill(skill);
            SkillTreeManager.Instance.RefreshAllNodes();
        }
        else
        {
            Debug.Log("Cannot unlock: " + skill.skillID);
        }
    }

    public bool IsUnlocked()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager missing");
            return false;
        }

        if (GameManager.Instance.playerData == null)
        {
            Debug.LogWarning("PlayerData missing");
            return false;
        }

        if (GameManager.Instance.playerData.unlockedSkills == null)
        {
            Debug.LogWarning("UnlockedSkills list missing");
            return false;
        }

        if (skill == null)
        {
            Debug.LogWarning("SkillData missing on node: " + gameObject.name);
            return false;
        }

        return GameManager.Instance.playerData.unlockedSkills.Contains(skill.skillID);
    }

    public void UpdateVisual()
    {
        bool unlocked = IsUnlocked();
        bool available = previousNode == null || previousNode.IsUnlocked();

        if (unlocked || available)
        {
            background.enabled = true;   // show your original pastel color
            button.interactable = true;
        }
        else
        {
            background.color = Color.gray;
            button.interactable = false;
        }
    }

    void OnEnable()
    {
        UpdateVisual();
    }
}