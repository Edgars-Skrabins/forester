using UnityEngine;

public class ElevatorButtonPanelInteractable : Interactable
{
    public ElevatorFloorsButtonPanelHandler m_panel;
    [SerializeField] private bool m_isInside;
    public override void Interact()
    {
        m_panel.Interaction(m_isInside);
    }
}
