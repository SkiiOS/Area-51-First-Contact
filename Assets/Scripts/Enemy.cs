using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement & Attack")]
    public float moveSpeed = 2f;
    public float attackDamage = 10f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;

    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    public BossHealthUI bossUI; // Tarik UI Boss ke sini jika ini Boss

    private Transform player;
    private float nextAttackTime;

    void Start()
    {
        currentHealth = maxHealth;

        // Mencari objek dengan Tag "Player" secara otomatis
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Jika ini Boss dan ada UI-nya, tampilkan
        if (bossUI != null)
        {
            bossUI.SetActive(true);
            bossUI.UpdateBossHealth(currentHealth, maxHealth);
        }
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > attackRange)
        {
            // Bergerak mendekati player
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        }
        else if (Time.time >= nextAttackTime)
        {
            // Menyerang jika sudah dekat
            Attack();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void Attack()
    {
        var pScript = player.GetComponent<PlayerMovementController_4Direction>();
        if (pScript != null)
        {
            pScript.TakeDamage(attackDamage);
            Debug.Log("Musuh Menyerang!");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (bossUI != null)
        {
            bossUI.UpdateBossHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Tambahkan di dalam script EnemyAI.cs

    public string nextSceneName; // Ketik nama scene tujuan di Inspector (misal: "WinScene")

    void Die()
    {
        if (bossUI != null) bossUI.SetActive(false);

        // Mencari SceneFader di dalam map dan menjalankan transisi
        SceneFader fader = Object.FindFirstObjectByType<SceneFader>();
        if (fader != null)
        {
            fader.FadeToScene(nextSceneName);
        }
        else
        {
            // Jika fader tidak ditemukan, langsung pindah (backup)
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }

        Destroy(gameObject);
    }
}