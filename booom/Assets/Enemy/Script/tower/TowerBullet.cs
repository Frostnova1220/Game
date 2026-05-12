using UnityEngine;

public class TowerBullet : MonoBehaviour
{
    public float speed = 8f;
    public float damage = 1f;
    public float turnRate = 5f;
    public float maxLifetime = 3f;

    private Transform target;
    private Vector3 moveDir;
    private Vector3 lastMoveDir;
    private float lockY;
    private bool hasHit;
    private bool hasLastDir;

    public void SetTarget(Transform target)
    {
        this.target = target;
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
        if (hasHit) return;

        // 追踪玩家，只在 XZ 平面转向
        if (target != null)
        {
            Vector3 toTarget = target.position - transform.position;
            toTarget.y = 0f;

            if (toTarget.magnitude > 0.01f)
            {
                Vector3 desiredDir = toTarget.normalized;
                moveDir = Vector3.Slerp(moveDir, desiredDir, turnRate * Time.deltaTime);
            }
        }

        // 检查上一帧和这一帧的方向 X、Z 是否有正负差
        if (hasLastDir)
        {
            bool xChanged = (lastMoveDir.x > 0 && moveDir.x < 0) || (lastMoveDir.x < 0 && moveDir.x > 0);
            bool zChanged = (lastMoveDir.z > 0 && moveDir.z < 0) || (lastMoveDir.z < 0 && moveDir.z > 0);

            if (xChanged || zChanged)
            {
                Destroy(gameObject);
                return;
            }
        }

        lastMoveDir = moveDir;
        hasLastDir = true;

        // Y 轴锁死
        Vector3 pos = transform.position;
        pos.y = lockY;
        transform.position = pos;

        // 移动
        transform.position += moveDir * speed * Time.deltaTime;

        // 3 秒后自动销毁
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            hasHit = true;

            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth == null)
                playerHealth = other.GetComponentInParent<Health>();

            if (playerHealth != null)
                playerHealth.TakeDamage(damage, transform);

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}