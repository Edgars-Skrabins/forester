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
    private Interactable currentInteractable;

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
            hit.collider.TryGetComponent(out Interactable hitInteractable);

            if (hitInteractable != null)
            {
                // If looking at a new interactable, disable the old one
                if (currentInteractable != hitInteractable)
                {
                    if (currentInteractable != null)
                        currentInteractable.DisableOutline();

                    currentInteractable = hitInteractable;
                    currentInteractable.EnableOutline();
                }

                if (lookAction.action.triggered)
                {
                    currentInteractable.Interact();
                }
            }
        }
        else if (currentInteractable != null)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }
}