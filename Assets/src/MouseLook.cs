using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f;
    //Mouse sensitivity could modify it in settings

    public Transform playerBody;
    //Dragged the character controller from unity

    private float xRotation = 0f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        //Locks the cursor in the center of the screen
    }
    
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        //Get mouse input
        
        xRotation -= mouseY;
        //- because if + then it moves in the opposite direction
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        //-90 and 90 to not allow the player to move too fast and see behind him
        
        transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
        //Rotates the character controller
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
