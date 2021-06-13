using Photon.Pun;
using UnityEngine;
using PlayerManagement.Movement;

namespace PlayerManagement
{
    public class PlayerMovementManager : MonoBehaviourPun
    {
        // Sets up the character controller
        [Header("Character Controller Reference")]
        public CharacterController controller;
        
        // Physics variables
        private Vector3 velocity;
        // Modified it to be stronger than on earth because the units are clearly not real/fucked up
        private float gravitationalConstant = -18f;

        [Tooltip("Collision check attributes")]
        // Ground collision check
        public Transform groundCheck;
        public float groundDistance = 0.4f;
        public LayerMask groundMask;

        bool isGrounded;

        // Roof collision check
        public Transform roofCheck;
        bool isHittingCeiling;

        [Tooltip("[TMP DEV] Player movement attributes")]
        // [TMP DEV] Set the player movement speed
        public float movementSpeed = 10f;
        public float jumpHeight = 2f;

        private bool isJumping;

        // How does our player jump ?
        private IJumpable jump;
        
        // Animation stuff
        private Animator _animator;
        void Start()
        {
            // Assign default movement(s)
            jump = new JumpBase();
            // e.g Assign default spell here
            //Assign Ground Layer for isGrounded
            groundMask = LayerMask.GetMask("Ground");
            // Don't mind me, just grabbing the animator
            _animator = GetComponentInChildren<Animator>();
        }

        void Update()
        {
            // Returns is this is not instancied by the controlled player
            if (PhotonNetwork.IsConnected && photonView.IsMine == false)
            {
                return;
            }

            // Check if player is grounded
            // Simulate a invisible sphere at players feet with ground distance radius
            // if sphere collide then the player is grounded
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            // Same for ceiling
            isHittingCeiling = Physics.CheckSphere(roofCheck.position, groundDistance, groundMask);

            // If player is grounded then we reset his velocity
            if (isGrounded && velocity.y < 0 || isHittingCeiling)
            {
                _animator.SetBool("inAir", false);
                _animator.SetTrigger("JUMP_END");
                isJumping = false;
                velocity.y = -0.5f; // not 0 because gravity goes brrrr
            }
            

            // RTFM
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Vector3 move = transform.right * x + transform.forward * z;

            _animator.SetFloat("x_axis", x);
            _animator.SetFloat("z_axis", z);
            
            _animator.SetBool("isMoving", x != 0 || z != 0);

            controller.Move(move * movementSpeed * Time.deltaTime);

            if (Input.GetButtonDown("Jump") && !isJumping)
            {
                isJumping = true;
                _animator.SetTrigger("JUMP_START");
                _animator.SetBool("inAir", true);
                
                velocity.y = jump.Jump().y;
                controller.Move(velocity * Time.deltaTime);
            }

            // Apply gravity
            velocity.y += gravitationalConstant * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
        }

        //Setter for gravity for later bonuses
        public void SetGravity(float grav)
        {
            gravitationalConstant = grav;
        }
    }
}
