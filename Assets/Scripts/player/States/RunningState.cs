using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FSM.Player
{
    public class RunningState : PlayerState
    {
        public RunningState(PlayerController p) : base(p)
        {
        }

        public override void Enter()
        {
            player.animator.Play("PlayerRunning");
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
            else if (player.ShouldIdle)
            {
                player.SwitchToIdle();
            }
        }
    }
}


