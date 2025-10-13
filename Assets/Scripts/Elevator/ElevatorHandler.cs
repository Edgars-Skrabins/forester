using System.Collections;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ElevatorHandler : Singleton<ElevatorHandler>
{
    [SerializeField] private string m_currentState = "Open";
    [SerializeField] private FloorManager m_floorManager;
    [SerializeField] private bool m_isPlayerInside = false;
    [SerializeField] private GameObject m_buttonPanel;
    [SerializeField] private GameObject m_outsideButtonPanel;
    [SerializeField] private ElevatorDoorHandler m_door;
    [SerializeField] private List<GameObject> m_buttons;
    public UnityEvent ElevatorButtonAdded;
    public UnityEvent ElevatorButtonPressed;
    public UnityEvent PlayerLeftElevator;
    public UnityEvent PlayerEnteredElevator;
    public UnityEvent LeavingFloor;
    public bool outsideButtonCanOpen = true;
    public int currentPanelParam = -10;
    public List<GameObject> Buttons { get { return m_buttons; } set { m_buttons = value; } }
    public string CurrentState { get { return m_currentState; } set { m_currentState = value; } }
    private Player m_player;
    private bool TESTING = true;
    private int TESTING_Counter = 0;
    private float m_timeElapsedSinceLastInteraction = 0f; // in seconds
    private float m_minTimeBetweenInteractions = 0.2f; // in seconds
    private bool m_canBeInteractedWith = false;
    private string m_currentTargetFloorSceneName;




    private void Awake()
    {
        //Initialize My_Events
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        ElevatorButtonAdded = new UnityEvent();
        ElevatorButtonPressed = new UnityEvent();
        PlayerLeftElevator = new UnityEvent();
        PlayerEnteredElevator = new UnityEvent();
        LeavingFloor = new UnityEvent();
        if(GameObject.FindGameObjectsWithTag("Elevator").Length > 1 && this.gameObject.scene.buildIndex != -1)
        {
            Destroy(this.gameObject, 0f);
        } else
        { DontDestroyOnLoad(this.gameObject); }
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("Player"));


    }

    private void Start()
    {

        try
        {
            this.GetComponentInChildren<ElevatorButtonPanelInteractable>().m_Elevator = this;
            m_floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
            m_outsideButtonPanel = m_floorManager.elevatorOutsidePanel;
            m_floorManager.elevatorOutsidePanel.GetComponent<ElevatorButtonPanelInteractable>().m_Elevator = this;
            Debug.Log("Found floor manager and outside button panel, references set.");
            //debug log the m_panel of elevatorOutsidePanel
            Debug.Log("m_panel of elevatorOutsidePanel: " + m_floorManager.elevatorOutsidePanel.GetComponent<ElevatorButtonPanelInteractable>().m_Elevator);
        }
        catch
        {
            Debug.Log("Missing floor manager / outside button panel, don't forget to add them to the scene and add them as references.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_isPlayerInside = true;
            PlayerEnteredElevator?.Invoke();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_isPlayerInside = false;
            PlayerLeftElevator?.Invoke();
        }
    }

    private void OnDestroy()
    {
        ElevatorButtonAdded = null;
        ElevatorButtonPressed = null;
    }

    private IEnumerator CloseBeforeLeaving(int nextFloorIndex)
    {
        m_door.CloseDoors(0f); 
        while (m_door.isOpened)
        {
            yield return null;
        }
        StartCoroutine(LeaveFloor(nextFloorIndex));
    }

    public IEnumerator LeaveFloor(int nextFloorIndex)
    {
        m_currentTargetFloorSceneName = m_buttons[nextFloorIndex].GetComponent<ElevatorButton>().targetFloorSceneName;
        if (m_isPlayerInside)
        {
            if (m_door.isOpened)
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
                    m_floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
                    m_outsideButtonPanel = m_floorManager.elevatorOutsidePanel;
                    m_floorManager.elevatorOutsidePanel.GetComponent<ElevatorButtonPanelInteractable>().SetElevatorReference(this);
                    Debug.Log("Found floor manager and outside button panel, references set.");
                    //debug log the m_panel of elevatorOutsidePanel
                    Debug.Log("m_panel of elevatorOutsidePanel: " + m_floorManager.elevatorOutsidePanel.GetComponent<ElevatorButtonPanelInteractable>().m_Elevator);
                    m_door.OpenDoors(1f);
                }
                catch
                {                    
                    Debug.Log("Missing floor manager / outside button panel, don't forget to add them to the scene and add them as references.");
                }
            }

        }
        else
        {
            TESTING_Counter--;
        }
    }

    public void Interaction(bool isPanelInside)
    {
        
        /*if (m_canBeInteractedWith)
        {
            m_canBeInteractedWith = false;
            m_timeElapsedSinceLastInteraction = 0f;
        */

            if (TESTING && isPanelInside)
            {
                Debug.Log(TESTING_Counter);
                Testing();
                return;
            }


            if (isPanelInside)
            {
                if (currentPanelParam >= 0 && currentPanelParam <= 9)
                {
                    Control(currentPanelParam);
                }else
                {
                    Control();
                }
            }
            else if (outsideButtonCanOpen)
            {
                Control();
            }
        //}
    }

    public void AddButton(int floorIndex)
    {
        m_buttons[floorIndex].SetActive(true);
        ElevatorButtonAdded?.Invoke();
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

    public void Control(int param = 0)
    {
        if (m_door.isSequencePlaying)
        {
            TESTING_Counter--;
            return;
        }
        switch (m_currentState){
            case "AddButton":
                if (m_floorManager.playerHasButton)
                {
                    AddButton(param);
                }
                break;
            case "LeaveFloor":
                if (m_floorManager.canLeaveFloor)
                {
                    StartCoroutine(LeaveFloor(param));
                }
                break;
            case "Open":
                m_door.OpenDoors(0f);
                break;
            case "Close":
                m_door.CloseDoors(0f);
                break;
            case "OpenClose":
                if (m_door.isOpened)
                {
                    m_door.CloseDoors(0f);
                }
                else
                {
                    m_door.OpenDoors(0f);
                }
                break;

        }
    }
    public void Control(Sequence sequenceParam, string soundParam)
    {
        if (m_door.isSequencePlaying)
        {
            TESTING_Counter--;
            return;
        }
        switch (m_currentState)
        {
            case "Open":
                m_door.OpenDoors(sequenceParam, soundParam);
                break;
            case "Close":
                m_door.CloseDoors(sequenceParam, soundParam);
                break;
            case "OpenClose":
                if (m_door.isOpened)
                {
                    m_door.CloseDoors(sequenceParam, soundParam);
                }
                else
                {
                    m_door.OpenDoors(sequenceParam, soundParam);
                }
                break;

        }
    }
    public void Control(Sequence openSequenceParam, string openSoundParam, Sequence closeSequenceParam, string closeSoundParam)
    {
        if (m_door.isSequencePlaying)
        {
            TESTING_Counter--;
            return;
        }
        switch (m_currentState)
        {
            case "Open":
                m_door.OpenDoors(openSequenceParam, openSoundParam);
                break;
            case "Close":
                m_door.CloseDoors(closeSequenceParam, closeSoundParam);
                break;
            case "OpenClose":
                if (m_door.isOpened)
                {
                    m_door.CloseDoors(closeSequenceParam, closeSoundParam);
                }
                else
                {
                    m_door.OpenDoors(openSequenceParam, openSoundParam);
                }
                break;

        }
    }
    public void Testing()
    {
        TESTING_Counter++;
        if (TESTING_Counter == 2)
        {
            m_floorManager.playerHasButton = true;
            CurrentState = "AddButton";
            Control(5);
            CurrentState = "OpenClose";
        }
        else if (TESTING_Counter == 3)
        {
            CurrentState = "LeaveFloor";
            Control(5);
            CurrentState = "OpenClose";
        }
        else if (TESTING_Counter == 8)
        {
            CurrentState = "LeaveFloor";
            //debug current state and value of canLeaveFloor
            Debug.Log("Current State: " + CurrentState + " canLeaveFloor: " + m_floorManager.canLeaveFloor);
            Control(3);
            CurrentState = "OpenClose";
        }
        else
        {
            Control();
        }

    }

    void Update()
    {
        IncrementInteractionTime();
    }
}
