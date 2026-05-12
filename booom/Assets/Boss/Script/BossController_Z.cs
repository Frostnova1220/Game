using System.Collections;
using UnityEngine;

public class BossController_Z : MonoBehaviour
{
    public Transform GroundPoint;          // 头部下落目标点

    [Header("刷怪笼")]
    public GameObject[] spawnCage;
    public float resetTime = 5f;
    private float[] recoveryStartTime;

    [Header("追踪")]
    public GameObject[] Arm;
    public GameObject Player;
    private float attackTimer;
    private float lastArmDamageTime;
    public float attackCooldown = 10f;
    public float armDamageCooldown = 0.5f;
    public float armDamage = 1f;
    public float armCheckRadius = 1.5f;
    public float MoveTime = 1f;
    public LayerMask playerLayer;

    [Header("Boss状态")]
    public ArmHealth leftArmHealth;
    public ArmHealth rightArmHealth;
    public HeadHealth headHealth;

    [Header("命中特效")]
    public GameObject hitSfxPrefab;
    public float hitSfxDuration = 0.5f;
    public float flashDuration = 0.1f;

    // 死亡标记，确保只执行一次
    private bool leftArmDead = false;
    private bool rightArmDead = false;
    private bool headFalling = false;

    void Start()
    {
        attackTimer = attackCooldown;

        recoveryStartTime = new float[spawnCage.Length];
        for (int i = 0; i < recoveryStartTime.Length; i++)
        {
            recoveryStartTime[i] = -1f;
        }
    }

    void Update()
    {
        DetectHealth();
        HeadMove();

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            Attack();
            attackTimer = Random.Range(5f, 10f);
        }

        // 刷怪笼恢复逻辑
        for (int i = 0; i < spawnCage.Length; i++)
        {
            if (spawnCage[i] == null)
                continue;

            bool isActive = spawnCage[i].activeSelf;

            if (!isActive)
            {
                if (recoveryStartTime[i] < 0)
                    recoveryStartTime[i] = Time.time;

                if (Time.time - recoveryStartTime[i] >= resetTime)
                {
                    spawnCage[i].SetActive(true);
                    recoveryStartTime[i] = -1f;
                }
            }
            else
            {
                recoveryStartTime[i] = -1f;
            }
        }
    }

    void Attack()
    {
        // 安全检查
        if (Arm == null || Arm.Length < 2) return;

        int attackType = Random.Range(0, 2);
        GameObject selectedArm = attackType == 0 ? Arm[0] : Arm[1];

        // 如果选中的手臂为空或已失活，尝试用另一只
        if (selectedArm != null && selectedArm.activeSelf)
        {
            StartCoroutine(TrackPlayer(selectedArm, Player));
        }
        else
        {
            GameObject otherArm = attackType == 0 ? Arm[1] : Arm[0];
            if (otherArm != null && otherArm.activeSelf)
                StartCoroutine(TrackPlayer(otherArm, Player));
        }
    }

    public void ResetSpawn()
    {
        for (int i = 0; i < spawnCage.Length; i++)
        {
            if (spawnCage[i] != null)
            {
                spawnCage[i].SetActive(true);
                recoveryStartTime[i] = -1f;
            }
        }
    }

    IEnumerator TrackPlayer(GameObject eye, GameObject player)
    {
        yield return StartCoroutine(MoveOverTime(eye, eye.transform.position, player.transform.position, MoveTime));
        CheckArmDamage(eye.transform.position);
    }

    void CheckArmDamage(Vector3 armPosition)
    {
        Collider[] hits = Physics.OverlapSphere(armPosition, armCheckRadius, playerLayer);
        foreach (Collider hit in hits)
        {
            ApplyDamage(hit, armPosition);
        }
    }

    IEnumerator MoveOverTime(GameObject obj, Vector3 start, Vector3 end, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.SmoothStep(0, 1, t);
            obj.transform.position = Vector3.Lerp(start, end, t);

            CheckArmDamageWithCooldown(obj.transform.position);

            yield return null;
        }
        obj.transform.position = end;
    }

    void CheckArmDamageWithCooldown(Vector3 armPosition)
    {
        if (Time.time < lastArmDamageTime + armDamageCooldown) return;

        Collider[] hits = Physics.OverlapSphere(armPosition, armCheckRadius, playerLayer);
        if (hits.Length > 0)
        {
            ApplyDamage(hits[0], armPosition);
            lastArmDamageTime = Time.time;
        }
    }

    void ApplyDamage(Collider playerCollider, Vector3 hitPoint)
    {
        Health playerHealth = playerCollider.GetComponentInParent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(armDamage, transform);
        }

        // 播放命中特效
        if (hitSfxPrefab != null)
        {
            GameObject sfx = Instantiate(hitSfxPrefab, hitPoint, Quaternion.identity);
            Destroy(sfx, hitSfxDuration);
        }

        // 玩家闪红
        SpriteRenderer sr = playerCollider.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
            StartCoroutine(FlashRed(sr, flashDuration));
    }

    IEnumerator FlashRed(SpriteRenderer sr, float duration)
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(duration);
        sr.color = Color.white;
    }

    void DetectHealth()
    {
        if (!leftArmDead && leftArmHealth != null && leftArmHealth.currentHealth <= 0)
        {
            leftArmDead = true;
            StartCoroutine(HandleArmDeath(Arm[0]));
        }

        if (!rightArmDead && rightArmHealth != null && rightArmHealth.currentHealth <= 0)
        {
            rightArmDead = true;
            StartCoroutine(HandleArmDeath(Arm[1]));
        }
    }

    IEnumerator HandleArmDeath(GameObject arm)
    {
        if (arm == null) yield break;

        ArmHealth ah = arm.GetComponent<ArmHealth>();
        if (ah != null)
        {
            ah.StopAllCoroutines(); 
            ah.enabled = false;
        }

        SpriteRenderer sr = arm.GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = Color.gray;


        yield return StartCoroutine(MoveOverTime(arm,
            arm.transform.position,
            arm.transform.position + new Vector3(0, -10, 0),
            MoveTime * 5f));

        arm.SetActive(false);
    }

    void HeadMove()
    {
        if (!headFalling && leftArmDead && rightArmDead)
        {
            headFalling = true;
            StartCoroutine(HandleHeadFall());
        }
    }

    IEnumerator HandleHeadFall()
    {
        GameObject head = headHealth != null ? headHealth.gameObject : null;
        if (head == null) yield break;

        Vector3 targetPos;
        if (GroundPoint != null)
            targetPos = GroundPoint.position;
        else
            targetPos = head.transform.position + new Vector3(0, -10, 0);

        yield return StartCoroutine(MoveOverTime(head,
            head.transform.position,
            targetPos,
            MoveTime * 5f));
    }
}