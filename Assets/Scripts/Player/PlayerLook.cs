using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerLook
{
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform lookCamTransform;
    [SerializeField] private float sensitivity = 0.5f;
    
    private float xRotation = 0f;
    
    public void Look()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>() * sensitivity;
        
        playerTransform.Rotate(Vector3.up * lookInput.x);

        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        lookCamTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
