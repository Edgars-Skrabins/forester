using UnityEngine;

public class ElevatorButtonPanelInteractable : Interactable
{
    public ElevatorHandler m_Elevator;
    [SerializeField] private bool m_isInside;
    public void SetElevatorReference(ElevatorHandler handler)
    {
        m_Elevator = handler;
        Debug.Log("Setting: " + m_Elevator + " To: " + handler);
    }
    public override void Interact()
    {
        m_Elevator.Interaction(m_isInside);
    }
}
