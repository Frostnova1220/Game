using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public GameObject Text;

    public void Text1()
    {
        Text.SetActive(true);
    }

    public void Text2()
    {
        Text.SetActive(false);
    }

    public void GetWay()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitWay()
    {
        Application.Quit();
    }
}
