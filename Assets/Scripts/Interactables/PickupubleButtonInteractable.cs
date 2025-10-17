using UnityEngine;

public class PickupubleButtonInteractable : Interactable
{
    public int buttonIndex;
    public string musicTriggerName;
    
    public override void Interact()
    {
        base.Interact();
        ElevatorHandler.Instance.CurrentSelectedFloor = buttonIndex;
        FloorManager.Instance.playerHasButton = true;
        Debug.Log("Picked up button: " + buttonIndex);
        AudioManager.Instance.PlaySound(musicTriggerName);
    }
}
