using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAgent : MonoBehaviour
{
    Rigidbody ourDrone;
    public float upForce;
    void Awake()
    {
        ourDrone = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        MovementUpDown();
        ourDrone.AddRelativeForce(Vector3.up * upForce);
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
}
