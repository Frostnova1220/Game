using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Ωªª•…Ë÷√")]
    [SerializeField] private float interactRange;
    [SerializeField] private LayerMask interactableLayerMask;
    private IInteractable currentTarget; 
    private Player_X Player_X;

    private void Start()
    {
        Player_X = GetComponent<Player_X>();
    }

    void Update()
    {
        DetectInteractable();

        if (currentTarget != null && Input.GetKeyDown(KeyCode.E))
        {
            currentTarget.Interact(gameObject);
        }
    }


    void DetectInteractable()
    {
        Ray ray = new Ray(transform.position, transform.right*interactRange*Player_X.facingDir);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, interactRange, interactableLayerMask))
        {
            IInteractable interactable = hitInfo.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentTarget = interactable;
                Debug.Log(currentTarget.getPrompt());
                return; 
            }
        }
        currentTarget = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.right * interactRange*Player_X.facingDir);
    }
}