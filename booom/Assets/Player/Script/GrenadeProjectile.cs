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
    }

    void Update()
    {
        // 和子弹一样，纯代码移动，不靠 Rigidbody
        transform.position += transform.forward * speed * Time.deltaTime;

        if (Time.time - spawnTime >= lifetime)
            Explode();
    }

    void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            Explode();
        }
        else if (!other.isTrigger)
        {
            Explode();
        }
    }

    void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyLayer);
        foreach (Collider hit in hits)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(damage, transform);
        }

        if (explosionVFX != null)
        {
            GameObject vfx = Instantiate(explosionVFX, transform.position, Quaternion.identity);
            Destroy(vfx, vfxDuration);
        }

        Destroy(gameObject);
    }
}