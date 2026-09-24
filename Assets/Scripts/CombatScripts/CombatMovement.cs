using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatMovement : MonoBehaviour
{
    //public Animator anim;
    public Rigidbody2D rb;
    public float speed, jumpHeight;

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocityX = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
    }
    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            rb.linearVelocityY += jumpHeight;
        }
        if (Input.GetButtonUp("Jump"))
        {
            rb.linearVelocityY *= 0.5f;
        }
    }
}
