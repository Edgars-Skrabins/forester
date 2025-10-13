using UnityEngine;

[RequireComponent(typeof(Outline))]
public abstract class Interactable : MonoBehaviour
{
    private Outline outline;
    
    protected virtual void Start()
    {
        if (outline == null)
        {
            outline = GetComponent<Outline>();
        }
        DisableOutline();
    }
    
    public abstract void Interact();
    
    public void EnableOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }
    
    public void DisableOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
