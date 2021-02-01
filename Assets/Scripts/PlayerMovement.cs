using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Character Controller  reference
    // Sets up the character controller
    public CharacterController controller;
    private Vector3 velocity;

    // [TMP DEV]Set the player movement speed
    public float movementSpeed = 16f; //Speed cap for game purposes, will be changed later
    protected bool isJumping = false;
    private float gravitationalConstant = -9.81f;
       
    void Update()
    {
        // Gets w a s d inputs
        // Vertical:
        //      W: Vertical = 1, S: Vertical = -1
        // Horizontal:
        //      A: Horizontal = -1, D: Horizontal = 1
        Debug.Log("Update:");
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Debug.Log($"got x: ${x} z: ${z}");

        Vector3 move = transform.right * x + transform.forward * z; //multiply the inputs by the unit vector
        controller.Move(move * movementSpeed * Time.deltaTime); // move player on x and z axis (A and D input)

        velocity.y += gravitationalConstant * Time.deltaTime;
        if(controller.isGrounded)
            velocity.y = 0;

        controller.Move(velocity * Time.deltaTime);
    }

    protected void jump()
    {
        // Player has landed
        if(isJumping && controller.isGrounded)
            velocity.y = 0;
    }
}