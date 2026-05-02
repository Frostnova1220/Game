using UnityEngine;

public class DimensionManager : MonoBehaviour
{
    public GameObject playerX;
    public GameObject playerZ;
    public CameraController cameraController;

    private bool isXDimension = true;

    void Start()
    {
        HealthContainer container = FindObjectOfType<HealthContainer>();
        if (container != null)
        {
            Player_X px = playerX.GetComponent<Player_X>();
            if (px != null) px.healthContainer = container;

            Player_Z pz = playerZ.GetComponent<Player_Z>();
            if (pz != null) pz.healthContainer = container;
        }

        SetXDimension();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleDimension();
        }
    }

    void ToggleDimension()
    {
        if (isXDimension)
            SetZDimension();
        else
            SetXDimension();
    }

    void SetXDimension()
    {
        isXDimension = true;

        playerX.transform.position = playerZ.transform.position;

        playerX.SetActive(true);
        playerZ.SetActive(false);

        Player_X px = playerX.GetComponent<Player_X>();
        if (px != null) px.OnX = true;

        Player_Z pz = playerZ.GetComponent<Player_Z>();
        if (pz != null) pz.OnX = false;

        Debug.Log("切换到 X 维度");
    }

    void SetZDimension()
    {
        isXDimension = false;

        playerZ.transform.position = playerX.transform.position;

        playerX.SetActive(false);
        playerZ.SetActive(true);

        Player_X px = playerX.GetComponent<Player_X>();
        if (px != null) px.OnX = false;

        Player_Z pz = playerZ.GetComponent<Player_Z>();
        if (pz != null) pz.OnX = false;

        Debug.Log("切换到 Z 维度");
    }
}