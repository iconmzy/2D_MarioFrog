using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.FSM
{
    public interface IFSMState
    {
        void Enter();
        void Exit();
        void Tick();
    }
}


