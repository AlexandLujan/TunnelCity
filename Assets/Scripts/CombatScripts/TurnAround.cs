using UnityEngine;

public class TurnAround : MonoBehaviour
{
    public Transform otherPlayer;
    public bool isFacingRight;

    // Update is called once per frame
    void FixedUpdate()
    {
        if(otherPlayer.position.x >= this.transform.position.x && isFacingRight == false)
        {
            Vector3 turn = new Vector3(0, 180, 0);
            this.transform.Rotate(turn);
            isFacingRight = true;
        }
        if (otherPlayer.position.x <= this.transform.position.x && isFacingRight == true)
        {
            Vector3 turn = new Vector3(0, 180, 0);
            this.transform.Rotate(turn);
            isFacingRight = false;
        }
    }
}
