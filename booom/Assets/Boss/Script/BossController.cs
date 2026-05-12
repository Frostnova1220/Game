using System.Collections;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("石锥坠落")]
    public GameObject Stone;
    public Transform[] LspawnPoint;
    public Transform[] RspawnPoint;

    [Header("部位获取")]
    public GameObject LeftArm;
    public GameObject RightArm;

    [Header("Attack1攻击点")]
    public Transform LeftRestPoint;
    public Transform LeftAttackPoint;
    public Transform LeftEndPoint;
    public Transform RightRestPoint;
    public Transform RightAttackPoint;
    public Transform RightEndPoint;

    [Header("Attack2攻击点")]
    public Transform LeftAttackPoint1;
    public Transform LeftAttackPoint2;
    public Transform LeftAttackPoint3;
    public Transform LeftAttackPoint4;
    public Transform LeftAttackPoint5;
    public Transform LeftAttackPoint6;
    public Transform RightAttackPoint1;
    public Transform RightAttackPoint2;
    public Transform RightAttackPoint3;
    public Transform RightAttackPoint4;
    public Transform RightAttackPoint5;
    public Transform RightAttackPoint6;
    public float DropSpeed;
    public float BackSpeed;

    [Header("Attack3攻击")]
    public Transform LStartPoint;
    public Transform LEndPoint;
    public Transform RStartPoint;
    public Transform REndPoint;

    [Header("手臂断掉")]
    public Transform LUpPoint;
    public Transform LDownPoint;
    public Transform RUpPoint;
    public Transform RDownPoint;

    [Header("时间设置")]
    public float attackCooldown = 10f;
    public float armMoveDuration = 2f;
    public float attackHoldTime = 0.5f;

    [Header("手臂伤害")]
    public float armDamage = 1f;
    public float armCheckRadius = 1.5f;
    public float armDamageCooldown = 0.5f;
    public LayerMask playerLayer;

    [Header("命中特效")]
    public GameObject hitSfxPrefab;
    public float hitSfxDuration = 0.5f;

    [Header("玩家闪红")]
    public float flashDuration = 0.1f;

    [Header("Boss状态")]
    public ArmHealth leftArmHealth;
    public ArmHealth rightArmHealth;

    [Header("手臂状态")]
    public bool isLeftArmBroken = false;
    public bool isRightArmBroken=false;

    private float attackTimer;
    private float lastArmDamageTime;

    void Start()
    {
        attackTimer = attackCooldown;
    }

    void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            Attack();
            attackTimer = Random.Range(5f, 10f);
        }

        DetectHealth();

    }

    void Attack()
    {
        int attackType = Random.Range(0, 5);
        switch (attackType)
        {
            case 0:
                if(isRightArmBroken==false)
                    StartCoroutine(ArmAttack2(RightArm, RightRestPoint, RightAttackPoint1, RightAttackPoint2, RightAttackPoint3, RightAttackPoint4,RightAttackPoint5,RightAttackPoint6));
                if (isLeftArmBroken == false)
                    StartCoroutine(ArmAttack2(LeftArm, LeftRestPoint, LeftAttackPoint1, LeftAttackPoint2, LeftAttackPoint3, LeftAttackPoint4,LeftAttackPoint5,LeftAttackPoint6));
                break;
            case 1:
                if (isRightArmBroken == false)
                    StartCoroutine(ArmAttackRoutine(RightArm, RightRestPoint, RightAttackPoint, RightEndPoint));
                break;
            case 2:
                if (isLeftArmBroken == false)
                    StartCoroutine(ArmAttackRoutine(LeftArm, LeftRestPoint, LeftAttackPoint, LeftEndPoint));
                break;
            case 3:
                if (isRightArmBroken == false)
                    StartCoroutine(Attack3(RightArm, RightRestPoint, RStartPoint, REndPoint, RspawnPoint));
                break;
            case 4:
                if (isLeftArmBroken == false)
                    StartCoroutine(Attack3(LeftArm, LeftRestPoint, LStartPoint, LEndPoint, LspawnPoint));
                break;
        }
    }

    IEnumerator ArmAttackRoutine(GameObject Arm,Transform rest, Transform attackPoint ,Transform attackEndPoint)
    {
        // 移动到攻击点
        yield return StartCoroutine(MoveOverTime(Arm, rest.position, attackPoint.position, armMoveDuration));
        CheckArmDamage(Arm.transform.position);

        yield return new WaitForSeconds(attackHoldTime);

        yield return StartCoroutine(MoveOverTime(Arm, attackPoint.position, attackEndPoint.position, armMoveDuration));
        CheckArmDamage(Arm.transform.position);
        yield return new WaitForSeconds(attackHoldTime);

        // 归位
        yield return StartCoroutine(MoveOverTime(Arm, attackEndPoint.position, rest.position, armMoveDuration));
    }

    IEnumerator ArmAttack2(GameObject arm, Transform rest, Transform attackPoint1, Transform attackPoint2 , Transform attackPoint3 ,Transform attackPoint4,Transform attackPoint5,Transform attackPoint6)
    {
        // 移动到攻击点
        yield return StartCoroutine(MoveOverTime(arm, rest.position, attackPoint1.position, armMoveDuration*BackSpeed));
        CheckArmDamage(arm.transform.position);

        yield return new WaitForSeconds(attackHoldTime);

        // 移动到结束点
        yield return StartCoroutine(MoveOverTime(arm, attackPoint1.position, attackPoint2.position, armMoveDuration*DropSpeed));
        CheckArmDamage(arm.transform.position);

        yield return new WaitForSeconds(attackHoldTime);

        yield return StartCoroutine(MoveOverTime(arm, attackPoint2.position, attackPoint3.position, armMoveDuration * BackSpeed));
        CheckArmDamage(arm.transform.position);

        yield return new WaitForSeconds(attackHoldTime);

        yield return StartCoroutine(MoveOverTime(arm, attackPoint3.position, attackPoint4.position, armMoveDuration * DropSpeed));
        CheckArmDamage(arm.transform.position);

        yield return new WaitForSeconds(attackHoldTime);

        yield return StartCoroutine(MoveOverTime(arm, attackPoint4.position, attackPoint5.position, armMoveDuration * BackSpeed));
        CheckArmDamage(arm.transform.position);

        yield return new WaitForSeconds(attackHoldTime);

        yield return StartCoroutine(MoveOverTime(arm, attackPoint5.position, attackPoint6.position, armMoveDuration * DropSpeed));
        CheckArmDamage(arm.transform.position);

        yield return new WaitForSeconds(attackHoldTime);

        // 归位
        yield return StartCoroutine(MoveOverTime(arm, attackPoint6.position, rest.position, armMoveDuration));
    }

    IEnumerator Attack3(GameObject arm,Transform RestPoint,Transform StartPoint,Transform EndPoint, Transform[] spawnPoints)
    {
        yield return StartCoroutine(MoveOverTime(arm, RestPoint.transform.position, StartPoint.position, armMoveDuration));
        CheckArmDamage(arm.transform.position);

        yield return new WaitForSeconds(attackHoldTime);

        yield return StartCoroutine(MoveOverTime(arm, StartPoint.position, EndPoint.position, armMoveDuration));
        yield return StartCoroutine(CreateStone(spawnPoints));
        CheckArmDamage(arm.transform.position);

        yield return new WaitForSeconds(attackHoldTime);

        yield return StartCoroutine(MoveOverTime(arm, EndPoint.position, RestPoint.position, armMoveDuration));
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

            // 移动期间每帧检测伤害，带冷却
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

    void CheckArmDamage(Vector3 armPosition)
    {
        Collider[] hits = Physics.OverlapSphere(armPosition, armCheckRadius, playerLayer);
        foreach (Collider hit in hits)
        {
            ApplyDamage(hit, armPosition);
        }
    }

    void ApplyDamage(Collider playerCollider, Vector3 hitPoint)
    {
        Health playerHealth = playerCollider.GetComponentInParent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(armDamage, transform);
            Debug.Log($"手臂造成 {armDamage} 伤害，剩余血量: {playerHealth.currentHealth}");
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

    void DetectHealth()
    {
        if (!isLeftArmBroken && leftArmHealth != null && leftArmHealth.currentHealth <= 0)
        {
            leftArmHealth.StopAllCoroutines();
            leftArmHealth.enabled = false;

            SpriteRenderer sr = LeftArm.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = Color.gray;
            isLeftArmBroken = true;
            StartCoroutine(AfterBroken());
        }

        if (!isRightArmBroken && rightArmHealth != null && rightArmHealth.currentHealth <= 0)
        {
            rightArmHealth.StopAllCoroutines();
            rightArmHealth.enabled = false;

            SpriteRenderer sr = RightArm.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = Color.gray;
            isRightArmBroken = true;
            StartCoroutine(AfterBroken());
        }
    }

    IEnumerator AfterBroken()
    {
        if(isLeftArmBroken&&isRightArmBroken!=true)
        {
            yield return StartCoroutine(MoveOverTime(LeftArm, LeftArm.transform.position, LeftRestPoint.position, armMoveDuration));
            yield return new WaitForSeconds(attackHoldTime);

            yield return StartCoroutine(MoveOverTime(LeftArm, LeftRestPoint.transform.position, LUpPoint.position, armMoveDuration));
            yield return new WaitForSeconds(attackHoldTime);

            yield return StartCoroutine(MoveOverTime(LeftArm, LUpPoint.transform.position, LDownPoint.position, armMoveDuration));
        }

        if(isRightArmBroken&&isLeftArmBroken!=true)
        {
            yield return StartCoroutine(MoveOverTime(RightArm, RightArm.transform.position, RightRestPoint.position, armMoveDuration));
            yield return new WaitForSeconds(attackHoldTime);

            yield return StartCoroutine(MoveOverTime(RightArm, RightRestPoint.transform.position, RUpPoint.position, armMoveDuration));
            yield return new WaitForSeconds(attackHoldTime);

            yield return StartCoroutine(MoveOverTime(RightArm, RUpPoint.transform.position, RDownPoint.position, armMoveDuration));
        }
    }


    IEnumerator FlashRed(SpriteRenderer sr, float duration)
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(duration);
        sr.color = Color.white;
    }

    IEnumerator CreateStone(Transform[] spawnPoints)
    {
        for(int spawnIndex = 0; spawnIndex < spawnPoints.Length; spawnIndex++)
        {
            GameObject stone = Instantiate(Stone, spawnPoints[spawnIndex].position, spawnPoints[spawnIndex].rotation);
            Destroy(stone, 5f);
            yield return null;
        }
    }
}