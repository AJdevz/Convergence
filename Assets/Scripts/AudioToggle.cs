using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    private bool isMuted = false; // Track mute state
    public Image tickImage; 

    void Start()
    {
        // Load previous mute state
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        ApplyAudioSettings();
    }

    public void ToggleAudio()
    {
        isMuted = !isMuted; // Toggle mute state
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0); // Save mute state
        ApplyAudioSettings();
    }

    void ApplyAudioSettings()
    {
        AudioListener.volume = isMuted ? 0 : 1; // Apply volume setting
        tickImage.gameObject.SetActive(isMuted); // Show/hide tick
    }
}
