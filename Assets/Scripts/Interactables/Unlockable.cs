using TMPro;
using UnityEngine;

public class Unlockable : Openable
{
    [SerializeField] private bool isLocked;
    [SerializeField] private string correctCode = "1234";

    public override void Interact()
    {
        if (isLocked)
        {
            Debug.Log("It's locked. You need a password to unlock it.");
            // Here you could add UI logic to prompt for a password
            AudioManager.Instance.PlaySound("SFX_Door_Locked", transform.position);
        }
        else
        {
            base.Interact();
        }
    }
    
    public void AttemptUnlock(string password)
    {
        if (password == correctCode)
        {
            isLocked = false;
            Debug.Log("Unlocked successfully!");
            base.Interact();
        }
        else
        {
            AudioManager.Instance.PlaySound("SFX_Door_Locked", transform.position);
            Debug.Log("Incorrect password.");
        }
    }
}