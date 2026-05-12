using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingBullet : MonoBehaviour
{
    public float speed = 15f;
    public float turnRate = 15f;
    public float maxLifetime = 3f;
    public float damage = 1f;
    public LayerMask whatIsEnemy;
    public float hitRadius = 0.8f;

    private Transform target;
    private bool hasTarget;

    public void SetTarget(Transform enemy)
    {
        target = enemy;
        hasTarget = enemy != null;
    }

    void Update()
    {
        if (hasTarget && target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist <= hitRadius)
            {
                HitTarget(target);
                return;
            }

            Vector3 toTarget = target.position - transform.position;
            transform.forward = Vector3.Slerp(transform.forward, toTarget.normalized, turnRate * Time.deltaTime);
        }

        transform.position += transform.forward * speed * Time.deltaTime;

        maxLifetime -= Time.deltaTime;
        if (maxLifetime <= 0) Destroy(gameObject);
    }

    void HitTarget(Transform enemy)
    {
        IDamageable d = enemy.GetComponent<IDamageable>();
        if (d != null)
            d.TakeDamage(damage, transform);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if ((whatIsEnemy.value & (1 << other.gameObject.layer)) != 0)
        {
            IDamageable d = other.GetComponent<IDamageable>();
            if (d != null)
                d.TakeDamage(damage, transform);
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}