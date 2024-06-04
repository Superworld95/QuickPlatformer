using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    //Most of this code is the same as the players', for sake of simplicity and for the potential to program unique input cycles.
    private Rigidbody2D enemy;
    
    public float raycastLength = 0.1f;
    public LayerMask whatIsSolid;
    public bool isGrounded = false, onWallForward = false;

    float maxHSpeed = 16, maxVSpeed = 20;
    //string enemyName = "";

    private Animator motion;
    int direction = 1;

    AudioSource hitting;

    // Start is called before the first frame update
    void Start()
    {        
        enemy = GetComponent<Rigidbody2D>();
        motion = GetComponent<Animator>();
        hitting = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {   
        Vector2 raycastDownOrigin = new Vector3(transform.position.x, transform.position.y - GetComponent<Collider2D>().bounds.extents.y, 0f);
        RaycastHit2D hitGround = Physics2D.Raycast(raycastDownOrigin, Vector3.down, raycastLength, whatIsSolid);
        if (hitGround.collider == null && isGrounded) {
            direction *= -1;
        }
        Vector2 raycastForwardOrigin = transform.position + new Vector3(GetComponent<Collider2D>().bounds.extents.x * transform.localScale.x, 0f, 0f);
        RaycastHit2D hitWallAhead = Physics2D.Raycast(raycastForwardOrigin, Vector2.right * transform.localScale.x, raycastLength, whatIsSolid);
        if (hitWallAhead.collider != null) {
            direction *= -1;
            }


        if (enemy.velocity.x < -0.1f) {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (enemy.velocity.x > 0.1f) {
            transform.localScale = new Vector3(1, 1, 1);
        }
        motion.SetFloat("Speed", Mathf.Abs(enemy.velocity.x));
        motion.SetBool("Grounded", isGrounded);

        if (enemy.velocity.x > maxHSpeed) { //This is a speed cap for horizontal movement.
            enemy.velocity = new Vector2(maxHSpeed, enemy.velocity.y);
        } else if (enemy.velocity.x < -maxHSpeed) {
            enemy.velocity = new Vector2(-maxHSpeed, enemy.velocity.y);
        }

        if (enemy.velocity.y > maxVSpeed) { //This is a speed cap for vertical movement.
            enemy.velocity = new Vector2(enemy.velocity.x, maxVSpeed);
        } else if (enemy.velocity.y < -maxVSpeed) {
            enemy.velocity = new Vector2(enemy.velocity.x, -maxVSpeed);
        }
        enemy.velocity = new Vector2(direction * (maxHSpeed/2), enemy.velocity.y);        
        }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject)
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.transform.position.y > enemy.transform.position.y + 0.1f)
            {
                PlaySound();
                Destroy(gameObject);
            }            
        }
    }

    void PlaySound()
    { //This is from a video tutorial.
        hitting.PlayOneShot(hitting.clip, 1);
    }
}
