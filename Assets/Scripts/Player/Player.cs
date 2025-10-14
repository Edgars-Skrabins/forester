using UnityEngine;

public class Player : Singleton<Player>
{
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerInteraction playerInteraction;
    [SerializeField] private PlayerFootStep playerFootStep;
    [SerializeField] private PlayerFlashlight playerFlashlight;
    [SerializeField] private Transform pickUpParent;
    public Transform PickUpParent => pickUpParent;
    
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        playerInteraction?.Initialize();
        playerFootStep?.Initialize();
        playerFlashlight?.Initialize(this);
    }

    private void Update()
    {
        playerMovement?.Move();
        playerLook?.Look();
        playerInteraction?.HandleInteractions();
        playerFootStep?.HandleFootsteps();
        playerFlashlight?.Update();
    }

    public PlayerFlashlight GetPlayerFlashlight()
    {
        return playerFlashlight;
    }
}
