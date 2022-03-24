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
    [Header("Values")]
    public float ClampVerticalSpeed;
    public float apex = 1.5f;
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
    public bool knockedBack = false;
    public bool insideOfArea = false;
    public bool canShoot=true;
    public bool onRightWall=false;
    public bool onLeftWall=false;
    public bool wallJumping;
    public bool wallSliding;
    //public bool grounded=false;
    public bool isDashing;
    public float horizontalMove = 0f;
    public float verticalMove;
    public bool abovePlatform;
    public float keys = 0f;
    public int direction;
    public Vector3 respawnPos;

    [Header("Objects")]
    public GameManager gameManager;
    public ParticleSystem dashReady;
    public LayerMask WhatIsWall;
    public LayerMask WhatIsGround;
    public Transform ground1;
    public Transform ground2;
    public Transform wallCheckPoint;
    public GameObject Particals;
    public GameObject fireball;
    public Animator landAnim;

    [HideInInspector] public Vector2 velocity;
    private ParticleSystem.EmissionModule dashReadyEmision;
    private Vector2 wallJumpAngle;
    private Vector2 dashAngle;
    private float lastImageXpos;
    private Vector2 workSpace;
    private Rigidbody2D RB2D;
    private Animator Anim;

    [Header("Controls")]
    [SerializeField] private bool onApexPoint;
    [SerializeField] private bool rise;
    [SerializeField] private bool stable;
    [SerializeField] private bool descend;
    [SerializeField] private bool canJump = true;
    [SerializeField] public bool canDash = true;
    [SerializeField] private bool canMove = true;
    [SerializeField] private Vector3 lastRespawnPos;
    [SerializeField] private int additionalJumps;
    [SerializeField] private float dashCounter = 3f;
    [SerializeField] private float wallJumpCounter = 0.1f;
    [SerializeField] private float coyoteWallCounter = 0.05f;
    [SerializeField] private float coyoteCounter = 0.1f;
    [SerializeField] private float fireballCounter = 2f;
    #endregion
    private bool oldGrounded;
    private void Awake()
    {
        Time.timeScale = 1f;
        respawnPos = transform.position;
        dashReadyEmision = dashReady.emission;
        Anim = GetComponent<Animator>();
        RB2D = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (Time.timeScale==1)
        {
            StartCoroutine("CheckForApexPoint");
            StartCoroutine("CheckHeight");
            BetterJump();
            CheckForAfterImage();
            CheckForCoyoteTime();
            CheckForWallJumpTime();

            CheckForDashTime();
            SetParticals();
            Reload();
        }
    }
    private void Update()
    {
        if (Time.timeScale == 1)
        {
            CheckSurroundings();
            velocity = RB2D.velocity;
            ExitState();

            PlayAnimtionsBasedOnMovement();
            CheckInput();
            SetState();
            CheckState();
        }
    }
    private bool _grounded;
    public bool grounded
    {
        get { return _grounded; }
        set
        {
            //Check if the bloolen variable changes from false to true
            if (_grounded == false && value == true)
            {
                // Do something
                //Debug.Log("Boolean variable chaged from:" + _grounded + " to: " + value);
                Particals.transform.position = new Vector3(transform.position.x,transform.position.y-0.45f,transform.position.z);
                landAnim.SetTrigger("land");
                Debug.Log("landed");
            }
            //Update the boolean variable
            _grounded = value;
        }
    }
    #region
    public void ChangeState(OnAirState onair,OnGroundState onground,OnWallState onwall,float direction)
    {
        onAirState = onair;
        onGroundState = onground;
        onWallState = onwall;
        if (direction>0)
            facingDirection = FacingDirection.Right;
        else if(direction<0)
            facingDirection = FacingDirection.Left;
    }
    private void ChangeOnGroundState(OnGroundState onGround,float direction)
    {
        onGroundState = onGround;
        if (direction > 0)
            facingDirection = FacingDirection.Right;
        else if (direction < 0)
            facingDirection = FacingDirection.Left;
    }
    private void ChangeOnAirState(OnAirState onAir, float direction)
    {
        onAirState = onAir;
        if (direction > 0)
            facingDirection = FacingDirection.Right;
        else if (direction < 0)
            facingDirection = FacingDirection.Left;
    }
    private void ChangeOnWallState(OnWallState onWall, float direction)
    {
        onWallState = onWall;
        if (direction > 0)
            facingDirection = FacingDirection.Right;
        else if (direction < 0)
            facingDirection = FacingDirection.Left;
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
                if (verticalMove!=0)
                    dashAngle = new Vector2(0, verticalMove);
                else
                    dashAngle = new Vector2(1*direction, verticalMove);
                break;
            case OnAirState.Moving:
                SetVelocity(horizontalMove * airAcceleration, RB2D.velocity.y);
                break;
            case OnAirState.Jumping:
                SetVelocity(RB2D.velocity.x, jumpHeight);
                FindObjectOfType<AudioManager>().PlaySound("Jump");
                Anim.SetTrigger("jump");
                additionalJumps--;
                break;
            case OnAirState.CoyoteJump:
                SetVelocity(RB2D.velocity.x*direction, jumpHeight);
                FindObjectOfType<AudioManager>().PlaySound("Jump");
                Anim.SetTrigger("jump");
                break;
        }
    }
    private void SetOnGroundState()
    {
        switch (onGroundState)
        {
            case OnGroundState.NotOnGround:
                landAnim.SetBool("run", false);
                break;
            case OnGroundState.Descend:
                landAnim.SetBool("run", false);
                SetVelocity(RB2D.velocity.x, -jumpHeight / 2);
                Anim.SetTrigger("jump");
                break;
            case OnGroundState.Idle:
                landAnim.SetBool("run", false);
                SetVelocity(0f, RB2D.velocity.y);
                if (verticalMove != 0)
                {
                    dashAngle = new Vector2(0, verticalMove);
                }
                else
                {
                    dashAngle = new Vector2(1 * direction, verticalMove);
                }
                break;
            case OnGroundState.Moving:
                Particals.transform.position = new Vector3(transform.position.x, transform.position.y - 0.6f, transform.position.z);
                landAnim.SetBool("run", true);
                SetVelocity(horizontalMove, RB2D.velocity.y);
                break;
            case OnGroundState.Jumping:
                landAnim.SetBool("run", false);
                Particals.transform.position = new Vector3(transform.position.x, transform.position.y - 0.6f, transform.position.z);
                landAnim.SetTrigger("jump");
                SetVelocity(RB2D.velocity.x, jumpHeight);
                coyoteCounter = 0;
                FindObjectOfType<AudioManager>().PlaySound("Jump");
                Anim.SetTrigger("jump");
                break;
        }
    }
    private void CheckInput()
    {
        if (canMove && !isDashing && !wallJumping && !knockedBack)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * horizontalMoveSpeed;
            verticalMove = Input.GetAxisRaw("Vertical");
            dashAngle = new Vector2(direction,verticalMove);
        }
        if (Input.GetButtonDown("Dash")  && canDash)
        {
            FindObjectOfType<AudioManager>().PlaySound("Dash");
            AfterImagePool.Instance.GetFromPool();
            lastImageXpos = transform.position.x;
            horizontalMove = 0;
            RB2D.gravityScale = 0;
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
            knockedBack = true;
            StartCoroutine("KnockbackReset");
            RB2D.AddForce(new Vector2(5f * -direction, 0f),ForceMode2D.Impulse);
            FindObjectOfType<AudioManager>().PlaySound("Fireball");
        }
    }
    void SetParticals()
    {
        if (canDash)
            dashReadyEmision.rateOverTime = 10;
        else
            dashReadyEmision.rateOverTime = 0;
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
        //While on ground
        if (grounded && !isDashing)
        {
            //When idling
            ChangeOnGroundState(OnGroundState.Idle, horizontalMove);
            if (horizontalMove != 0)
            {
                //When moving
                ChangeOnGroundState(OnGroundState.Moving, horizontalMove);
            }
            if (Input.GetButtonDown("Jump") && coyoteCounter > 0 && canJump)
            {
                //When pressed jump while still on ground
                ChangeOnGroundState(OnGroundState.Jumping, horizontalMove);
                Debug.Log("normal jumped");
            }
            if (Input.GetButtonDown("Jump") && verticalMove == -1 && abovePlatform && canJump)
            {
                //When pressed jump while on top of platform
                ChangeOnGroundState(OnGroundState.Descend, horizontalMove);
                FindObjectOfType<AudioManager>().PlaySound("Jump");
            }
        }
        //While on air
        if (!grounded && !isDashing && !wallJumping && !wallSliding)
        {
            //When air accelerating
            ChangeOnAirState(OnAirState.Idle, horizontalMove);
            if (horizontalMove != 0)
            {
                //When idling
                ChangeOnAirState(OnAirState.Moving, horizontalMove);
            }
            //When pressed jump
            if (Input.GetButtonDown("Jump")  && canJump)
            {
                if (coyoteCounter > 0)
                {
                    //When tried to coyote jump
                    ChangeOnAirState(OnAirState.CoyoteJump, horizontalMove);
                    coyoteCounter = 0;
                    Debug.Log("coyote jumped");
                }
                else if (coyoteWallCounter > 0)
                {
                    //When tried to coyote jump from wall
                    ChangeOnAirState(OnAirState.CoyoteJump, horizontalMove);
                    coyoteWallCounter = 0;
                    Debug.Log("coyotewall jumped");
                }
                else if (additionalJumps > 0 && coyoteCounter == 0)
                {
                    //When jump for twice on air
                    ChangeOnAirState(OnAirState.Jumping, horizontalMove);
                    Debug.Log("double jumped");
                }
            }
            
        }
        //While on wall
        if (!grounded && (onLeftWall || onRightWall) && horizontalMove != 0 && !isDashing)
        {
            if (onLeftWall && descend && horizontalMove<0 && !wallJumping)
            {
                ChangeOnWallState(OnWallState.Sliding, 1);
                wallSliding = true;
                if (Input.GetButtonDown("Jump"))
                {
                    //When tried to jump while moving toward the wall
                    FindObjectOfType<AudioManager>().PlaySound("Jump");
                    Debug.Log("walljumped");
                    wallSliding = false;
                    wallJumping = true;
                    canMove = false;
                    canJump = false;
                    wallJumpAngle = new Vector2(1, 2);
                    ChangeOnWallState(OnWallState.Jumping, 1);
                }
            }
            if (onRightWall && descend && horizontalMove > 0 && !wallJumping)
            {
                ChangeOnWallState(OnWallState.Sliding, -1);
                wallSliding = true;
                if (Input.GetButtonDown("Jump"))
                {
                    //When tried to jump while moving towards the wall
                    //ChangeState(OnAirState.NotOnAir, OnGroundState.NotOnGround, OnWallState.Jumping, -horizontalMove);
                    FindObjectOfType<AudioManager>().PlaySound("Jump");
                    Debug.Log("walljumped");
                    wallSliding = false;
                    wallJumping = true;
                    wallJumpAngle = new Vector2(1, -2);
                    ChangeOnWallState(OnWallState.Jumping, -1);
                }
            }
        }
    }
    private void ExitState()
    {
        //While on ground
        if (grounded)
        {
            ChangeOnAirState(OnAirState.NotOnAir, horizontalMove);
            ChangeOnWallState(OnWallState.NotOnWall, horizontalMove);
        }
        if (!grounded && !wallJumping && !wallSliding)
        {
            ChangeOnWallState(OnWallState.NotOnWall, horizontalMove);
            ChangeOnGroundState(OnGroundState.NotOnGround, horizontalMove);
        }
        if (!grounded && (onLeftWall || onRightWall) && horizontalMove != 0 && !wallJumping)
        {
            ChangeOnGroundState(OnGroundState.NotOnGround, -horizontalMove);
        }
        if (horizontalMove==0 && wallSliding)
        {
            ChangeOnAirState(OnAirState.Idle, horizontalMove);
            ChangeOnWallState(OnWallState.NotOnWall, horizontalMove);
        }
        if (onLeftWall && horizontalMove > 0)
        {
            ChangeOnWallState(OnWallState.NotOnWall, horizontalMove);
            ChangeOnAirState(OnAirState.Moving, horizontalMove);
        }
        if (onRightWall && horizontalMove < 0)
        {
            ChangeOnWallState(OnWallState.NotOnWall, horizontalMove);
            ChangeOnAirState(OnAirState.Moving, horizontalMove);
        }
        if (isDashing)
        {
            //While dashing disable all others
            ChangeState(OnAirState.NotOnAir, OnGroundState.NotOnGround, OnWallState.NotOnWall, direction);           
        }
        if (!onLeftWall && !onRightWall && !wallJumping)
        {
            ChangeOnWallState(OnWallState.NotOnWall, horizontalMove);
        }

    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform"))
        {
            abovePlatform = true;
            if (Input.GetButton("Jump") && verticalMove == -1 && abovePlatform)
            {
                other.gameObject.GetComponent<Collider2D>().isTrigger = true;
                other.gameObject.GetComponent<Platforms>().StartCoroutine("DoubleSidedPlatform");
            }
        }
        else
            abovePlatform = false;
    }
    void CheckForDashTime()
    {
        if (isDashing)
        {
            SetVelocity(dashDistance*direction,dashAngle,direction);
            dashCounter -= Time.deltaTime;
            if (dashCounter < 0)
            {
                dashCounter = 0;
                SetVelocity(0f, 0f);
                RB2D.gravityScale = 4;
                isDashing = false;
                canMove = true;
                canJump = true;
            }
        }
        if (dashCounter == 0 && !isDashing )
        {
            RB2D.gravityScale = 4;
            canMove = true;
            canJump = true;
            if ((grounded || wallSliding))
            {
                dashCounter = dashTime;
                StartCoroutine("DashCooldown");
            }
        }
        if (canDash && !isDashing)
        {
            RB2D.gravityScale = 4;
            canMove = true;
            canJump = true;
            dashCounter = dashTime;
        }
    }
    public IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(afterDashCooldown);
        canDash = true;
    }
    public IEnumerator KnockbackReset()
    {
        yield return new WaitForSeconds(0.2f);
        knockedBack = false;
    }
    void CheckForWallJumpTime()
    {
        if (wallSliding)
        {
            coyoteWallCounter = coyoteTimeWall;
        }
        else
        {
            coyoteWallCounter -= Time.deltaTime;
            if (coyoteWallCounter < 0)
            {
                coyoteWallCounter = 0;
            }
        }
      if (wallJumping)
      {

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
        if (verticalMove<0)
        {
            ClampVerticalSpeed = 15;
        }
        else
        {
            ClampVerticalSpeed = 10;
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
        workSpace.Set(angle.x * velocity * direction, angle.y * velocity*direction);
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
        onRightWall = Physics2D.Raycast(wallCheckPoint.position, Vector2.right * 1, wallCheckRadius, WhatIsWall);
        onLeftWall = Physics2D.Raycast(wallCheckPoint.position, Vector2.right * -1, wallCheckRadius, WhatIsWall);
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
        if (other.CompareTag("BombIndicator"))
        {
            insideOfArea = true;
        }
        if (other.CompareTag("FinishArea"))
        {
            gameManager.EndScene();
        }
        if (other.CompareTag("DashReset"))
        {
            other.gameObject.SetActive(false);
            canDash = true;
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        lastRespawnPos = respawnPos;
        if (collision.gameObject.CompareTag("Enemy"))
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
        Gizmos.DrawLine(wallCheckPoint.position, new Vector3(wallCheckPoint.position.x + wallCheckRadius * 1, wallCheckPoint.position.y, wallCheckPoint.position.z));
        Gizmos.DrawLine(wallCheckPoint.position, new Vector3(wallCheckPoint.position.x + wallCheckRadius * -1, wallCheckPoint.position.y, wallCheckPoint.position.z));
        Gizmos.DrawLine(ground2.position, new Vector3(ground2.position.x, ground2.position.y - groundRadius, ground2.position.z));
        Gizmos.DrawLine(ground1.position, new Vector3(ground1.position.x, ground1.position.y - groundRadius, ground1.position.z));
    }
    #endregion

}
