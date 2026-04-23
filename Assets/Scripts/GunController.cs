using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class GunController : MonoBehaviour
{
    public enum GunType { AssaultRifle, Shotgun, Sniper }
    public GunType currentGun = GunType.AssaultRifle;
    public int damage = 20; // Default damage

    public bool isFiring;

    public BulletController bullet;
    public float shootSpeed;
    public float timeBetweenShots;
    private float shotCounter;
    public ParticleSystem muzzleFlash;

    public Transform firePoint;

    public int shotgunPellets = 6;
    public float shotgunSpread = 8f;
    public int sniperDamageMultiplier = 12;

    public GameObject assaultRifleModel;
    public GameObject shotgunModel;
    public GameObject sniperModel;

    // 🎵 Audio setup
    public AudioSource audioSource;
    public AudioMixerGroup gunMixerGroup;
    public AudioClip assaultRifleSound;
    public AudioClip shotgunSound;
    public AudioClip sniperSound;

    public bool explosiveShots;
    public bool chainLightning;
    public bool piercing;
    public bool freezeEffect;

    public float lifestealPercent;

    [Header("Upgrade Scaling")]
    public float explosionRadius = 3f;
    public int explosionDamage = 20;

    public int chainCount = 3;

    public int pierceCount = 2;

    public float freezeStrength = 0.5f;
    public float freezeDuration = 2f;


    void Awake()
    {
        // Load the saved gun selection from PlayerPrefs first
        int savedGunIndex = PlayerPrefs.GetInt("SelectedGun", -1);

        if (savedGunIndex != -1)
        {
            currentGun = (GunType)savedGunIndex;
        }
        else if (GameManager.Instance != null)
        {
            // fallback to GameManager if PlayerPrefs not set
            currentGun = GameManager.Instance.SelectedGun;
        }

        // Make sure audioSource is assigned
        if (audioSource != null)
        {
            audioSource.outputAudioMixerGroup = gunMixerGroup;
        }

        UpdateGunModel();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // Left click or hold
        {
            if (shotCounter <= 0) // Only fire if enough time has passed
            {
                FireWeapon();
                shotCounter = timeBetweenShots; // Reset the cooldown
            }
        }

        if (shotCounter > 0)
            shotCounter -= Time.deltaTime;
    }

    void FireWeapon()
    {
        switch (currentGun)
        {
            case GunType.AssaultRifle:
                FireSingleBullet();
                PlayGunSound(assaultRifleSound);
                break;

            case GunType.Shotgun:
                FireShotgun();
                PlayGunSound(shotgunSound);
                break;

            case GunType.Sniper:
                FireSniper();
                PlayGunSound(sniperSound);
                break;
        }
    }

    void FireSingleBullet()
    {
        BulletController newBullet = Instantiate(bullet, firePoint.position, firePoint.rotation);

        newBullet.speed = shootSpeed;
        newBullet.GiveDamage = damage;

        ApplyUpgradesToBullet(newBullet); // 🔥 THIS IS THE KEY

        muzzleFlash.Play();
    }

    void FireShotgun()
    {
        for (int i = 0; i < shotgunPellets; i++)
        {
            Quaternion spreadAngle = Quaternion.Euler(0, Random.Range(-shotgunSpread, shotgunSpread), 0);

            BulletController newBullet = Instantiate(bullet, firePoint.position, firePoint.rotation * spreadAngle);

            newBullet.speed = shootSpeed;
            newBullet.GiveDamage = damage;

            ApplyUpgradesToBullet(newBullet); // 🔥 IMPORTANT

            muzzleFlash.Play();
        }
    }

    void FireSniper()
    {
        BulletController newBullet = Instantiate(bullet, firePoint.position, firePoint.rotation);

        newBullet.speed = shootSpeed;
        newBullet.GiveDamage = damage * sniperDamageMultiplier;

        ApplyUpgradesToBullet(newBullet); // 🔥 IMPORTANT

        muzzleFlash.Play();
    }

    void PlayGunSound(AudioClip gunSound)
    {
        if (audioSource != null && gunSound != null)
        {
            audioSource.PlayOneShot(gunSound, 0.05f); // Adjust volume if needed
        }
    }

    // Call this to change the gun from menus
    public void SetGunType(int gunTypeIndex)
    {
        currentGun = (GunType)gunTypeIndex;

        // Save selection to PlayerPrefs
        PlayerPrefs.SetInt("SelectedGun", gunTypeIndex);

        // Update GameManager persistent data
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SelectedGun = currentGun;
        }

        UpdateGunModel();
        Debug.Log("Gun changed to: " + currentGun);
    }

    void UpdateGunModel()
    {
        if (assaultRifleModel) assaultRifleModel.SetActive(currentGun == GunType.AssaultRifle);
        if (shotgunModel) shotgunModel.SetActive(currentGun == GunType.Shotgun);
        if (sniperModel) sniperModel.SetActive(currentGun == GunType.Sniper);
    }

    void ApplyUpgradesToBullet(BulletController newBullet)
    {
        // 🔥 Flags
        newBullet.explosiveShots = explosiveShots;
        newBullet.chainLightning = chainLightning;
        newBullet.piercing = piercing;
        newBullet.freezeEffect = freezeEffect;

        // 🩸 Stats
        newBullet.lifestealPercent = lifestealPercent;

        // 💥 Explosion scaling
        newBullet.explosionRadius = explosionRadius;
        newBullet.explosionDamage = explosionDamage;

        // ⚡ Chain scaling
        newBullet.chainCount = chainCount;

        // 🧩 Piercing scaling
        newBullet.pierceCount = pierceCount;

        // ❄️ Freeze scaling
        newBullet.freezeStrength = freezeStrength;
        newBullet.freezeDuration = freezeDuration;
    }
}
