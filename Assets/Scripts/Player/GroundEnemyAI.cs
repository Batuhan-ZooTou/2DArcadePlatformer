using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GroundEnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    public Transform target;
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;
    public Vector2 walkTimeRange;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpHeight;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;


    [Header("Collision Checks")]
    public float wallCheckRadius;
    public float groundRadius;
    private bool onRightWall = false;
    private bool onLeftWall = false;
    public LayerMask WhatIsWall;
    public LayerMask WhatIsGround;
    public Transform ground1;
    public Transform ground2;
    public Transform wallCheckPoint;
    private bool canJump=true;

    private float walkCounter = 0;
    private Path path;
    private int currentWaypoint = 0;
    Seeker seeker;
    Rigidbody2D rb;
    Animator anim;

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
                Debug.Log("landed");
            }
            //Update the boolean variable
            _grounded = value;
        }
    }
    public void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
    }

    private void FixedUpdate()
    {
        if (TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
        
        CheckSurroundings();
    }
    private void Update()
    {
            CheckIfTurn();

    }

    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
            anim.SetBool("Run", true);
        }
        else
        {
            anim.SetBool("Run", false);
        }
    }

    private void PathFollow()
    {
        if (path == null)
        {
            return;
        }

        // Reached end of path
        if (currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        // Jump
        if (jumpEnabled && grounded)
        {
            if (onRightWall || onLeftWall)
            {
                Debug.Log("jumpded");
                rb.AddForce(Vector2.up * jumpHeight);
                anim.SetTrigger("Jump");

            }
            else if(direction.y > 0.8f && canJump && grounded)
            {
                canJump = false;
                StartCoroutine("ResetJump");
                Debug.Log("jumpded");
                rb.AddForce(Vector2.up * jumpHeight*2.5f);
                anim.SetTrigger("Jump");
            }
        }

        // Movement
        rb.AddForce(force);

        // Next Waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // Direction Graphics Handling
        if (directionLookEnabled)
        {
            if (rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(1,1,1);
                
            }
            else if (rb.velocity.x < -0.05)
            {
                transform.localScale = new Vector3(-1,1,1);
                
            }
        }
    }
    void CheckIfTurn()
    {
        if (!TargetInDistance())
        {
            if (walkCounter<=0)
            {
                walkCounter = Random.Range(walkTimeRange.x, walkTimeRange.y);
            }
            else if (transform.localScale.x == 1)
            {
                walkCounter -= Time.deltaTime;
                rb.velocity = new Vector2(5,rb.velocity.y);
                anim.SetBool("Run", true);
                if (walkCounter<=0 || onRightWall)
                {
                    walkCounter = Random.Range(walkTimeRange.x, walkTimeRange.y);

                    transform.localScale = new Vector3(-1, 1, 1);
                }
            }
            else if (transform.localScale.x == -1)
            {
                walkCounter -= Time.deltaTime;
                rb.velocity = new Vector2(-5, rb.velocity.y);
                anim.SetBool("Run", true);
                if (walkCounter <= 0 || onLeftWall)
                {
                    walkCounter = Random.Range(walkTimeRange.x, walkTimeRange.y);

                    transform.localScale = new Vector3(1, 1, 1);
                }
            }
        }
    }
    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(3f);
        canJump = true;
    }
    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
    }
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    private void CheckSurroundings()
    {
        bool grounded1 = Physics2D.Raycast(ground1.position, Vector2.down, -(groundRadius), WhatIsGround);
        bool grounded2 = Physics2D.Raycast(ground2.position, Vector2.down, -(groundRadius), WhatIsGround);
        if (grounded1 || grounded2)
        {
            grounded = true;
            anim.SetBool("Ground", true);
        }
        else
        {
            grounded = false;
            anim.SetBool("Ground", false);
        }
        onRightWall = Physics2D.Raycast(wallCheckPoint.position, Vector2.right * 1, wallCheckRadius, WhatIsWall);
        onLeftWall = Physics2D.Raycast(wallCheckPoint.position, Vector2.right * -1, wallCheckRadius, WhatIsWall);
    }
}
