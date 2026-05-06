using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_qu : MonoBehaviour, IDamageable
{
    public enum State { Idle, Move, Attack, Dead }
    public State currentState;

    [Header("移动")]
    public float speed = 3f;
    public float idleTime = 2f;

    [Header("检测")]
    public float wallCheckDistance = 0.5f;
    public float groundCheckDistance = 1f;
    public float playerCheckDistance = 8f;
    public float attackDistance = 1.5f;
    public float forwardGroundCheckDistance = 0.5f;
    public LayerMask whatIsGround;
    public LayerMask whatIsAirWall;
    public LayerMask whatIsPlayer;
    public Transform wallCheckPoint;
    public Transform groundCheckPoint;
    public Transform playerCheckPoint;
    public Transform forwardGroundCheckPoint;

    [Header("攻击")]
    public float attackDamage = 10f;
    public float attackCooldown = 1f;
    public float maxHealth = 3f;

    public Rigidbody rb;
    public Animator anim;
    public Collider col;
    public Transform player;
    public bool onGround;
    public bool wallDetected;
    public bool airWallDetected;
    public bool playerDetected;
    public int facingDir = 1;
    public float idleTimer;
    public float currentHealth;
    public bool isDead;
    public float lastAttackTime;
    public bool isAttacking;

    // 动画哈希值
    private int yVelocityHash;
    private int xVelocityHash;
    private int idleHash;
    private int moveHash;
    private int attackHash;
    private int deadHash;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        col = GetComponent<Collider>();

        if (wallCheckPoint == null) wallCheckPoint = transform;
        if (groundCheckPoint == null) groundCheckPoint = transform;
        if (playerCheckPoint == null) playerCheckPoint = transform;
        if (forwardGroundCheckPoint == null) forwardGroundCheckPoint = transform;

        yVelocityHash = Animator.StringToHash("yVelocity");
        xVelocityHash = Animator.StringToHash("xVelocity");
        idleHash = Animator.StringToHash("Idle");
        moveHash = Animator.StringToHash("Move");
        attackHash = Animator.StringToHash("Attack");
        deadHash = Animator.StringToHash("Dead");

        AlignToMembrane();

        currentHealth = maxHealth;
        currentState = State.Idle;
        idleTimer = idleTime;
        lastAttackTime = -attackCooldown;

        Debug.Log("Enemy_qu 初始化完成");
    }

    void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        CheckGround();
        CheckWall();
        CheckAirWall();
        CheckPlayer();
        UpdateAnimatorParameters();

        switch (currentState)
        {
            case State.Idle:
                HandleIdleState();
                break;
            case State.Move:
                HandleMoveState();
                break;
            case State.Attack:
                HandleAttackState();
                break;
            case State.Dead:
                break;
        }
    }

    void UpdateAnimatorParameters()
    {
        anim.SetFloat(yVelocityHash, rb.velocity.y);
        anim.SetFloat(xVelocityHash, Mathf.Abs(rb.velocity.x));
    }

    void CheckGround()
    {
        onGround = Physics.Raycast(
            groundCheckPoint.position,
            Vector3.down,
            groundCheckDistance,
            whatIsGround
        );
    }

    bool CheckForwardGround()
    {
        if (forwardGroundCheckPoint == null) return true;

        return Physics.Raycast(
            forwardGroundCheckPoint.position,
            Vector3.down,
            forwardGroundCheckDistance,
            whatIsGround
        );
    }

    void CheckWall()
    {
        Vector3 dir = facingDir == 1 ? Vector3.right : Vector3.left;
        wallDetected = Physics.Raycast(
            wallCheckPoint.position,
            dir,
            wallCheckDistance,
            whatIsGround
        );
    }

    void CheckAirWall()
    {
        Vector3 dir = facingDir == 1 ? Vector3.right : Vector3.left;
        airWallDetected = Physics.Raycast(
            wallCheckPoint.position,
            dir,
            wallCheckDistance,
            whatIsAirWall
        );
    }

    void CheckPlayer()
    {
        if (player == null)
        {
            playerDetected = false;
            return;
        }

        Vector3 dir = facingDir == 1 ? Vector3.right : Vector3.left;

        playerDetected = Physics.Raycast(
            playerCheckPoint.position,
            dir,
            playerCheckDistance,
            whatIsPlayer
        );

        if (playerDetected)
        {
            Debug.Log("敌人检测到玩家");
        }
    }

    bool IsPlayerInAttackRange()
    {
        if (player == null) return false;

        Vector3 dir = facingDir == 1 ? Vector3.right : Vector3.left;
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, out hit, attackDistance, whatIsPlayer))
        {
            return true;
        }

        return false;
    }

    public void TakeDamage(float damage, Transform damageDealer)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"敌人受到 {damage} 伤害，剩余血量: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        currentState = State.Dead;

        if (col != null)
            col.enabled = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        anim.SetBool(idleHash, false);
        anim.SetBool(moveHash, false);
        anim.SetBool(attackHash, false);
        anim.SetBool(deadHash, true);

        Destroy(gameObject, 3f);
    }

    void HandleIdleState()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        anim.SetBool(idleHash, true);
        anim.SetBool(moveHash, false);
        anim.SetBool(attackHash, false);

        idleTimer -= Time.deltaTime;

        if (playerDetected)
        {
            ChangeState(State.Attack);
        }
        else if (idleTimer <= 0)
        {
            ChangeState(State.Move);
        }
    }

    void HandleMoveState()
    {
        anim.SetBool(idleHash, false);
        anim.SetBool(moveHash, true);
        anim.SetBool(attackHash, false);

        if (playerDetected)
        {
            ChangeState(State.Attack);
            return;
        }

        if (wallDetected || airWallDetected || !CheckForwardGround())
        {
            Flip();
            return;
        }

        rb.velocity = new Vector3(facingDir * speed, rb.velocity.y, 0);
    }

    void HandleAttackState()
    {
        if (!playerDetected)
        {
            ChangeState(State.Idle);
            return;
        }

        bool inAttackRange = IsPlayerInAttackRange();

        if (inAttackRange)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

            if (Time.time >= lastAttackTime + attackCooldown && !isAttacking)
            {
                isAttacking = true;
                lastAttackTime = Time.time;
                anim.SetTrigger("Attack");
                Debug.Log("敌人触发攻击动画");
            }
        }
        else
        {
            rb.velocity = new Vector3(facingDir * speed, rb.velocity.y, 0);

            anim.SetBool(moveHash, true);
            anim.SetBool(attackHash, false);
        }
    }

    // 动画事件
    public void OnAttackHit()
    {
        Debug.Log("OnAttackHit 被调用");
        PerformAttack();
    }

    public void OnAttackEnd()
    {
        Debug.Log("OnAttackEnd 被调用");
        isAttacking = false;
        ChangeState(State.Idle);
    }

    void PerformAttack()
    {
        Debug.Log("PerformAttack 被调用");

        if (player == null)
        {
            Debug.Log("player 为空");
            return;
        }

        Vector3 dir = facingDir == 1 ? Vector3.right : Vector3.left;
        RaycastHit hit;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Debug.DrawRay(origin, dir * attackDistance, Color.red, 1f);

        if (Physics.Raycast(origin, dir, out hit, attackDistance, whatIsPlayer))
        {
            Debug.Log($"射线击中: {hit.collider.name}");

            // 方式1：尝试直接获取玩家脚本
            Player_X playerX = hit.collider.GetComponent<Player_X>();
            if (playerX != null)
            {
                playerX.TakeDamage(attackDamage, transform);
                Debug.Log($"攻击 Player_X 成功，伤害 {attackDamage}");
                return;
            }

            Player_Z playerZ = hit.collider.GetComponent<Player_Z>();
            if (playerZ != null)
            {
                playerZ.TakeDamage(attackDamage, transform);
                Debug.Log($"攻击 PlayerZ 成功，伤害 {attackDamage}");
                return;
            }

            // 方式2：通过接口
            IDamageable damageable = hit.collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage, transform);
                Debug.Log($"通过接口造成伤害 {attackDamage}");
            }
            else
            {
                Debug.LogWarning($"击中物体 {hit.collider.name} 没有可伤害的组件");
            }
        }
        else
        {
            Debug.Log("射线未击中任何物体");
        }
    }

    void Flip()
    {
        facingDir *= -1;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.flipX = facingDir == -1;
        Debug.Log($"敌人翻转，facingDir = {facingDir}");
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;

        Debug.Log($"状态切换: {currentState} -> {newState}");
        currentState = newState;

        switch (currentState)
        {
            case State.Idle:
                idleTimer = idleTime;
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
                break;
            case State.Move:
                break;
            case State.Attack:
                break;
            case State.Dead:
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (wallCheckPoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 dir = facingDir == 1 ? Vector3.right : Vector3.left;
            Gizmos.DrawRay(wallCheckPoint.position, dir * wallCheckDistance);
        }

        if (groundCheckPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(groundCheckPoint.position, Vector3.down * groundCheckDistance);
        }

        if (forwardGroundCheckPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(forwardGroundCheckPoint.position, Vector3.down * forwardGroundCheckDistance);
        }

        if (playerCheckPoint != null)
        {
            Gizmos.color = Color.green;
            Vector3 dir = facingDir == 1 ? Vector3.right : Vector3.left;
            Gizmos.DrawRay(playerCheckPoint.position, dir * playerCheckDistance);
        }

        Gizmos.color = new Color(1, 0.5f, 0);
        Vector3 attackDir = facingDir == 1 ? Vector3.right : Vector3.left;
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, attackDir * attackDistance);
    }

    void AlignToMembrane()
    {
        Vector3 pos = transform.position;
        pos.z = 0f;
        transform.position = pos;
    }
}