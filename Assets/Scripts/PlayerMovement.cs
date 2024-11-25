using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] ContactFilter2D groundFilter;

    Rigidbody2D rb;
    Animator animator;
    Collider2D bodyColl;
    Collider2D feetColl;
    ParticleSystem dust;

    Vector2 moveInput;
    bool shouldJump;
    bool isGrounded;
    string currentState;

    //Animation states
    const string PLAYER_IDLE = "Player_Idle";
    const string PLAYER_RUN = "Player_Run";
    const string PLAYER_JUMP = "Player_Jump";


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        dust = GetComponentInChildren<ParticleSystem>(); 
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        if (isGrounded)
        ChangeAnimationState(PLAYER_RUN);
    }

    void OnJump()
    {
        if (isGrounded)
        shouldJump = true;
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
        Vector2 playerVelocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);
        rb.velocity = playerVelocity;

        //Mirror the sprite if moving left
        if (moveInput.x != 0)
        {
            transform.localScale = new Vector2(Mathf.Sign(moveInput.x), transform.localScale.y);
        }
        
        //Jump function
        if (shouldJump)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            ChangeAnimationState(PLAYER_JUMP);
            shouldJump = false;
            //dust.Play(); //Play particle effect
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
