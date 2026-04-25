using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public GameObject Title;

    public void SceneJump(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void UIJump()
    {
        Title.SetActive(true);
    }




}
