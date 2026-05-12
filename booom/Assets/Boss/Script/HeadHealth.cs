using UnityEngine;
using System.Collections;

public class HeadHealth : MonoBehaviour, IDamageable
{
    public int maxHealth;
    public int currentHealth { get; private set; }

    [Header("闪红效果")]
    public float flashDuration = 0.1f;

    [Header("爆炸特效")]
    public GameObject explosionVFX;
    public float vfxDuration = 1.5f;

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

    void OnDisable()
    {
        GameObject boom=Instantiate(explosionVFX, transform.position, transform.rotation);
        Destroy(boom, vfxDuration);
    }

    public bool TakeDamage(float damage, Transform damageDealer)
    {
        if (currentHealth <= 0)
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = Color.gray;
            gameObject.SetActive(false);
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