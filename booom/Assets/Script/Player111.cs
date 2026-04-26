using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private PlayerInputSet input;
    float changeAngle = 0;

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }



    [Header("Move")]
    public float moveSpeed;
    public Vector2 moveInput { get; private set; }
    public int facingDir=1;


    [Header("Animation")]
    public bool isWalking;
    public bool idle=true;

    [Header("Setup")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public GameObject cam;


    [Header("Config")]
    public float speed = 20f;
    public float lifeTime = 3f;

    [Header("Shoot")]
    public bool isShooting;
    public bool cameraChange;



    private void OnEnable()
    {
        input.Enable();

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
        input.Player.Shoot.started += ctx => isShooting=true;
        input.Player.CameraChange.started += ctx => cameraChange = true;

    }





    private void Awake()
    {
        input = new PlayerInputSet();


        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }


    private void Start()
    {
        
    }

    private void Update()
    {


        Attack();

        MoveController();

        AnimationController();

        CameraChange();
    }


    private void OnDisable()
    {
        input.Disable();
    }


    public void MoveController()
    {
        Move();
    }

    private void Move()
    {
        float xInput = Input.GetAxisRaw("Horizontal");

        if (xInput != 0)
        {
            idle = false;
            isWalking = true;
        }

        else
        {
            idle=true;
            isWalking=false;
        }

        SetVelocity(xInput * moveSpeed, 0);
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.velocity = new Vector2(xVelocity, yVelocity);
    }

    public void AnimationController()
    {
        anim.SetBool("isWalking",isWalking );
        anim.SetBool("idle", idle);
        anim.SetBool("isShooting", isShooting);
    }






    public void Attack()
    {
        if (isShooting)
        {
            idle = false;
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);//Instantiate是克隆函数 第一个是预制件 第二个是生成位置 第三个是生成时的朝向

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = new Vector2(facingDir*speed,0);

            Destroy(bullet, lifeTime);
            isShooting = !isShooting;
        }
    }

    public void CameraChange()
    {
        if(cameraChange)
        {
            float changeSpeed=10f;

            if (cam.transform.eulerAngles.y < 90)
            {
                changeAngle += Time.deltaTime * changeSpeed;
                cam.transform.rotation = Quaternion.Euler(0, changeAngle, 0);
            }

            if(cam.transform.eulerAngles.y > 90)
            {
                cam.transform.rotation = Quaternion.Euler(0, 90, 0);
                cameraChange = !cameraChange;
                return;
            }

        }
    }

}
