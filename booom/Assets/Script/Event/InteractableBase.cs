using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableBase : MonoBehaviour, IInteractable
{
    [SerializeField]
    protected string interactPrompt;
    public virtual string getPrompt() => interactPrompt;
    public abstract void Interact(GameObject interactor);
}
