using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    private float speed;
    private float damage;
    private float radius;
    private float lifetime;
    private GameObject explosionVFX;
    private float vfxDuration;
    private LayerMask enemyLayer;

    private float spawnTime;
    private Vector3 lastPosition;

    public void Initialize(float speed, float damage, float radius,
                           float lifetime, GameObject explosionVFX, float vfxDuration, LayerMask enemyLayer)
    {
        this.speed = speed;
        this.damage = damage;
        this.radius = radius;
        this.lifetime = lifetime;
        this.explosionVFX = explosionVFX;
        this.vfxDuration = vfxDuration;
        this.enemyLayer = enemyLayer;

        spawnTime = Time.time;
        lastPosition = transform.position;
    }

    void Update()
    {
        // 直线飞行
        transform.position += transform.forward * speed * Time.deltaTime;

        // 射线自检：是否击中敌人或墙壁
        Vector3 rayDirection = transform.position - lastPosition;
        float rayDistance = rayDirection.magnitude;

        if (rayDistance > 0)
        {
            RaycastHit hit;
            // 对敌人层和默认环境层（假设是0）做检测，这样碰到墙壁也会爆炸
            if (Physics.Raycast(lastPosition, rayDirection.normalized, out hit, rayDistance, enemyLayer | (1 << 0)))
            {
                Explode();
                return;
            }
        }
        lastPosition = transform.position;

        // 超时自爆
        if (Time.time - spawnTime >= lifetime)
        {
            Explode();
        }
    }

    void Explode()
    {
        // 范围伤害（这部分用 OverlapSphere 不受 IsTrigger 限制，完全没问题）
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage, transform);
            }
        }

        // 爆炸特效
        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(vfx, vfxDuration);
        }

        Destroy(gameObject);
    }
}