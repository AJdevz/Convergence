using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("General Sounds")]
    public AudioClip hitEnemy;
    public AudioClip explosion;
    public AudioClip lightning;
    public AudioClip heal;
    public AudioClip xpCollect;
    public AudioClip nextWave;

    [Header("Zombie Sounds")]
    public AudioClip[] zombieGrowls;   // idle / ambient
    public AudioClip[] zombieHurt;     // when hit
    public AudioClip[] zombieDeath;    // optional (future)

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // 🔊 Generic play
    public void PlaySound(AudioClip clip, float volume = 1f, float pitchMin = 0.95f, float pitchMax = 1.05f)
    {
        if (clip == null) return;

        sfxSource.pitch = Random.Range(pitchMin, pitchMax);
        sfxSource.PlayOneShot(clip, volume);
    }

    // 🔥 RANDOM ARRAY PLAY (IMPORTANT)
    public void PlayRandom(AudioClip[] clips, float volume = 1f, float pitchMin = 0.95f, float pitchMax = 1.05f)
    {
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[Random.Range(0, clips.Length)];

        sfxSource.pitch = Random.Range(pitchMin, pitchMax);
        sfxSource.PlayOneShot(clip, volume);
    }

    // 🔥 HIT ENEMY
    // 🔥 HIT ENEMY
    public void PlayHitEnemy()
    {
        PlaySound(hitEnemy, 0.01f, 0.9f, 1.05f);
    }

    // 🧟 ZOMBIE GROWL (random)
    public void PlayZombieGrowl()
    {
        PlayRandom(zombieGrowls, 0.01f, 0.85f, 1f);
    }

    // 💥 ZOMBIE HURT (random)
    public void PlayZombieHurt()
    {
        PlayRandom(zombieHurt, 0.01f, 0.9f, 1.05f);
    }

    // ☠️ ZOMBIE DEATH
    public void PlayZombieDeath()
    {
        PlayRandom(zombieDeath, 0.3f, 0.9f, 1.05f);
    }

    // 💣 EXPLOSION
    public void PlayExplosion()
    {
        PlaySound(explosion, 0.02f, 0.9f, 1f);
    }

    // ⚡ LIGHTNING
    public void PlayLightning()
    {
        PlaySound(lightning, 0.02f, 0.9f, 1.05f);
    }

    // ❤️ HEAL
    public void PlayHeal()
    {
        PlaySound(heal, 0.02f, 0.95f, 1.05f);
    }

    // ✨ XP
    public void PlayXP()
    {
        PlaySound(xpCollect, 0.02f, 0.95f, 1.05f);
    }

    // 🌊 NEXT WAVE
    public void PlayNextWave()
    {
        PlaySound(nextWave, 0.5f, 0.95f, 1.05f);
    }
}