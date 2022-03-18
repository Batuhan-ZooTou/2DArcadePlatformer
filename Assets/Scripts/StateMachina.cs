using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachina : MonoBehaviour
{
    #region
    public enum OnAirState
    {
        Idle,
        Moving,
        Jumping,
        CoyoteJump,
        NotOnAir
    }
    public enum OnWallState
    {
        Sliding,
        Jumping,
        NotOnWall
    }
    public enum OnGroundState
    {
        Idle,
        Moving,
        Jumping,
        Descend,
        NotOnGround
    }
    public enum FacingDirection
    {
        Right,
        Left
    }
    [Header("Movement States (enums)")]
    public OnAirState onAirState;
    public OnGroundState onGroundState;
    public OnWallState onWallState;
    public FacingDirection facingDirection;
    #endregion

    #region
    public float apex = 1.5f;
    public float ClampVerticalSpeed;
    [Header("Values")]
    public float horizontalMoveSpeed = 15f;
    public float jumpHeight;
    public float groundRadius;
    public float wallCheckRadius;
    public float wallSlidingSpeed;
    public float wallJumpHeight;
    public float coyoteTime = 0.2f;
    public float coyoteTimeWall = 0.2f;
    public float reloadTime = 2f;
    public float dashTime = 3f;
    public float wallJumpTime = 0.5f;
    public int defaultAdditionalJumps = 1;
    public float jumpMultiplier;
    public float fallMultiplier;
    public float afterDashCooldown;
    public float dashDistance;
    public float distanceBetweenImage;
    public float airAcceleration;

    [Header("Checks")]
    public bool insideOfArea = false;
    public bool onRightWall;
    public bool canShoot=true;
    public bool onLeftWall;
    public bool wallJumping;
    public bool hugingWall;
    public bool backWall;
    public bool wallSliding;
    public bool grounded;
    public bool facingRight;
    public bool isDashing;
    public float horizontalMove = 0f;
    public float verticalMove;
    public bool abovePlatform;
    public float keys = 0f;
    public Vector3 respawnPos;
    [Header("Objects")]
    public GameManager gameManager;
    public ParticleSystem dashReady;
    public LayerMask WhatIsWall;
    public LayerMask WhatIsGround;
    public Transform ground1;
    public Transform ground2;
    public Transform wallCheckPoint;
    public GameObject fireball;

    private ParticleSystem.EmissionModule dashReadyEmision;
    private Vector2 wallJumpAngle = new Vector2(1, 2);
    private float lastImageXpos;
    private Vector2 workSpace;
    private bool canJump = true;
    public bool canDash = true;
    private bool canMove = true;
    private bool canWallJump = true;
    private Rigidbody2D RB2D;
    private Animator Anim;
    [Header("Controls")]

    [SerializeField] private bool onApexPoint;
    [SerializeField] private bool rise;
    [SerializeField] private bool stable;
    [SerializeField] private bool descend;
    [SerializeField] public int direction;
    [SerializeField] private Vector3 lastRespawnPos;
    [SerializeField] private int additionalJumps;
    [SerializeField] private float dashCounter = 3f;
    [SerializeField] private float wallJumpCounter = 0.1f;
    [SerializeField] private float coyoteWallCounter = 0.05f;
    [SerializeField] private float coyoteCounter = 0.1f;
    [SerializeField] private float fireballCounter = 2f;
    #endregion

    //[Header("data")]
    private void Awake()
    {
        respawnPos = transform.position;
        dashReadyEmision = dashReady.emission;
        Anim = GetComponent<Animator>();
        RB2D = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        StartCoroutine("CheckForApexPoint");
        StartCoroutine("CheckHeight");
        BetterJump();
        CheckSurroundings();
        CheckForAfterImage();
        CheckForWallJumpTime();
        CheckForCoyoteTime();
        CheckForDashTime();
        SetParticals();
        Reload();

    }
    private void Update()
    {
        PlayAnimtionsBasedOnMovement();
        CheckInput();
        SetState();
        CheckState();
        if (!hugingWall && !backWall)
        {
            onRightWall = false;
            onLeftWall = false;
        }
    }
    #region
    public void ChangeState(OnAirState onair,OnGroundState onground,OnWallState onwall,float direction)
    {
        onAirState = onair;
        onGroundState = onground;
        onWallState = onwall;
        if (direction>0)
        {
            facingDirection = FacingDirection.Right;
        }
        else if(direction<0)
        {
            facingDirection = FacingDirection.Left;
        }
        
    }
    public void SetState()
    {
        SetOnGroundState();
        SetOnAirState();
        SetOnWallState();
        SetCharacterDirection();
    }
    #endregion
    private void SetCharacterDirection()
    {
        Vector2 scale = transform.localScale;
        switch (facingDirection)
        {

            case FacingDirection.Right:
                scale.x = 0.75f;
                transform.localScale = scale;
                direction = 1;
                break;
            case FacingDirection.Left:
                scale.x = -0.75f;
                transform.localScale = scale;
                direction = -1;
                break;
        }
    }
    private void SetOnWallState()
    {
        switch (onWallState)
        {
            case OnWallState.NotOnWall:
                wallSliding = false;
                break;
            case OnWallState.Sliding:
                SetVelocity(RB2D.velocity.x, -wallSlidingSpeed);
                additionalJumps = defaultAdditionalJumps;
                break;
            case OnWallState.Jumping:
                SetVelocity(wallJumpHeight, wallJumpAngle, direction);
                FindObjectOfType<AudioManager>().Play("Jump");
                Anim.SetTrigger("jump");
                break;
        }
    }

    private void SetOnAirState()
    {
        switch (onAirState)
        {
            case OnAirState.NotOnAir:
                break;
            case OnAirState.Idle:
                break;
            case OnAirState.Moving:
                SetVelocity(horizontalMove * airAcceleration, RB2D.velocity.y);

                break;
            case OnAirState.Jumping:
                SetVelocity(RB2D.velocity.x, jumpHeight);
                FindObjectOfType<AudioManager>().Play("Jump");
                Anim.SetTrigger("jump");
                additionalJumps--;

                break;
            case OnAirState.CoyoteJump:
                SetVelocity(RB2D.velocity.x, jumpHeight);
                FindObjectOfType<AudioManager>().Play("Jump");
                Anim.SetTrigger("jump");
                break;
        }
    }
    private void SetOnGroundState()
    {
        switch (onGroundState)
        {
            case OnGroundState.NotOnGround:
                break;
            case OnGroundState.Descend:
                SetVelocity(RB2D.velocity.x, -jumpHeight / 2);

                break;
            case OnGroundState.Idle:
                SetVelocity(0f, RB2D.velocity.y);

                break;
            case OnGroundState.Moving:
                SetVelocity(horizontalMove, RB2D.velocity.y);

                break;
            case OnGroundState.Jumping:
                SetVelocity(RB2D.velocity.x, jumpHeight);
                coyoteCounter = 0;
                FindObjectOfType<AudioManager>().Play("Jump");
                Anim.SetTrigger("jump");
                break;
        }
    }
    private void CheckInput()
    {
        if (canMove && !isDashing)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * horizontalMoveSpeed;
            verticalMove = Input.GetAxisRaw("Vertical");
        }
        if (Input.GetButtonDown("Dash")  && canDash)
        {
            FindObjectOfType<AudioManager>().Play("Dash");
            AfterImagePool.Instance.GetFromPool();
            lastImageXpos = transform.position.x;
            horizontalMove = 0;
            RB2D.gravityScale = 0;
            isDashing = true;
            canDash = false;
            canJump = false;
            canMove = false;
            SetVelocity(0f, 0f);
            isDashing = true;
            canDash = false;
            canJump = false;
            canMove = false;
        }
        if (Input.GetAxisRaw("Fire1")>0 && canShoot)
        {
            canShoot = false;
            fireball.transform.position = new Vector2(wallCheckPoint.position.x + (0.5f*direction), wallCheckPoint.position.y);
            fireball.SetActive(true);
        }
    }
    void SetParticals()
    {
        if (canDash)
        {
            dashReadyEmision.rateOverTime = 10;
        }
        else
        {
            dashReadyEmision.rateOverTime = 0;
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
    void CheckState()
    {
        //when on ground
        if (grounded && !isDashing)
        {
            ChangeState(OnAirState.NotOnAir, OnGroundState.Idle,OnWallState.NotOnWall,horizontalMove);
            if (horizontalMove != 0)
            {
                ChangeState(OnAirState.NotOnAir, OnGroundState.Moving, OnWallState.NotOnWall,  horizontalMove);

            }
            if (Input.GetButtonDown("Jump") && coyoteCounter > 0 && canJump)
            {
                ChangeState(OnAirState.NotOnAir, OnGroundState.Jumping, OnWallState.NotOnWall,  horizontalMove);
                Debug.Log("normal jumped");
            }
            if (Input.GetButton("Jump") && verticalMove == -1 && abovePlatform && canJump)
            {
                ChangeState(OnAirState.NotOnAir, OnGroundState.Descend, OnWallState.NotOnWall,  horizontalMove);
            }
        }
        //when on air
        if (!grounded && !isDashing && !wallJumping && !isDashing)
        {
            ChangeState(OnAirState.Moving, OnGroundState.NotOnGround, OnWallState.NotOnWall,  horizontalMove);
            if (horizontalMove == 0 && !wallSliding)
            {
                ChangeState(OnAirState.Idle, OnGroundState.NotOnGround, OnWallState.NotOnWall, horizontalMove);
            }
            if (Input.GetButtonDown("Jump") && !wallSliding && canJump)
            {
                if (coyoteCounter > 0)
                {
                    ChangeState(OnAirState.CoyoteJump, OnGroundState.NotOnGround, OnWallState.NotOnWall,  horizontalMove);
                    Debug.Log("coyote jumped");

                }
                else if (coyoteWallCounter>0 || canWallJump )
                {
                    ChangeState(OnAirState.CoyoteJump, OnGroundState.NotOnGround, OnWallState.NotOnWall, horizontalMove);
                    Debug.Log("coyotewall jumped");
                }
                else if (additionalJumps > 0 && coyoteCounter == 0)
                {
                    ChangeState(OnAirState.Jumping, OnGroundState.NotOnGround, OnWallState.NotOnWall,  horizontalMove);
                    Debug.Log("double jumped");
                }
            }
        }
        //when wall slide
        if (!grounded && (hugingWall || backWall) && descend && horizontalMove != 0 && !isDashing)
        {
            //sliding from left
            if ((direction == -1 && horizontalMove < 0 && !onRightWall && !wallJumping) || (onLeftWall && !onRightWall && !wallJumping))
            {
                ChangeState(OnAirState.NotOnAir, OnGroundState.NotOnGround, OnWallState.Sliding,  -horizontalMove);
                wallSliding = true;
                onRightWall = false;
                onLeftWall = true;
                if (Input.GetButtonDown("Jump") && wallSliding && horizontalMove<0)
                {
                    ChangeState(OnAirState.NotOnAir, OnGroundState.NotOnGround, OnWallState.Jumping,  -horizontalMove);
                    Debug.Log("walljumped");
                    wallSliding = false;
                    wallJumping = true;
                }
                //moving right from left
                else if (horizontalMove > 0)
                {
                    ChangeState(OnAirState.Moving, OnGroundState.NotOnGround, OnWallState.NotOnWall,  horizontalMove);
                }
            }         
            //sliding from right
            if ((direction == 1 && horizontalMove > 0 && !onLeftWall && !wallJumping) || (!onLeftWall && onRightWall && !wallJumping))
            {
                ChangeState(OnAirState.NotOnAir, OnGroundState.NotOnGround, OnWallState.Sliding,  -horizontalMove);
                wallSliding = true;
                onLeftWall = false;
                onRightWall = true;
                if (Input.GetButtonDown("Jump") && wallSliding && horizontalMove > 0)
                {
                    ChangeState(OnAirState.NotOnAir, OnGroundState.NotOnGround, OnWallState.Jumping, -horizontalMove);
                    Debug.Log("walljumped");
                    wallSliding = false;
                    wallJumping = true;
                }
                //moving left from right
                else if (horizontalMove < 0)
                {
                    ChangeState(OnAirState.Moving, OnGroundState.NotOnGround, OnWallState.NotOnWall,  horizontalMove);
                }
            }
            

        }
        if (isDashing)
        {
            ChangeState(OnAirState.NotOnAir, OnGroundState.NotOnGround, OnWallState.NotOnWall, horizontalMove);
        }

    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            abovePlatform = true;
            if (Input.GetButton("Jump") && verticalMove == -1 && abovePlatform)
            {
                //other.gameObject.GetComponent<PlatformEffector2D>().surfaceArc = 0;
                other.gameObject.GetComponent<Collider2D>().isTrigger = true;
                other.gameObject.GetComponent<Platforms>().StartCoroutine("DoubleSidedPlatform");
            }
        }
        else
        {
            abovePlatform = false;
        }
    }
    void CheckForDashTime()
    {
        if (isDashing)
        {
            SetVelocity((dashDistance * direction), 0f);


            dashCounter -= Time.deltaTime;
            if (dashCounter < 0)
            {
                dashCounter = 0;
                SetVelocity(0f, 0f);

            }
        }
        if (dashCounter == 0)
        {
            RB2D.gravityScale = 4;
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
    public IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(afterDashCooldown);
        canDash = true;
    }
    void CheckForWallJumpTime()
    {
        if (wallSliding)
        {
            canWallJump = true;
            //canJump = false;
            coyoteWallCounter = coyoteTimeWall;
        }
        else if (!wallSliding)
        {
            coyoteWallCounter -= Time.deltaTime;
            if (coyoteWallCounter < 0)
            {
                coyoteWallCounter = 0;
                canWallJump = false;
                canJump = true;
            }
        }
      if (wallJumping)
      {
          canMove = false;
          canJump = false;
          wallJumpCounter -= Time.deltaTime;
          if (wallJumpCounter < 0)
          {
              wallJumpCounter = 0;
          }
      }
      if (wallJumpCounter == 0)
      {
          wallJumping = false;
          canMove = true;
          canJump = true;
          wallJumpCounter = wallJumpTime;
      }
    }
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
    void Reload()
    {
        if (canShoot)
        {
            fireballCounter = reloadTime;
        }
        else
        {
            fireballCounter -= Time.deltaTime;
            if (fireballCounter < 0)
            {
                fireballCounter = 0;
            }
            if (fireballCounter==0)
            {
                canShoot = true;
            }
        }
    }
    public IEnumerator CheckHeight()
    {
        float posY1;
        float posY2;

        posY1 = transform.position.y;
        yield return new WaitForFixedUpdate();
        posY2 = transform.position.y;
        //Debug.Log(posY1 + " " + posY2);
        if ((posY2 - posY1) > 0 && (posY2 - posY1) > 0.01f)
        {
            rise = true;
            descend = false;
            stable = false;
        }
        else if (posY2 == posY1)
        {
            stable = true;
            rise = false;
            descend = false;
        }
        else if ((posY2 - posY1) < 0 && Mathf.Abs(posY2 - posY1) > 0.01f)
        {
            descend = true;
            rise = false;
            stable = false;
        }
    }
    public IEnumerator CheckForApexPoint()
    {
        bool rised1;
        bool rised2;

        rised1 = rise;
        yield return new WaitForEndOfFrame();
        rised2 = rise;
        if (rised1 == true && rised2 == false)
        {
            if (!grounded)
            {
                onApexPoint = true;
                yield return new WaitForSeconds(0.2f);
                onApexPoint = false;
            }
        }
        if (wallSliding)
        {
            onApexPoint = false;
        }
    }
    public IEnumerator LimitMovement(bool canmove,bool canjump,float sec)
    {
        canMove = canmove;
        canJump = canjump;
        yield return new WaitForSeconds(sec);
        canMove = true;
        canJump = true;
        wallJumping = false;
        
    }
    void BetterJump()
    {
        if (descend&&RB2D.velocity.y<-ClampVerticalSpeed)
        {
            SetVelocity(RB2D.velocity.x, -ClampVerticalSpeed);
        }
        if (descend && !(Input.GetButton("Jump")))
        {
            RB2D.velocity += Vector2.up * Physics.gravity.y * fallMultiplier * Time.deltaTime;
        }
        if (rise && !(Input.GetButton("Jump")))
        {
            RB2D.velocity += Vector2.up * Physics.gravity.y * jumpMultiplier * Time.deltaTime;
        }
        if (descend && onApexPoint && (Input.GetButton("Jump")))
        {
            RB2D.velocity -= Vector2.up * Physics.gravity.y * apex * Time.deltaTime;
        }
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
    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workSpace.Set(angle.x * velocity * direction, angle.y * velocity);
        RB2D.velocity = workSpace;
    }
    #region

    private void CheckSurroundings()
    {
        bool grounded1 = Physics2D.Raycast(ground1.position, Vector2.down, -(groundRadius), WhatIsGround);
        bool grounded2 = Physics2D.Raycast(ground2.position, Vector2.down, -(groundRadius), WhatIsGround);
        if (grounded1 || grounded2)
        {
            grounded = true;
            additionalJumps = defaultAdditionalJumps;
            onApexPoint = false;
        }
        else
        {
            grounded = false;
        }
        hugingWall = Physics2D.Raycast(wallCheckPoint.position, Vector2.right * direction, wallCheckRadius, WhatIsWall);
        backWall = Physics2D.Raycast(wallCheckPoint.position, Vector2.right * -direction, wallCheckRadius, WhatIsWall);
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
        if (other.CompareTag("FallDetector") || other.CompareTag("Enemy"))
        {
            gameManager.Respawn();
        }
        if (other.CompareTag("Chest") && keys > 0)
        {
            respawnPos = other.transform.position;
        }
        if (other.CompareTag("BombIndicator"))
        {
            insideOfArea = true;
        }
        if (other.CompareTag("FinishArea"))
        {
            gameManager.EndScene();
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        lastRespawnPos = respawnPos;
        if (collision.gameObject.CompareTag("FallDetector"))
        {
            gameManager.Respawn();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("BombIndicator"))
        {
            insideOfArea = false;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheckPoint.position, new Vector3(wallCheckPoint.position.x + wallCheckRadius * direction, wallCheckPoint.position.y, wallCheckPoint.position.z));
        Gizmos.DrawLine(ground2.position, new Vector3(ground2.position.x, ground2.position.y - groundRadius, ground2.position.z));
        Gizmos.DrawLine(ground1.position, new Vector3(ground1.position.x, ground1.position.y - groundRadius, ground1.position.z));
    }
    #endregion

}
