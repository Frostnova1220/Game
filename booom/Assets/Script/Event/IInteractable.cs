using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string getPrompt();
    void Interact(GameObject interactor);
}