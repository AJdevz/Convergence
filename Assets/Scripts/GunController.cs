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

    [Header("Runtime Stats")]
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

    [Header("Upgrades (toggles + values)")]
    public bool explosiveShots;
    public bool chainLightning;
    public bool piercing;
    public bool freezeEffect;

    public float explosionRadius = 3f;
    public int chainCount = 1;
    public int pierceCount = 2;
    public float freezeStrength = 0.5f;
    public float freezeDuration = 2f;
    public float lifestealPercent;
    public float freezeChance = 0f;

    [Header("Scaling (STACKING SYSTEM)")]
    public float damageMultiplier = 1f;
    public float fireRateMultiplier = 1f;
    public float explosionMultiplier = 0.5f;
    public float chainMultiplier = 0.5f;

    void Awake()
    {
        int savedGunIndex = PlayerPrefs.GetInt("SelectedGun", -1);

        if (savedGunIndex != -1)
            currentGun = (GunType)savedGunIndex;
        else if (GameManager.Instance != null)
            currentGun = GameManager.Instance.SelectedGun;

        damage = baseDamage;
        timeBetweenShots = baseTimeBetweenShots;

        if (audioSource != null)
            audioSource.outputAudioMixerGroup = gunMixerGroup;

        UpdateGunModel();
    }

    public void ResetStats()
    {
        damageMultiplier = 1f;
        fireRateMultiplier = 1f;

        damage = baseDamage;
        timeBetweenShots = baseTimeBetweenShots;
    }

    void Update()
    {
        float finalFireRate = timeBetweenShots;

        if (Input.GetMouseButton(0))
        {
            if (shotCounter <= 0f)
            {
                FireWeapon();
                shotCounter = finalFireRate;
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

    void Start()
    {
        ApplyPlayerStats();
    }

    void ApplyPlayerStats()
    {
        if (GameManager.Instance == null) return;

        var data = GameManager.Instance.playerData;

        damageMultiplier = data.damageMultiplier;
        fireRateMultiplier = data.fireRateMultiplier;
        lifestealPercent = data.lifesteal;

        RecalculateStats();

        Debug.Log("Gun stats applied from PlayerData");
    }

    void FireSingleBullet()
    {
        Debug.Log("FINAL DAMAGE: " + Mathf.RoundToInt(damage * damageMultiplier));
        BulletController b = Instantiate(bullet, firePoint.position, firePoint.rotation);
        b.speed = shootSpeed;
        b.GiveDamage = Mathf.RoundToInt(damage * damageMultiplier);

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
            b.GiveDamage = Mathf.RoundToInt(damage * damageMultiplier);

            ApplyUpgrades(b);
        }

        muzzleFlash.Play();
    }

    void FireSniper()
    {
        BulletController b = Instantiate(bullet, firePoint.position, firePoint.rotation);
        b.speed = shootSpeed;
        b.GiveDamage = Mathf.RoundToInt(damage * sniperDamageMultiplier * damageMultiplier);

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
        b.freezeChance = freezeChance;

        b.explosionMultiplier = explosionMultiplier;
        b.chainMultiplier = chainMultiplier;
    }

    public int GetCurrentDamage()
    {
        return Mathf.RoundToInt(baseDamage * damageMultiplier);
    }

    public void RecalculateStats()
    {
        damage = Mathf.RoundToInt(baseDamage * damageMultiplier);

        timeBetweenShots = baseTimeBetweenShots * fireRateMultiplier;

        // safety clamp (prevents insane spam bugs)
        float minFireRate = GetFireRateClamp();

        timeBetweenShots = Mathf.Max(timeBetweenShots, minFireRate);
    }

    public float GetFireRateClamp()
    {
        switch (currentGun)
        {
            case GunType.AssaultRifle: return 0.05f;
            case GunType.Shotgun: return 0.1f;
            case GunType.Sniper: return 0.2f;
            default: return 0.05f;
        }
    }
}