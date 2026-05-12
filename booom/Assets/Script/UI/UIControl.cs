using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    public GameObject ExitUI;


    public GameObject Gun1;
    public GameObject Gun2;
    public bool isGun1;
    public bool isGun2;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // ÔÝÍ£
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

    public void GunPicture()
    {
        if(isGun1)
        {
            Gun1.SetActive(true);
            Gun2.SetActive(false);
        }
        if(isGun2)
        {
            Gun1.SetActive(false);
            Gun2.SetActive(true);
        }
    }
}