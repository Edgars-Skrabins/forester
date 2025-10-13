using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    private Outline outline;
    public UnityEvent interacted;
    [Space]
    [Header("Settings:")]
    public bool DestroyOnInteract = false;
    public bool CallInteracted = false;
    protected virtual void Start()
    {
        if (CallInteracted) { interacted = new UnityEvent(); }
        if (outline == null) { outline = GetComponent<Outline>(); }
        DisableOutline();
    }

    public virtual void Interact()
    {
        if(CallInteracted) { interacted?.Invoke(); }
        if (DestroyOnInteract) { Destroy(this); }
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
