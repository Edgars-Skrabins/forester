using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private PlayerFootStep playerFootStep;
    [SerializeField] private PlayerFlashlight playerFlashlight;

    private void Start()
    {
        playerFootStep?.Initialize();
        playerFlashlight?.Initialize(this);
    }

    private void Update()
    {
        playerMovement?.Move();
        playerLook?.Look();
        playerInteraction?.LookForInteraction();
        playerFootStep?.HandleFootsteps();
        playerFlashlight?.Update();
    }
}
