using System.Collections;
using UnityEngine;

public class RaiderDrillAI : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;
    public float upSpeed, sideSpeed;
    public Transform target, shootPoint1, shootPoint2;
    public GameObject boulder;
    public bool isFacingRight, isMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(battleState());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving == false)
        {
            if (target.position.x >= this.transform.position.x && isFacingRight == false)
            {
                Vector3 turn = new Vector3(0, 180, 0);
                this.transform.Rotate(turn);
                isFacingRight = true;
            }
            if (target.position.x <= this.transform.position.x && isFacingRight == true)
            {
                Vector3 turn = new Vector3(0, 180, 0);
                this.transform.Rotate(turn);
                isFacingRight = false;
            }
        }
    }

    public IEnumerator battleState()
    {
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Jump());

    }

    public IEnumerator Jump()
    {
        isMoving = true;
        yield return new WaitForSeconds(1f);
        rb.linearVelocityY = upSpeed;
        if (isFacingRight)
        {
            rb.linearVelocityX = sideSpeed / 2;
        }
        else
        {
            rb.linearVelocityX = -sideSpeed / 2;
        }
        anim.SetBool("Jump", true);
        yield return new WaitForSeconds(0.8f);
        anim.SetBool("Jump", false);
        Instantiate(boulder, shootPoint1.position, shootPoint1.rotation);
        Instantiate(boulder, shootPoint1.position, shootPoint1.rotation);

        rb.linearVelocityX = 0;
        isMoving = false;
        StartCoroutine(battleState());
    }
    public int RandomNum(int min, int max)
    {
        int result = Random.Range(min, max + 1);

        return result;
    }
}
