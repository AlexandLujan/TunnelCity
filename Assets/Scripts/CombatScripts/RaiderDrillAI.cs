using System.Collections;
using UnityEngine;

public class RaiderDrillAI : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator anim;
    public float upSpeed, sideSpeed;
    public Transform target, shootPoint1, shootPoint2;
    public GameObject boulder, boulder2;
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
        yield return new WaitForSeconds(0.9f);
        anim.SetBool("Jump", false);
        Instantiate(boulder, shootPoint1.position, shootPoint1.rotation).GetComponent<Rigidbody2D>().linearVelocityY = upSpeed/1.5f;
        Instantiate(boulder2, shootPoint2.position, shootPoint2.rotation).GetComponent<Rigidbody2D>().linearVelocityY = upSpeed/1.5f;

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
