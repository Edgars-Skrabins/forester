using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [Header("Settings:")]
    [SerializeField] protected bool DestroyOnInteract = false;
    [SerializeField] protected bool CallInteracted = false;
    
    [Space]
    [SerializeField] private string interactionDescription = "Interact";
    [SerializeField] private Outline outline;
    public UnityEvent interacted;
    
    protected virtual void Awake()
    {
        if (outline == null) { outline = GetComponent<Outline>(); }
        if (outline != null)
        {
            outline.OutlineMode = Outline.Mode.OutlineVisible;
            outline.OutlineWidth = 20f;
            outline.OutlineColor = Color.red;
            DisableOutline();
        }
    }

    public virtual void Interact()
    {
        if (CallInteracted)
        {
            interacted?.Invoke();
            Debug.Log("Interacted");
        }

        if (DestroyOnInteract)
        {
            Invoke(nameof(DisableGameObject), 0.1f);
        }
    }
    
    private void DisableGameObject()
    {
        interactionDescription = "";
        DisableOutline(); // disable safely first
        gameObject.SetActive(false);
    }

    public string GetInteractionDescription() => interactionDescription;

    public void EnableOutline()
    {
        if (outline == null || outline.Equals(null)) return;
        if (!outline.gameObject || outline.gameObject.Equals(null)) return;
        if (!outline.enabled) outline.enabled = true;
    }

    public void DisableOutline()
    {
        if (outline == null || outline.Equals(null)) return;
        if (!outline.gameObject || outline.gameObject.Equals(null)) return;
        if (outline.enabled) outline.enabled = false;
    }
}
