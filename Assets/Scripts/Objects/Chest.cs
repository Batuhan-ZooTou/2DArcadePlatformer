using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private SpriteRenderer SR;
    public Sprite openState;
    public ParticleSystem partical;
    private GameManager gameManager;
    public int scorePoint;
    private bool chestOpened=false;
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && other.GetComponent<StateMachina>().keys >0 && chestOpened==false)
        {
            gameManager.AddToScore(scorePoint);
            other.GetComponent<StateMachina>().keys -= 1;
            partical.Play();
            FindObjectOfType<AudioManager>().Play("PickUp");
            SR.sprite = openState;
            partical = null;
            chestOpened = true;
        }
    }
}
