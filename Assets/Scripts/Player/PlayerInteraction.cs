using TMPro;
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
    
    [SerializeField] private TMP_Text interactionDescription;
    [SerializeField] private InputActionReference interactAction;

    public void Initialize()
    {
    }

    public void HandleInteractions()
    {
        try
        {
            if (GameManager.Instance.gameState != GameState.Playing)
            {
                return;
            }
        }
        catch
        {
            Debug.LogWarning("No GameManager In Scene");
        }
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
                    
                    if (currentInteractable == null || currentInteractable.Equals(null))
                        return;
                    
                    currentInteractable.EnableOutline();
                    interactionDescription.text = currentInteractable.GetInteractionDescription();
                }

                if (interactAction.action.WasPerformedThisFrame())
                {
                    currentInteractable.Interact();
                }
            }
            else if (currentInteractable != hitInteractable && currentInteractable != null)
            {
                currentInteractable.DisableOutline();
                interactionDescription.text = "";
                currentInteractable = null; 
            }
        }
        else if (currentInteractable != null)
        {
            currentInteractable.DisableOutline();
            interactionDescription.text = "";
            currentInteractable = null;
        }
    }
}