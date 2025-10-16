using UnityEngine;

public class Battery : Interactable
{
    public override void Interact()
    {
        base.Interact();
        Player.Instance.GetPlayerFlashlight().RechargeFlashlight();
    }
}
