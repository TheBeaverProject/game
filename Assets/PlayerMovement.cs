using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    //Sets up the character controller
    public float speed = 12f;
	private float accel = 1f;
	private float groundAccel = 1f;
	private float airAccel = 1.4f;
	private float maxSpeed = 18f;
    //Base speed will change
    public float gravity = -9.81f;
    public float jumpHeight = 3f;
    public bool hasJumped = false;
    //Base settings for jump and gravity
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;
    private bool canDoubleJump = false;
    //Setting up for ground check
    private Vector3 velocity;
    //Velocity vector for movement
    public bool blocked = false;
    public Transform roofCheck;
    public float roofDistance = 0.1f;
    //Roof check so that if a player jumps and is blocked on a roof surface he doesn't levitate
    void Update()
    {
        blocked = Physics.CheckSphere(roofCheck.position, roofDistance, groundMask);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        //Sets up a bool that makes a virtual sphere to check if the shpere collides with the layer ground (setted up in 
        // unity to the plane object
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f; // If the player is ground and velocity too low sets the velocity at a normal state
        }

        if (blocked)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
		// Gets w a s d inputs
		
		if (!isGrounded) // For bhop accel: If (look left+move left) or (move right+look right)
			if (Input.GetAxis("Mouse X") > 0 && Input.GetKey(KeyCode.D) || Input.GetAxis("Mouse X") < 0 && Input.GetKey(KeyCode.A))
				accel = airAccel; // Set the acceleration to airAccel (higher than ground accel)

		speed = speed * accel; // Moduling speed in function of accel (default accel is 1)

		if (speed > maxSpeed) // If speed is > maxSpeed set speed to maxSpeed
			speed = maxSpeed;
	
        Vector3 move = transform.right * x + transform.forward * z; //multiply the inputs by the unit vector
        controller.Move(move * speed * Time.deltaTime); //Moves the player (Time.deltaTime allows to not depend on
        //fps
        
		if (Input.GetButtonDown("Jump")) //Check if spacebar button is pressed
        {
            if (isGrounded) //Check if grounded for simple jump
            {
                velocity.y = 0; //Resets velocity Y vector
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); //Physics formula
                canDoubleJump = true; //Allows to double jump
            }
            else
            {
                if (canDoubleJump || !hasJumped) //if player didn't jump and falled allows to jump
                {
                    if (!hasJumped)
                        hasJumped = true;
                    canDoubleJump = false;
                    velocity.y = 0;
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }
        }

        if (isGrounded)
		{
            hasJumped = false; //when player touches the ground resets hasJumped
			accel = groundAccel; // If player is grounded, reset the accel to groundAccel
		}
        velocity.y += gravity * Time.deltaTime; //Gravity force
        controller.Move(velocity * Time.deltaTime); // moves the player downwards
    }
}
