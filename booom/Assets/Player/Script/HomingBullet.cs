using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public float speed = 15f;           // 更快
    public float turnRate = 15f;        // 更灵敏
    public float maxLifetime = 3f;
    public float damage = 10f;
    public LayerMask whatIsEnemy;
    public float hitRadius = 0.8f;      // 命中判定距离

    private Transform target;
    private bool hasTarget;

    public void SetTarget(Transform enemy)
    {
        target = enemy;
        hasTarget = enemy != null;
    }

    void Update()
    {
        // 1. 距离检测（100% 命中）
        if (hasTarget && target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= hitRadius)
            {
                HitTarget(target);
                return;
            }

            // 预测目标位置
            Rigidbody targetRb = target.GetComponent<Rigidbody>();
            Vector3 targetVelocity = targetRb != null ? targetRb.velocity : Vector3.zero;
            Vector3 predictedPos = target.position + targetVelocity * 0.3f;

            Vector3 toTarget = predictedPos - transform.position;
            if (dist < 1.5f)
                transform.forward = toTarget.normalized;
            else
                transform.forward = Vector3.Slerp(transform.forward, toTarget.normalized, turnRate * Time.deltaTime);
        }

        transform.position += transform.forward * speed * Time.deltaTime;

        if (transform.childCount > 0 && Camera.main != null)
            transform.GetChild(0).forward = Camera.main.transform.forward;

        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Destroy(gameObject);
    }

    void HitTarget(Transform enemy)
    {
        IDamageable d = enemy.GetComponent<IDamageable>();
        if (d != null)
            d.TakeDamage(damage, transform);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        // 物理碰撞作为备用
        if ((whatIsEnemy.value & (1 << other.gameObject.layer)) != 0)
        {
            IDamageable d = other.GetComponent<IDamageable>();
            if (d != null)
                d.TakeDamage(damage, transform);
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}