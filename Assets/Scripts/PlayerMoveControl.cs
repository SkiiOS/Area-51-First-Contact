using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(AudioSource))] // Menambahkan komponen AudioSource secara otomatis
public class PlayerMoveControl : MonoBehaviour
{
    [Header("Character Stats (Composition)")]
    public CharacterStats stats;

    [Header("References")]
    public Animator playerAnimator;

    [Header("Audio Settings")]
    public AudioSource footstepAudioSource; // Slot untuk AudioSource
    public AudioClip footstepClip;         // Slot untuk file sound effect jalan

    private Rigidbody2D playerRigidbody2D;
    private Vector2 movementInputVector;
    private Vector2 lastMovementDirection;
    private bool isDead = false;

    private void Awake()
    {
        playerRigidbody2D = GetComponent<Rigidbody2D>();

        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();

        // Setup AudioSource otomatis jika belum diisi di Inspector
        if (footstepAudioSource == null)
            footstepAudioSource = GetComponent<AudioSource>();

        if (stats != null)
        {
            stats.currentHealth = stats.maxHealth;
        }

        playerRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        playerRigidbody2D.gravityScale = 0f;
        playerRigidbody2D.freezeRotation = true;
    }

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;
    private Vector2 lastDir = Vector2.right;

    private void Update()
    {
        if (isDead) return;

        HandleInput();
        UpdateAnimatorParameters();
        HandleFootstepSound(); // Panggil fungsi suara langkah kaki

        // Logika menembak
        if (Input.GetKeyDown(KeyCode.Space) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void FixedUpdate()
    {
        if (isDead) { playerRigidbody2D.linearVelocity = Vector2.zero; return; }
        MovePlayer();
    }

    private void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // LOGIKA ANTI-DIAGONAL: Prioritaskan input horizontal jika keduanya ditekan
        // Jika sedang menekan tombol kiri/kanan, maka input atas/bawah diabaikan (set ke 0)
        if (h != 0)
        {
            v = 0;
        }

        movementInputVector = new Vector2(h, v).normalized;

        if (movementInputVector.sqrMagnitude > 0.01f)
        {
            lastMovementDirection = movementInputVector;
            lastDir = movementInputVector; // Update arah tembakan juga di sini agar selaras
        }
    }

    private void MovePlayer()
    {
        float speed = (stats != null) ? stats.moveSpeed : 3f;
        playerRigidbody2D.linearVelocity = movementInputVector * speed;
    }

    private void HandleFootstepSound()
    {
        // Jika sedang bergerak dan suara belum bunyi, maka mainkan
        if (movementInputVector.sqrMagnitude > 0.01f)
        {
            if (!footstepAudioSource.isPlaying)
            {
                footstepAudioSource.clip = footstepClip;
                footstepAudioSource.loop = true; // Agar suaranya terus berulang saat jalan
                footstepAudioSource.Play();
            }
        }
        else
        {
            // Jika berhenti bergerak, hentikan suara
            if (footstepAudioSource.isPlaying)
            {
                footstepAudioSource.Stop();
            }
        }
    }

    private void UpdateAnimatorParameters()
    {
        float currentSpeed = movementInputVector.sqrMagnitude;
        playerAnimator.SetFloat("MoveX", movementInputVector.x);
        playerAnimator.SetFloat("MoveY", movementInputVector.y);
        playerAnimator.SetFloat("Speed", currentSpeed);

        if (currentSpeed > 0.01f)
        {
            playerAnimator.SetFloat("LastMoveX", movementInputVector.x);
            playerAnimator.SetFloat("LastMoveY", movementInputVector.y);
        }
        else
        {
            playerAnimator.SetFloat("LastMoveX", lastMovementDirection.x);
            playerAnimator.SetFloat("LastMoveY", lastMovementDirection.y);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead || stats == null) return;
        stats.TakeDamage(damageAmount);
        if (stats.currentHealth <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        footstepAudioSource.Stop(); // Matikan suara langkah jika mati
        playerAnimator.SetTrigger("Die");
        SceneFader fader = FindObjectOfType<SceneFader>();
        if (fader != null) StartCoroutine(ExecuteDeath(fader));
        else Invoke("RestartScene", 2f);
    }

    System.Collections.IEnumerator ExecuteDeath(SceneFader fader)
    {
        yield return new WaitForSeconds(1.5f);
        fader.FadeToScene("Start scene");
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            float angle = Mathf.Atan2(lastDir.y, lastDir.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}