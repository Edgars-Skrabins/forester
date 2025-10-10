using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class ElevatorFloorsButtonPanel : MonoBehaviour, IInteractable
{
    public bool m_hasButton = true;
    public bool m_CanLeaveFloor = true;
    private float m_timeElapsedSinceLastInteraction = 0f; // in seconds
    private float m_minTimeBetweenInteractions = 0.2f; // in seconds
    private bool m_canBeInteractedWith = false;
    [SerializeField] private List<GameObject> m_buttons;
    public List<GameObject> Buttons { get { return m_buttons; } set { m_buttons = value; } }
    //List of Events called upon interaction with panel
    /*
        public UnityEvent<List<UnityEvent>> m_SendFloorEvents;
        Each floor sends its own event group when invoking this event
        This event is currently in the ElevatorFloorsButtonPanel script, but will have to be moved the the Floor script.
        When in the floor script the floor will have to add "  AddFloorEventGroupEventsToList  " as a listener.
        And invoke when m_RequestFloorEvents event parameter is equal to the floors number
     */
    public UnityEvent m_ButtonAdded;
    public UnityEvent m_ButtonPressed;
    public UnityEvent m_DoorOpened;
    public UnityEvent m_DoorClosed;
    public UnityEvent m_LeavingFloor;

    private void Awake()
    {
        //Initialize My_Events
        m_ButtonAdded = new UnityEvent();
        m_DoorOpened = new UnityEvent();
        m_DoorClosed = new UnityEvent();
        m_ButtonPressed = new UnityEvent();


    }

    public void LeaveFloor(int nextFloorIndex)
    {

    }

    public void Interact()
    {
        if (m_canBeInteractedWith)
        {
            m_canBeInteractedWith = false;
            m_timeElapsedSinceLastInteraction = 0f;
        }
    }

    public void AddButton(int floorIndex)
    {
        m_buttons[floorIndex].SetActive(true);
        m_ButtonAdded.Invoke();
    }

    private void IncrementInteractionTime()
    {
        if (m_timeElapsedSinceLastInteraction >= m_minTimeBetweenInteractions)
        {
            m_canBeInteractedWith = true;
        }
        else if (m_timeElapsedSinceLastInteraction + Time.deltaTime < float.MaxValue)
        {
            m_timeElapsedSinceLastInteraction += Time.deltaTime;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        IncrementInteractionTime();   
    }
}
