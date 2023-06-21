using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public PlayerController player;
    
    private float sensitivity = 500f;
    private float clampAngle = 85f;
    
    private float verticalRotation;
    private float horizontalRotation;

    private void Start()
    {
        this.verticalRotation = this.transform.localEulerAngles.x;
        this.horizontalRotation = this.transform.eulerAngles.y;
    }
    private void Update()
    {
        Look();
        Debug.DrawRay(this.transform.position, this.transform.forward * 2, Color.red);
    }
}
