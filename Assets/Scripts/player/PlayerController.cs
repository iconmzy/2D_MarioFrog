using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FSM.Player
{
    public class PlayerController : MonoBehaviour
    {
        // === 组件引用 ===
        private Rigidbody2D rb;
        private SpriteRenderer spriteRenderer;
        public Animator animator;

        public PlayerFSM playerFSM;

        // === 移动参数 ===
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float jumpForce = 6f;

        // === 地面检测 ===
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;

        // === 跳跃系统 ===
        private int jumpCount = 0;
        private const int MAX_JUMP_COUNT = 2;

        // === 状态判断常量 ===
        private const float INPUT_THRESHOLD = 0.1f;
        private const float VELOCITY_THRESHOLD = 0.3f;

        // === 外部访问属性 ===
        public float DirX { get; private set; } = 0f;
        public float MoveSpeed => moveSpeed;
        public float JumpForce => jumpForce;
        public int JumpCount => jumpCount;
        public int MaxJumpCount => MAX_JUMP_COUNT;
        public bool IsGrounded { get; private set; } = false;
        public Rigidbody2D Rigidbody => rb;
        public SpriteRenderer SpriteRenderer => spriteRenderer;

        // Start is called before the first frame update
        void Start()
        {
            InitializeComponents();
            InitializeGroundCheck();
            InitializeFSM();
        }

        private void InitializeComponents()
        {
            rb = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
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

        private void InitializeFSM()
        {
            playerFSM = new PlayerFSM();

            var idle = new IdleState(this);
            var run = new RunningState(this);
            var jump = new JumpingState(this);
            var fall = new FallingState(this);
            var doubleJump = new DoubleJumpingState(this);

            playerFSM.AddState(idle);
            playerFSM.AddState(run);
            playerFSM.AddState(jump);
            playerFSM.AddState(fall);
            playerFSM.AddState(doubleJump);

            playerFSM.SwitchOn(idle);
        }

        // Update is called once per frame
        void Update()
        {
            UpdateGroundDetection();
            HandleInput();
            playerFSM.OnUpdate();
        }

        private void UpdateGroundDetection()
        {
            bool wasGrounded = IsGrounded;
            IsGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            // 着地瞬间立即重置跳跃计数
            if (IsGrounded && !wasGrounded)
            {
                jumpCount = 0;
            }
        }

        private void HandleInput()
        {
            // 水平移动输入
            DirX = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(DirX * moveSpeed, rb.velocity.y);

            // 跳跃输入
            if (Input.GetButtonDown("Jump"))
            {
                TryJump();
            }

            // 更新朝向
            UpdateSpriteDirection();
        }

        private void TryJump()
        {
            if (IsGrounded || jumpCount == 0)
            {
                // 地面或第一次跳跃
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCount = 1;
            }
            else if (jumpCount < MAX_JUMP_COUNT)
            {
                // 二段跳
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * 0.8f);
                jumpCount = MAX_JUMP_COUNT;
            }
        }

        private void UpdateSpriteDirection()
        {
            if (DirX > INPUT_THRESHOLD)
            {
                spriteRenderer.flipX = false;
            }
            else if (DirX < -INPUT_THRESHOLD)
            {
                spriteRenderer.flipX = true;
            }
        }

        // === 动画参数设置 ===
        public void SetAnimatorBool(string name, bool value)
        {
            animator.SetBool(name, value);
        }

        public void ResetAllAnimatorBools()
        {
            animator.SetBool("isIdle", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            animator.SetBool("isDoubleJumping", false);
        }

        // === 状态切换辅助方法 ===
        public void SwitchToIdle()
        {
            playerFSM.ChangeState(typeof(IdleState));
        }

        public void SwitchToRunning()
        {
            playerFSM.ChangeState(typeof(RunningState));
        }

        public void SwitchToJumping()
        {
            playerFSM.ChangeState(typeof(JumpingState));
        }

        public void SwitchToFalling()
        {
            playerFSM.ChangeState(typeof(FallingState));
        }

        public void SwitchToDoubleJumping()
        {
            playerFSM.ChangeState(typeof(DoubleJumpingState));
        }

        // === 状态判断 ===
        public bool IsMoving => Mathf.Abs(DirX) > INPUT_THRESHOLD;
        public bool ShouldRun => IsGrounded && IsMoving;
        public bool ShouldIdle => IsGrounded && !IsMoving;
        public bool IsRising => rb.velocity.y > 0.1f;
        public bool IsFalling => rb.velocity.y < -0.1f;
        public bool HasDoubleJumped => jumpCount >= MAX_JUMP_COUNT;

        // === 编辑器调试 ===
        private void OnDrawGizmosSelected()
        {
            if (groundCheck != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
            }
        }
    }
}
