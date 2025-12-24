using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Game.FSM;

namespace Game.FSM.Player
{
    public abstract class PlayerState : IFSMState
    {
        protected PlayerController player;
        public PlayerState(PlayerController p) => player = p;
        public abstract void Enter();
        public abstract void Exit();
        public abstract void Tick();
    }
}
