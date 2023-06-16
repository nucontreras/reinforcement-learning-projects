using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator anim;
    private Rigidbody rb;
    public LayerMask layerMask;
    public bool grounded;

    void Start()
    {
        this.rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Grounded();
        Jump();
        Move();
    }
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && this.grounded)
        {
            this.rb.AddForce(Vector3.up * 4, ForceMode.Impulse);
        }
    }
    private void Grounded()
    {
        if (Physics.CheckSphere(this.transform.position + Vector3.down, 0.2f, layerMask))
        {
            this.grounded = true;
        }
        else
        {
            this.grounded = false;
        }
        this.anim.SetBool("jump", this.grounded);
    }
    private void Move()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");
    }
}
