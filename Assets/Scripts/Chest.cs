using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private SpriteRenderer SR;
    public Sprite openState;
    public ParticleSystem partical;
    private PlayerMovement playerMovement;
    private GameManager gameManager;
    public AudioSource pickedUp;
    public int scorePoint;
    private bool chestOpened=false;
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        playerMovement = FindObjectOfType<PlayerMovement>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && playerMovement.keys >0 && chestOpened==false)
        {
            gameManager.AddToScore(scorePoint);
            playerMovement.keys--;
            partical.Play();
            pickedUp.Play();
            SR.sprite = openState;
            partical = null;
            chestOpened = true;
        }
    }
}
