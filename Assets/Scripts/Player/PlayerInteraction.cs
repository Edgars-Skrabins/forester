using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerInteraction
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableMask;
    [SerializeField] private Transform inHandTransform;
    private Outline targetOutline;
    private Interactable currentInteractable;
    private Pickable currentPickable;
    
    [SerializeField] private InputActionReference interactAction;

    public void Initialize()
    {
    }

    public void HandleInteractions()
    {
        if (interactAction.action.WasPerformedThisFrame() && inHandTransform.childCount > 0)
        {
            currentPickable = inHandTransform.GetComponentInChildren<Pickable>();
            if (currentPickable != null)
            {
                currentPickable.Interact();
                Debug.Log(currentPickable.name + " was dropped!");
            }
        }
        else if (Physics.Raycast(
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

                if (interactAction.action.WasPerformedThisFrame())
                {
                    currentInteractable.Interact();
                }
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