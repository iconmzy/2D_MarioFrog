using System.Collections;  
using System.Collections.Generic;

using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    // === 组件引用 ===
    private Rigidbody2D rb;
    private SpriteRenderer rbSprite;
    private Animator anim;

    // === 移动参数 ===
    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 6f;

    // === 落地缓冲 ===
    private float groundedBufferTimer = 0f;
    private const float GROUNDED_BUFFER_DURATION = 0.15f;
    private bool wasGroundedLastFrame = false;


    // === 跳跃相关 ===
    private bool isGrounded = false;
    private int jumpCount = 0;
    private const int MAX_JUMP_COUNT = 2;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private enum MoveState
    {
        Idle = 0,
        Running = 1,
        Jumping = 2,
        Falling = 3,
        DoubleJumping = 4
    }


    private MoveState currentMoveState = MoveState.Idle;

    // 状态转换阈值
    private const float VELOCITY_THRESHOLD = 0.5f;
    private const float INPUT_THRESHOLD = 0.1f;
    private const float GROUNDED_VELOCITY_THRESHOLD = 0.1f;

    // === 初始化 ===
    private void Start()
    {
        // 获取组件引用
        rb = GetComponent<Rigidbody2D>();
        rbSprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // 初始化地面检测点
        InitializeGroundCheck();
    }

   
    private void InitializeGroundCheck()
    {
        if (groundCheck == null)
        {
            groundCheck = transform.Find("GroundCheck");
            if (groundCheck == null)
            {
                GameObject obj = new GameObject("GroundCheck");
                groundCheck = obj.transform;
                groundCheck.parent = transform;
                groundCheck.localPosition = new Vector3(0, -0.5f, 0);
            }
        }
    }

   
    private void Update()
    {
        
        UpdateGroundDetection();

        
        HandleHorizontalMovement();

        
        HandleJumpInput();

        
        UpdateStateAndAnimation();
    }


    private void UpdateGroundDetection()
    {

        wasGroundedLastFrame = isGrounded;

     
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (!wasGroundedLastFrame && isGrounded)
        {
            groundedBufferTimer = GROUNDED_BUFFER_DURATION;
        }

    
        if (groundedBufferTimer > 0)
        {
            groundedBufferTimer -= Time.deltaTime;
        }


        if (isGrounded && rb.velocity.y <= 0.2f)
        {
            jumpCount = 0;
        }
    }


    private void HandleHorizontalMovement()
    {
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
    }


    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
      
            if (isGrounded || jumpCount == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount = 1;
            }

            else if (jumpCount < MAX_JUMP_COUNT)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.8f);
                jumpCount = MAX_JUMP_COUNT;
            }
        }
    }

    // 更新状态和动画
    private void UpdateStateAndAnimation()
    {
        // 更新状态
        MoveState newState = DetermineState();

        // 只在状态改变时更新
        if (newState != currentMoveState)
        {
            currentMoveState = newState;
            UpdateAnimatorState();
        }

        // 更新精灵方向
        UpdateSpriteDirection();
    }

    private MoveState DetermineState()
    {

        bool isValidGrounded = isGrounded && (
            Mathf.Abs(rb.velocity.y) < 0.3f ||      
            groundedBufferTimer > 0                 
        );

        if (isValidGrounded)
        {

            return (Mathf.Abs(dirX) > INPUT_THRESHOLD) ? MoveState.Running : MoveState.Idle;
        }


        float verticalVelocity = rb.velocity.y;


        if (jumpCount == MAX_JUMP_COUNT && verticalVelocity > VELOCITY_THRESHOLD)
        {
            return MoveState.DoubleJumping;
        }

        if (verticalVelocity > VELOCITY_THRESHOLD)
        {
            return MoveState.Jumping;
        }

        if (verticalVelocity < -VELOCITY_THRESHOLD)
        {
            return MoveState.Falling;
        }

  
        if (currentMoveState == MoveState.Jumping || currentMoveState == MoveState.DoubleJumping)
        {
            return currentMoveState;
        }


        return MoveState.Falling;
    }


    private void UpdateAnimatorState()
    {

        anim.SetInteger("moveState", (int)currentMoveState);
    }


    private void UpdateSpriteDirection()
    {
        
        if (dirX > INPUT_THRESHOLD)
        {
            rbSprite.flipX = false;
        }
        else if (dirX < -INPUT_THRESHOLD)
        {
            rbSprite.flipX = true;
        }
        // dirX接近0时保持当前方向不变
    }

}