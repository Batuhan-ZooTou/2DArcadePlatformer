using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void AppMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void AppQuit()
    {
        Application.Quit();
        Debug.Log("exited");
    }
}
