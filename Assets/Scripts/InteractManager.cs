using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractManager : MonoBehaviour
{
    [SerializeField] private Transform InteractOrigin;
    [SerializeField] private float interactDistance = 1f;
    [SerializeField] private Image crosshair;
    [SerializeField] private Color onHoverColor = Color.red;
    [SerializeField] private Color defaultColor = Color.gray;
    Vector3 hitPosition;
    [SerializeField] Inventory CurrentInventory;
        
    RaycastHit hit;
    void Update()
    {
// Casting ray
        if (Physics.Raycast(InteractOrigin.position, InteractOrigin.forward, out hit, interactDistance))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            crosshair.color = interactable ? onHoverColor : defaultColor;
        }
        else
        {
            crosshair.color = defaultColor;
        }
    }
    public void TryInteract()
    { 
        if (Physics.Raycast(InteractOrigin.position, InteractOrigin.forward, out hit, interactDistance))
        {
            Interactable interactable = hit.collider.GetComponent<Interactable>();
            if (interactable != null)
            {
                if(interactable.HasItem)
                        interactable.ConnectedItem.CurrentInventory = CurrentInventory;
                interactable.Interact(gameObject);

            }

        }
    }

 
    //draw the ray in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(InteractOrigin.position, InteractOrigin.forward * interactDistance);
        
        if(hitPosition != Vector3.zero)
            Gizmos.DrawSphere(hitPosition, 0.1f);
    }
}
