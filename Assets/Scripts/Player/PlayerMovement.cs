using UnityEngine;
using UnityEngine.InputSystem;   // New Input System

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public bool isMovingRight;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float acceleration = 18f;
    public float deceleration = 28f;
    public float velocityPower = 1f;

    [Header("Jump")]
    public float jumpForce = 11f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;

    [Header("Jump Feel")]
    public float baseGravity = 3.5f;
    public float fallGravityMultiplier = 2.2f;
    public float hangGravityMultiplier = 0.5f;
    public float hangVelocityThreshold = 0.1f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;

    private float inputX;
    private float coyoteCounter;
    private float jumpBufferCounter;
    private float maxFallSpeed;
    private bool isGrounded;
    //private bool wasGrounded;
    private bool facingRight = true;
    private bool isListening;

    [SerializeField] private GameObject listeningUI;
    //[SerializeField] private HiddenPlatform hiddenPlatform;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        //hiddenPlatform = FindAnyObjectByType<HiddenPlatform>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.None; // חשוב לפיקסל־ארט
        rb.gravityScale = baseGravity;
    }

    void Update()
    {
        inputX = moveAction.ReadValue<Vector2>().x;

        if (inputX >= 1)
            isMovingRight = true;
        else isMovingRight = false;

        // Ground check
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        // Stores hardest fall
        if (!isGrounded)
        {
            maxFallSpeed = Mathf.Min(maxFallSpeed, rb.linearVelocity.y);
        }

        /*// Landing detection
        if (!wasGrounded && isGrounded && rb.linearVelocity.y < -2f)
        {
            anim.SetTrigger("Land");
        }*/

        //wasGrounded = isGrounded;

        anim.SetBool("Grounded", isGrounded);
        anim.SetFloat("VerticalSpeed", rb.linearVelocity.y);

        // Coyote time
        if (isGrounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        // Jump buffer
        if (jumpAction.WasPressedThisFrame())
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // Jump
        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpBufferCounter = 0;
            coyoteCounter = 0;
            maxFallSpeed = 0f;

            anim.SetBool("Grounded", false);
            anim.SetBool("HardLanding", false);
        }

        // Hard Landing
        if (isGrounded && maxFallSpeed < -8f)
        {
            anim.SetBool("HardLanding", true);
        }
        else
        {
            anim.SetBool("HardLanding", false);
        }

        // Variable jump height
        if (jumpAction.WasReleasedThisFrame() && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }

        UpdateAnimation();
        HandleFlip();
        ApplyBetterJump();

        // Checks whether the player is listening or not
        bool isListening =
            isGrounded &&
                Mathf.Abs(rb.linearVelocity.x) < 0.1f;

        // Activate Listening UI ICON
        listeningUI.SetActive(isListening);

        //if (hiddenPlatform != null)
            //hiddenPlatform.UpdateListening(isListening);
    }

    void FixedUpdate()
    {
        float targetSpeed = inputX * moveSpeed;
        float speedDif = targetSpeed - rb.linearVelocity.x;

        float accelRate = Mathf.Abs(targetSpeed) > 0.01f ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velocityPower) * Mathf.Sign(speedDif);

        rb.AddForce(movement * Vector2.right);
    }

    void UpdateAnimation()
    {
        anim.SetBool("Grounded", isGrounded);
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetFloat("VerticalSpeed", rb.linearVelocity.y);
    }

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

    void ApplyBetterJump()
    {
        if (rb.linearVelocity.y < -0.01f)
        {
            // Falling – heavier gravity
            rb.gravityScale = baseGravity * fallGravityMultiplier;
        }
        else if (Mathf.Abs(rb.linearVelocity.y) < hangVelocityThreshold)
        {
            // Hang time at jump peak
            rb.gravityScale = baseGravity * hangGravityMultiplier;
        }
        else
        {
            // Rising normally
            rb.gravityScale = baseGravity;
        }
    }

    public bool IsGrounded => isGrounded;

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
