using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    PlayerLook playerLook;
    PlayerInteraction playerInteraction;

    private void Update()
    {
        playerMovement.Move();
    }
}
