using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D RB2D;
    private Animator Anim;
    public GameManager gameManager;
    public LayerMask WhatIsGround;
    public Transform groundCheck;
    public float groundRadius;
    public float horizontalMove = 0f;
    public float horizontalMoveSpeed = 15f;
    public float jumpHeight;
    public int defaultAdditionalJumps = 1;
    private int additionalJumps;
    public bool grounded;
    public bool facingRight;
    public Vector3 respawnPos;
    public Vector3 lastRespawnPos;
    public float coyoteTime=0.2f;
    private float coyoteCounter=0.2f;
    void Start()
    {
        respawnPos = new Vector3(0, -1, 0);
        RB2D = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        Grounded();
        PlayAnimtionsBasedOnMovement();
        CheckForCoyoteTime();
    }

    void Update()
    {
        
        horizontalMove = Input.GetAxisRaw("Horizontal")*horizontalMoveSpeed;
        
        if (horizontalMove>0)
        {
            RB2D.velocity = new Vector2(horizontalMove, RB2D.velocity.y);
            Flip();
        }
        else if (horizontalMove <0)
        {
            RB2D.velocity = new Vector2(horizontalMove, RB2D.velocity.y);
            Flip();
        }
        else
        {
            RB2D.velocity = new Vector2(0f, RB2D.velocity.y);
        }
        // let the player jump after falling down from platform
        if (Input.GetButtonDown("Jump") && coyoteCounter>0)
        {
            RB2D.velocity = new Vector2(RB2D.velocity.x, jumpHeight);
            Anim.SetTrigger("jump");
        }
        // let player double jump when coyote time is no longer
        if (Input.GetButtonDown("Jump") && additionalJumps > 0 && coyoteCounter ==0)
        {
            RB2D.velocity = new Vector2(RB2D.velocity.x, jumpHeight);
            Anim.SetTrigger("jump");
            additionalJumps--;
        }
    }
    void Grounded()
    {
        grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundRadius, WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                additionalJumps = defaultAdditionalJumps;
            }
            else
            {
                grounded = false;  
            }

        }
    }
    /// <summary>
    /// Give time to jump after falling from platforms
    /// </summary>
    void CheckForCoyoteTime()
    {
        if (grounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
            if (coyoteCounter < 0)
            {
                coyoteCounter = 0;
            }
        }
    }
    
    void Flip()
    {
        if (horizontalMove > 0 && !facingRight)
        {
            Vector2 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            facingRight = true;
        }
        else if (horizontalMove < 0 && facingRight)
        {
            Vector2 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            facingRight = false;
        }
    }
    void PlayAnimtionsBasedOnMovement()
    {
        Anim.SetFloat("velocityY", RB2D.velocity.y);
        Anim.SetBool("grounded", grounded);
        Anim.SetFloat("velocityX", Mathf.Abs(horizontalMove));
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        lastRespawnPos = respawnPos;
        if (other.CompareTag("FallDetector"))
        {
            gameManager.Respawn();
        }
        if (other.CompareTag("Chest"))
        {
            respawnPos = other.transform.position;
        }
    }
}
