using UnityEngine;
using System.Collections;

public class UIScaling : MonoBehaviour
{
    [Header("Scaling Settings")]
    public float hoverScale = 1.1f;   // scale on hover
    public float normalScale = 1f;    // default scale

    [Header("Decoration Effects")]
    public GameObject leftEffect;     // assign your left decoration
    public GameObject rightEffect;    // assign your right decoration
    public float fadeSpeed = 5f;      // how fast the effects fade in/out

    private CanvasGroup leftGroup;
    private CanvasGroup rightGroup;

    private void Awake()
    {
        // Add CanvasGroups to decorations for fading
        if (leftEffect != null)
        {
            leftGroup = leftEffect.GetComponent<CanvasGroup>();
            if (leftGroup == null) leftGroup = leftEffect.AddComponent<CanvasGroup>();
            leftGroup.alpha = 0f;
        }

        if (rightEffect != null)
        {
            rightGroup = rightEffect.GetComponent<CanvasGroup>();
            if (rightGroup == null) rightGroup = rightEffect.AddComponent<CanvasGroup>();
            rightGroup.alpha = 0f;
        }
    }

    public void PointerEnter()
    {
        // Scale button
        transform.localScale = new Vector3(hoverScale, hoverScale, hoverScale);

        // Fade in effects
        StopAllCoroutines();
        StartCoroutine(FadeInEffects());
    }

    public void PointerExit()
    {
        // Reset scale
        transform.localScale = new Vector3(normalScale, normalScale, normalScale);

        // Fade out effects
        StopAllCoroutines();
        StartCoroutine(FadeOutEffects());
    }

    IEnumerator FadeInEffects()
    {
        while ((leftGroup != null && leftGroup.alpha < 1f) || (rightGroup != null && rightGroup.alpha < 1f))
        {
            if (leftGroup != null) leftGroup.alpha += Time.deltaTime * fadeSpeed;
            if (rightGroup != null) rightGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    IEnumerator FadeOutEffects()
    {
        while ((leftGroup != null && leftGroup.alpha > 0f) || (rightGroup != null && rightGroup.alpha > 0f))
        {
            if (leftGroup != null) leftGroup.alpha -= Time.deltaTime * fadeSpeed;
            if (rightGroup != null) rightGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }
}
