using UnityEngine;
using UnityEngine.SceneManagement;
public class Health : MonoBehaviour
{
    public int health, maxHealth;
    public float invincibilityTime;
    public bool isPlayer;
    public Color hurtColor;
    public SpriteRenderer spriteR;
    public string gameOverSceneName;
    float timer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    public void FixedUpdate()
    {
        timer -= Time.deltaTime;
        if(timer >= 0)
        {
            spriteR.color = hurtColor;
        }
        else
        {
            spriteR.color = Color.white;
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (timer <= 0)
        {
            if (collision.tag == "PlayerAttack" && isPlayer == false)
            {
                health -= 1;
                timer = invincibilityTime;
            }
            else if (collision.tag == "EnemyAttack" && isPlayer)
            {
                health -= 1;
                timer = invincibilityTime;
            }

            if(health <= 0)
            {
                SceneManager.LoadScene(gameOverSceneName);
            }
        }
    }
}
