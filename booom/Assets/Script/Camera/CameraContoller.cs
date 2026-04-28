using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("跟随目标")]
    public Transform target;

    [Header("基准偏移（相对角色前方）")]
    public Vector3 offset;

    [Header("旋转设置")]
    public float rotationStep = 90f;
    public float rotationSpeed = 120f;

    [Header("平滑效果")]
    public float positionSmoothSpeed = 8f;
    public float rotationSmoothSpeed = 10f;

    private float currentAngle = 0f;        // 当前旋转角度（绕Y轴）
    private float XAngle = 0f;
    private float YAngle = 90f;
    private bool isRotating = false;        // 是否正在旋转
    private bool isX = true;
    private bool isY = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isRotating)
        {
            isX=!isX;
            isY=!isY;
            currentAngle += rotationStep;
            isRotating = true;
        }
    }

    void LateUpdate()
    { 
        if (isRotating&&isY)
        {
            currentAngle = Mathf.MoveTowards(currentAngle, YAngle, rotationSpeed * Time.deltaTime);
            if (Mathf.Approximately(currentAngle, YAngle))
            {
                currentAngle = YAngle;
                isRotating = false;
            }
        }

        if (isRotating&&isX)
        {
            currentAngle = Mathf.MoveTowards(currentAngle, XAngle, rotationSpeed * Time.deltaTime);
            if (Mathf.Approximately(currentAngle, XAngle))
            {
                currentAngle = XAngle;
                isRotating = false;
            }
        }



        Quaternion angleRotation = Quaternion.Euler(0f, currentAngle, 0f);
        Vector3 desiredPosition = target.position + angleRotation * offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed * Time.deltaTime);

        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSmoothSpeed * Time.deltaTime);
    }
}