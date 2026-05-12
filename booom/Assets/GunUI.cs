using UnityEngine;

public class GunUI : MonoBehaviour
{
    public GameObject gun1;  // 拖 Gun1 UI 对象
    public GameObject gun2;  // 拖 Gun2 UI 对象

    private bool isGun1 = true;

    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isGun1 = !isGun1;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        if (gun1 != null) gun1.SetActive(isGun1);
        if (gun2 != null) gun2.SetActive(!isGun1);
    }
}