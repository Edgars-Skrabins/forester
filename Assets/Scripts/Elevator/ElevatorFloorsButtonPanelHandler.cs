using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class ElevatorFloorsButtonPanelHandler : MonoBehaviour
{

    public bool m_hasButton = true; //Gonna be read from the data of the current floor
    public bool m_CanLeaveFloor = true; //Gonna be read from the data of the current floor
    private float m_timeElapsedSinceLastInteraction = 0f; // in seconds
    private float m_minTimeBetweenInteractions = 0.2f; // in seconds
    private bool m_canBeInteractedWith = false;
    [SerializeField]private GameObject m_buttonPanel;
    [SerializeField]private ElevatorDoorHandler m_door;

    [SerializeField] private List<GameObject> m_buttons;
    public List<GameObject> Buttons { get { return m_buttons; } set { m_buttons = value; } }
    public UnityEvent m_ElevatorButtonAdded;
    public UnityEvent m_ElevatorButtonPressed;
    public UnityEvent m_LeavingFloor;


    private void Awake()
    {
        //Initialize My_Events
        m_ElevatorButtonAdded = new UnityEvent();
        m_ElevatorButtonPressed = new UnityEvent();


    }

    private void OnDestroy()
    {
        m_ElevatorButtonAdded = null;
        m_ElevatorButtonPressed = null;
    }

    public void LeaveFloor(int nextFloorIndex)
    {
        m_door.CloseDoors(0f);
        //Play Door Closing Animation
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
        m_ElevatorButtonAdded.Invoke();
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

    // Update is called once per frame
    void Update()
    {
        IncrementInteractionTime();   
    }
}
