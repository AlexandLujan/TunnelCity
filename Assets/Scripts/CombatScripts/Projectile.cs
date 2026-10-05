using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public int playerNum;
    public void Start()
    {
        
    }
    public void FixedUpdate()
    {
        rb.linearVelocityX = speed * Time.deltaTime;
    }

    public void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.transform.tag == "Ground")
        {
            Destroy(this.gameObject);
        }
    }
}
