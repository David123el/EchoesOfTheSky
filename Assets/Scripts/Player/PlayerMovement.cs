using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float acceleration = 18f;
    [SerializeField] private float deceleration = 28f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 11f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    [Header("Gravity Feel")]
    [SerializeField] private float baseGravity = 3.5f;
    [SerializeField] private float fallGravityMultiplier = 2.2f;
    [SerializeField] private float hangGravityMultiplier = 0.5f;
    [SerializeField] private float hangVelocityThreshold = 0.1f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Listening")]
    [SerializeField] private GameObject listeningUI;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerInput input;

    private InputAction moveAction;
    private InputAction jumpAction;

    private float inputX;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float maxFallSpeed;

    private bool isGrounded;
    private bool facingRight = true;

    public float VelocityX => rb.linearVelocity.x;

    public bool IsMovingRight { get; private set; }
    public bool IsGrounded => isGrounded;

    public bool IsListening =>
        isGrounded && Mathf.Abs(rb.linearVelocity.x) < 0.1f;

    private ListeningZone currentListeningZone;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out ListeningZone zone))
            currentListeningZone = zone;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out ListeningZone zone) &&
            currentListeningZone == zone)
        {
            currentListeningZone = null;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        input = GetComponent<PlayerInput>();

        moveAction = input.actions["Move"];
        jumpAction = input.actions["Jump"];

        rb.freezeRotation = true;
        rb.gravityScale = baseGravity;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        inputX = moveAction.ReadValue<Vector2>().x;

        if (Mathf.Abs(inputX) > 0.01f)
        {
            IsMovingRight = inputX > 0f;
        }

        bool canListen =
    isGrounded &&
    Mathf.Abs(rb.linearVelocity.x) < 0.05f &&
    currentListeningZone != null;

        ListeningManager.Instance?.SetCanListen(canListen);

        // UI
        listeningUI.SetActive(canListen);

        CheckGround();
        HandleJumpBuffer();
        HandleJump();
        HandleGravity();
        HandleFlip();
        UpdateAnimation();

        listeningUI.SetActive(IsListening);
    }

    void FixedUpdate()
    {
        float targetSpeed = inputX * moveSpeed;
        float speedDiff = targetSpeed - rb.linearVelocity.x;

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float movement = speedDiff * accelRate;

        rb.AddForce(Vector2.right * movement);
    }

    // =====================
    // Ground & Jump
    // =====================

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
            maxFallSpeed = 0f;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
            maxFallSpeed = Mathf.Min(maxFallSpeed, rb.linearVelocity.y);
        }
    }

    void HandleJumpBuffer()
    {
        if (jumpAction.WasPressedThisFrame())
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;
    }

    void HandleJump()
    {
        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0;
            coyoteCounter = 0;
        }

        if (jumpAction.WasReleasedThisFrame() && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * 0.5f
            );
        }
    }

    // =====================
    // Gravity Feel
    // =====================

    void HandleGravity()
    {
        if (!isGrounded && rb.linearVelocity.y <= 0f)
        {
            rb.gravityScale = baseGravity * fallGravityMultiplier;
        }
        else if (Mathf.Abs(rb.linearVelocity.y) < hangVelocityThreshold)
        {
            rb.gravityScale = baseGravity * hangGravityMultiplier;
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    // =====================
    // Visuals
    // =====================

    void HandleFlip()
    {
        if (inputX > 0.01f && !facingRight)
            Flip();
        else if (inputX < -0.01f && facingRight)
            Flip();
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void UpdateAnimation()
    {
        bool moveIntent = Mathf.Abs(inputX) > 0.01f;

        anim.SetBool("Move", moveIntent);
        anim.SetBool("IsListening", !moveIntent && ListeningManager.Instance.IsListening);
        anim.SetBool("Grounded", isGrounded);
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
