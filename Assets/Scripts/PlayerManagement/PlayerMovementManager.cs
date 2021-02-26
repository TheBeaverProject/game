using Photon.Pun;
using UnityEngine;
using Movement;

namespace PlayerManagement
{
    public class PlayerMovementManager : MonoBehaviourPun
    {
        // Character Controller  reference
        // Sets up the character controller
        public CharacterController controller;

        // Physics var
        private Vector3 velocity;
        private float gravitationalConstant = -9.81f;

        // == Collision check attributs ==
        // Ground collision check
        public Transform groundCheck;
        public float groundDistance = 0.4f;
        public LayerMask groundMask;

        bool isGrounded;

        // Roof collision check
        public Transform roofCheck;
        bool isHittingCeiling;

        // [TMP DEV]Set the player movement speed
        public float movementSpeed = 16f;
        public float jumpHeight = 3f;

        private bool canDoubleJump;
        private bool isJumping;
        private IJumpable jump; 
        
        void Start()
        {
            // Assign default movement(s)
            jump = new Jumpbase();
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
            isHittingCeling = Physics.CheckSphere(roofCheck.position, groundDistance, groundMask);

            // If player is grounded then we reset his velocity
            if (isJumping && velocity.y < 0 || isHittingCeiling)
            {
                isJumping = false;
                velocity.y = -0.5f; // not 0 because gravity goes brrrr
            }
            
            // RTFM
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            //multiply the inputs by the unit vector
            Vector3 move = transform.right * x + transform.forward * z;
            controller.Move(move * movementSpeed * Time.deltaTime);
            
            if (Input.GetButtonDown("Jump") && !isJumping)
            {
                velocity.y = jump.Jump().y;
                controller.Move(velocity * Time.deltaTime);
                isJumping = true;
            }
            
            // Apply gravity
            velocity.y += gravitationalConstant * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
    }
}
