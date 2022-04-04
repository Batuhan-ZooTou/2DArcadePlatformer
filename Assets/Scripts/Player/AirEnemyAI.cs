using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public class AirEnemyAI : MonoBehaviour, IDamageable, IKnockable
{
    public AIPath aIPath;
    public AIDestinationSetter aIDestinationSetter;
    private float maxSpeed;

    private Vector2 workSpace;
    Rigidbody2D rb;
    public float healt = 100;
    private bool invincible = false;
    public Transform player;
    public Transform wanderPoint;
    public float wanderSpeed;
    public float followRange;
    public float maxDistance;
    public LayerMask cantTrack;
    private Vector2 wayPoint;
    private Vector2 spawnPoint;

    private void Awake()
    {
        spawnPoint = transform.position;
        maxSpeed = aIPath.maxSpeed;
        SetNewDestinationToWander();
        rb = GetComponent<Rigidbody2D>();

    }
    void Update()
    {

        if (PlayerInDistance())
        {
            aIPath.maxSpeed = maxSpeed;
            aIDestinationSetter.target = player;
        }
        else
        {
            wanderPoint.position = wayPoint;
            aIPath.maxSpeed = wanderSpeed;
            aIDestinationSetter.target = wanderPoint;
        }

        //Turns
        if (aIPath.desiredVelocity.x >= 0.01f)
        {
            transform.localScale = new Vector3(-1.7f, 1.7f, 1.7f);
        }
        else if (aIPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(1.7f, 1.7f, 1.7f);
        }

        //SetNewWander point
        if (Vector2.Distance(transform.position, wayPoint) < 0.5f)
        {
            SetNewDestinationToWander();
        }
        else
        {
            Collider2D[] ground = Physics2D.OverlapCircleAll(transform.position, 0.52f, cantTrack);
            foreach (Collider2D point in ground)
            {
                if (point == null)
                {
                    return;
                }
                else
                {
                    SetNewDestinationToWander();
                }
            }
        }
    }
    public void damage(float amount)
    {
        if (!invincible)
        {
            //Instantiate(hitParticles, transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
            Debug.Log(amount + "delt");
            healt -= amount;
            StartCoroutine("ResetInvincible");
            CheckIfDead();
        }
    }
    public void Knockback(Vector2 angle, float strength, int direction)
    {
        if (!invincible)
        {
            rb.AddForce(angle*strength*250*direction*Time.deltaTime);
            invincible = true;
        }
    }
    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workSpace.Set(angle.x * velocity * direction, angle.y * velocity);
        rb.velocity = workSpace;
    }
    public void CheckIfDead()
    {
        if (healt <= 0)
        {
            healt = 0;
            Destroy(this.gameObject);
        }
    }
    IEnumerator ResetInvincible()
    {
        yield return new WaitForSeconds(0.2f);
        invincible = false;
    }
    void SetNewDestinationToWander()
    {
        wayPoint = new Vector2(Random.Range(spawnPoint.x - maxDistance, spawnPoint.x + maxDistance), Random.Range(spawnPoint.y - maxDistance, spawnPoint.y + maxDistance));
    }
    private bool PlayerInDistance()
    {
        return Vector2.Distance(transform.position, player.transform.position) < followRange;
    }
}
