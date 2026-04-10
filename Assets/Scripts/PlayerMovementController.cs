using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController_4Direction : MonoBehaviour
{
    [Header("Character Stats (Composition)")]
    public CharacterStats stats;

    [Header("References")]
    public Animator playerAnimator;

    private Rigidbody2D playerRigidbody2D;
    private Vector2 movementInputVector;
    private Vector2 lastMovementDirection;
    private bool isDead = false;

    private void Awake()
    {
        // Mengambil komponen Rigidbody2D
        playerRigidbody2D = GetComponent<Rigidbody2D>();

        // Cek Animator
        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();

        // --- TARUH DI SINI ---
        if (stats != null)
        {
            // Reset HP jadi penuh setiap masuk scene agar saat restart darah tidak 0
            stats.currentHealth = stats.maxHealth;
        }
        // ---------------------

        // Setup Physics
        playerRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        playerRigidbody2D.gravityScale = 0f;
        playerRigidbody2D.freezeRotation = true;
    }

    private void Update()
    {
        if (isDead) return;
        HandleInput();
        UpdateAnimatorParameters();
    }

    private void FixedUpdate()
    {
        if (isDead) { playerRigidbody2D.linearVelocity = Vector2.zero; return; }
        MovePlayer();
    }

    private void HandleInput()
    {
        movementInputVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        if (movementInputVector.sqrMagnitude > 0.01f) lastMovementDirection = movementInputVector;
    }

    private void MovePlayer()
    {
        float speed = (stats != null) ? stats.moveSpeed : 3f;
        playerRigidbody2D.linearVelocity = movementInputVector * speed;
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

    // FIX ERROR CS1503: Gunakan FLOAT di sini
    public void TakeDamage(float damageAmount)
    {
        if (isDead || stats == null) return;

        stats.TakeDamage(damageAmount);

        // FIX ERROR CS1061: Pastikan currentHealth ada di CharacterStats
        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        playerAnimator.SetTrigger("Die");

        // Cari objek SceneFader dan jalankan animasinya
        SceneFader fader = FindObjectOfType<SceneFader>();
        if (fader != null)
        {
            // Tunggu sebentar agar animasi mati terlihat, baru fade out
            StartCoroutine(ExecuteDeath(fader));
        }
        else
        {
            // Jika tidak ada fader, langsung restart seperti biasa
            Invoke("RestartScene", 2f);
        }
    }

    System.Collections.IEnumerator ExecuteDeath(SceneFader fader)
    {
        // Tunggu 1.5 detik agar animasi mati player selesai
        yield return new WaitForSeconds(1.5f);

        // Ganti "StartScene" dengan nama Scene Menu Utama kamu di Unity
        // Pastikan tulisannya sama persis (Besar/Kecil hurufnya)
        fader.FadeToScene("Start scene");
    }
}