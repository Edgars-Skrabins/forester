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
    [SerializeField] private List<GameObject> m_buttons;
    public ElevatorDoorHandler door;
    public UnityEvent ElevatorButtonAdded;
    public UnityEvent ElevatorButtonPressed;
    public UnityEvent PlayerLeftElevator;
    public UnityEvent PlayerEnteredElevator;
    public UnityEvent LeavingFloor;
    public bool outsideButtonCanOpen = true;
    public int currentSelectedFloor = -10;
    public int collectedUnplacedButton = -10;
    public List<GameObject> Buttons { get { return m_buttons; } set { m_buttons = value; } }
    public string CurrentState { get { return m_currentState; } set { m_currentState = value; } }
    private Player m_player;
    private float m_timeElapsedSinceLastInteraction = 0f; // in seconds
    private float m_minTimeBetweenInteractions = 0.2f; // in seconds
    private bool m_canBeInteractedWith = false;
    private string m_currentTargetFloorSceneName;


    private void EventInitializationAndReferenceGrabbing()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        ElevatorButtonAdded = new UnityEvent();
        ElevatorButtonPressed = new UnityEvent();
        PlayerLeftElevator = new UnityEvent();
        PlayerEnteredElevator = new UnityEvent();
        LeavingFloor = new UnityEvent();
    }

    private void Awake()
    {
        //Initialize My_Events
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
            m_floorManager.SetElevatorReferences();
            m_floorManager.elevatorOutsidePanel.GetComponent<ElevatorButtonPanelInteractable>().m_Elevator = this;
            Debug.Log("Found floor manager and outside button panel, references set.");
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

    private IEnumerator CloseBeforeLeaving(int nextFloorIndex)
    {
        door.CloseDoors(0f); 
        while (door.isOpened)
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
                    m_floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
                    m_outsideButtonPanel = m_floorManager.elevatorOutsidePanel;
                    m_floorManager.elevatorOutsidePanel.GetComponent<ElevatorButtonPanelInteractable>().SetElevatorReference(this);
                    m_floorManager.SetElevatorReferences();
                    door.OpenDoors(1.3f);
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
            if (currentSelectedFloor >= 0 && currentSelectedFloor <= 9)
            {
                Control(currentSelectedFloor);
            }else
            {
                Control();
            }
        }
        else if (outsideButtonCanOpen)
        {
            Control(); 
        }
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

    public void Control(float delay = 0f, int param = 0)
    {
        if (door.isSequencePlaying)
        {
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
                door.OpenDoors(delay);
                break;
            case "Close":
                door.CloseDoors(delay);
                break;
            case "OpenClose":
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
    public void Control(Sequence sequenceParam, string soundParam)
    {
        if (door.isSequencePlaying)
        {
            return;
        }
        switch (m_currentState)
        {
            case "Open":
                door.OpenDoors(sequenceParam, soundParam);
                break;
            case "Close":
                door.CloseDoors(sequenceParam, soundParam);
                break;
            case "OpenClose":
                if (door.isOpened)
                {
                    door.CloseDoors(sequenceParam, soundParam);
                }
                else
                {
                    door.OpenDoors(sequenceParam, soundParam);
                }
                break;

        }
    }
    public void Control(Sequence openSequenceParam, string openSoundParam, Sequence closeSequenceParam, string closeSoundParam)
    {
        if (door.isSequencePlaying)
        {
            return;
        }
        switch (m_currentState)
        {
            case "Open":
                door.OpenDoors(openSequenceParam, openSoundParam);
                break;
            case "Close":
                door.CloseDoors(closeSequenceParam, closeSoundParam);
                break;
            case "OpenClose":
                if (door.isOpened)
                {
                    door.CloseDoors(closeSequenceParam, closeSoundParam);
                }
                else
                {
                    door.OpenDoors(openSequenceParam, openSoundParam);
                }
                break;

        }
    }

    void Update()
    {
        IncrementInteractionTime();
    }
}
