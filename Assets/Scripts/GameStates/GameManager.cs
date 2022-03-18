using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public LevelLoader levelLoader;
    public StateMachina player;
    public float respawnDelay;
    public float score;
    public Canvas canvas;
    public Text endText;
    public TextMeshProUGUI chestOpened;
    [HideInInspector]public float littedBomb;

    void Awake()
    {
        player = FindObjectOfType<StateMachina>();
        levelLoader = FindObjectOfType<LevelLoader>();
        StartCoroutine("StartMusic");
    }
    IEnumerator StartMusic()
    {
        yield return new WaitForSeconds(0.5f);
        FindObjectOfType<AudioManager>().PlaySound("LevelSound");

    }


    public void Respawn()
    {
        FindObjectOfType<AudioManager>().PlaySound("Death");
        StartCoroutine("RespawnCoroutine");
        
    }
    public IEnumerator RespawnCoroutine()
    {
        player.gameObject.SetActive(false);
        yield return new WaitForSeconds(respawnDelay);
        player.transform.position = player.respawnPos;
        player.gameObject.SetActive(true);
        player.canDash = true;
        player.grounded = true;
        FindObjectOfType<AudioManager>().PlaySound("Respawn");
    }
    public void AddToScore(int scorePoint)
    {
        score += scorePoint;
        chestOpened.text = ("Treasure Claýmed:" + score.ToString());
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
            levelLoader.LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
            //StartCoroutine("LoadScene");
        }
    }
    
}
