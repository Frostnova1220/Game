using UnityEngine;

public class CursorSetter : MonoBehaviour
{
    public Texture2D cursorTexture; // 在Inspector里把刚才设置好的图片拖到这里

    private void Start()
    {
        // 设置鼠标指针样式
        Vector2 hotspot = Vector2.zero; // 指针的热点（点击位置），默认用图片左上角(0,0)
        Cursor.SetCursor(cursorTexture, hotspot, CursorMode.Auto);
    }
}
