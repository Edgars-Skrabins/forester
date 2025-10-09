using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerLook playerLook;
    [SerializeField] PlayerInteraction playerInteraction;

    private void Update()
    {
        playerMovement.Move();
        playerLook.Look();
        playerInteraction.LookForInteraction();
    }
}
