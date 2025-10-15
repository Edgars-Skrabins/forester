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
        if (CallInteracted) { interacted = new UnityEvent(); }
        if (outline == null) { outline = GetComponent<Outline>(); }
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.OutlineWidth = 20f;
        outline.OutlineColor = Color.red;
        DisableOutline();
    }

    public virtual void Interact()
    {
        if(CallInteracted) { interacted?.Invoke(); }
        if (DestroyOnInteract) { Destroy(this.gameObject, 0.1f);}
    }
    
    public string GetInteractionDescription()
    {
        return interactionDescription;
    }
    
    public void EnableOutline()
    {
        if (outline != null) { outline.enabled = true; }
    }
    
    public void DisableOutline()
    {
        if (outline != null) { outline.enabled = false; }
    }
}
