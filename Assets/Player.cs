using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    
    // code taken from here: 
    //

    
    private UnityEvent exampleEvent;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI healthText;
    
    public int score = 0;
    
    // HP
    [Header("Health & DMG")] 
    [SerializeField] public int maxHealth = 5;
    [SerializeField] public int currentHealth = 5;
    
    
    // movement
    [Header("Movement")]
    private float horizontal;
    private bool isFacingRight = true;
    private bool doubleJump;
    public float walkSpeed = 7f;
    public float runSpeed = 10f;
    public float jumpingPower = 16f;
    public bool isRunning = false;

    // dashing
    [Header("dashing")]
    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 0.2f;
    public float dashingCooldown = 1f;

    // sliding
    [Header("sliding")]
    private bool isWallSliding;
    public float wallSlidingSpeed = 2f;

    // wall jumping
    [Header("wall jumping")]
    private bool isWallJumping;
    private float wallJumpingDirection;
    public float wallJumpingTime = 0.2f;
    private float wallJumpingCounter;
    public float wallJumpingDuration = 0.4f;
    public Vector2 wallJumpingPower = new Vector2(8f, 16f);


    [SerializeField] public Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private TrailRenderer tr;

    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private Animator animator;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        scoreText.text = "Score: " + score;
        healthText.text = "Health: " + currentHealth;

        if (currentHealth <= 0)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
        
        if (isDashing)
            return;

        //move control
        horizontal = Input.GetAxisRaw("Horizontal");

        if (horizontal > 0.1f || horizontal < -0.1f)
        {
            if (!isRunning)
            {
                animator.SetBool("isRunning", true);
                isRunning = true;
            }
        }
        else if(horizontal < 0.1f && horizontal > -0.1f)
        {
            if (isRunning)
            {
                animator.SetBool("isRunning", false);
                isRunning = false;   
            }
        }

        //jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
        }
        


        //double jump
        if (Input.GetKeyDown(KeyCode.Space) && !IsGrounded() && doubleJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpingPower);
            doubleJump = false;
        }
        

        //jump power
        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0f)
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);

        //can double jump
        if (IsGrounded())
            doubleJump = true;

        if (Input.GetKeyDown(KeyCode.X) && canDash)
            StartCoroutine(Dash());

        WallSlide();
        WallJump();

        if (!isWallJumping)
        {
            Flip();
        }
        

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "DamageTrigger")
        {
            TakeDamage(999);
        }
    }

    public void GainScore(int amount)
    {
        score += amount;
    }
    private void FixedUpdate()
    {
        if (!isWallJumping)
        {
            if (isDashing)
                return;
            
            
            
            if (Input.GetKey(KeyCode.LeftShift) && IsGrounded())
            {
                rb.linearVelocity = new Vector2(horizontal * runSpeed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(horizontal * walkSpeed, rb.linearVelocity.y);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }

    private void isMoving()
    {
        if (rb.linearVelocity.magnitude > .1f)
        {
            animator.SetBool("isRunning", true);
        }
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private bool IsWalled()
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpingDirection = -transform.localScale.x;
            wallJumpingCounter = wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if(Input.GetKeyDown(KeyCode.Space) && wallJumpingCounter > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);
            wallJumpingCounter = 0f;

            if(transform.localScale.x != wallJumpingDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }

            Invoke(nameof(StopWallJumping), wallJumpingDuration);
        }
    }

    private void StopWallJumping()
    {
        isWallJumping = false;
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && horizontal != 0f)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}