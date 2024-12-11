using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement_Force : MonoBehaviour
{
    [SerializeField] float accRate, maxSpeed, decelRate, jumpForce;
    [SerializeField] ContactFilter2D groundFilter;

    bool isJumping = false;
    bool isGrounded = false;
    
    Animator animator;
    Rigidbody2D rb;
    Vector2 moveInput;

    //Animation states
    string currentState;
    const string PLAYER_IDLE = "Player_Idle";
    const string PLAYER_RUN = "Player_Run";
    const string PLAYER_JUMP = "Player_Jump";

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }
    void OnJump()
    {
        if (isGrounded)
        { isJumping = true; }
    }

    void Update()
    {
        if (moveInput.Equals(Vector2.zero) && isGrounded)
        {
            ChangeAnimationState(PLAYER_IDLE);
        }
    }

    void FixedUpdate()
    {
        isGrounded = rb.IsTouching(groundFilter);
        Run();
        Jump();
    }

    void Run()
    {
        // Apply horizontal movement with acceleration and deceleration
        if (moveInput.x != 0)
        {
            // Accelerate the player in the input direction
            rb.AddForce(new Vector2(moveInput.x * accRate, 0));

            // Clamp the player's horizontal velocity to the max speed
            if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            {
                rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
            }
            transform.localScale = new Vector2(Mathf.Sign(moveInput.x), transform.localScale.y);
            ChangeAnimationState(PLAYER_RUN);
        }
        else
        {
            // Decelerate the player when no input is detected
            if (Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                float decelForce = Mathf.Min(decelRate * Time.fixedDeltaTime, Mathf.Abs(rb.velocity.x));
                rb.velocity = new Vector2(rb.velocity.x - Mathf.Sign(rb.velocity.x) * decelForce, rb.velocity.y);
            }
            else
            {
                // Stop the player completely if the velocity is very low
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        
    }

    void Jump()
    {
        if (isJumping && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce);
            isJumping = false;
            ChangeAnimationState(PLAYER_JUMP);
        }
    }

    void ChangeAnimationState(string newState)
    {
        //Stop the same animation from over writhing it self
        if (currentState == newState) return;

        //Play animation
        animator.Play(newState);

        //Update current animation
        currentState = newState;
    }
}
