using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.FSM.Player
{
    [SerializeField]
    public class PlayerFSM : FSM<PlayerState>
    {
        public PlayerState CurState => curState;
    }

}