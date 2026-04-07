using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController_4Direction : MonoBehaviour
{
    [Header("Character Stats (Composition)")]
    [Tooltip("Referensi ke CharacterStats agar moveSpeed dan health diatur dari sana.")]
    public CharacterStats stats;

    [Header("References")]
    [Tooltip("Animator dari Player.")]
    public Animator playerAnimator;

    private Rigidbody2D playerRigidbody2D;
    private Vector2 movementInputVector;
    private Vector2 lastMovementDirection;

    // Parameter untuk Blend Tree Animator
    private readonly string moveXParameter = "MoveX";
    private readonly string moveYParameter = "MoveY";
    private readonly string speedParameter = "Speed";
    private readonly string lastMoveXParameter = "LastMoveX";
    private readonly string lastMoveYParameter = "LastMoveY";

    private void Awake()
    {
        playerRigidbody2D = GetComponent<Rigidbody2D>();

        if (playerAnimator == null)
            playerAnimator = GetComponent<Animator>();

        if (stats == null)
            Debug.LogWarning("CharacterStats belum di-assign di Inspector!");

        playerRigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }

    private void Update()
    {
        HandleInput();
        UpdateAnimatorParameters();

        // Debug log only when moving to avoid spamming the console
        if (movementInputVector.sqrMagnitude > 0.01f)
        {
            float effectiveSpeed = (stats != null) ? stats.moveSpeed : 3f;
            Debug.Log($"Player moving: {movementInputVector} | Speed: {effectiveSpeed} | SqrMag: {movementInputVector.sqrMagnitude}");
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void HandleInput()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        movementInputVector = new Vector2(horizontalInput, verticalInput);

        if (movementInputVector.sqrMagnitude > 0.01f)
        {
            lastMovementDirection = movementInputVector.normalized;
        }

        movementInputVector = movementInputVector.normalized;
    }

    private void MovePlayer()
    {
        // Gunakan kecepatan dari CharacterStats
        float speed = (stats != null) ? stats.moveSpeed : 3f;
        Vector2 newPosition = (Vector2)transform.position + (movementInputVector * speed * Time.fixedDeltaTime);
        playerRigidbody2D.MovePosition(newPosition);
    }

    private void UpdateAnimatorParameters()
    {
        float currentSpeed = movementInputVector.sqrMagnitude;

        playerAnimator.SetFloat(moveXParameter, movementInputVector.x);
        playerAnimator.SetFloat(moveYParameter, movementInputVector.y);
        playerAnimator.SetFloat(speedParameter, currentSpeed);

        if (currentSpeed > 0.01f)
        {
            playerAnimator.SetFloat(lastMoveXParameter, movementInputVector.x);
            playerAnimator.SetFloat(lastMoveYParameter, movementInputVector.y);
        }
        else
        {
            playerAnimator.SetFloat(lastMoveXParameter, lastMovementDirection.x);
            playerAnimator.SetFloat(lastMoveYParameter, lastMovementDirection.y);
        }
    }
}
