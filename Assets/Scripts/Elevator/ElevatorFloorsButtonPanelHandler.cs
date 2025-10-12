using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class ElevatorFloorsButtonPanelHandler : MonoBehaviour
{

    public bool hasButton = true; //Gonna be read from the data of the current floor
    public bool CanLeaveFloor = true; //Gonna be read from the data of the current floor
    private float m_timeElapsedSinceLastInteraction = 0f; // in seconds
    private float m_minTimeBetweenInteractions = 0.2f; // in seconds
    private bool m_canBeInteractedWith = false;
    [SerializeField]private GameObject m_buttonPanel;
    [SerializeField]private ElevatorDoorHandler m_door;

    [SerializeField] private List<GameObject> m_buttons;
    public List<GameObject> Buttons { get { return m_buttons; } set { m_buttons = value; } }
    public UnityEvent ElevatorButtonAdded;
    public UnityEvent ElevatorButtonPressed;
    public UnityEvent LeavingFloor;


    private void Awake()
    {
        //Initialize My_Events
        ElevatorButtonAdded = new UnityEvent();
        ElevatorButtonPressed = new UnityEvent();


    }

    private void OnDestroy()
    {
        ElevatorButtonAdded = null;
        ElevatorButtonPressed = null;
    }

    public void LeaveFloor(int nextFloorIndex)
    {
        m_door.CloseDoors(0f);
        //Intermission elevator music
        //Enter next scene / floor
        m_door.OpenDoors(0f);
    }

    public void Interaction()
    {
        if (m_canBeInteractedWith)
        {
            m_canBeInteractedWith = false;
            m_timeElapsedSinceLastInteraction = 0f;
            Debug.Log("Interacting wit panel");
            Testing();
        }
    }

    public void AddButton(int floorIndex)
    {
        m_buttons[floorIndex].SetActive(true);
        ElevatorButtonAdded.Invoke();
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

        /*
    The function of the Button panel is going to be state wise, meaning: one state will only enable adding the button, another will only enable pressing the button to leave the floor, another will allow player only to open,
    another only to close, another to open and close.
        */
    public void Testing()
    {
        if (m_door.isOpened)
        {
            m_door.CloseDoors(0f);
        }
        else
        {
            m_door.OpenDoors(0f);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        IncrementInteractionTime();   
    }
}
