using UnityEngine;

public class BackgroundX : MonoBehaviour
{
    [Header("移动速度")]
    public float scrollSpeed = 2.5f;

    private bool onX = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
            onX = !onX;

        if (!onX) return;  // OnX = false 时不移动

        float moveInput = Input.GetAxisRaw("Horizontal");
        float moveAmount = -moveInput * scrollSpeed * Time.deltaTime;

        transform.position += new Vector3(moveAmount, 0, 0);
    }
}