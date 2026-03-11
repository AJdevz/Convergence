using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class WaveTextDisplay : MonoBehaviour
{
    public TextMeshProUGUI waveText; // Reference to the UI TextMeshProUGUI element
    private bool isDisplayingText = false;

    void Start()
    {
        if (waveText == null)
        {
            Debug.LogError("WaveTextDisplay: waveText is not assigned! Please assign it in the Inspector.");
        }
    }

    // Now takes a **string message** instead of just a number
    public void DisplayWaveText(string message)
    {
        if (!isDisplayingText)
        {
            StartCoroutine(DisplayTextCoroutine(message));
        }
    }

    private IEnumerator DisplayTextCoroutine(string message)
    {
        isDisplayingText = true;

        waveText.text = message;
        waveText.gameObject.SetActive(true); // Makes sure the text is visible

        yield return new WaitForSeconds(2f); // Wait for 2 seconds to display the text

        waveText.gameObject.SetActive(false); // Hides the text after 3 seconds
        isDisplayingText = false;
    }
}
