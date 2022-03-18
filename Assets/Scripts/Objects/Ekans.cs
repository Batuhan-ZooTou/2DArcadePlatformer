using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ekans : MonoBehaviour
{
    public Transform wallCheckPoint;
    public float wallCheckRadius=0.5f;
    public bool nearWall;
    public LayerMask WhatIsWall;
    Tracking tracking;
    Vector2 spawnPos;
    public float moveRadius;
    Vector2 pos;
    public float moveSpeed=5f;
    Vector2 offset;

    public float direction=1;
    // Start is called before the first frame update
    void Start()
    {
        spawnPos = transform.position;
        tracking = GetComponentInChildren<Tracking>();
    }

    // Update is called once per frame
    void Update()
    {
        pos = tracking.playerPos;
        pos.y = transform.position.y;
        nearWall = Physics2D.Raycast(wallCheckPoint.position, Vector2.right * direction, wallCheckRadius, WhatIsWall);
        CheckIfShouldTurn();
        Vector2 scale = transform.localScale;
        if (tracking.isTracking==true)
        {
           

            if (pos.x>transform.position.x)
            {
                scale.x = 1;
                transform.localScale = scale;
                direction = 1;
            }
            else
            {
                scale.x = -1;
                transform.localScale = scale;
                direction = -1;
            }
            transform.position = Vector2.MoveTowards(transform.position,pos , Time.deltaTime * moveSpeed);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, offset, Time.deltaTime * (moveSpeed - 1));
        }

    }
    void CheckIfShouldTurn()
    {
        Vector2 scale = transform.localScale;
        if (direction==1)
        {
            offset = new Vector2(spawnPos.x + moveRadius, spawnPos.y);
            if (transform.position.x >= offset.x || nearWall)
            {
                scale.x *= -1;
                transform.localScale = scale;
                direction *= -1;
            }
        }
        else
        {
            offset = new Vector2(spawnPos.x - moveRadius, spawnPos.y);
            if (transform.position.x <= offset.x || nearWall)
            {
                scale.x *= -1;
                transform.localScale = scale;
                direction *= -1;
            }

        }

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(wallCheckPoint.position, new Vector3(wallCheckPoint.position.x + wallCheckRadius * direction, wallCheckPoint.position.y, wallCheckPoint.position.z));
    }
}
