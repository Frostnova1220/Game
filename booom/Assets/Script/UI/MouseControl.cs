using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseControl : MonoBehaviour
{
    public GameObject ExitUI;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 按Escape键时，解锁并显示光标（例如：打开暂停菜单）
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitUI.SetActive(true);
            Time.timeScale = 0f;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Back()
    {
        ExitUI.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitWay()
    {
        SceneManager.LoadScene("StartGame");
        ExitUI.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;
    }
}