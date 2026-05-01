using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("基准偏移")]
    public Vector3 offset;

    [Header("旋转设置")]
    public float rotationSpeed = 120f;

    [Header("平滑效果")]
    public float positionSmoothSpeed = 8f;
    public float rotationSmoothSpeed = 10f;

    private float currentAngle = 0f;
    private float targetAngle = 0f;
    private bool isXView = true;   // true = 正面 0°, false = 侧面 90°

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Tab))
        {
            isXView = !isXView;
            targetAngle = isXView ? 0f : 90f;
        }
    }

    void LateUpdate()
    {
        currentAngle = Mathf.MoveTowards(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        Quaternion angleRotation = Quaternion.Euler(0f, currentAngle, 0f);
        Vector3 desiredPosition = target.position + angleRotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed * Time.deltaTime);

        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}