using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FSM.Player
{
    public class DoubleJumpingState : PlayerState
    {
        public DoubleJumpingState(PlayerController p) : base(p)
        {
        }

        public override void Enter()
        {
            player.ResetAllAnimatorBools();
            player.SetAnimatorBool("isDoubleJumping", true);
            player.animator.Play("PlayerDoubleJump");
        }

        public override void Exit()
        {
        }

        public override void Tick()
        {
            // 状态转换检测
            if (player.IsGrounded)
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
            else if (player.IsFalling)
            {
                // 下落状态
                player.SwitchToFalling();
            }
        }
    }
}
