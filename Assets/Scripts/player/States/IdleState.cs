using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FSM.Player
{
    public class IdleState : PlayerState
    {
        public IdleState(PlayerController p) : base(p)
        {
        }

        public override void Enter()
        {
            player.ResetAllAnimatorBools();
            player.SetAnimatorBool("isIdle", true);
            player.animator.Play("PlayerIdle");
        }

        public override void Exit()
        {

        }

        public override void Tick()
        {
            // 状态转换检测
            if (!player.IsGrounded)
            {
                // 离地状态判断
                if (player.IsRising)
                {
                    player.SwitchToJumping();
                }
                else if (player.IsFalling)
                {
                    player.SwitchToFalling();
                }
            }
            else if (player.ShouldRun)
            {
                player.SwitchToRunning();
            }
        }
    }
}
