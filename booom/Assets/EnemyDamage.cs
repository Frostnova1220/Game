using System.Collections;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float damage = 1f;
    public float cooldown = 1f;
    public float knockbackX = 3f;
    public float knockbackY = 1f;
    public GameObject hitSFX;

    private bool canDamage = true;

    void OnTriggerEnter(Collider other)
    {
        if (!canDamage) return;

        if (other.CompareTag("Player"))
        {
            Health targetHealth = other.GetComponent<Health>();
            if (targetHealth == null)
                targetHealth = other.GetComponentInParent<Health>();

            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage, transform);
            }

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb == null)
                rb = other.GetComponentInParent<Rigidbody>();

            if (rb != null)
            {
                Vector3 knockDir = (other.transform.position - transform.position).normalized;
                rb.velocity = new Vector3(knockDir.x * knockbackX, knockbackY, 0);
            }

            if (hitSFX != null)
            {
                GameObject sfx = Instantiate(hitSFX, other.transform.position, Quaternion.identity);
                Destroy(sfx, 5f);
            }

            StartCoroutine(DamageCooldown());
        }
    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(cooldown);
        canDamage = true;
    }
}