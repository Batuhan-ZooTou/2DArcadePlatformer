using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerMovement player;
    public float respawnDelay;
    public float score;
    public Canvas canvas;
    public Text chestOpened;
    public Text endText;
    [HideInInspector]public float littedBomb;
    public AudioSource deadSound;
    public AudioSource respawnSound;

    void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
    }
    


    public void Respawn()
    {
        deadSound.Play();
        StartCoroutine("RespawnCoroutine");
        player.canDash = true;
    }
    public IEnumerator RespawnCoroutine()
    {
        player.gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        player.transform.position = player.respawnPos;
        player.gameObject.SetActive(true);
        respawnSound.Play();
    }
    public void AddToScore(int scorePoint)
    {
        score += scorePoint;
        chestOpened.text = ("Treasure Claimed: " + score.ToString());
    }
    public IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void EndScene()
    {
        if (score==6)
        {
            endText.gameObject.SetActive(true);
            chestOpened.gameObject.SetActive(false);
            StartCoroutine("LoadScene");
        }
    }
    
}
