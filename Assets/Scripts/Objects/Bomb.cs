using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Animator animator;
    public Sprite bomb;
    private SpriteRenderer sr;
    private GameManager gameManager;
    private bool isLitted=false;
    [SerializeField] private GameObject indicator;
    [SerializeField] private float expDelay = 0.5f;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        gameManager = FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") & !isLitted)
        {
            sr.sprite = bomb;
            isLitted = true;
            indicator.SetActive(true);
            StartCoroutine("AnimateAndWait");
        }
    }
    public IEnumerator AnimateAndWait()
    {
        gameManager.littedBomb++;
        yield return new WaitForSeconds(expDelay);
        if (gameManager.player.insideOfArea==true && gameManager.littedBomb==1)
        {
            gameManager.Respawn();
        }
        gameManager.littedBomb--;
        indicator.SetActive(false);
        animator.SetTrigger("isLitted");
        FindObjectOfType<AudioManager>().PlaySound("BombExp");
        Destroy(this.gameObject);
    }
}
