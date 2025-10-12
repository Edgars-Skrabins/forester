using UnityEngine;

public class ElevatorButtonPanelInteractable : MonoBehaviour, IInteractable
{
    public ElevatorFloorsButtonPanelHandler m_panel;
    [SerializeField] private bool m_isInside;
    public void Interact()
    {
        m_panel.Interaction(m_isInside);
    }
}
