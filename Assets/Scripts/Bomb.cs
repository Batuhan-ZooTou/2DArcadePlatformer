using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Animator animator;
    private GameManager gameManager;
    [SerializeField] private GameObject indicator;
    [SerializeField] private float expDelay = 0.5f;
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            indicator.SetActive(true);
            StartCoroutine("AnimateAndWait");
        }
    }
    public IEnumerator AnimateAndWait()
    {
        yield return new WaitForSeconds(expDelay);
        if (gameManager.player.insideOfArea==true)
        {
            gameManager.Respawn();
        }
        indicator.SetActive(false);
        animator.SetTrigger("isLitted");
        Destroy(this.gameObject);
    }
}
