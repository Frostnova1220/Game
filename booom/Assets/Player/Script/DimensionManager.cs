using UnityEngine;

public class DimensionManager : MonoBehaviour
{
    public Player_X playerX;
    public Player_Z playerZ;

    private bool isXDimension = true;

    void Start()
    {
        playerX.OnX = true;
        playerZ.OnX = false;
        playerX.gameObject.SetActive(true);
        playerZ.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            ToggleDimension();
    }

    void ToggleDimension()
    {
        if (isXDimension)
        {
            // ÇÐµ½ Z
            isXDimension = false;
            playerZ.transform.position = playerX.transform.position;
            playerX.OnX = false;
            playerZ.OnX = false;
            playerX.gameObject.SetActive(false);
            playerZ.gameObject.SetActive(true);
        }
        else
        {
            // ÇÐµ½ X
            isXDimension = true;
            playerX.transform.position = playerZ.transform.position;
            playerX.OnX = true;
            playerZ.OnX = false;
            playerX.gameObject.SetActive(true);
            playerZ.gameObject.SetActive(false);
        }
    }
}