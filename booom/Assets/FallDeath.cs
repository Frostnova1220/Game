using UnityEngine;

public class FallDeath : MonoBehaviour
{
    public float killY = -5f;

    void Update()
    {
        if (transform.position.y < killY)
        {
            IDamageable damageable = GetComponent<IDamageable>();
            Health health = GetComponent<Health>();

            if (health == null)
                health = GetComponentInParent<Health>();

            if (damageable != null && health != null && health.currentHealth > 0)
            {
                damageable.TakeDamage(health.maxHealth, transform);
            }
        }
    }
}