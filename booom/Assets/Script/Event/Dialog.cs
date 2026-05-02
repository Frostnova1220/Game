using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : InteractableBase
{
    public GameObject DialogUI;


    void Start()
    {
        interactPrompt = "按E交谈";   // 初始化提示，也可以在Inspector里直接填
    }

    public override void Interact(GameObject interactor)
    {
        StartDialog();
    }

    public void StartDialog()
    {
        DialogUI.SetActive(true);
    }
}
