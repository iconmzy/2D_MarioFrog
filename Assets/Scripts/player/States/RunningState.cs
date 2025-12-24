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

        }
    }
}


