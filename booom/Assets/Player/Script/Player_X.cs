using UnityEngine;

public class Player_X : MonoBehaviour, IDamageable
{
    public enum State { Idle, Move, Jump, Attack }
    public State currentState;
    public CameraController cameraController;

    [Header("移动")]
    public float speed = 5f;
    public float jumpForce = 8f;

    [Header("地面检测")]
    public float groundCheckDistance = 0.2f;
    public LayerMask whatIsGround;
    public Transform groundCheckPoint;

    [Header("维度")]
    public bool OnX = true;
    public GameObject playerZ;

    [Header("射击")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float lifeTime = 2f;
    public LayerMask whatIsEnemy;

    [Header("血量")]
    public HealthContainer healthContainer;

    private Rigidbody rb;
    private Animator anim;
    private bool onGround;
    private int facingDir = 1;
    public bool triggerCalled;
    private bool isDead;

    private int yVelocityHash = Animator.StringToHash("yVelocity");
    private int xVelocityHash = Animator.StringToHash("xVelocity");

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        if (groundCheckPoint == null) groundCheckPoint = transform;
        if (firePoint == null) firePoint = transform;

        currentState = State.Idle;
    }
  

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        }
        if (!OnX || isDead) return;

        CheckGround();
        UpdateAnimatorParameters();

        float moveX = Input.GetAxisRaw("Horizontal");

        if (moveX != 0)
            facingDir = moveX > 0 ? 1 : -1;

        switch (currentState)
        {
            case State.Idle:
                HandleIdleState(moveX);
                break;
            case State.Move:
                HandleMoveState(moveX);
                break;
            case State.Jump:
                HandleJumpState(moveX);
                break;
            case State.Attack:
                HandleAttackState();
                break;
        }

        transform.localScale = new Vector3(facingDir == 1 ? 1 : -1, 1, 1);
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

        Debug.DrawRay(groundCheckPoint.position, Vector3.down * groundCheckDistance, onGround ? Color.green : Color.red);
    }

    void HandleIdleState(float moveX)
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        if (Input.GetKeyDown(KeyCode.J))
            ChangeState(State.Attack);
        else if (Input.GetKeyDown(KeyCode.Space) && onGround)
            ChangeState(State.Jump);
        else if (moveX != 0)
            ChangeState(State.Move);
    }

    void HandleMoveState(float moveX)
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangeState(State.Attack);
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            ChangeState(State.Jump);
            return;
        }
        else if (moveX == 0)
        {
            ChangeState(State.Idle);
            return;
        }

        rb.velocity = new Vector3(moveX * speed, rb.velocity.y, 0);
    }

    void HandleJumpState(float moveX)
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangeState(State.Attack);
            return;
        }

        rb.velocity = new Vector3(moveX * speed, rb.velocity.y, 0);

        if (onGround && rb.velocity.y <= 0)
            ChangeState(State.Idle);
    }

    void HandleAttackState()
    {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        if (triggerCalled)
            ChangeState(State.Idle);
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;

        currentState = newState;

        if (anim != null)
        {
            anim.SetBool("Idle", false);
            anim.SetBool("Move", false);
            anim.SetBool("Jump", false);
            anim.SetBool("Attack", false);
        }

        triggerCalled = false;

        switch (newState)
        {
            case State.Idle:
                if (anim != null) anim.SetBool("Idle", true);
                break;
            case State.Move:
                if (anim != null) anim.SetBool("Move", true);
                break;
            case State.Jump:
                if (anim != null) anim.SetBool("Jump", true);
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
                break;
            case State.Attack:
                if (anim != null) anim.SetBool("Attack", true);
                Shoot();
                break;
        }
    }

    void Shoot()
    {
        Vector3 origin = firePoint != null ? firePoint.position : transform.position;
        Vector3 forwardDir = facingDir == 1 ? Vector3.right : Vector3.left;

        // 道具使用
        ItemManager itemMgr = FindObjectOfType<ItemManager>();
        if (itemMgr != null)
        {
            if (itemMgr.TryUseEquippedItem(origin, forwardDir, bulletPrefab, firePoint, whatIsEnemy, out GameObject specialBullet))
            {
                if (specialBullet != null)
                    Destroy(specialBullet, lifeTime);
                return;
            }
        }

        // 普通子弹
        Collider[] hits = Physics.OverlapSphere(origin, 15f, whatIsEnemy);
        Transform closest = null;
        float closestDist = 15f;

        for (int i = 0; i < hits.Length; i++)
        {
            Vector3 toEnemy = hits[i].transform.position - origin;
            float dot = Vector3.Dot(toEnemy.normalized, forwardDir);
            if (dot > -0.1f)
            {
                float dist = toEnemy.magnitude;
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = hits[i].transform;
                }
            }
        }

        Vector3 dir = closest != null ? (closest.position - origin).normalized : forwardDir;
        GameObject bullet = Instantiate(bulletPrefab, origin, Quaternion.LookRotation(dir));
        HomingBullet homing = bullet.GetComponent<HomingBullet>();
        if (homing != null)
        {
            homing.whatIsEnemy = whatIsEnemy;
            homing.SetTarget(closest);
        }
        Destroy(bullet, lifeTime);
    }

 

    public void TakeDamage(float damage, Transform damageDealer)
    {
        if (healthContainer == null) return;

        healthContainer.TakeDamage(damage);

        if (anim != null) anim.SetTrigger("Hurt");

        Vector3 knockbackDir = (transform.position - damageDealer.position).normalized;
        knockbackDir.y = 0.5f;
        rb.velocity = knockbackDir * 8f;

        if (healthContainer.IsDead())
            Die();
    }

    void Die()
    {
        isDead = true;

        if (anim != null)
        {
            anim.SetBool("Idle", false);
            anim.SetBool("Move", false);
            anim.SetBool("Jump", false);
            anim.SetBool("Attack", false);
            anim.SetBool("Dead", true);
        }

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (GetComponent<Collider>() != null)
            GetComponent<Collider>().enabled = false;

        Destroy(gameObject, 3f);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = groundCheckPoint != null ? groundCheckPoint.position : transform.position;
        Gizmos.DrawLine(origin, origin + Vector3.down * groundCheckDistance);
    }
}