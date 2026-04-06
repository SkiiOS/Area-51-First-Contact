using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Animator (Optional)")]
    public Animator animator;

    // Components
    private Rigidbody2D rb;
    private Vector2 moveInput;

    // States
    private bool isGrounded;
    private bool isFacingRight = true;
    public static bool canMove = true;

    // Animation parameters
    private static readonly int HorizontalHash = Animator.StringToHash("Horizontal");
    private static readonly int VerticalHash = Animator.StringToHash("Vertical");
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!canMove)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        // Ground check
        CheckGrounded();

        // Handle input
        HandleMovement();

        // Update animator
        UpdateAnimator();
    }

    // Called by Unity Input System
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    // Called by Unity Input System
    public void OnJump(InputValue value)
    {
        if (isGrounded && canMove)
        {
            Jump();
        }
    }

    void HandleMovement()
    {
        // Horizontal movement
        float horizontalInput = moveInput.x;

        // Flip character
        if (horizontalInput > 0 && !isFacingRight)
            Flip();
        else if (horizontalInput < 0 && isFacingRight)
            Flip();

        // Apply velocity
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;

        // Trigger jump animation if exists
        if (animator != null)
            animator.SetTrigger("Jump");
    }

    void CheckGrounded()
    {
        if (groundCheck != null)
        {
            // Check using overlap circle
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            // Fallback: Raycast down
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundLayer);
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        float horizontal = rb.linearVelocity.x;
        float vertical = rb.linearVelocity.y;
        float speed = new Vector2(horizontal, vertical).magnitude;

        // Check apakah parameter ada sebelum set (menghindari error)
        if (HasParameter("Horizontal"))
            animator.SetFloat(HorizontalHash, horizontal);

        if (HasParameter("Vertical"))
            animator.SetFloat(VerticalHash, vertical);

        if (HasParameter("Speed"))
            animator.SetFloat(SpeedHash, speed);

        if (HasParameter("IsGrounded"))
            animator.SetBool(IsGroundedHash, isGrounded);
    }

    bool HasParameter(string paramName)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
    }

    // Alternative: Old input system (keyboard only)
    void HandleOldInput()
    {
        if (!canMove) return;

        float horizontal = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            horizontal = -1f;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            horizontal = 1f;

        moveInput = new Vector2(horizontal, 0);

        // Jump with space
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();
    }

    void OnDrawGizmosSelected()
    {
        // Draw ground check in editor
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
