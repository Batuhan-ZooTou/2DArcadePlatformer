using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    public StateMachina player;
    public float respawnDelay;


    void Awake()
    {
        player = FindObjectOfType<StateMachina>();
    }
    public void Respawn()
    {
        StartCoroutine("RespawnCoroutine");
        player.canDash = true;
    }
    public IEnumerator RespawnCoroutine()
    {
        player.gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        player.transform.position = player.respawnPos;
        player.gameObject.SetActive(true);
    }

}