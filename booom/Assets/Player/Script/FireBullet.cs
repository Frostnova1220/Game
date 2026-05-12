using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class FireBullet : MonoBehaviour
{
    public float speed = 15f;
    public float maxLifetime = 3f;
    public float damage = 1f;
    public LayerMask whatIsEnemy;

    private Vector3 lastPosition;


    [Header("命中特效")]
    public GameObject hitSfxPrefab;
    public float hitSfxDuration = 0.5f;

    void Start()
    {
        // 记录初始位置，用于射线检测
        lastPosition = transform.position;
    }

    void Update()
    {
        // 移动子弹
        transform.position += transform.forward * speed * Time.deltaTime;

        // 使用射线自检：检测从上一帧位置到当前帧位置之间，有没有敌人
        Vector3 rayDirection = transform.position - lastPosition;
        float rayDistance = rayDirection.magnitude;

        if (rayDistance > 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(lastPosition, rayDirection.normalized, out hit, rayDistance, whatIsEnemy))
            {
                // 击中了敌人！
                IDamageable d = hit.collider.GetComponent<IDamageable>();
                if (d != null)
                    d.TakeDamage(damage, transform);

                Destroy(gameObject);
                return; // 击中后立即退出，防止后续逻辑报错
            }
        }

        // 更新上一帧位置
        lastPosition = transform.position;

        // 生命周期计算
        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Destroy(gameObject);
    }
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"子弹碰到: {other.name}, Layer: {LayerMask.LayerToName(other.gameObject.layer)}");

        if (((1 << other.gameObject.layer) & whatIsEnemy) != 0)
        {

            IDamageable d = other.GetComponent<IDamageable>();
            if (d == null) d = other.GetComponentInParent<IDamageable>();

            Debug.Log($"找到 IDamageable: {d != null}");

            if (d != null)
            {
                GameObject sfx = Instantiate(hitSfxPrefab, other.transform.position, Quaternion.identity);
                Destroy(sfx, hitSfxDuration);

                bool result = d.TakeDamage(damage, transform);
                Debug.Log($"TakeDamage 返回: {result}");
            }
            Destroy(gameObject);
        }
    }
}