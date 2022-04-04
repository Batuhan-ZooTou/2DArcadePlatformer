using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    public float speed;
    public int spawnPoint;
    public Transform[] points;
    //[HideInInspector]
    public float direction;
    private int i;
    void Start()
    {
        transform.position = points[spawnPoint].position; //which point to start
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position,points[i].position)<0.1f)
        {
            i++;
            if (i==points.Length)
            {
                i = 0;
            }
        }
        CheckDirection();
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);
    }
    void CheckDirection()
    {
        if (transform.position.x<points[i].position.x)
        {
            direction= 1*speed;
        }
        else if (transform.position.x > points[i].position.x)
        {
            direction= - 1*speed;
        }
    }
}
