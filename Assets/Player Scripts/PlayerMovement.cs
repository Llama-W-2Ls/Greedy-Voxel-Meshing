using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;

    [Header("Movement Values")]
    public float MovementSpeed;
    public float JumpForce;
    public float DragForce;
    public float Gravity;

    [Header("Ground Checking")]
    public Transform Feet;
    public float Radius;
    public LayerMask GroundLayer;

    public bool IsGrounded
    {
        get
        {
            return Physics.CheckSphere(Feet.position, Radius, GroundLayer);
        }
    }

    void Update()
    {
        Jump();
    }

    void FixedUpdate()
    {
        Move();
        AddGravity();
        AddDrag();
    }

    void Move()
    {
        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");

        var direction = transform.right * x + transform.forward * z;
        direction = MovementSpeed * direction.normalized;

        rb.AddForce(direction);
    }

    void Jump()
    {
        if (!Input.GetKeyDown(KeyCode.Space) || !IsGrounded)
            return;

        rb.velocity = new Vector3(rb.velocity.x, JumpForce, rb.velocity.z);
    }

    void AddGravity()
    {
        rb.AddForce(Gravity * Vector3.down);
    }

    void AddDrag()
    {
        rb.velocity = new Vector3
        (
            rb.velocity.x * DragForce,
            rb.velocity.y,
            rb.velocity.z * DragForce
        );
    }
}
