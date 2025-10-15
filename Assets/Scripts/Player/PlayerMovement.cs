using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class PlayerMovement
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float walkingSpeed = 3f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float gravityMultiplier = 10f;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference sprintAction;
    
    private float speed = 3f;
    private Vector3 velocity;

    public void Move()
    {
        if(GameManager.Instance.gameState != GameState.Playing)
            return;
        
        if (moveAction == null || controller == null)
            return;

        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 input = new Vector3(moveInput.x, 0, moveInput.y);

        if (input.sqrMagnitude > 0.01f)
        {
            if (sprintAction != null && sprintAction.action.IsPressed())
            {
                speed = walkingSpeed * sprintMultiplier;
            }
            else
            {
                speed = walkingSpeed;
            }
            Vector3 move = controller.transform.TransformDirection(input.normalized) * speed;
            controller.Move(move * Time.deltaTime);
        }
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        if (!controller.isGrounded)
        {
            velocity += Physics.gravity * gravityMultiplier * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f; // small push down to keep grounded
        }

        controller.Move(velocity * Time.deltaTime);
    }
}