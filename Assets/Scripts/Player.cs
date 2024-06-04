using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    /*
    Title: Unity 2 Player Game Tutorial (series)
    Author: gamesplusjames
    Date: 2017
    Code Version: Unity 5.5.0f3
    Availability: https://www.youtube.com/watch?v=p23J5-1OTAM&list=PLiyfvmtjWC_Ugm9c9Q7WaoRFGBZh_Z6ys
    
    Title: How do I find the location of a GameObject in C#
    Author: User on stackexchange.com
    Date: 2016
    Code Version: Unity 5.4.0
    Availability:  https://gamedev.stackexchange.com/questions/118441/how-do-i-find-the-location-of-a-gameobject-in-c
    
    Title: How to Play Audio from a Script in Unity
    Author: Game Dev Beginner
    Date: 2020
    Code Version: Unity 2018.4.19f1
    Availability: https://www.youtube.com/watch?v=p8KswsmGlpc&embeds_referring_euri=https%3A%2F%2Fgamedevbeginner.com%2F&feature=emb_logo

    Also, from ChatGPT
    Query: How to make hitboxes and hurtboxes?
    */
    private Rigidbody2D player;
    private GameObject firstPlayer; //This is useful in a case where there is a primary player. A player with priority over the camera or other such perks.
    public KeyCode jump, dash, up, left, right, down;
    public bool jumpC, dashC, upC, leftC, rightC, downC;
    int multiJumpCount = 0, multiJumpMax = 2, jumpSave = 30, jumpSaveTimer, pressRefreshTimer, pressRefresh = 30, wallJumpSpeed = 10, wallJumpRefresh = 90, wallJumpTimer, dashCount = 0, dashMax = 1, coinCountPlayer = 0;

    public float raycastLength = 0.1f;
    public LayerMask whatIsSolid;
    public bool isGrounded = false, onWallForward = false;

    float maxHSpeed = 16, maxVSpeed = 20, maxAccel = 1f, originX = 0, originY = 0, originSizeX = 0, originSizeY = 0;
    string playerName = "";

    private Animator motion;
    AudioSource playerJump;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Rigidbody2D>();
        originX = player.position.x;
        originY = player.position.y;
        originSizeX = player.transform.localScale.x;
        originSizeY = player.transform.localScale.y;
        playerName = player.name;
        firstPlayer = GameObject.Find("P1");

        motion = GetComponent<Animator>();
        playerJump = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrounded)
        {
            multiJumpCount = multiJumpMax;
            jumpSaveTimer = jumpSave; //I only want these variables reset while grounded.
        }
        else
        {
            jumpSaveTimer--;
            if (jumpSaveTimer < 0)
            {
                jumpSaveTimer = 0;
            }
        }
        if (wallJumpTimer < 0)
        {
            wallJumpTimer = 0;
        }
        else
        {
            wallJumpTimer--;
        }
        if (isGrounded || onWallForward)
        {
            dashCount = dashMax; //Dashes refresh when against a surface.
        }
        //Here are the actual input checks.
        if (Input.GetKeyDown(jump))
        {
            jumpC = true;
            pressRefreshTimer += pressRefresh;
        }
        if (Input.GetKeyDown(dash))
        {
            dashC = true;
            pressRefreshTimer += pressRefresh;
        }
        if (pressRefreshTimer == 0)
        {
            jumpC = false;
            dashC = false;
        }
        else
        {
            pressRefreshTimer--; //This ensures that a jump or dash input does not stick to true even if the player let go of those inputs.
        }
        if (Input.GetKey(up))
        {
            upC = true;
        }
        if (Input.GetKey(left))
        {
            leftC = true;
        }
        if (Input.GetKey(right))
        {
            rightC = true;
        }
        if (Input.GetKey(down))
        {
            downC = true;
        }
    }

    void FixedUpdate()
    {   //Trying different ways to try and find a collision detection system that detects only one edge of the player's collider.
        //Raycast and use of trigger BoxCollider suggested by ChatGPT
        Vector2 raycastForwardOrigin = transform.position + new Vector3(GetComponent<Collider2D>().bounds.extents.x * transform.localScale.x, 0f, 0f);
        RaycastHit2D hitWallAhead = Physics2D.Raycast(raycastForwardOrigin, Vector2.right * transform.localScale.x, raycastLength, whatIsSolid);
        if (hitWallAhead.collider != null)
        {
            onWallForward = true;
        }
        else
        {
            onWallForward = false;
        }
        //The checks themselves are code that is more original.
        if (onWallForward)
        {
            wallJumpTimer = 0;
        }

        if (player.velocity.x < -0.1f)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (player.velocity.x > 0.1f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        motion.SetFloat("Speed", Mathf.Abs(player.velocity.x));
        motion.SetBool("Grounded", isGrounded);

        if (player.velocity.x > maxHSpeed)
        { //This is a speed cap for horizontal movement.
            player.velocity = new Vector2(maxHSpeed, player.velocity.y);
        }
        else if (player.velocity.x < -maxHSpeed)
        {
            player.velocity = new Vector2(-maxHSpeed, player.velocity.y);
        }

        if (player.velocity.y > maxVSpeed)
        { //This is a speed cap for vertical movement.
            player.velocity = new Vector2(player.velocity.x, maxVSpeed);
        }
        else if (player.velocity.y < -maxVSpeed)
        {
            player.velocity = new Vector2(player.velocity.x, -maxVSpeed);
        }
        //You either jump when on the ground, jump with leniency from the jumpSave timer, or jump off a wall.
        if (jumpC && isGrounded || jumpC && jumpSaveTimer > 0 || jumpC && onWallForward && player.velocity.y < 1)
        { //Making the velocity check for less than 1 let's the player pass through semi-solids from below without counting it as ground until the player lands.
            player.velocity = new Vector2(player.velocity.x, player.velocity.y + 8);
            if (onWallForward)
            { //This is just to add the opposing force off the wall the player is holding against.
                player.velocity = new Vector2((player.velocity.x - wallJumpSpeed) * player.transform.localScale.x, player.velocity.y);
                wallJumpTimer = wallJumpRefresh;
            }
            jumpC = false;
            isGrounded = false; //Jumping takes you off the ground. Setting it here ensures the jump does not refresh while jumping.
            jumpSaveTimer = 0; //As soon as you jump, there should not be a chance to do a normal jump again.
        }
        else if (jumpC && multiJumpCount > 0)
        { //This is for multijumps, as their heights are different than normal jumps.
            player.velocity = new Vector2(player.velocity.x, 6);
            jumpC = false;
            multiJumpCount--;
        }

        if (dashC && leftC && dashCount > 0 && !isGrounded)
        { //Can only dash midair.
            player.velocity = new Vector2(player.velocity.x - maxHSpeed, 0); //With the 0 y velocity, it is possible to stop a fast fall, allowing for greater movement control.
            dashC = false;
            dashCount--;
        }
        else if (dashC && rightC && dashCount > 0 && !isGrounded)
        {
            player.velocity = new Vector2(player.velocity.x + maxHSpeed, 0);
            dashC = false;
            dashCount--;
        }

        if (wallJumpTimer > 0)
        { //One-sided wall jumps should be less realistic to do with input restrictions.
            leftC = false;
            rightC = false;
        }
        if (leftC && player.velocity.x > -maxHSpeed / 2)
        {
            player.velocity = new Vector2(player.velocity.x - maxAccel, player.velocity.y);
            leftC = false;
        }
        else if (rightC && player.velocity.x < maxHSpeed / 2)
        {
            player.velocity = new Vector2(player.velocity.x + maxAccel, player.velocity.y);
            rightC = false;
        }

        if (upC)
        {
            player.transform.localScale = new Vector2(player.transform.localScale.x, originSizeY * 2f);

        }
        else if (downC)
        {
            player.transform.localScale = new Vector2(player.transform.localScale.x, originSizeY * 0.5f);

        }
        else
        {
            player.transform.localScale = new Vector2(player.transform.localScale.x, originSizeY);
        }
        if (downC && !isGrounded)
        {
            player.velocity = new Vector2(0, player.velocity.y - 1);
        }
        upC = false;
        downC = false;

        if (player.position.y < -20)
        { //Falling off should not be a fear! That's why there is no consequence to falling.
            if (player.name == "P1")
            {
                player.position = new Vector2(originX, originY);
                player.velocity = new Vector2(0, 0);
                Debug.Log(playerName + " has respawned!");
            }
            else
            {
                originX = firstPlayer.transform.position.x;
                originY = firstPlayer.transform.position.y;
                player.position = new Vector2(originX, originY + 1);
                player.velocity = new Vector2(0, 0);
                Debug.Log(playerName + " has respawned at the leader's position!"); //This tidbit should encourage players to try beat the level off screen! Or to keep up to not be left behind!
            }

        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject && !other.gameObject.CompareTag("Coin") && player.velocity.y < 1) //Making the velocity check for less than 1 let's the player pass through semi-solids from below without counting it as ground until the player lands.
        {
            isGrounded = true;
        }
        if (other.gameObject.CompareTag("Enemy"))
        { //Enemies, on the other hand, will destroy you. This is to make failure fairer.
            if (other.gameObject.transform.position.y + 0.1f > player.transform.position.y)
            {
                if (playerName == "P1")
                {
                    print(playerName + " is down! Restarting!");
                    Coins.coinCount = 0;
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
                else
                {
                    print(playerName + " is down!"); //This is your penalty as not P1! The stage does not restart when you are downed! So just remember the enemy locations, and you're set.
                    Destroy(gameObject);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject && !other.gameObject.CompareTag("Coin"))
        {
            isGrounded = false;
            if (player.velocity.y > 0.1)
            {
                PlaySound();
            }
            
        }
    }
    public void IncreaseCoinCount() //Method provided by ChatGPT
    {
        coinCountPlayer++;
        print(playerName + " coins: " + coinCountPlayer);
    }

    void PlaySound()
    { //This is from a video tutorial.
        playerJump.PlayOneShot(playerJump.clip, 1);
    }
}
