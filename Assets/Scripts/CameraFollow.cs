using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Player;
    public StateMachina player;
    public float offSet;
    public Vector3 offSetPos;
    public float moveSpeed;
    private void Start()
    {
        transform.position =new Vector3(Player.position.x, Player.position.y+2, Player.position.z - 10);
    }

    void Update()
    {
        if (player.velocity.y < -11)
        {
            offSetPos = new Vector3(Player.position.x, Player.position.y, Player.position.z - 10);
        }
        else if (player.horizontalMove>0 && !player.wallSliding)
        {
            offSetPos = new Vector3(Player.position.x + offSet, Player.position.y+2, Player.position.z - 10);
        }
        else if (player.horizontalMove<0 && !player.wallSliding)
        {
            offSetPos = new Vector3(Player.position.x - offSet, Player.position.y+2, Player.position.z - 10);
        }
        else
        {
            offSetPos = new Vector3(Player.position.x, Player.position.y+2, Player.position.z - 10);
        }
        transform.position = Vector3.Lerp(transform.position, offSetPos, Time.deltaTime*moveSpeed);
    }
}
