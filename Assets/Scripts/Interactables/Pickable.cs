using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public abstract class Pickable : Interactable
{
    private Rigidbody rigidbody;
    private bool pickeUp;
    
    protected void Start()
    {
        if (rigidbody == null) { rigidbody = GetComponent<Rigidbody>(); }
        if (rigidbody != null) { rigidbody.isKinematic = false; }
    }
    
    public override void Interact()
    {
        base.Interact();
        if (!pickeUp)
        {
            PickUp(Player.Instance.PickUpParent);
            pickeUp = true;
        }
        else
        {
            Drop(Player.Instance.PickUpParent);
            pickeUp = false;
        }
    }
    
    private void PickUp(Transform parent)
    {
        if (rigidbody == null) { rigidbody = GetComponent<Rigidbody>(); }
        if (rigidbody != null) { rigidbody.isKinematic = true; }
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }
    
    private void Drop(Transform parent)
    {
        if (rigidbody == null) { rigidbody = GetComponent<Rigidbody>(); }
        
        float throwForce = 50f;
        
        if (rigidbody != null)
        {
            rigidbody.isKinematic = false;
            // rigidbody.AddForce(transform.position-parent.position * throwForce, ForceMode.Impulse);
        }
        transform.SetParent(null);
    }
}
