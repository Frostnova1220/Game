using UnityEngine;

public class Tower : MonoBehaviour, IDamageable
{
    public enum State { Idle, Attack, Dead }
    public State currentState;

    [Header("×Óµ¯")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;
    public float bulletLifetime = 1.5f;
    public float attackInterval = 1.5f;
    public float attackDamage = 1f;
    public LayerMask whatIsPlayer;
    public LayerMask whatIsEnemy;

    [Header("ÑªÁ¿")]
    public Health health;

    [Header("³¯Ïò")]
    public bool OnX = true;

    private float lastAttackTime;
    private bool isDead;
    private Transform playerTarget;

    void Start()
    {
        currentState = State.Idle;
        lastAttackTime = -attackInterval;
    }

    void Update()
    {
        if (isDead) return;

        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTarget = playerObj.transform;
        }

        switch (currentState)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.Attack:
                HandleAttackState();
                break;
        }
    }

    void HandleIdleState()
    {
        if (Time.time >= lastAttackTime + attackInterval)
            ChangeState(State.Attack);
    }

    void HandleAttackState()
    {
        ChangeState(State.Idle);
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (newState)
        {
            case State.Idle:
                break;
            case State.Attack:
                lastAttackTime = Time.time;
                FireBullet();
                break;
        }
    }

    void FireBullet()
    {
        if (playerTarget == null) return;

        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Vector3 dir = playerTarget.position - origin;
        dir.y = 0f;

        if (dir.magnitude < 0.01f) dir = Vector3.right;
        dir.Normalize();

        GameObject bullet = Instantiate(bulletPrefab, origin, Quaternion.LookRotation(dir));
        TowerBullet tb = bullet.GetComponent<TowerBullet>();
        if (tb != null)
        {
            tb.speed = bulletSpeed;
            tb.damage = attackDamage;
            tb.SetTarget(playerTarget);
        }

        Destroy(bullet, bulletLifetime);
    }

    public bool TakeDamage(float damage, Transform damageDealer)
    {
        if (health == null || isDead) return false;

        bool tookDamage = health.TakeDamage(damage, damageDealer);
        if (health.currentHealth <= 0) Die();
        return tookDamage;
    }

    void Die()
    {
        isDead = true;
        currentState = State.Dead;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Destroy(gameObject, 3f);
    }
}