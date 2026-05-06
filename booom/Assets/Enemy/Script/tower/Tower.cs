using UnityEngine;

public class Tower : MonoBehaviour, IDamageable
{
    public enum State { Idle, Attack, Dead }
    public State currentState;

    [Header("子弹")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 8f;
    public float bulletLifetime = 1.5f;
    public float attackInterval = 1.5f;
    public float attackDamage = 10f;
    public LayerMask whatIsPlayer;
    public LayerMask whatIsEnemy;

    [Header("血量")]
    public float maxHealth = 5f;
    private float currentHealth;

    [Header("朝向")]
    public Transform spriteHolder;
    public bool OnX = true;

    private float lastAttackTime;
    private bool isDead;
    private Transform playerTarget;

    private Animator anim;
    private int idleHash = Animator.StringToHash("Idle");
    private int attackHash = Animator.StringToHash("Attack");
    private int deadHash = Animator.StringToHash("Dead");

    void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        currentState = State.Idle;
        lastAttackTime = -attackInterval;
    }

    void Update()
    {
        if (isDead) return;

        FacePlayer();

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

    void FacePlayer()
    {
        if (playerTarget == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTarget = playerObj.transform;
        }

        if (playerTarget != null && spriteHolder != null)
        {
            Vector3 dir = playerTarget.position - transform.position;
            dir.y = 0f;
            if (dir.magnitude > 0.01f)
                spriteHolder.rotation = Quaternion.LookRotation(dir);
        }
    }

    void HandleIdleState()
    {
        if (Time.time >= lastAttackTime + attackInterval)
            ChangeState(State.Attack);
    }

    void HandleAttackState()
    {
        // 发完子弹立刻回 Idle，等下一个间隔
        ChangeState(State.Idle);
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (newState)
        {
            case State.Idle:
                anim.SetBool(idleHash, true);
                anim.SetBool(attackHash, false);
                break;
            case State.Attack:
                anim.SetBool(idleHash, false);
                anim.SetBool(attackHash, true);
                lastAttackTime = Time.time;
                FireBullet();
                break;
        }
    }

    void FireBullet()
    {
        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Vector3 dir = spriteHolder != null ? spriteHolder.forward : Vector3.right;

        GameObject bullet = Instantiate(bulletPrefab, origin, Quaternion.LookRotation(dir));
        TowerBullet tb = bullet.GetComponent<TowerBullet>();
        if (tb != null)
        {
            tb.speed = bulletSpeed;
            tb.damage = attackDamage;
            tb.whatIsEnemy = whatIsEnemy;
            tb.SetTarget(playerTarget);   // 传入玩家目标
        }

        Destroy(bullet, bulletLifetime);
    }

    public void TakeDamage(float damage, Transform damageDealer)
    {
        if (OnX) return;
        if (isDead) return;

        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        currentState = State.Dead;
        anim.SetBool(deadHash, true);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Destroy(gameObject, 3f);
    }
}