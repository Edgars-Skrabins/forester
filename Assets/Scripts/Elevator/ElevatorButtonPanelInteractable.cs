using UnityEngine;

public class ElevatorButtonPanelInteractable : Interactable
{
    [SerializeField] private bool m_isInside;
    
    public override void Interact()
    {
        ElevatorHandler.Instance.Interaction(m_isInside);
    }
}
