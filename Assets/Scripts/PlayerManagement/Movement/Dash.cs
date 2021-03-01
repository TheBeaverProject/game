using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    public Vector3 dash;
    public Vector3 dashY;
    public float dashDistance = 2f;
    public float dashYDistance = 3.5f;
    public float dashTime = 0.5f;
    public float dashStart;
    public float dashSpeed = 5f;
        
    public float dashCd = 3f;
    public float nextDash = 0;

    public CharacterController controller;

    private void Update()
    {
        float thisTime =Time.time;
        
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (thisTime >= nextDash)
            {
                dashStart = thisTime;
                nextDash = thisTime + dashCd;
                
                if (Input.GetKey(KeyCode.S))
                    dash = -transform.forward * dashDistance;
                else if (Input.GetKey(KeyCode.D))
                    dash = transform.right * dashDistance;
                else if (Input.GetKey(KeyCode.A))
                    dash = -transform.right * dashDistance;
                else if (Input.GetKey(KeyCode.W))
                    dash = transform.forward * dashDistance;
            }
        }

        if (Input.GetAxis("Mouse Y") > 0)
        {
            dashY = transform.up * dashYDistance;
        }
        else
        {
            dashY = Vector3.zero;
        }
        
        if (thisTime <= dashStart + dashTime)
        {
            controller.Move(dash * dashSpeed * Time.deltaTime);
            controller.Move(dashY * dashSpeed / 2 * Time.deltaTime);
        }
        else
        {
            dash = Vector3.zero;
            dashY = Vector3.zero;
        }
    }
}
