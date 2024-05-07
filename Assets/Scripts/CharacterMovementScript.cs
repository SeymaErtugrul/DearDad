using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class CharacterMovementScript : MonoBehaviour
{
    public float moveSpeed = 7f;
    private Vector2 moveInput;
    private Vector2 jumpInput;
    private Rigidbody2D rb;

    public Animator animator;

    private PlayerInput playerInput; // PlayerInput component reference
    public bool isFacingRight = true;

    public float jumpPower = 5f;

    private bool isGrounded; // Karakterin yere basıp basmadığını kontrol et

    public Transform groundCheck; // Yere temas kontrol noktası
    public float groundCheckRadius = 0.2f; // Yere temas kontrol çapı
    public LayerMask groundLayer; // Yere temas için katman maskesi

    public bool isReadyToJump = false;

    public float jumpCooldown = 1f; // Zıplama cooldown süresi (saniye olarak)
    private float lastJumpTime; // Son zıplama zamanını sakla

    private bool isAscending = false;

    private float runningThreshold = 0.1f;
    private float lastMoveTime;

    bool isMoving = false;

    private bool canProcessInput = false;

    public Transform groundCheckLeft;  // Karakterin sol tarafı için yere temas kontrol noktası
    public Transform groundCheckRight;

    public bool groundedRight;


    public bool groundedLeft;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        lastJumpTime = -jumpCooldown;
        animator.SetBool("isRunning", false);
        StartCoroutine(EnableInputProcessingAfterDelay(0.1f));
    }

    IEnumerator EnableInputProcessingAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canProcessInput = true;
    }
    private void Update()
    {
        if (!canProcessInput)
        {
            return; 

        }
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        jumpInput = playerInput.actions["Jump"].ReadValue<Vector2>();

        if (Time.time - lastJumpTime >= jumpCooldown && isGrounded)
        {
            isReadyToJump = true;
        }

        isMoving = Mathf.Abs(moveInput.x) > 0.01f; 

        if (isMoving)
        {

            lastMoveTime = Time.time; // Karakter hareket ediyorsa, son hareket zamanını güncelle
        }


        if (Time.time - lastMoveTime < runningThreshold)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void FixedUpdate()
    {
        Vector2 movement = moveInput * moveSpeed;
        rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        Debug.Log(jumpInput);
        Debug.Log("rb velocity"+rb.velocity);
       groundedLeft = Physics2D.OverlapCircle(groundCheckLeft.position, groundCheckRadius, groundLayer);
       groundedRight = Physics2D.OverlapCircle(groundCheckRight.position, groundCheckRadius, groundLayer);
        isGrounded= Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (moveInput == Vector2.zero || rb.velocity.magnitude < 0.01f)
        {
            animator.SetBool("isRunning", false);
        }
        else
        {
            animator.SetBool("isRunning", true);
        }

        if (moveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            Flip();
        }

        if (isGrounded && Mathf.Abs(rb.velocity.y) < 0.01f)
        {

            animator.SetBool("isJumping", false);
            animator.SetBool("isGrounded", true);
            animator.SetBool("isTop", false);
            rb.gravityScale = 1f;
            moveSpeed = 19;
            int sideJumpLayerIndex = animator.GetLayerIndex("SideJump");
            animator.SetLayerWeight(sideJumpLayerIndex, 0f);
        }
        if (rb.velocity.y > 0)
        {
            isAscending = true;
            animator.SetBool("isGrounded", false);
            animator.SetBool("jumpStarted", true);
        }

         if (isAscending && rb.velocity.y <= 20)
        {
            Debug.Log("Zirveye ulaşıldı.");
            isAscending = false; 
            animator.SetBool("isGrounded", false);
            animator.SetBool("isTop", true);

        }

        if (!isGrounded && rb.velocity.y < 0)
        {
            rb.gravityScale = 10f; 
        }

        animator.SetFloat("speed", Mathf.Abs(rb.velocity.x));

    
    }

    private void Flip()
    {
        Debug.Log("flip");
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;


    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && isReadyToJump && !animator.GetBool("isJumping"))
        {
            animator.SetBool("isJumping", true);
            rb.AddForce(new Vector2(0f, jumpPower), ForceMode2D.Impulse);


            if (isMoving)
            {
                int sideJumpLayerIndex = animator.GetLayerIndex("SideJump");
                animator.SetLayerWeight(sideJumpLayerIndex, 1f);
            }


            isReadyToJump = false;
            lastJumpTime = Time.time;

            rb.gravityScale = 3f;
            moveSpeed = 8;
        }
    }
}
