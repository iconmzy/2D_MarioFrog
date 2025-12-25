using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FSM.Player
{
    public class JumpingState : PlayerState
    {
        public JumpingState(PlayerController p) : base(p)
        {
        }

        public override void Enter()
        {
            player.ResetAllAnimatorBools();
            player.SetAnimatorBool("isJumping", true);
            player.animator.Play("PlayerJump");
        }

        public override void Exit()
        {
        }

        public override void Tick()
        {
            // 状态转换检测
            if (!player.IsGrounded)
            {
                // 空中状态转换
                if (player.IsFalling)
                {
                    player.SwitchToFalling();
                }
                else if (player.HasDoubleJumped && player.IsRising)
                {
                    player.SwitchToDoubleJumping();
                }
            }
            else
            {
                // 着地转换
                if (player.ShouldRun)
                {
                    player.SwitchToRunning();
                }
                else
                {
                    player.SwitchToIdle();
                }
            }
        }
    }
}
