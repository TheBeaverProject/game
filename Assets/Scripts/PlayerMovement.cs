using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
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
    bool isHittingRoof;

    // [TMP DEV]Set the player movement speed
    public float movementSpeed = 16f;
    public float jumpHeight = 3f;

    void Update()
    {
        // Check if player is grounded
        // Simulate a invisible sphere at players feet with ground distance radius
        // if sphere collide then the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // Same for ceiling
        isHittingRoof = Physics.CheckSphere(roofCheck.position, groundDistance, groundMask);

        // If player is grounded then we reset his velocity
        if (isGrounded && velocity.y < 0 || isHittingRoof)
            velocity.y = -0.5f;           // not 0 because gravity goes brrrr

        // Gets w a s d inputs
        // Vertical:
        //      W: Vertical = 1, S: Vertical = -1
        // Horizontal:
        //      A: Horizontal = -1, D: Horizontal = 1
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //multiply the inputs by the unit vector
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * movementSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump"))
        {
            jump();
        }

        velocity.y += gravitationalConstant * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    protected void jump()
    {
    }
}
