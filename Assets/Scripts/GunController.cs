using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class GunController : MonoBehaviour
{
    public enum GunType { AssaultRifle, Shotgun, Sniper }
    public GunType currentGun = GunType.AssaultRifle;

    [Header("Base Stats (per gun source values)")]
    public int baseDamage = 20;
    public float baseTimeBetweenShots = 0.2f;

    [Header("Runtime Stats (modified by upgrades)")]
    public int damage;
    public float timeBetweenShots;

    [Header("Core")]
    public BulletController bullet;
    public float shootSpeed;
    private float shotCounter;
    public Transform firePoint;
    public ParticleSystem muzzleFlash;

    [Header("Gun Types")]
    public int shotgunPellets = 6;
    public float shotgunSpread = 8f;
    public int sniperDamageMultiplier = 12;

    [Header("Models")]
    public GameObject assaultRifleModel;
    public GameObject shotgunModel;
    public GameObject sniperModel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioMixerGroup gunMixerGroup;
    public AudioClip assaultRifleSound;
    public AudioClip shotgunSound;
    public AudioClip sniperSound;

    [Header("Upgrades")]
    public bool explosiveShots;
    public bool chainLightning;
    public bool piercing;
    public bool freezeEffect;

    [Header("Upgrade Values")]
    public float explosionRadius = 3f;
    public int chainCount = 1;
    public int pierceCount = 2;
    public float freezeStrength = 0.5f;
    public float freezeDuration = 2f;
    public float lifestealPercent;

    [Header("Scaling")]
    public float explosionMultiplier = 0.5f;
    public float chainMultiplier = 0.5f;

    void Awake()
    {
        // Load selected gun FIRST (this is important)
        int savedGunIndex = PlayerPrefs.GetInt("SelectedGun", -1);

        if (savedGunIndex != -1)
        {
            currentGun = (GunType)savedGunIndex;
        }
        else if (GameManager.Instance != null)
        {
            currentGun = GameManager.Instance.SelectedGun;
        }

        // ONLY set base stats once (no reset spam)
        damage = baseDamage;
        timeBetweenShots = baseTimeBetweenShots;

        if (audioSource != null)
            audioSource.outputAudioMixerGroup = gunMixerGroup;

        UpdateGunModel();
    }

    public void ResetStats()
    {
        damage = baseDamage;
        timeBetweenShots = baseTimeBetweenShots;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (shotCounter <= 0)
            {
                FireWeapon();
                shotCounter = timeBetweenShots;
            }
        }

        shotCounter -= Time.deltaTime;
    }

    void FireWeapon()
    {
        switch (currentGun)
        {
            case GunType.AssaultRifle:
                FireSingleBullet();
                PlaySound(assaultRifleSound);
                break;

            case GunType.Shotgun:
                FireShotgun();
                PlaySound(shotgunSound);
                break;

            case GunType.Sniper:
                FireSniper();
                PlaySound(sniperSound);
                break;
        }
    }

    void FireSingleBullet()
    {
        BulletController b = Instantiate(bullet, firePoint.position, firePoint.rotation);
        b.speed = shootSpeed;
        b.GiveDamage = damage;
        ApplyUpgrades(b);
        muzzleFlash.Play();
    }

    void FireShotgun()
    {
        for (int i = 0; i < shotgunPellets; i++)
        {
            Quaternion spread = Quaternion.Euler(0, Random.Range(-shotgunSpread, shotgunSpread), 0);

            BulletController b = Instantiate(bullet, firePoint.position, firePoint.rotation * spread);
            b.speed = shootSpeed;
            b.GiveDamage = damage;

            ApplyUpgrades(b);
        }

        muzzleFlash.Play();
    }

    void FireSniper()
    {
        BulletController b = Instantiate(bullet, firePoint.position, firePoint.rotation);
        b.speed = shootSpeed;
        b.GiveDamage = damage * sniperDamageMultiplier;

        ApplyUpgrades(b);
        muzzleFlash.Play();
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource && clip)
            audioSource.PlayOneShot(clip, 0.05f);
    }

    void UpdateGunModel()
    {
        assaultRifleModel.SetActive(currentGun == GunType.AssaultRifle);
        shotgunModel.SetActive(currentGun == GunType.Shotgun);
        sniperModel.SetActive(currentGun == GunType.Sniper);
    }

    public void ApplyUpgrades(BulletController b)
    {
        b.explosiveShots = explosiveShots;
        b.chainLightning = chainLightning;
        b.piercing = piercing;
        b.freezeEffect = freezeEffect;

        b.lifestealPercent = lifestealPercent;

        b.explosionRadius = explosionRadius;
        b.chainCount = chainCount;
        b.pierceCount = pierceCount;

        b.freezeStrength = freezeStrength;
        b.freezeDuration = freezeDuration;

        b.explosionMultiplier = explosionMultiplier;
        b.chainMultiplier = chainMultiplier;
    }
}