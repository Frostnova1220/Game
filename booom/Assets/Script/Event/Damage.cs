using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Damage : MonoBehaviour
{
    [Header("目标玩家")]
    public Health playerHealth;  // 直接拖入玩家的 Health 组件

    [Header("特效")]
    public GameObject sfx;
    public SpriteRenderer characterRenderer;

    public UnityEvent onTriggerEnter;
    public float cooldown;
    public float backDistance;
    public float upDistance;
    private bool canDamage = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!canDamage) return;
        if (other.CompareTag("Player"))
        {
            Debug.Log($"触发伤害，碰到: {other.name}");

            characterRenderer = other.GetComponentInChildren<SpriteRenderer>();

            // 直接找：先自身，再父对象
            Health targetHealth = other.GetComponent<Health>();
            if (targetHealth == null)
                targetHealth = other.GetComponentInParent<Health>();

            if (targetHealth != null)
            {
                targetHealth.TakeDamage(1f, transform);
                Debug.Log($"直接调用成功，剩余血量: {targetHealth.currentHealth}");
            }
            else
            {
                Debug.LogError("直接调用也找不到 Health！");
            }

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb == null)
                rb = other.GetComponentInParent<Rigidbody>();

            if (rb != null)
                rb.velocity = new Vector3(backDistance, upDistance, 0);

            PlayBulletSfx(other.transform.position);
            StartCoroutine(DamageCooldown());
        }
    }

    public void PlayBulletSfx(Vector3 position)
    {
        GameObject sfx1 = Instantiate(sfx, position, Quaternion.identity);
        Destroy(sfx1, 5f);
    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;
        if (characterRenderer != null)
            characterRenderer.color = Color.red;

        yield return new WaitForSeconds(cooldown);

        if (characterRenderer != null)
            characterRenderer.color = Color.white;
        canDamage = true;
    }
}