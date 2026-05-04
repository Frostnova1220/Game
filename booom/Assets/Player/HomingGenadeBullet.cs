using UnityEngine;

public class HomingGrenadeBullet : MonoBehaviour
{
    [Header("追踪")]
    public float speed = 10f;
    public float turnRate = 8f;
    public LayerMask whatIsEnemy;

    [Header("碰撞检测")]
    public float hitRadius = 0.8f;       // 命中判定半径

    [Header("单体伤害")]
    public float directDamage = 10f;     // 直击伤害

    [Header("爆炸")]
    public float explosionRadius = 3f;   // 爆炸范围
    public float explosionDamage = 20f;  // 爆炸伤害
    public float maxLifetime = 3f;

    private Transform target;
    private bool hasTarget;

    [Header("爆炸特效")]
    public GameObject explosionVFXPrefab;  // 爆炸精灵预制体
    public float vfxDuration = 2f;         // 特效显示时间

    public void SetTarget(Transform enemy)
    {
        target = enemy;
        hasTarget = enemy != null;
    }

    void Update()
    {
        // 1. 追踪目标
        if (hasTarget && target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= hitRadius)
            {
                HitAndExplode(target);
                return;
            }

            Vector3 toTarget = target.position - transform.position;
            transform.forward = Vector3.Slerp(transform.forward, toTarget.normalized, turnRate * Time.deltaTime);
        }

        transform.position += transform.forward * speed * Time.deltaTime;

        if (transform.childCount > 0 && Camera.main != null)
            transform.GetChild(0).forward = Camera.main.transform.forward;

        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0)
            Explode(); // 超时也爆炸
    }

    void HitAndExplode(Transform firstTarget)
    {
        // 1. 先对直击目标造成单体伤害
        IDamageable d = firstTarget.GetComponent<IDamageable>();
        if (d != null)
            d.TakeDamage(directDamage, transform);

        // 2. 然后爆炸
        Explode();
    }

    void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, whatIsEnemy);
        foreach (Collider hit in hits)
        {
            IDamageable d = hit.GetComponent<IDamageable>();
            if (d != null)
                d.TakeDamage(explosionDamage, transform);
        }
        if (explosionVFXPrefab != null)
        {
            GameObject vfx = Instantiate(explosionVFXPrefab, transform.position, Quaternion.identity);
            Destroy(vfx, vfxDuration);  // 几秒后消失
        }
        Destroy(gameObject);
    }
}