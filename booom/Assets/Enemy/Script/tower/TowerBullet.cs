using UnityEngine;

public class TowerBullet : MonoBehaviour
{
    public float speed = 8f;
    public float damage = 10f;
    public LayerMask whatIsEnemy;

    private Vector3 moveDir;
    private float lockY;

    public void SetTarget(Transform target)
    {
        lockY = transform.position.y;

        if (target != null)
        {
            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;
            moveDir = toTarget.normalized;
        }
        else
        {
            moveDir = transform.forward;
            moveDir.y = 0f;
            moveDir.Normalize();
        }
    }

    void Update()
    {
        // Y 轴锁死
        Vector3 pos = transform.position;
        pos.y = lockY;
        transform.position = pos;

        // 检查方向是否反转
        Vector3 newDir = transform.forward;
        newDir.y = 0f;
        newDir.Normalize();

        float dotX = newDir.x * moveDir.x;
        float dotZ = newDir.z * moveDir.z;

        // X 或 Z 方向符号反转 → 飞过头了 → 销毁
        if (dotX < 0 || dotZ < 0)
        {
            Destroy(gameObject);
            return;
        }

        moveDir = newDir;
        transform.position += moveDir * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
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