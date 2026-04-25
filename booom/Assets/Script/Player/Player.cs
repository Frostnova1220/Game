using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }



    [Header("Move")]
    public float moveSpeed;
    public Vector2 moveInput { get; private set; }



    [Header("Animation")]
    public bool isWalking;
    public bool idle=true;


    [Header("Setup")]
    public GameObject bulletPrefab;
    public Transform firePoint;



    [Header("Config")]
    public float speed = 20f;
    public float lifeTime = 3f;




    private void Awake()
    {
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
    }






    public void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);//Instantiate是克隆函数 第一个是预制件 第二个是生成位置 第三个是生成时的朝向

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = firePoint.forward * speed;

            Destroy(bullet, lifeTime);
        }
    }


}
