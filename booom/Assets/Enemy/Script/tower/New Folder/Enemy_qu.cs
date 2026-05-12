using UnityEngine;

public class Enemy_qu : MonoBehaviour, IDamageable
{
    public enum State { Idle, Move, Chase, Dead }
    public State currentState;

    [Header("ÒÆ¶¯")]
    public float speed = 3f;
    public float chaseSpeed = 5f;
    public float idleTime = 2f;

    [Header("¼ì²â")]
    public float wallCheckDistance = 0.5f;
    public float groundCheckDistance = 1f;
    public float playerCheckDistance = 8f;
    public float forwardGroundCheckDistance = 0.5f;
    public LayerMask whatIsGround;
    public LayerMask whatIsSolidWall;
    public LayerMask whatIsPlayer;
    public Transform wallCheckPoint;
    public Transform groundCheckPoint;
    public Transform playerCheckPoint;
    public Transform forwardGroundCheckPoint;

    [Header("ÑªÁ¿")]
    public Health health;

    public Rigidbody rb;
    public Animator anim;
    public Collider col;
    public Transform player;
    public bool onGround;
    public bool wallDetected;
    public bool forwardGroundDetected;
    public bool playerDetected;
    public int facingDir = 1;
    public float idleTimer;
    public bool isDead;
    private float playerLostTimer;
    public float playerLostThreshold = 0.5f;

    private int idleHash;
    private int moveHash;
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

        idleHash = Animator.StringToHash("Idle");
        moveHash = Animator.StringToHash("Move");
        deadHash = Animator.StringToHash("Dead");

        currentState = State.Idle;
        idleTimer = idleTime;
    }

    void Update()
    {
        if (isDead) return;

        if (player == null)
        {
            GameObject obj = GameObject.FindGameObjectWithTag("Player");
            if (obj != null) player = obj.transform;
        }

        CheckGround();
        CheckWall();
        CheckForwardGround();
        CheckPlayer();
        FaceTarget();

        switch (currentState)
        {
            case State.Idle: HandleIdle(); break;
            case State.Move: HandleMove(); break;
            case State.Chase: HandleChase(); break;
        }
    }

    void CheckGround()
    {
        onGround = Physics.Raycast(groundCheckPoint.position, Vector3.down, groundCheckDistance, whatIsGround);
    }

    void CheckWall()
    {
        Vector3 dir = facingDir == 1 ? Vector3.right : Vector3.left;
        wallDetected = Physics.Raycast(wallCheckPoint.position, dir, wallCheckDistance, whatIsSolidWall)
                    || Physics.Raycast(wallCheckPoint.position, dir, wallCheckDistance, whatIsGround)
                    || Physics.Raycast(wallCheckPoint.position, dir, wallCheckDistance, LayerMask.GetMask("EnemyWall"));
    }

    void CheckForwardGround()
    {
        if (forwardGroundCheckPoint == null) { forwardGroundDetected = true; return; }
        forwardGroundDetected = Physics.Raycast(forwardGroundCheckPoint.position, Vector3.down, forwardGroundCheckDistance, whatIsGround);
    }

    void CheckPlayer()
    {
        if (player == null) { playerDetected = false; return; }
        Vector3 dir = facingDir == 1 ? Vector3.right : Vector3.left;
        playerDetected = Physics.Raycast(playerCheckPoint.position, dir, playerCheckDistance, whatIsPlayer);
    }

    void FaceTarget()
    {
        if (player == null) return;
        if (currentState != State.Chase) return;
        float toPlayer = player.position.x - transform.position.x;
        if (toPlayer > 0 && facingDir == -1) Flip();
        else if (toPlayer < 0 && facingDir == 1) Flip();
    }

    public bool TakeDamage(float damage, Transform damageDealer)
    {
        if (health == null || isDead) return false;
        bool took = health.TakeDamage(damage, damageDealer);
        if (health.currentHealth <= 0) Die();
        return took;
    }

    void Die()
    {
        isDead = true;
        currentState = State.Dead;
        if (col != null) col.enabled = false;
        if (rb != null) { rb.velocity = Vector3.zero; rb.isKinematic = true; }
        anim.SetBool(deadHash, true);
        Destroy(gameObject, 3f);
    }

    void HandleIdle()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        anim.SetBool(idleHash, true);
        anim.SetBool(moveHash, false);
        idleTimer -= Time.deltaTime;
        if (playerDetected) ChangeState(State.Chase);
        else if (idleTimer <= 0) ChangeState(State.Move);
    }

    void HandleMove()
    {
        anim.SetBool(idleHash, false);
        anim.SetBool(moveHash, true);
        if (playerDetected) { ChangeState(State.Chase); return; }
        if (wallDetected || !forwardGroundDetected) { Flip(); return; }
        rb.velocity = new Vector3(facingDir * speed, rb.velocity.y, 0);
    }

    void HandleChase()
    {
        anim.SetBool(idleHash, false);
        anim.SetBool(moveHash, true);
        if (!playerDetected)
        {
            playerLostTimer += Time.deltaTime;
            if (playerLostTimer >= playerLostThreshold) ChangeState(State.Idle);
            return;
        }
        playerLostTimer = 0f;
        if (wallDetected || !forwardGroundDetected) { Flip(); return; }
        rb.velocity = new Vector3(facingDir * chaseSpeed, rb.velocity.y, 0);
    }

    void Flip()
    {
        facingDir *= -1;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.flipX = facingDir == -1;
    }

    void ChangeState(State s)
    {
        currentState = s;
        if (s == State.Idle) idleTimer = idleTime;
    }
}