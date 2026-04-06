using UnityEngine;

public class PlayerControllerV2 : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    // Input
    private Vector2 movementInput;
    private float lastHorizontal;
    private float lastVertical;
    private bool isMoving;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;

        // Default menghadap bawah
        lastHorizontal = 0;
        lastVertical = -1;
    }

    void Update()
    {
        // Get input
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Deadzone
        if (Mathf.Abs(h) < 0.1f) h = 0;
        if (Mathf.Abs(v) < 0.1f) v = 0;

        movementInput = new Vector2(h, v);

        // Normalize diagonal
        if (movementInput.magnitude > 1)
            movementInput.Normalize();

        // Update last direction untuk idle
        if (movementInput.x != 0 || movementInput.y != 0)
        {
            isMoving = true;
            // Simpan arah hanya saat benar-benar bergerak
            lastHorizontal = movementInput.x;
            lastVertical = movementInput.y;
        }
        else
        {
            isMoving = false;
        }

        UpdateAnimation();
        HandleFlip();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movementInput * moveSpeed;
    }

    void UpdateAnimation()
    {
        if (animator == null) return;

        // Parameter untuk Blend Tree
        animator.SetFloat("MoveX", movementInput.x);
        animator.SetFloat("MoveY", movementInput.y);

        // Parameter untuk idle direction (selalu update)
        animator.SetFloat("LastMoveX", lastHorizontal);
        animator.SetFloat("LastMoveY", lastVertical);

        // Optional: parameter tambahan
        animator.SetBool("IsMoving", isMoving);
        animator.SetFloat("Speed", movementInput.magnitude);
    }

    void HandleFlip()
    {
        if (spriteRenderer == null) return;

        // CARA BENER: Cek arah sprite asli
        // Kalau sprite asli menghadap KANAN (standard):
        // - Jalan ke kanan: flipX = false
        // - Jalan ke kiri: flipX = true

        // Kalau sprite asli menghadap KIRI:
        // - Jalan ke kanan: flipX = true
        // - Jalan ke kiri: flipX = false

        // Default: sprite menghadap kanan
        if (movementInput.x > 0.1f)
        {
            spriteRenderer.flipX = false;  // Kanan
        }
        else if (movementInput.x < -0.1f)
        {
            spriteRenderer.flipX = true;   // Kiri
        }
        // Kalau x = 0, jangan ubah flip (biarkan sesuai arah terakhir)
    }

    // DEBUG: Lihat nilai di Inspector saat runtime
    void OnGUI()
    {
        GUILayout.Label($"Input: {movementInput}");
        GUILayout.Label($"Last: ({lastHorizontal}, {lastVertical})");
        GUILayout.Label($"IsMoving: {isMoving}");
        GUILayout.Label($"FlipX: {spriteRenderer?.flipX}");
    }
}
