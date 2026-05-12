using UnityEngine;

public class EnableOnZ : MonoBehaviour
{
    private bool onX = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            onX = !onX;

        gameObject.SetActive(!onX);
    }
}