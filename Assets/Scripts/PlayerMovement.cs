using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Values")]
    public float horizontalMoveSpeed = 15f;
    public float jumpHeight;
    public float groundRadius;
    public float coyoteTime=0.2f;
    public float dashTime = 3f;
    public int defaultAdditionalJumps = 1;
    public float jumpMultiplier;
    public float fallMultiplier;
    public float afterDashCooldown;
    public float dashDistance;
    public float distanceBetweenImage;
    
    [Header("Checks")]
    public bool grounded;
    public bool facingRight;
    public bool isDashing;
    public float horizontalMove = 0f;
    public float keys=0f;
    public Vector3 respawnPos;
    [Header("Objects")]
    public GameManager gameManager;
    public LayerMask WhatIsGround;
    public Transform ground1;
    public Transform ground2;
    public ParticleSystem dashReady;
    public ParticleSystem run;

    private ParticleSystem.ShapeModule runShape;
    private ParticleSystem.EmissionModule runEmision;
    private ParticleSystem.EmissionModule dashReadyEmision;
    private float lastImageXpos;
    private Vector2 workSpace;
    private bool canJump = true;
    private bool canDash=true;
    private bool canMove=true;
    private int facingDirection;
    private Rigidbody2D RB2D;
    private Animator Anim;
    [Header("Controls")]
    [SerializeField] private Vector3 lastRespawnPos;
    [SerializeField] private int additionalJumps;
    [SerializeField] private float dashCounter = 3f;
    [SerializeField] private float coyoteCounter=0.2f;
    void Start()
    {
        runShape = run.shape;
        runEmision = run.emission;
        dashReadyEmision = dashReady.emission;
        facingDirection = 1;
        respawnPos = new Vector3(0, -1, 0);
        RB2D = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
    }
    private void FixedUpdate()
    {
        SetParticals();
        CheckForAfterImage();
        CanMove();
        Dash();
        BetterJump();
        Grounded();
        PlayAnimtionsBasedOnMovement();
        CheckForCoyoteTime();
        CheckForDashTime();
    }

    void Update()
    {
        if (canMove)
        {
            dashDistance = 10;
            horizontalMove = Input.GetAxisRaw("Horizontal") * horizontalMoveSpeed;
        }
        if (horizontalMove>0)
        {
            SetVelocity(horizontalMove, RB2D.velocity.y);
            Flip();
        }
        else if (horizontalMove <0)
        {
            SetVelocity(horizontalMove, RB2D.velocity.y);
            Flip();
        }
        else
        {
            dashDistance = 20;
            SetVelocity(0f, RB2D.velocity.y);
        }
        // let the player jump after falling down from platform
        if (Input.GetButtonDown("Jump") && coyoteCounter>0 && canJump)
        {
            SetVelocity(RB2D.velocity.x, jumpHeight);
            Anim.SetTrigger("jump");
        }
        // let player double jump when coyote time is no longer
        if (Input.GetButtonDown("Jump") && additionalJumps > 0 && coyoteCounter ==0&& canJump)
        {
            SetVelocity(RB2D.velocity.x, jumpHeight);
            Anim.SetTrigger("jump");
            additionalJumps--;
        }
        if (Input.GetButtonDown("Dash") && canDash)
        {
            AfterImagePool.Instance.GetFromPool();
            lastImageXpos = transform.position.x;
            RB2D.gravityScale = 0;
            isDashing = true;
            canDash = false;
            canJump = false;
            canMove = false;
        }
    }
    void SetParticals()
    {
        if (horizontalMove>0 && grounded)
        {
            runShape.rotation = new Vector3(1f, 270f, 0f);
            runEmision.rateOverTime= 10;
        }
        else if (horizontalMove < 0 && grounded)
        {
            runShape.rotation = new Vector3(1f, 90f, 0f);
            runEmision.rateOverTime = 10;
        }
        else
        {
            runEmision.rateOverTime = 0;
        }
        if (canDash)
        {
            dashReadyEmision.rateOverTime = 10;
        }
        else
        {
            dashReadyEmision.rateOverTime = 0;
        }
    }
    void BetterJump()
    {
        if (RB2D.velocity.y<0 && !(Input.GetButton("Jump")))
        {
            RB2D.velocity += Vector2.up * Physics.gravity.y * fallMultiplier * Time.deltaTime;
        }
        if (RB2D.velocity.y > 0 &&!(Input.GetButton("Jump")))
        {
            RB2D.velocity += Vector2.up * Physics.gravity.y * jumpMultiplier * Time.deltaTime;
        }
    }
    void Dash()
    {
        if (isDashing)
        {
            SetVelocity(RB2D.velocity.x + (dashDistance * facingDirection), 0f);
        }
    }
    public void CheckForAfterImage()
    {
        if (isDashing)
        {
            if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImage)
            {
                AfterImagePool.Instance.GetFromPool();
                lastImageXpos = transform.position.x;
            }
        }
    }
    void CanMove()
    {
        if (!isDashing)
        {
            canMove = true;
            canJump = true;
        }
    }
    void Grounded()
    {
        grounded = false;
        RaycastHit2D hit1 = Physics2D.Raycast(ground1.position, Vector2.down, -(groundRadius), WhatIsGround);
        if (hit1.collider != null)
        {
            grounded = true;
            additionalJumps = defaultAdditionalJumps;
        }
        RaycastHit2D hit2 = Physics2D.Raycast(ground2.position, Vector2.down, -(groundRadius), WhatIsGround);
        if (hit2.collider != null)
        {
            grounded = true;
            additionalJumps = defaultAdditionalJumps;
        }
    }
    //Timers
    #region
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
    void CheckForDashTime()
    {
        if (isDashing)
        {
            dashCounter -= Time.deltaTime;
            if (dashCounter<0)
            {
                dashCounter = 0;
            }
        }
        if (dashCounter==0)
        {
            RB2D.gravityScale = 5;
            isDashing = false;
        }
        if (dashCounter==0 && grounded)
        {
            dashCounter = dashTime;
            StartCoroutine("DashCooldown");
        }
    }
    #endregion
    public IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(afterDashCooldown);
        canDash = true;
    }
    //set velocity
    #region
    void SetYVelocity(float value)
    {
        if (isDashing)
        {
            workSpace = new Vector2(0f, value);
            RB2D.velocity = workSpace;
        }
    }
    void SetXVelocity(float value)
    {
        if (isDashing)
        {
            workSpace = new Vector2(value, 0f);
            RB2D.velocity = workSpace;
        }
    }
    void SetVelocity(float value1,float value2)
    {
        workSpace = new Vector2(value1, value2);
        RB2D.velocity = workSpace;
        if (isDashing)
        {
            workSpace = new Vector2(value1, value2);
            RB2D.velocity = workSpace;
        }
    }
    #endregion

    void Flip()
    {
        if (horizontalMove > 0 && !facingRight)
        {
            Vector2 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            facingRight = true;
            facingDirection = 1;
        }
        else if (horizontalMove < 0 && facingRight)
        {
            Vector2 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            facingRight = false;
            facingDirection = -1;
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
        if (other.CompareTag("Chest") && keys>0)
        {
            respawnPos = other.transform.position;
        }

    }
    
}
