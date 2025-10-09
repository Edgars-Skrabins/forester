using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerInteraction
{
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private LayerMask interactableMask;
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
        }

        if (currentTarget != null)
        {
            currentTarget = null;
        }
    }
}