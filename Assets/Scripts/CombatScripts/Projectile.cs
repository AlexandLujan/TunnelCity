using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public int playerNum;
    public void Start()
    {
        if(this.transform.rotation.y > 0)
        {
            speed *= -1;
        }
    }
    public void FixedUpdate()
    {
        rb.linearVelocityX = speed * Time.deltaTime;
    }

    public void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.transform.tag == "Player1" && playerNum == 2)
        {

        }
        if (coll.transform.tag == "Player2" && playerNum == 1)
        {

        }
        if(coll.transform.tag == "Ground")
        {
            Destroy(this.gameObject);
        }
    }
}
