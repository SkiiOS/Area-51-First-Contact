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
    public BossHealthUI bossUI;

    [Header("Animations")]
    public Animator anim; // Tarik komponen Animator ke sini di Inspector
    public string attackTriggerName = "Attack"; // Nama parameter Trigger di Animator
    public string moveBoolName = "isMoving";    // Nama parameter Bool di Animator (opsional)

    private Transform player;
    private float nextAttackTime;

    void Start()
    {
        currentHealth = maxHealth;

        // Mencari Animator otomatis jika lupa narik di Inspector
        if (anim == null) anim = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

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

            // Set animasi jalan jika ada
            if (anim != null) anim.SetBool(moveBoolName, true);
        }
        else
        {
            // Berhenti jalan saat sudah di jarak serang
            if (anim != null) anim.SetBool(moveBoolName, false);

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void Attack()
    {
        // 1. Jalankan Animasi Serang
        if (anim != null)
        {
            anim.SetTrigger(attackTriggerName);
        }

        // 2. Beri Damage ke Player
        var pScript = player.GetComponent<PlayerMoveControl>();
        if (pScript != null)
        {
            pScript.TakeDamage(attackDamage);
            Debug.Log("Musuh Menyerang dengan Animasi!");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Opsional: Tambahkan anim.SetTrigger("Hurt") di sini jika ada animasi kena pukul

        if (bossUI != null)
        {
            bossUI.UpdateBossHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    [Header("Scene Transition")]
    public string nextSceneName;

    void Die()
    {
        if (bossUI != null) bossUI.SetActive(false);

        SceneFader fader = Object.FindFirstObjectByType<SceneFader>();
        if (fader != null)
        {
            fader.FadeToScene(nextSceneName);
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        }

        Destroy(gameObject);
    }
}