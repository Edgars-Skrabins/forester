using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerInteraction
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableMask;
    private Outline targetOutline;
    private Interactable currentInteractable;

    [SerializeField] private InputActionReference interactAction;

    public void Initialize()
    {
        interactAction.action.performed += ctx => currentInteractable?.Interact();
    }
    
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
                if (currentInteractable != hitInteractable)
                {
                    if (currentInteractable != null)
                        currentInteractable.DisableOutline();

                    currentInteractable = hitInteractable;
                    currentInteractable.EnableOutline();
                }

                // if (interactAction.action.triggered == true)
                // {
                //     currentInteractable.Interact();
                // }
            }
            else if (currentInteractable != hitInteractable && currentInteractable != null)
            {
                currentInteractable.DisableOutline();
                currentInteractable = null;
            }
        }
        else if (currentInteractable != null)
        {
            currentInteractable.DisableOutline();
            currentInteractable = null;
        }
    }
}