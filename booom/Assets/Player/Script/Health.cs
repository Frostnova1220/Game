using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [Header("血量设置")]
    public int maxHealth = 5;
    public int currentHealth { get; private set; }

    [Header("UI/音效（玩家用）")]
    public GameObject[] healthUI;
    public GameObject deathUI;
    public AudioController audioController;

    public System.Action<int> onHealthChanged;
    public System.Action<GameObject> onDeath;  // 传死亡对象

    void Awake()
    {
        currentHealth = maxHealth;
    }

    void Start()
    {
        DetectUI();
    }

    public bool TakeDamage(float damage, Transform damageDealer)
    {
        if (currentHealth <= 0) return false;

        currentHealth -= Mathf.CeilToInt(damage);
        if (currentHealth < 0) currentHealth = 0;

        onHealthChanged?.Invoke(currentHealth);
        DetectUI();

        if (currentHealth <= 0)
            Die();

        return true;
    }

    void DetectUI()
    {
        if (healthUI.Length >= 1) healthUI[0]?.SetActive(currentHealth >= 1);
        if (healthUI.Length >= 2) healthUI[1]?.SetActive(currentHealth >= 2);
        if (healthUI.Length >= 3) healthUI[2]?.SetActive(currentHealth >= 3);
        if (healthUI.Length >= 4) healthUI[3]?.SetActive(currentHealth >= 4);
        if (healthUI.Length >= 5) healthUI[4]?.SetActive(currentHealth >= 5);
    }

    void Die()
    {
        if (audioController != null)
            audioController.PlaySfx(audioController.Dead);

        onDeath?.Invoke(gameObject);

        if (deathUI != null)
            deathUI.SetActive(true);

        gameObject.SetActive(false);
    }
}