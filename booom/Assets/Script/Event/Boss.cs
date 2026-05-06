using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : InteractableBase
{
    public GameObject boss;


    void Start()
    {
        interactPrompt = "∞¥EÃÙ’Ω";
    }

    public override void Interact(GameObject interactor)
    {
        boss.SetActive(true);
    }
}
