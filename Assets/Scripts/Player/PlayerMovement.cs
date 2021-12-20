using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Values")]
    public float horizontalMoveSpeed = 15f;
    public float jumpHeight;
    public float groundRadius;
    public float wallCheckRadius;
    public float wallSlidingSpeed;
    public float wallJumpHeight;
    public float coyoteTime = 0.2f;
    public float coyoteTimeWall = 0.2f;
    public float dashTime = 3f;
    public float wallJumpTime=0.5f;
    public int defaultAdditionalJumps = 1;
    public float jumpMultiplier;
    public float fallMultiplier;
    public float afterDashCooldown;
    public float dashDistance;
    public float distanceBetweenImage;
    public float airAcceleration;

    [Header("Checks")]
    public bool wallJumping;
    public bool hugingWall;
    public bool backWall;
    public bool wallSliding;
    public bool grounded;
    public bool facingRight;
    public bool isDashing;
    public float horizontalMove = 0f;
    public float keys = 0f;
    public Vector3 respawnPos;
    [Header("Objects")]
    public GameManager gameManager;
    public LayerMask WhatIsGround;
    public Transform ground1;
    public Transform ground2;
    public Transform wallCheckPoint;
    public ParticleSystem dashReady;
    public ParticleSystem run;

    private Vector2 wallJumpAngle = new Vector2(1,2);
    private ParticleSystem.ShapeModule runShape;
    private ParticleSystem.EmissionModule runEmision;
    private ParticleSystem.EmissionModule dashReadyEmision;
    private float lastImageXpos;
    private Vector2 workSpace;
    private bool canJump = true;
    private bool canDash = true;
    private bool canMove = true;
    private bool canWallJump = true;
    private Rigidbody2D RB2D;
    private Animator Anim;
    [Header("Controls")]

    [SerializeField] private int facingDirection;
    [SerializeField] private Vector3 lastRespawnPos;
    [SerializeField] private int additionalJumps;
    [SerializeField] private float dashCounter = 3f;
    [SerializeField] private float wallJumpCounter = 0.5f;
    [SerializeField] private float coyoteWallCounter = 0.2f;
    [SerializeField] private float coyoteCounter = 0.2f;
    void Start()
    {
        wallJumpAngle.Normalize();
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
        CheckSurroundings();
        SetParticals();
        CheckForAfterImage();
        CheckForCoyoteTime();
        CheckForDashTime();
        CheckForWallJumpTime();
    }
    void Update()
    {
        BetterJump();
        ApplyMovement();
        CheckIfWallSliding();
        CheckMovementDirection();
        CheckInput();
        PlayAnimtionsBasedOnMovement();
    }
    private void CheckInput()
    {
        if (canMove)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * horizontalMoveSpeed;
        }
        if (Input.GetButtonDown("Jump") && canJump && !wallSliding)
        {
            Jump();
        }
        if (Input.GetButtonDown("Jump") && canWallJump && wallSliding && !grounded)
        {
            wallJumping = true;
            canMove = false;
            canJump = false;
            WallJump();
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
            Dash();
        }
    }
    void Jump()
    {
        //when grounded
        if (coyoteCounter > 0)
        {
            SetVelocity(RB2D.velocity.x, jumpHeight);
            coyoteCounter = 0;
            additionalJumps--;
            Anim.SetTrigger("jump");
        }
        // let player double jump when coyote time is no longer
        else if (additionalJumps > 0 && coyoteCounter == 0)
        {
            SetVelocity(RB2D.velocity.x, jumpHeight);
            Anim.SetTrigger("jump");
            additionalJumps--;
        }  
    }
    void WallJump()
    {
        if (wallJumping && horizontalMove != 0)
        {
            SetVelocity(0f, 0f);
            Vector2 forcetoadd = new Vector2(wallJumpHeight * wallJumpAngle.x * facingDirection, jumpHeight * wallJumpAngle.y);
            RB2D.AddForce(forcetoadd, ForceMode2D.Impulse);
            //SetVelocity(wallJumpHeight, wallJumpAngle, facingDirection);
        }
        else if (wallJumping && horizontalMove == 0)
        {
            SetVelocity(0f, 0f);
            Vector2 forcetoadd = new Vector2(wallJumpHeight * wallJumpAngle.x * facingDirection, jumpHeight+5 * wallJumpAngle.y);
            RB2D.AddForce(forcetoadd, ForceMode2D.Impulse);
        }

    }
    void CheckMovementDirection()
    {
        if (!facingRight &&horizontalMove > 0)
        {
            Flip();
        }
        else if (facingRight && horizontalMove < 0)
        {
            Flip();
        }
    }
    void ApplyMovement()
    {
        //set x velocity
        if (canMove)
        {
            SetVelocity(horizontalMove, RB2D.velocity.y);
            if (horizontalMove == 0)
            {
                SetVelocity(0f, RB2D.velocity.y);
            }
        }
        //set air acceleration
        if (!grounded && !wallSliding && horizontalMove != 0 && !isDashing)
        {
            SetVelocity(horizontalMove*airAcceleration, RB2D.velocity.y);
        }
        //set wallslide speed
        if (wallSliding)
        {
            if (RB2D.velocity.y < -wallSlidingSpeed)
            {
                SetVelocity(RB2D.velocity.x, -wallSlidingSpeed);
            }
        }
    }
    void SetParticals()
    {
        if (horizontalMove > 0 && grounded)
        {
            runShape.rotation = new Vector3(1f, 270f, 0f);
            runEmision.rateOverTime = 10;
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
        if (RB2D.velocity.y < 0 && !(Input.GetButton("Jump")))
        {
            RB2D.velocity += Vector2.up * Physics.gravity.y * fallMultiplier * Time.deltaTime;
        }
        if (RB2D.velocity.y > 0 && !(Input.GetButton("Jump")))
        {
            RB2D.velocity += Vector2.up * Physics.gravity.y * jumpMultiplier * Time.deltaTime;
        }
    }
    void Dash()
    {
        if (isDashing)
        {
            SetVelocity(0f, 0f);
            Vector2 forcetoadd = new Vector2((dashDistance * facingDirection), 0f);
            RB2D.AddForce(forcetoadd, ForceMode2D.Impulse);
            //SetVelocity((dashDistance * facingDirection), 0f);
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
    private void CheckSurroundings()
    {
        grounded = Physics2D.Raycast(ground1.position, Vector2.down, -(groundRadius), WhatIsGround);
        grounded = Physics2D.Raycast(ground2.position, Vector2.down, -(groundRadius), WhatIsGround);
        if (grounded)
        {
            additionalJumps = defaultAdditionalJumps;
        }

        hugingWall = Physics2D.Raycast(wallCheckPoint.position, Vector2.right * facingDirection, wallCheckRadius, WhatIsGround);
        backWall = Physics2D.Raycast(wallCheckPoint.position, Vector2.right * -facingDirection, wallCheckRadius, WhatIsGround);
    }
    /// <summary>
    /// checks if sliding over wall
    /// </summary>
    public void CheckIfWallSliding()
    {
        if ((hugingWall || backWall) && grounded == false && RB2D.velocity.y < 0 && !isDashing)
        {
            Flip();
            wallSliding = true;
            additionalJumps = defaultAdditionalJumps;
        }
        else
        {
            wallSliding = false;
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
    void CheckForWallJumpTime()
    {
        if (wallSliding)
        {
            canWallJump = true;
            coyoteWallCounter = coyoteTimeWall;
        }
        else if (!wallSliding)
        {
            coyoteWallCounter -= Time.deltaTime;
            if (coyoteWallCounter<0)
            {
                coyoteWallCounter = 0;
                canWallJump = false;
            }
        }
        if (wallJumping)
        {
            wallJumpCounter -= Time.deltaTime;
            if (wallJumpCounter<0)
            {
                wallJumpCounter = 0;
            }
        }
        if (wallJumpCounter==0)
        {
            wallJumping = false;
            canMove = true;
            canJump = true;
            wallJumpCounter = wallJumpTime;
        }
    }
    void CheckForDashTime()
    {
        if (isDashing)
        {
            dashCounter -= Time.deltaTime;
            if (dashCounter < 0)
            {
                dashCounter = 0;
            }
        }
        if (dashCounter == 0)
        {

            RB2D.gravityScale = 5;
            isDashing = false;
            canMove = true;
            canJump = true;
        }
        if (dashCounter == 0 && (grounded || hugingWall))
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
    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workSpace.Set(angle.x * velocity * direction, angle.y * velocity);
        RB2D.velocity = workSpace;
    }
    void SetVelocity(float value1, float value2)
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
        if (!wallSliding)
        {
            Vector2 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            facingRight = !facingRight;
            facingDirection *= -1;
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
        if (other.CompareTag("Chest") && keys > 0)
        {
            respawnPos = other.transform.position;
        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheckPoint.position, new Vector3(wallCheckPoint.position.x + wallCheckRadius *facingDirection, wallCheckPoint.position.y, wallCheckPoint.position.z));
        Gizmos.DrawLine(ground2.position, new Vector3(ground2.position.x, ground2.position.y - groundRadius, ground2.position.z));
        Gizmos.DrawLine(ground1.position, new Vector3(ground1.position.x, ground1.position.y - groundRadius,ground1.position.z));
    }

}
