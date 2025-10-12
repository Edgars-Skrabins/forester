using UnityEngine;

public class ElevatorButtonPanelInsideInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] ElevatorFloorsButtonPanelHandler m_panel;
    public void Interact()
    {
        m_panel.Interaction();
    }
}
