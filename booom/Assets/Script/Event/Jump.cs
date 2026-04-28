using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : InteractableBase
{
    public Rigidbody rb;

    void Start()
    {
        interactPrompt = "按E跳跃";   // 初始化提示，也可以在Inspector里直接填
    }



    public override void Interact(GameObject interactor)
    {
        rb=interactor.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(rb.velocity.x, 10f,0);
    }

}
