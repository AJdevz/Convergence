using System.Collections;
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
    private Color originalColor;

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
        originalColor = background.color;

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
            return false;

        if (GameManager.Instance.playerData == null)
            return false;

        if (GameManager.Instance.playerData.unlockedSkills == null)
            return false;

        if (skill == null)
            return false;

        return GameManager.Instance.playerData.unlockedSkills.Contains(skill.skillID);
    }

    void OnEnable()
    {
        StartCoroutine(DelayedVisualUpdate());
    }

    IEnumerator DelayedVisualUpdate()
    {
        while (GameManager.Instance == null)
            yield return null;

        yield return new WaitForSeconds(0.05f);

        UpdateVisual();
    }

    public void UpdateVisual()
    {
        bool unlocked = IsUnlocked();
        bool available = previousNode == null || previousNode.IsUnlocked();

        if (unlocked)
        {
            background.color = Color.green;
            button.interactable = false;
        }
        else if (available)
        {
            background.color = originalColor;
            button.interactable = true;
        }
        else
        {
            background.color = Color.gray;
            button.interactable = false;
        }
    }
}