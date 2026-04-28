using UnityEngine;

public class Player_X : MonoBehaviour
{
    public enum State { Idle, Move, Jump, Attack }
    public State currentState;
    public CameraController cameraController;   


    [Header("移动")]
    public float speed = 5f;
    public float jumpForce = 8f;

    [Header("地面检测")]
    public float groundCheckDistance = 0.1f;
    public LayerMask whatIsGround;
    public Transform groundCheckPoint;

    [Header("维度")]
    public bool OnX = true;

    private Rigidbody rb;
    private Animator anim;
    private bool onGround;
    private int facingDir = 1;
    public bool triggerCalled;

    [Header("跳跃")]

    [Header("射击")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float lifeTime = 2f;

    [Header("旋转")]
    public float changeSpeed = 100f;

    // Animator 参数
    private int yVelocityHash = Animator.StringToHash("yVelocity");

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        if (groundCheckPoint == null)
            groundCheckPoint = transform;
        /*        Debug.Log($"检测点位置: {groundCheckPoint.position}");
                Debug.Log($"检测距离: {groundCheckDistance}");
                Debug.Log($"检测层级: {whatIsGround.value}");*/
        currentState = State.Idle;
    }

    void Update()
    {
        if (!OnX) return;
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

        // 翻转
        transform.rotation = Quaternion.Euler(0, facingDir == 1 ? 0 : 180, 0);
    }

    void UpdateAnimatorParameters()
    {
        anim.SetFloat("yVelocityHash", rb.velocity.y);
    }

    void HandleIdleState(float moveX)
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangeState(State.Attack);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            ChangeState(State.Jump);
        }
        else if (moveX != 0)
        {
            ChangeState(State.Move);
        }
        else if (onGround && rb.velocity.y == 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void HandleMoveState(float moveX)
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangeState(State.Attack);
            return;  // 立即返回，不执行后面的移动代码
        }
        else if (Input.GetKeyDown(KeyCode.Space) && onGround)
        {
            ChangeState(State.Jump);
            return;  // 立即返回
        }
        else if (moveX == 0)
        {
            ChangeState(State.Idle);
            return;
        }

        // 只有不切换状态时才设置移动速度
        rb.velocity = new Vector3(moveX * speed, rb.velocity.y, 0);
    }

    void HandleJumpState(float moveX)
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChangeState(State.Attack);
            return;
        }

        // 空中可以水平移动
        rb.velocity = new Vector3(moveX * speed, rb.velocity.y, 0);

        // 落地检测（需要稍微下降才能落地）
        if (onGround && rb.velocity.y == 0)
        {
            ChangeState(State.Idle);
        }
    }

    void HandleAttackState()
    {
        // 攻击时不能移动
        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        if (triggerCalled)
        {
            ChangeState(State.Idle);
        }

    }

    void CheckGround()
    {
        onGround = Physics.Raycast(
            groundCheckPoint.position,
            Vector3.down,
            groundCheckDistance,
            whatIsGround
        );
        /*        Debug.Log($"地面检测: onGround = {onGround}");*/
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;
        /*
                Debug.Log($"状态切换: {currentState} -> {newState}");*/

        // 退出当前状态
        currentState = newState;

        // 重置动画参数
        anim.SetBool("Idle", false);
        anim.SetBool("Move", false);
        anim.SetBool("Jump", false);
        anim.SetBool("Attack", false);

        triggerCalled = false;

        // 进入新状态
        switch (newState)
        {
            case State.Idle:
                anim.SetBool("Idle", true);
                break;
            case State.Move:
                anim.SetBool("Move", true);
                break;
            case State.Jump:
                anim.SetBool("Jump", true);
                // 强制设置向上的速度
                rb.velocity = new Vector3(rb.velocity.x, jumpForce, 0);
                /*                Debug.Log($"跳跃！速度: {rb.velocity}");*/
                break;
            case State.Attack:
                anim.SetBool("Attack", true);
                ShootX();
                break;
        }
    }

    void ShootX()
    {
        Vector3 dir = facingDir == 1 ? Vector3.right : Vector3.left;
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, transform.rotation);//Instantiate是克隆函数 第一个是预制件 第二个是生成位置 第三个是生成时的朝向
        Rigidbody2D Bulletrb = bullet.GetComponent<Rigidbody2D>();
        Bulletrb.velocity = new Vector2(facingDir * speed, 0);
        Destroy(bullet, lifeTime);
    }


    public void OnTabPressed()
    {
    }

    public void Trigger()
    {
        triggerCalled = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = groundCheckPoint != null ? groundCheckPoint.position : transform.position;
        Gizmos.DrawLine(origin, origin + Vector3.down * groundCheckDistance);
    }
}