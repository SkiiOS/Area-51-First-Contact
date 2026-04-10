using UnityEngine;

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
        playerRigidbody2D = GetComponent<Rigidbody2D>();
        if (playerAnimator == null) playerAnimator = GetComponent<Animator>();

        // Inisialisasi darah saat mulai
        if (stats != null) stats.currentHealth = stats.maxHealth;

        playerRigidbody2D.gravityScale = 0;
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
        GetComponent<Collider2D>().enabled = false;
        Debug.Log("Player Mati!");
    }
}