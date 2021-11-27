using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerMovement player;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    // Update is called once per frame
    public void Respawn()
    {
        player.gameObject.SetActive(false);
        player.transform.position = player.respawnPos;
        player.gameObject.SetActive(true);
    }
}
