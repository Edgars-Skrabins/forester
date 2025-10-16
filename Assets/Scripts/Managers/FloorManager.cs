using UnityEngine;
using System.Collections.Generic;

public abstract class FloorManager : Singleton<FloorManager>
{
    [Header("This list contains references to\n\t- Objects\n\t- Interactable Objects\n\n\tused in the scripted sequence\n")]
    [SerializeField] protected List<GameObject> ScriptedObjectReferences;
    [Space]
    public bool playerHasButton = true; //Gonna be read from the data of the current floor
    public bool canLeaveFloor = true; //Gonna be read from the data of the current floor
    protected GameObject elevator;
    protected ElevatorHandler elevatorHandler;
    protected int m_currentEventID = 0;

    public void SetElevatorReferences()
    {
        elevator = GameObject.FindWithTag("Elevator");
        elevatorHandler = elevator.GetComponent<ElevatorHandler>();
    }

    public void CheckIfCalledEventIDIsCurrentEventID(int i)
    {
        if (i == m_currentEventID) { ScriptedEvents(i); }
        
    }

    public void IncrementEventID()
    {
        m_currentEventID++;
    }

    public virtual void Start()
    {        
        try
        {
            SetElevatorReferences();
        }
        catch
        {
            Debug.LogError("No Elevator found in the scene?. Please make sure there is a GameObject with the tag 'Elevator'.");
        }        
    }

    public virtual void ScriptedEvents(int EventID)
    {
        switch (EventID)
        {
            case 0:
                Debug.Log("Event 0 triggered.");
                break;
            case 1:
                Debug.Log("Event 1 triggered.");
                break; // Ten more example debug logs
            case 2:
                Debug.Log("Event 2 triggered.");
                break;
            case 3:
                Debug.Log("Event 3 triggered.");
                break;
            default:
                Debug.LogWarning($"No scripted event found for EventID {EventID}.");
                break;
        }
    }

}
