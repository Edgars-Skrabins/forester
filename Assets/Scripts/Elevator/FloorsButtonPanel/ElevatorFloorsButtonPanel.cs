using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class ElevatorFloorsButtonPanel : MonoBehaviour, IInteractable
{
    private int m_curentFloorEventGroopToInitialize = 0; //Index of Current Event Group to Initialize || respectively floor number - 1
    private int m_floorEventGroupsToInitialize = 10; //Number of Event Groups to initialize    Set on Awake to Number of Floors
    private float m_timeElapsedSinceLastInteraction = 0f; // in seconds
    private float m_minTimeBetweenInteractions = 0.2f; // in seconds
    private int m_TestNumberOfInteractions = 0;
    private bool m_canBeInteractedWith = false;
    [SerializeField] private List<GameObject> m_buttons;
    public List<GameObject> Buttons { get { return m_buttons; } set { m_buttons = value; } }
    //List of Events called upon interaction with panel
    private List<UnityEvent> m_EventsOnInteraction;

    //Events
    public UnityEvent<int> m_RequestFloorEvents;
    public UnityEvent<List<UnityEvent>> m_SendFloorEvents;

    //      Temporary parameters for testing
    private UnityEvent m_TestEvent1;
    private UnityEvent m_TestEvent2;
    private UnityEvent m_TestEvent3;
    private UnityEvent m_TestEvent4;
    private List<UnityEvent> m_TestFloorEventList;

    /*
        public UnityEvent<List<UnityEvent>> m_SendFloorEvents;
        Each floor sends its own event group when invoking this event
        This event is currently in the ElevatorFloorsButtonPanel script, but will have to be moved the the Floor script.
        When in the floor script the floor will have to add "  AddFloorEventGroupEventsToList  " as a listener.
        And invoke when m_RequestFloorEvents event parameter is equal to the floors number
     */
    public UnityEvent m_ButtonAdded;//name subject to change
    public UnityEvent m_CalledCurrentEventInList;//temporary name

    private void Awake()
    {
        //Initialize My_Events
        m_ButtonAdded = new UnityEvent();
        m_RequestFloorEvents = new UnityEvent<int>();
        m_SendFloorEvents = new UnityEvent<List<UnityEvent>>();

        //Add EventListeners
        m_SendFloorEvents.AddListener(AddFloorEventGroupEventsToList);
        
        //Testing
        m_TestEvent1 = new UnityEvent();
        m_TestEvent2 = new UnityEvent();
        m_TestEvent3 = new UnityEvent();
        m_TestEvent4 = new UnityEvent();
        m_TestEvent1.AddListener(TestFunction_HasButton1);
        m_TestEvent2.AddListener(() => Debug.Log("Test Event 2 Triggered"));
        m_TestEvent3.AddListener(TestFunction_HasButton3);
        m_TestEvent4.AddListener(() => Debug.Log("Test Event 4 Triggered"));
        m_TestFloorEventList = new List<UnityEvent> { m_TestEvent1, m_TestEvent2, m_TestEvent3, m_TestEvent4 };
        
        Debug.Log(m_TestFloorEventList==null);

        //Testing


        //Initialize Events List if not already done
        if (m_EventsOnInteraction == null)
        {
            m_EventsOnInteraction = new List<UnityEvent>();
        }

        //m_floorEventGroupsToInitialize = Floors.Count-1; //Set to Number of Floors -1
        while (m_curentFloorEventGroopToInitialize <= m_floorEventGroupsToInitialize)
        {
            m_RequestFloorEvents.Invoke(m_curentFloorEventGroopToInitialize);
            m_curentFloorEventGroopToInitialize++;
        }
    }

    private void TestFunction_HasButton1()
    {
        AddButton(1);
    }
    private void TestFunction_HasButton3()
    {
        AddButton(3);
    }

    public void Interact()
    {
        if (m_canBeInteractedWith)
        {
            m_TestNumberOfInteractions++;
            m_canBeInteractedWith = false;
            m_timeElapsedSinceLastInteraction = 0f;
            InvokeCurrentEvent();
        }
    }

    private void InvokeCurrentEvent()
    {
        if(m_EventsOnInteraction.Count >= 1)
        {
            m_EventsOnInteraction[0].Invoke();
            m_EventsOnInteraction.RemoveAt(0);
        } else
        {
            Debug.Log("Ran out of Elevator button panel events");
        }
    }

    private void EventConditionCheck(Player player) // temporarily in this script for testing purposes, will be in the floor script
    {
        
    }

    private void AddButton(int floorNumber)
    {
        m_buttons[floorNumber].SetActive(true);
        m_ButtonAdded.Invoke();
    }

    private void AddFloorEventGroupEventsToList(List<UnityEvent> floorEventGroup)
    {
        foreach (UnityEvent floorEvent in floorEventGroup)
        {
            m_EventsOnInteraction.Add(floorEvent);
        }
    }

    void Start()
    {

        m_SendFloorEvents.Invoke(m_TestFloorEventList);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_timeElapsedSinceLastInteraction >= m_minTimeBetweenInteractions)
        {
            m_canBeInteractedWith = true;
        } else if(m_timeElapsedSinceLastInteraction + Time.deltaTime < float.MaxValue)
        {
            m_timeElapsedSinceLastInteraction += Time.deltaTime;
        }
    }
}
