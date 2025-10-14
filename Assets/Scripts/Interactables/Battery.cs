using UnityEngine;

public class Battery : Interactable
{
    public override void Interact()
    {
        Player.Instance.GetPlayerFlashlight().RechargeFlashlight();
        Destroy(this.gameObject, 0.1f);
    }
}
