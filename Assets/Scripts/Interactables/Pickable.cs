using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public abstract class Pickable : Interactable
{
    private Rigidbody rbody;
    private bool pickeUp;
    
    protected void Start()
    {
        if (rbody == null) { rbody = GetComponent<Rigidbody>(); }
        // if (rbody != null) { rbody.isKinematic = false; }
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
        if (rbody == null) { rbody = GetComponent<Rigidbody>(); }
        if (rbody != null) { rbody.isKinematic = true; }
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        GameManager.Instance.GetUIManager().HideAll();
        AudioManager.Instance.PlaySound("SFX_Paper_Pickup",transform.position);
    }
    
    private void Drop(Transform parent)
    {
        if (rbody == null) { rbody = GetComponent<Rigidbody>(); }
        
        if (rbody != null)
        {
            rbody.isKinematic = false;
        }
        transform.SetParent(null);
        GameManager.Instance.GetUIManager().ShowHUD();
        AudioManager.Instance.PlaySound("SFX_Paper_Drop",transform.position);
    }
}
