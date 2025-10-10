using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private PlayerFootStep playerFootStep;

    private void Start()
    {
        playerFootStep?.Initialize();
    }

    private void Update()
    {
        playerMovement.Move();
        playerLook.Look();
        playerInteraction.LookForInteraction();
        playerFootStep?.HandleFootsteps();
    }
}
