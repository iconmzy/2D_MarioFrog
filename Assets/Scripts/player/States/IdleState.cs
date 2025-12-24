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
            player.animator.Play("PlayerIdle");
        }

        public override void Exit()
        {

        }

        public override void Tick()
        {

        }
    }
}
