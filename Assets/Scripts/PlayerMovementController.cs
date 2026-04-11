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
        // Ambil input langsung di sini agar pasti ada nilainya
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Catat arah terakhir selama ada tombol yang ditekan
        if (h != 0 || v != 0)
        {
            lastDir = new Vector2(h, v);
        }

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
    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // Menghitung sudut dalam derajat berdasarkan arah terakhir (lastDir)
            // Atan2 mengembalikan radian, lalu kita ubah ke derajat (Rad2Deg)
            float angle = Mathf.Atan2(lastDir.y, lastDir.x) * Mathf.Rad2Deg;

            // Terapkan rotasi ke firePoint berdasarkan sudut tersebut
            firePoint.rotation = Quaternion.Euler(0, 0, angle);

            // Munculkan peluru
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}