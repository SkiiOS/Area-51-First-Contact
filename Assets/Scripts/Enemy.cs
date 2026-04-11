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
    public Animator anim;
    public string attackTriggerName = "Attack";
    public string moveBoolName = "isMoving";

    [Header("Rotation Settings")]
    public float rotationOffset = 0f;

    [Header("Audio Settings")]
    public AudioSource audioSource; // Slot untuk komponen AudioSource
    public AudioClip attackSound;   // Slot untuk file sound effect menyerang

    private Transform player;
    private float nextAttackTime;

    void Start()
    {
        currentHealth = maxHealth;
        if (anim == null) anim = GetComponent<Animator>();

        // Setup AudioSource otomatis jika lupa ditarik di Inspector
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;

        if (bossUI != null)
        {
            bossUI.SetActive(true);
            bossUI.UpdateBossHealth(currentHealth, maxHealth);
        }
    }

    void Update()
    {
        if (player == null) return;

        RotateTowardsPlayer();

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist > attackRange)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            if (anim != null) anim.SetBool(moveBoolName, true);
        }
        else
        {
            if (anim != null) anim.SetBool(moveBoolName, false);

            if (Time.time >= nextAttackTime)
            {
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
        }
    }

    void RotateTowardsPlayer()
    {
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }

    void Attack()
    {
        // 1. Jalankan Animasi
        if (anim != null) anim.SetTrigger(attackTriggerName);

        // 2. Jalankan Suara Menyerang
        PlayAttackSound();

        // 3. Beri Damage
        PlayerMovementController pScript = player.GetComponent<PlayerMovementController>();
        if (pScript != null)
        {
            pScript.TakeDamage(attackDamage);
            Debug.Log("Berhasil menyerang!");
        }
    }

    // Fungsi internal untuk memutar suara
    void PlayAttackSound()
    {
        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (bossUI != null) bossUI.UpdateBossHealth(currentHealth, maxHealth);
        if (currentHealth <= 0) Die();
    }

    [Header("Scene Transition")]
    public string nextSceneName;

    void Die()
    {
        if (bossUI != null) bossUI.SetActive(false);
        SceneFader fader = Object.FindFirstObjectByType<SceneFader>();
        if (fader != null) fader.FadeToScene(nextSceneName);
        else UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
        Destroy(gameObject);
    }
}