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
    private float endSceneTime=1f;

    void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
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
    public void AddToScore(int scorePoint)
    {
        score += scorePoint;
        chestOpened.text = ("Treasure Claimed: " + score.ToString());
    }
    public void EndScene()
    {
        if (score==6)
        {
            endText.gameObject.SetActive(true);
            chestOpened.gameObject.SetActive(false);
            endSceneTime -= Time.deltaTime;
            if (endSceneTime < 0)
            {
                endSceneTime = 0;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

                //Time.timeScale = 0;
            }
        }
    }
}
