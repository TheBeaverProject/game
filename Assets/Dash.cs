using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dash : MonoBehaviour
{
    public Vector3 dash;
    public const float maxDashTime = 2.5f;
    public float dashDistance = 5f;
    public float dashStoppingSpeed = 0.1f;
    float currentDashTime = maxDashTime;
    private float dashSpeed = 6;
    public CharacterController controller;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentDashTime = 0;
        }

        if (currentDashTime < maxDashTime)
        {
            dash = transform.forward * dashDistance;
            currentDashTime += dashStoppingSpeed;
        }
        else
        {
            dash = Vector3.zero;
        }

        controller.Move(dash * Time.deltaTime * dashSpeed);
    }
}
