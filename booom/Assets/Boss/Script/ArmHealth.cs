using UnityEngine;
using System.Collections;

public class ArmHealth : MonoBehaviour, IDamageable
{
    public int maxHealth;
    public int currentHealth { get; private set; }

    [Header("ÉÁºìÐ§¹û")]
    public float flashDuration = 0.1f;

    public GameObject ArmCollider;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Coroutine flashCoroutine;

    void Awake()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    private void OnDisable()
    {
        ArmCollider.SetActive(true);
    }



    public bool TakeDamage(float damage, Transform damageDealer)
    {
        if (currentHealth <= 0)
        {
            this.enabled = false;
            return false;
        }


        currentHealth -= Mathf.CeilToInt(damage);
        if (currentHealth < 0) currentHealth = 0;
        FlashRed();
        return true;
    }

    private void FlashRed()
    {
        if (spriteRenderer == null) return;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(DoFlashRed());
    }

    private IEnumerator DoFlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
        if (spriteRenderer != null)
            spriteRenderer.color = originalColor;
    }
}