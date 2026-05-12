using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{
    public static GrenadeLauncher instance;

    [Header("榴弹发射设置")]
    public GameObject grenadePrefab;
    public float launchSpeed = 15f;
    public float cooldown = 0f;
    public LayerMask enemyLayer;

    [Header("榴弹参数")]
    public float grenadeDamage = 2f;
    public float explosionRadius = 3f;
    public float grenadeLifetime = 3f;

    [Header("爆炸特效")]
    public GameObject explosionVFX;
    public float vfxDuration = 1.5f;

    [Header("弹药设置")]
    public int maxAmmo = 3;

    private int currentAmmo = 0;
    private float lastFireTime;

    public System.Action<int> onAmmoChanged;

    void Awake()
    {
        instance = this;
        currentAmmo = 0;
    }

    public bool TryFire(Vector3 origin, Vector3 direction)
    {
        if (currentAmmo <= 0) return false;
        if (Time.time < lastFireTime + cooldown) return false;

        currentAmmo--;
        lastFireTime = Time.time;

        FireGrenade(origin, direction);
        onAmmoChanged?.Invoke(currentAmmo);
        return true;
    }

    private void FireGrenade(Vector3 origin, Vector3 direction)
    {
        if (grenadePrefab == null) return;

        GameObject grenade = Instantiate(grenadePrefab, origin, Quaternion.LookRotation(direction));

        GrenadeProjectile projectile = grenade.GetComponent<GrenadeProjectile>();
        if (projectile == null)
            projectile = grenade.AddComponent<GrenadeProjectile>();

        projectile.Initialize(launchSpeed, grenadeDamage, explosionRadius,
                              grenadeLifetime, explosionVFX, vfxDuration, enemyLayer);

        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb == null)
            rb = grenade.AddComponent<Rigidbody>();

        rb.velocity = direction * launchSpeed;
    }

    public void PickupGrenade()
    {
        currentAmmo = Mathf.Min(currentAmmo + 3, maxAmmo);
        onAmmoChanged?.Invoke(currentAmmo);
    }

    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
}