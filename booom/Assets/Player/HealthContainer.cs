using UnityEngine;
using System;

public class HealthContainer : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth { get; private set; }

    public event Action<float> OnHealthChanged;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth <= 0)
            currentHealth = 0;
    }

    public bool IsDead()
    {
        return currentHealth <= 0;
    }
}
