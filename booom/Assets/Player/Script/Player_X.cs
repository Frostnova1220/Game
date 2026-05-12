using UnityEngine;

public class Player_X : MonoBehaviour, IDamageable
{
    public enum State { Idle, Move, Jump, Attack }
    public State currentState;
    public AudioController audioController;

    [Header("移动")]
    public float speed = 5f;
    public float jumpForce = 8f;
    private float footstepTimer;
    public float footstepInterval = 0.4f;

    [Header("地面检测")]
    public float groundCheckDistance = 0.2f;
    public LayerMask whatIsGround;
    public Transform groundCheckPoint;

    [Header("维度")]
    public bool OnX = true;

    [Header("射击")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletLifetime = 2f;
    public LayerMask whatIsEnemy;

    [Header("武器")]
    public bool Gun1 = true;
    public bool Gun2 = false;

    [Header("攻击时间")]
    public float attackDuration = 0.1f;

    [Header("血量")]
    public Health health;

    private Rigidbody rb;
    public Animator anim;
    private bool onGround;
    public int facingDir = 1;
    private bool isDead;
    private float attackTimer;

    void Start()
    {
        audioController = FindObjectOfType<AudioController>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        anim.SetBool("Gun1", Gun1);
        anim.SetBool("Gun2", Gun2);

        if (groundCheckPoint == null) groundCheckPoint = transform;
        if (firePoint == null) firePoint = transform;

        if (health != null)
            health.onDeath += OnPlayerDeath;

        currentState = State.Idle;
    }

    void OnDestroy()
    {
        if (health != null)
            health.onDeath -= OnPlayerDeath;
    }

    void OnPlayerDeath(GameObject deadObj)
    {
        if (deadObj != gameObject) return;
        Die();
    }

    void Update()
    {
        if (!OnX || isDead) return;

        CheckGround();

        if (Input.GetKeyDown(KeyCode.Q)&& GrenadeLauncher.instance != null)
        {
            Gun1 = !Gun1;
            Gun2 = !Gun2;
            anim.SetBool("Gun1", Gun1);
            anim.SetBool("Gun2", Gun2);
        }

        float moveX = Input.GetAxisRaw("Horizontal");
        if (moveX != 0) facingDir = moveX > 0 ? 1 : -1;

        switch (currentState)
        {
            case State.Idle: HandleIdle(moveX); break;
            case State.Move: HandleMove(moveX); break;
            case State.Jump: HandleJump(moveX); break;
            case State.Attack: HandleAttack(); break;
        }

        transform.localScale = new Vector3(facingDir == 1 ? 1 : -1, 1, 1);
        HandleFootsteps();
    }

    void CheckGround()
    {
        onGround = Physics.Raycast(groundCheckPoint.position, Vector3.down, groundCheckDistance, whatIsGround);
    }

    void HandleIdle(float moveX)
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        if (Input.GetKeyDown(KeyCode.J)) ChangeState(State.Attack);
        else if (Input.GetKeyDown(KeyCode.Space) && onGround) ChangeState(State.Jump);
        else if (moveX != 0) ChangeState(State.Move);
    }

    void HandleMove(float moveX)
    {
        if (Input.GetKeyDown(KeyCode.J)) { ChangeState(State.Attack); return; }
        if (Input.GetKeyDown(KeyCode.Space) && onGround) { ChangeState(State.Jump); return; }
        if (moveX == 0) { ChangeState(State.Idle); return; }
        rb.velocity = new Vector3(moveX * speed, rb.velocity.y, 0);
    }

    void HandleJump(float moveX)
    {
        if (Input.GetKeyDown(KeyCode.J)) { ChangeState(State.Attack); return; }
        rb.velocity = new Vector3(moveX * speed, rb.velocity.y, 0);
        if (onGround && rb.velocity.y <= 0) ChangeState(State.Idle);
    }

    void HandleAttack()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        attackTimer += Time.deltaTime;
        if (attackTimer > attackDuration) ChangeState(State.Idle);
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        anim.SetBool("Idle", false);
        anim.SetBool("Move", false);
        anim.SetBool("Jump", false);
        anim.SetBool("Attack", false);

        attackTimer = 0f;

        switch (newState)
        {
            case State.Idle:
                anim.SetBool("Idle", true);
                anim.SetBool("Gun2", Gun2);
                break;
            case State.Move:
                footstepTimer = 0f;
                anim.SetBool("Move", true);
                anim.SetBool("Gun2", Gun2);
                break;
            case State.Jump:
                audioController?.PlaySfx(audioController.Jump);
                anim.SetBool("Jump", true);
                anim.SetBool("Gun2", Gun2);
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
                break;
            case State.Attack:
                anim.SetBool("Attack", true);
                anim.SetBool("Gun2", Gun2);
                audioController?.PlaySfx(audioController.Shoot);
                Shoot();
                break;
        }
    }

    void Shoot()
    {
        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Vector3 dir = facingDir == 1 ? Vector3.right : Vector3.left;

        if (Gun2)
        {
            if (GrenadeLauncher.instance != null)
                GrenadeLauncher.instance.TryFire(origin, dir);
        }
        if(Gun1)
        {
            GameObject bullet = Instantiate(bulletPrefab, origin, Quaternion.LookRotation(dir));
            FireBullet fb = bullet.GetComponent<FireBullet>();
            if (fb != null) { fb.damage = 1f; fb.whatIsEnemy = whatIsEnemy; }
            Destroy(bullet, bulletLifetime);
        }
    }

    public bool TakeDamage(float damage, Transform damageDealer)
    {
        if (health == null || isDead) return false;
        bool took = health.TakeDamage(damage, damageDealer);
        if (took)
        {
            Vector3 knockDir = (transform.position - damageDealer.position).normalized;
            knockDir.y = 0.5f;
            rb.velocity = knockDir * 8f;
        }
        return took;
    }

    void Die()
    {
        isDead = true;
        anim.SetBool("Dead", true);
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 3f);
    }

    void HandleFootsteps()
    {
        if (currentState != State.Move || !onGround) { footstepTimer = 0f; return; }
        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0f)
        {
            footstepTimer = footstepInterval;
            audioController?.PlaySfx(audioController.Walk);
        }
    }
}