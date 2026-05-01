using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacked : MonoBehaviour, IDamageable
{
    public float health = 10f;

    public void TakeDamage(float damage, Transform damageDealer)
    {
        health -= damage;
        Debug.Log($"被 {damageDealer.name} 打了，扣了 {damage} 血，剩余 {health}");

        if (health <= 0)
        {
            Debug.Log("死了");
            Destroy(gameObject);
        }
    }
}
