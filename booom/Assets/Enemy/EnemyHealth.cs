using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public int maxHealth;
    public int currentHealth { get; private set; }

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public bool TakeDamage(float damage, Transform damageDealer)
    {
        if (currentHealth <= 0) return false;

        currentHealth -= Mathf.CeilToInt(damage);
        if (currentHealth < 0) currentHealth = 0;

        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
            currentHealth = maxHealth;
        }


        return true;
    }
}