using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FSM.Player
{
    public class FallingState : PlayerState
    {
        public FallingState(PlayerController p) : base(p)
        {
        }

        public override void Enter()
        {
            player.ResetAllAnimatorBools();
            player.SetAnimatorBool("isFalling", true);
            player.animator.Play("PlayerFall");
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
            else if (player.IsRising)
            {
                // 再次上升（二段跳）
                if (player.HasDoubleJumped)
                {
                    player.SwitchToDoubleJumping();
                }
                else
                {
                    player.SwitchToJumping();
                }
            }
        }
    }
}
