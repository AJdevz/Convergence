using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("Sounds")]
    public AudioClip hitEnemy;
    public AudioClip zombie;
    public AudioClip explosion;
    public AudioClip lightning;
    public AudioClip heal;
    public AudioClip xpCollect;
    public AudioClip nextWave;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void PlaySound(AudioClip clip, float volume = 1f, float pitchMin = 0.95f, float pitchMax = 1.05f)
    {
        if (clip == null) return;

        sfxSource.pitch = Random.Range(pitchMin, pitchMax);
        sfxSource.PlayOneShot(clip, volume);
    }
}