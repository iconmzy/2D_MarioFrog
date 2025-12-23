using System.Collections;  
using System.Collections.Generic; 
using UnityEngine;

public class CharactermoveMent : MonoBehaviour
{
   
    private Rigidbody2D rb;
    private SpriteRenderer rbSprite;
    private Animator anim;


    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 6f;


    private bool isGrounded = false;
    private int jumpCount = 0;
    private const int MAX_JUMP_COUNT = 2;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;


    private int moveState = 0;
    private const int STATE_IDLE = 0;
    private const int STATE_RUNNING = 1;
    private const int STATE_JUMPING = 2;
    private const int STATE_FALLING = 3;
    private const int STATE_DOUBLEJUMPING = 4;

    // 状态转换阈值
    private const float VELOCITY_THRESHOLD = 0.5f;
    private const float INPUT_THRESHOLD = 0.1f;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rbSprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // 初始化地面检测点
        InitializeGroundCheck();
    }

    // 初始化地面检测点
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

    // Update is called once per frame
    private void Update()
    {
        
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        
        if (isGrounded && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            jumpCount = 0;
        }

        // 水平移动
        dirX = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

        // 跳跃输入处理
        HandleJumpInput();

        // 更新状态和动画
        UpdateState();
        UpdateAnimation();
    }

    // 处理跳跃输入
    private void HandleJumpInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            // 第一次跳跃（在地面或还没跳）
            if (isGrounded || jumpCount == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount = 1;
            }
            // 双跳（已跳过一次且还有跳跃次数）
            else if (jumpCount < MAX_JUMP_COUNT)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.8f); // 双跳稍弱
                jumpCount = MAX_JUMP_COUNT;
            }
        }
    }

    // 更新状态
    private void UpdateState()
    {
        int previousState = moveState;

        // 判断新状态
        if (isGrounded)
        {
            // 地面状态
            if (Mathf.Abs(dirX) > INPUT_THRESHOLD)
            {
                moveState = STATE_RUNNING;
            }
            else
            {
                moveState = STATE_IDLE;
            }
        }
        else
        {
            // 空中状态
            float verticalVelocity = rb.velocity.y;

            // 双跳特殊状态
            if (jumpCount == MAX_JUMP_COUNT && verticalVelocity > VELOCITY_THRESHOLD)
            {
                moveState = STATE_DOUBLEJUMPING;
            }
            // 上升状态
            else if (verticalVelocity > VELOCITY_THRESHOLD)
            {
                moveState = STATE_JUMPING;
            }
            // 下降状态
            else if (verticalVelocity < -VELOCITY_THRESHOLD)
            {
                moveState = STATE_FALLING;
            }
            // 速度接近0时的状态保持
            else
            {
                // 如果之前是跳跃状态，保持短暂时间
                if (previousState == STATE_JUMPING || previousState == STATE_DOUBLEJUMPING)
                {
                    moveState = previousState;
                }
                else
                {
                    moveState = STATE_FALLING;
                }
            }
        }

        // 特殊处理：如果突然落地，确保状态切换
        if (isGrounded && (previousState == STATE_FALLING || previousState == STATE_JUMPING))
        {
            if (Mathf.Abs(dirX) > INPUT_THRESHOLD)
            {
                moveState = STATE_RUNNING;
            }
            else
            {
                moveState = STATE_IDLE;
            }
        }
    }

    // 更新动画
    private void UpdateAnimation()
    {
        // 根据moveState设置对应的动画参数
        anim.SetBool("isIdle", moveState == STATE_IDLE);
        anim.SetBool("isRunning", moveState == STATE_RUNNING);
        anim.SetBool("isJumping", moveState == STATE_JUMPING);
        anim.SetBool("isFalling", moveState == STATE_FALLING);
        anim.SetBool("isDoubleJumping", moveState == STATE_DOUBLEJUMPING);

        // 更新精灵方向
        UpdateSpriteDirection();
    }

    // 更新精灵方向
    private void UpdateSpriteDirection()
    {
        // 只在有明确水平输入时翻转，避免在空中突然翻转
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

    // 调试信息
    private void OnGUI()
    {
        // 显示当前状态和调试信息
        string stateName = GetStateName(moveState);
        GUI.Label(new Rect(10, 10, 300, 20),
                 $"状态: {stateName} ({moveState}) | 速度Y: {rb.velocity.y:F2} | 跳跃次数: {jumpCount}/{MAX_JUMP_COUNT}");
        GUI.Label(new Rect(10, 30, 300, 20),
                 $"是否在地面: {isGrounded} | 水平输入: {dirX:F2}");
    }

    // 获取状态名称（调试用）
    private string GetStateName(int state)
    {
        switch (state)
        {
            case STATE_IDLE: return "待机";
            case STATE_RUNNING: return "奔跑";
            case STATE_JUMPING: return "跳跃";
            case STATE_FALLING: return "下落";
            case STATE_DOUBLEJUMPING: return "双跳";
            default: return "未知";
        }
    }

    // 在Scene视图中显示地面检测范围（调试用）
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    // 如果需要通过其他脚本获取当前状态
    public int GetCurrentState()
    {
        return moveState;
    }

    // 获取状态名称的公共方法
    public string GetCurrentStateName()
    {
        return GetStateName(moveState);
    }

    // 检查是否在特定状态
    public bool IsInState(int state)
    {
        return moveState == state;
    }
}
