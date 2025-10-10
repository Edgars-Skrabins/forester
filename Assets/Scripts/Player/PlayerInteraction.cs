using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerInteraction
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableMask;
    private GameObject currentInteractableObject;
    private Outline targetOutline;
    private IInteractable currentTarget;

    [SerializeField] private InputActionReference lookAction;

    public void LookForInteraction()
    {
        if (Physics.Raycast(
                Camera.main.transform.position,
                Camera.main.transform.forward,
                out RaycastHit hit,
                interactRange,
                interactableMask
            ))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            targetOutline = hit.collider.GetComponent<Outline>();
            EnableTargetOutline(targetOutline);
            DisableUnfocusedOutline(hit.collider.gameObject);
            if (interactable != null)
            {
                if (currentTarget != interactable)
                {
                    currentTarget = interactable;
                }

                if (lookAction.action.IsPressed())
                {
                    interactable.Interact();
                }

                return;
            }
        } else if(currentInteractableObject != null)
        {
            DisableUnfocusedOutline(null);
        }

        if (currentTarget != null)
        {
            currentTarget = null;
        }
    }
    private void DisableUnfocusedOutline(GameObject newFocus)
    {
        if(currentInteractableObject != null)
        {
            Debug.Log("Checking Outline Removal Condition");
            if (currentInteractableObject != newFocus)
            {
                DisableTargetOutline(currentInteractableObject.GetComponent<Outline>());
            }
        }
        currentInteractableObject = newFocus;
    }
    private void EnableTargetOutline(Outline outline)
    {
        if(outline != null)
        {
            outline.OutlineMode = Outline.Mode.OutlineVisible;
        }
    }
    private void DisableTargetOutline(Outline outline)
    {
        Debug.Log("Removing Outline");
        if (outline != null)
        {
            outline.OutlineMode = Outline.Mode.OutlineHidden;
        }
    }
}