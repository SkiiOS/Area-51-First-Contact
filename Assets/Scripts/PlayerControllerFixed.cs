using UnityEngine;

public class PlayerControllerFixed : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float inputThreshold = 0.1f;  // Threshold untuk input (mencegah bleeding)

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    // Input
    private Vector2 movementInput;
    private Vector2 lastNonZeroInput;

    // Animation parameters
    private static readonly int MoveXHash = Animator.StringToHash("MoveX");
    private static readonly int MoveYHash = Animator.StringToHash("MoveY");
    private static readonly int IsMovingHash = Animator.StringToHash("IsMoving");
    private static readonly int LastMoveXHash = Animator.StringToHash("LastMoveX");
    private static readonly int LastMoveYHash = Animator.StringToHash("LastMoveY");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        // Pastikan Rigidbody2D setup benar
        if (rb != null)
        {
            rb.gravityScale = 0;  // Top-down, tidak perlu gravity
            rb.freezeRotation = true;  // Jangan rotasi
        }
    }

    void Update()
    {
        HandleInput();
    }

    void FixedUpdate()
    {
        MoveCharacter();
    }

    void LateUpdate()
    {
        // Update animator di LateUpdate untuk menghindari race condition
        UpdateAnimator();
    }

    void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Apply threshold untuk mencegah input bleeding
        if (Mathf.Abs(horizontal) < inputThreshold) horizontal = 0;
        if (Mathf.Abs(vertical) < inputThreshold) vertical = 0;

        movementInput = new Vector2(horizontal, vertical);

        // Normalize hanya jika magnitude > 1 (cegah diagonal lebih cepat)
        if (movementInput.magnitude > 1)
        {
            movementInput.Normalize();
        }

        // Simpan last direction untuk idle
        if (movementInput.sqrMagnitude > 0.01f)
        {
            lastNonZeroInput = movementInput;
        }

    }

    void MoveCharacter()
    {
        rb.linearVelocity = movementInput * moveSpeed;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        bool isMoving = movementInput.sqrMagnitude > 0.01f;

        // Set movement parameters
        animator.SetFloat(MoveXHash, movementInput.x);
        animator.SetFloat(MoveYHash, movementInput.y);
        animator.SetBool(IsMovingHash, isMoving);

        // Set last direction untuk idle
        if (isMoving)
        {
            animator.SetFloat(LastMoveXHash, lastNonZeroInput.x);
            animator.SetFloat(LastMoveYHash, lastNonZeroInput.y);
        }

        // Handle flip (horizontal only) menggunakan localScale agar semua child ikut berbalik
        HandleFlipAlternative();
    }

    void HandleFlip()
    {
        if (spriteRenderer == null) return;

        // Gunakan flipX daripada scale (lebih stabil)
        if (movementInput.x > inputThreshold)
        {
            spriteRenderer.flipX = false;  // Kanan
        }
        else if (movementInput.x < -inputThreshold)
        {
            spriteRenderer.flipX = true;   // Kiri
        }
        // Jika x = 0, jangan ubah flip (pertahankan arah terakhir)
    }

    // Alternative: Gunakan transform.localScale dengan pivot center
    void HandleFlipAlternative()
    {
        if (movementInput.x > inputThreshold && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (movementInput.x < -inputThreshold && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void OnValidate()
    {
        // Pastikan threshold tidak terlalu tinggi
        if (inputThreshold > 0.5f) inputThreshold = 0.5f;
    }
}
