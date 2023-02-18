using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAgent : MonoBehaviour
{
    Rigidbody ourDrone;
    public float upForce;
    private float movementForwardSpeed = 100f;  // 500
    private float tiltAmountForward = 0f;
    private float tiltVelocityForward;
    private float wantedYRotation;
    private float currentYRotation;
    private float rotateAmoutByKeys = 2.5f;
    private float rotationYVelocity;

    void Awake()
    {
        ourDrone = GetComponent<Rigidbody>();
        //ourDrone.transform.localRotation = Quaternion.Euler(0, 180, 0);
    }
    private void FixedUpdate()
    {
        MovementUpDown();
        MovementForward();
        Rotation();
        ourDrone.AddRelativeForce(Vector3.up * upForce);
        ourDrone.rotation = Quaternion.Euler(new Vector3(tiltAmountForward, currentYRotation, ourDrone.rotation.z));
    }
    void MovementUpDown()
    {
        if (Input.GetKey(KeyCode.I))
        {
            upForce = 450f;
        }
        else if (Input.GetKey(KeyCode.K))
        {
            upForce = -200f;
        } 
        else if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K))
        {
            upForce = 98.1f;
        }
    }
    void MovementForward()
    {
        if(Input.GetAxis("Vertical") != 0)
        {
            ourDrone.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * movementForwardSpeed);
            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 20 * Input.GetAxis("Vertical"), ref tiltVelocityForward, 0.1f);
        }
    }
    void Rotation()
    {
        if (Input.GetKey(KeyCode.J))
        {
            wantedYRotation -= rotateAmoutByKeys;
        }
        if (Input.GetKey(KeyCode.L))
        {
            wantedYRotation += rotateAmoutByKeys;
        }
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, 0.25f);
    }
}
