using UnityEngine;
using UnityEngine.UI;

public class AudioToggle : MonoBehaviour
{
    private bool isMuted;

    public AudioSource musicSource; // drag your music AudioSource here

    public GameObject mutedIcon;
    public GameObject unmutedIcon;

    void Start()
    {
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        ApplyState();
    }

    public void ToggleAudio()
    {
        isMuted = !isMuted;
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
        ApplyState();
    }

    void ApplyState()
    {
        if (musicSource != null)
        {
            musicSource.mute = isMuted;
        }

        mutedIcon.SetActive(isMuted);
        unmutedIcon.SetActive(!isMuted);
    }
}