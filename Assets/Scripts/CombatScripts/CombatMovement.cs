using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatMovement : MonoBehaviour
{
    //public Animator anim;
    public Rigidbody2D rb;
    public float speed, jumpHeight;
    public Animator anim;

    bool isFacingRight = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocityX = Input.GetAxisRaw("Horizontal") * Time.deltaTime * speed;
        if(isFacingRight == false && rb.linearVelocityX >= 0.1)
        {
            Flip();
        }
        if (isFacingRight && rb.linearVelocityX < -0.1)
        {
            Flip();
        }
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
        if (Input.GetButtonDown("Attack"))
        {
            anim.SetTrigger("Attack");
        }
    }
    void Flip()
    {
        Vector3 turn = new Vector3(0, 180, 0);
        this.transform.Rotate(turn);
        isFacingRight = !isFacingRight;
    }
}
