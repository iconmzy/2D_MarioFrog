using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.FSM.Player
{
    public class PlayerController : MonoBehaviour
    {


        // === componet ===
        private Rigidbody2D rb;
        private SpriteRenderer rbSprite;
        public Animator animator;

        public PlayerFSM playerFSM;


        // parameter
        private float dirX = 0f;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float jumpForce = 6f;

        // Start is called before the first frame update
        void Start()
        {
            playerFSM = new PlayerFSM();

            var idle = new IdleState(this);
            var run = new Runstate(this);

            playerFSM.AddState(idle);
            playerFSM.AddState(run);

            playerFSM.SwitchOn(idle);

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
