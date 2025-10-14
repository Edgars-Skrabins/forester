using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField] private Outline outline;
    public UnityEvent interacted;
    [Space]
    [Header("Settings:")]
    public bool DestroyOnInteract = false;
    public bool CallInteracted = false;
    protected virtual void Awake()
    {
        if (CallInteracted) { interacted = new UnityEvent(); }
        if (outline == null) { outline = GetComponent<Outline>(); }
        DisableOutline();
    }

    public virtual void Interact()
    {
        if(CallInteracted) { interacted?.Invoke(); }
        if (DestroyOnInteract) { Destroy(this.gameObject, 0.1f);}
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
