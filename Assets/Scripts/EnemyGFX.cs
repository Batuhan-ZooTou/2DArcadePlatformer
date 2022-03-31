using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyGFX : MonoBehaviour
{
    public AIPath aIPath;
    
    void Update()
    {
        if (aIPath.desiredVelocity.x>=0.01f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (aIPath.desiredVelocity.x <= -0.01f)
        {
            transform.localScale = new Vector3(-1, 1, 1);

        }
    }
}
