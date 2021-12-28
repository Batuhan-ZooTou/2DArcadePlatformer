using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            SceneManager.LoadScene(0);

        }
    }

    public void AppMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void PlayApp()
    {
        SceneManager.LoadScene(1);
    }
    public void AppQuit()
    {
        Application.Quit();
        Debug.Log("exited");
    }
}
