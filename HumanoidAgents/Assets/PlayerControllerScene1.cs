using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScene1 : MonoBehaviour
{
    public Animator anim;
    private Rigidbody rb;
    public LayerMask layerMask;
    public bool grounded;
    public bool running;

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
            Debug.Log(Physics.CheckSphere(this.transform.position + Vector3.down, 0.2f, layerMask));
            this.grounded = true;
        }
        else
        {
            this.grounded = false;
        }
        this.anim.SetBool("jump", !this.grounded);
    }
    private void Move()
    {
        float verticalAxis = Input.GetAxis("Vertical");
        float horizontalAxis = Input.GetAxis("Horizontal");

        Vector3 movement = this.transform.forward * verticalAxis + this.transform.right * horizontalAxis;
        movement.Normalize();

        if (horizontalAxis != 0 && verticalAxis == 0)
        {
            this.transform.position += movement * 0.04f;
        }
        else if (verticalAxis >= 0)
        {
            this.transform.position += movement * 0.08f;
        } 

        else
        {
            this.transform.position += movement * 0.03f;
        }

        this.anim.SetFloat("vertical", verticalAxis);
        this.anim.SetFloat("horizontal", horizontalAxis);
    }
}
