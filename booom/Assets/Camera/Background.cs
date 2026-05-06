using UnityEngine;

public class Background : MonoBehaviour
{
    [Header("背景图片")]
    public Sprite backgroundSprite;

    [Header("视差系数（0=不动，1=跟紧摄像机）")]
    public float parallaxFactor = 0.5f;

    [Header("排序层级")]
    public int sortingOrder = -10;

    [Header("起始位置偏移")]
    public Vector3 startOffset = Vector3.zero;

    [Header("锁定轴")]
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = true;

    private Transform[] backgrounds = new Transform[3];
    private float bgWidth;
    private Vector3 previousCameraPosition;
    private bool initialized;

    void Start()
    {
        if (backgroundSprite == null)
        {
            Debug.LogError("请拖入一张背景 Sprite！");
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject obj = new GameObject("BG_" + i);
            obj.transform.parent = transform;

            SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
            sr.sprite = backgroundSprite;
            sr.sortingOrder = sortingOrder;

            backgrounds[i] = obj.transform;
        }

        bgWidth = backgroundSprite.bounds.size.x;

        // 以摄像机位置 + startOffset 作为中间那张的初始位置
        Vector3 centerPos = Camera.main.transform.position + startOffset;
        backgrounds[1].position = centerPos;
        backgrounds[0].position = centerPos + Vector3.left * bgWidth;
        backgrounds[2].position = centerPos + Vector3.right * bgWidth;

        previousCameraPosition = Camera.main.transform.position;
        initialized = true;
    }

    void Update()
    {
        if (!initialized) return;

        Vector3 rawDelta = Camera.main.transform.position - previousCameraPosition;
        Vector3 cameraDelta = new Vector3(
            lockX ? 0 : rawDelta.x,
            lockY ? 0 : rawDelta.y,
            lockZ ? 0 : rawDelta.z
        );

        Vector3 moveDelta = cameraDelta * parallaxFactor;

        foreach (Transform bg in backgrounds)
            bg.position += moveDelta;

        float centerX = backgrounds[1].position.x;

        if (backgrounds[0].position.x >= centerX)
            ShiftLeft();
        else if (backgrounds[2].position.x <= centerX)
            ShiftRight();

        previousCameraPosition = Camera.main.transform.position;
    }

    void ShiftLeft()
    {
        Transform temp = backgrounds[2];
        backgrounds[2] = backgrounds[1];
        backgrounds[1] = backgrounds[0];
        backgrounds[0] = temp;
        backgrounds[0].position = backgrounds[1].position + Vector3.left * bgWidth;
    }

    void ShiftRight()
    {
        Transform temp = backgrounds[0];
        backgrounds[0] = backgrounds[1];
        backgrounds[1] = backgrounds[2];
        backgrounds[2] = temp;
        backgrounds[2].position = backgrounds[1].position + Vector3.right * bgWidth;
    }
}