using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    public GameObject ExitUI;
    public GrenadeLauncher grenadeLauncher;

    public GameObject Gun1;
    public GameObject Gun2;

    public TextMeshProUGUI Gun;
    public Player_X player_x;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        GunPicture();

        // ÔÝÍ£
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitUI.SetActive(true);
            Time.timeScale = 0f;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Gun.text = $"{grenadeLauncher.currentAmmo}";

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
        if(player_x.Gun1)
        {
            Gun1.SetActive(true);
            Gun2.SetActive(false);
        }
        if(player_x.Gun2)
        {
            Gun1.SetActive(false);
            Gun2.SetActive(true);
        }
    }
}