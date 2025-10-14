using System.Collections;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public enum ElevatorState
{
    AddButton,
    AddButtonAndLeave,
    LeaveFloor,
    Open,
    Close,
    OpenClose
}

public class ElevatorHandler : Singleton<ElevatorHandler>
{
    [SerializeField] private ElevatorState _currentState = ElevatorState.Open;

    public ElevatorState GetCurrentState()
    {
        return _currentState;
    }

    public void SetState(ElevatorState _newState)
    {
        _currentState = _newState;
    }
    
    [SerializeField] private FloorManager m_floorManager;
    [SerializeField] private bool m_isPlayerInside = false;
    [SerializeField] private GameObject m_buttonPanel;
    [SerializeField] private List<GameObject> m_buttons;
    public ElevatorDoorHandler door;
    public UnityEvent ElevatorButtonAdded;
    public UnityEvent ElevatorButtonPressed;
    public UnityEvent PlayerLeftElevator;
    public UnityEvent PlayerEnteredElevator;
    public UnityEvent LeavingFloor;
    
    public bool outsideButtonCanOpen = true;
    [SerializeField] private int m_currentSelectedFloor = -10;
    public int CurrentSelectedFloor { get { return m_currentSelectedFloor; } set { m_currentSelectedFloor = value; } }
    private float m_timeElapsedSinceLastInteraction = 0f; // in seconds
    private float m_minTimeBetweenInteractions = 0.2f; // in seconds
    private bool m_canBeInteractedWith = false;
    private string m_currentTargetFloorSceneName;
    
    private void Start()
    {
        EventInitializationAndReferenceGrabbing();

        try
        {
            m_floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
            // m_outsideButtonPanel = m_floorManager.elevatorOutsidePanel;
            m_floorManager.SetElevatorReferences();
            Debug.Log("Found floor manager and outside button panel, references set.");
        }
        catch
        {
            Debug.Log("Missing floor manager / outside button panel, don't forget to add them to the scene and add them as references.");
        }
        
        m_floorManager.ScriptedEvents(0);
    }
    
    private void EventInitializationAndReferenceGrabbing()
    {
        ElevatorButtonAdded = new UnityEvent();
        ElevatorButtonPressed = new UnityEvent();
        PlayerLeftElevator = new UnityEvent();
        PlayerEnteredElevator = new UnityEvent();
        LeavingFloor = new UnityEvent();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_isPlayerInside = true;
            PlayerEnteredElevator?.Invoke();
            other.transform.SetParent(this.transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_isPlayerInside = false;
            PlayerLeftElevator?.Invoke();
            other.transform.SetParent(null);
        }
    }

    private IEnumerator CloseBeforeLeaving(int nextFloorIndex)
    {
        door.CloseDoors(0f); 
        while (door.isOpened)
        {
            yield return null;
        }
        StartCoroutine(LeaveFloor(nextFloorIndex));
    }

    private IEnumerator LeaveFloor(int nextFloorIndex)
    {
        m_currentTargetFloorSceneName = m_buttons[nextFloorIndex].GetComponent<ElevatorButton>().targetFloorSceneName;
        if (m_isPlayerInside)
        {
            if (door.isOpened)
            {
                
                StartCoroutine(CloseBeforeLeaving(nextFloorIndex));
            } else
            {
                LeavingFloor?.Invoke();
                //Intermission elevator music
                Scene scene = SceneManager.GetActiveScene();
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(m_currentTargetFloorSceneName, LoadSceneMode.Additive);
                asyncLoad.allowSceneActivation = true;

                while (!asyncLoad.isDone)
                {
                    yield return null;
                }

                AsyncOperation asyncUnLoad = SceneManager.UnloadSceneAsync(scene); 

                while (!asyncUnLoad.isDone)
                {
                    yield return null;
                }
                try { 
                    m_floorManager = FindFirstObjectByType<FloorManager>();
                    // m_outsideButtonPanel = m_floorManager.elevatorOutsidePanel;
                    m_floorManager.SetElevatorReferences();
                    m_floorManager.ScriptedEvents(0);
                }
                catch
                {                    
                    Debug.Log("Missing floor manager / outside button panel, don't forget to add them to the scene and add them as references.");
                }
            }

        }
    }

    public void Interaction(bool isPanelInside)
    {
        if (isPanelInside)
        {
            if (m_currentSelectedFloor >= 0 && m_currentSelectedFloor <= 9)
            {
                Control(m_currentSelectedFloor);
            }
            else
            {
                Control();
            }
        }
        else if (outsideButtonCanOpen)
        {
            Control(); 
        }
    }

    private void AddButton(int floorIndex)
    {
        m_buttons[floorIndex].SetActive(true);
        ElevatorButtonAdded?.Invoke();
    }

    public void Control(float delay, int param = 0)
    {
        switch (_currentState){
            case ElevatorState.AddButton:
                if (m_floorManager.playerHasButton)
                {
                    AddButton(param);
                }
                break;
            case ElevatorState.AddButtonAndLeave:
                if (m_floorManager.playerHasButton)
                {
                    AddButton(param);
                    StartCoroutine(LeaveFloor(param));
                }
                break;
            case ElevatorState.LeaveFloor:
                if (m_floorManager.canLeaveFloor)
                {
                    StartCoroutine(LeaveFloor(param));
                }
                break;
            case ElevatorState.Open:
                door.OpenDoors(delay);
                break;
            case ElevatorState.Close:
                door.CloseDoors(delay);
                break;
            case ElevatorState.OpenClose:
                if (door.isOpened)
                {
                    door.CloseDoors(delay);
                }
                else
                {
                    door.OpenDoors(delay);
                }
                break;

        }
    }
    public void Control( int param = 0)
    {
        switch (_currentState)
        {
            case ElevatorState.AddButton:
                if (m_floorManager.playerHasButton)
                {
                    AddButton(param);
                }
                break;
            case ElevatorState.AddButtonAndLeave:
                if (m_floorManager.playerHasButton)
                {
                    AddButton(param);
                    StartCoroutine(LeaveFloor(param));
                }
                break;
            case ElevatorState.LeaveFloor:
                if (m_floorManager.canLeaveFloor)
                {
                    StartCoroutine(LeaveFloor(param));
                }
                break;
            case ElevatorState.Open:
                door.OpenDoors(0f);
                break;
            case ElevatorState.Close:
                door.CloseDoors(0f);
                break;
            case ElevatorState.OpenClose:
                if (door.isOpened)
                {
                    door.CloseDoors(0f);
                }
                else
                {
                    door.OpenDoors(0f);
                }
                break;

        }
    }
    public void Control(Sequence sequenceParam)
    {
        switch (_currentState)
        {
            case ElevatorState.Open:
                door.OpenDoors(sequenceParam);
                break;
            case ElevatorState.Close:
                door.CloseDoors(sequenceParam);
                break;
            case ElevatorState.OpenClose:
                if (door.isOpened)
                {
                    door.CloseDoors(sequenceParam);
                }
                else
                {
                    door.OpenDoors(sequenceParam);
                }
                break;

        }
    }
    public void Control(Sequence openSequenceParam,  Sequence closeSequenceParam)
    {
        switch (_currentState)
        {
            case ElevatorState.Open:
                door.OpenDoors(openSequenceParam);
                break;
            case ElevatorState.Close:
                door.CloseDoors(closeSequenceParam);
                break;
            case ElevatorState.OpenClose:
                if (door.isOpened)
                {
                    door.CloseDoors(closeSequenceParam);
                }
                else
                {
                    door.OpenDoors(openSequenceParam);
                }
                break;

        }
    }
}
