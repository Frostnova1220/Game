using System.Collections;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("部位获取")]
    public GameObject LeftArm;
    public GameObject RightArm;

    [Header("攻击点")]
    public Transform LeftRestPoint;      // 左手起始/归位点
    public Transform LeftAttackPoint;    // 左手攻击目标点
    public Transform LeftEndPoint;
    public Transform RightRestPoint;
    public Transform RightAttackPoint;
    public Transform RightEndPoint;

    [Header("时间设置")]
    public float attackCooldown = 10f;    // 攻击冷却
    public float armMoveDuration = 2f; // 手臂移动耗时
    public float attackHoldTime = 0.5f;  // 攻击停留时间

    [Header("攻击设置")]
    private float attackTimer;


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
            attackTimer = Random.Range(5f, 10f); // 重置冷却
        }
    }

    void Attack()
    {
        int attackType = Random.Range(0, 2);
        switch (attackType)
        {
            case 0:
                StartCoroutine(ArmAttackRoutine(LeftArm, LeftRestPoint, LeftAttackPoint, LeftEndPoint));
                break;
            case 1:
                StartCoroutine(ArmAttackRoutine(RightArm, RightRestPoint, RightAttackPoint, RightEndPoint));
                break;
        }
    }

    IEnumerator ArmAttackRoutine(GameObject arm, Transform rest, Transform attackPoint, Transform attackEndPoint)
    {
        yield return StartCoroutine(MoveOverTime(arm, rest.position, attackPoint.position, armMoveDuration));

        yield return new WaitForSeconds(attackHoldTime);

        yield return StartCoroutine(MoveOverTime(arm, attackPoint.position, attackEndPoint.position, armMoveDuration));

        yield return new WaitForSeconds(attackHoldTime);

        yield return StartCoroutine(MoveOverTime(arm, attackEndPoint.position, rest.position, armMoveDuration));
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
            yield return null;
        }
        obj.transform.position = end;
    }
}