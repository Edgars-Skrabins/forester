using System.Collections;
using DG.Tweening;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ElevatorFloorsButtonPanelHandler : MonoBehaviour
{
    private bool TESTING = true;
    private int TESTING_Counter = 0;
    [SerializeField] private FloorManager m_floorManager;
    public bool outsideButtonCanOpen = true;
    private float m_timeElapsedSinceLastInteraction = 0f; // in seconds
    private float m_minTimeBetweenInteractions = 0.2f; // in seconds
    private bool m_canBeInteractedWith = false;
    [SerializeField] private string m_currentState = "Open";
    private string m_currentTargetFloorSceneName;
    public int currentPanelParam = -10;
    [SerializeField] private GameObject m_buttonPanel;
    [SerializeField] private GameObject m_outsideButtonPanel;
    [SerializeField] private ElevatorDoorHandler m_door;

    [SerializeField] private List<GameObject> m_buttons;
    public List<GameObject> Buttons { get { return m_buttons; } set { m_buttons = value; } }
    public UnityEvent ElevatorButtonAdded;
    public UnityEvent ElevatorButtonPressed;
    public UnityEvent LeavingFloor;

    public string CurrentState { get { return m_currentState; } set { m_currentState = value; } }

    private void Awake()
    {
        //Initialize My_Events
        ElevatorButtonAdded = new UnityEvent();
        ElevatorButtonPressed = new UnityEvent();
        LeavingFloor = new UnityEvent();
        if(GameObject.FindGameObjectsWithTag("Elevator").Length > 1 && this.gameObject.scene.buildIndex != -1)
        {
            Destroy(this, 0f);
        } else
        { DontDestroyOnLoad(this); }
        DontDestroyOnLoad(GameObject.FindGameObjectWithTag("Player"));


    }

    private void Start()
    {

        try
        {
            m_floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
            m_outsideButtonPanel = m_floorManager.elevatorOutsidePanel;
            m_floorManager.elevatorOutsidePanel.GetComponent<ElevatorButtonPanelInteractable>().m_panel = this;
        }
        catch
        {
            Debug.Log("Missing floor manager / outside button panel, don't forget to add them to the scene and add them as references.");
        }
    }

    private void OnDestroy()
    {
        ElevatorButtonAdded = null;
        ElevatorButtonPressed = null;
    }

    public IEnumerator LeaveFloor(int nextFloorIndex)
    {
        
        if (m_door.isOpened)
        {
            m_door.CloseDoors(0f);
            TESTING_Counter--;
        }
        else
        {
            //Intermission elevator music
            Scene scene = SceneManager.GetActiveScene();
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(m_currentTargetFloorSceneName, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = true;

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            SceneManager.UnloadSceneAsync(scene);

            m_floorManager = GameObject.Find("FloorManager").GetComponent<FloorManager>();
            m_outsideButtonPanel = m_floorManager.elevatorOutsidePanel;
            m_floorManager.elevatorOutsidePanel.GetComponent<ElevatorButtonPanelInteractable>().m_panel = this;
            m_door.OpenDoors(1f);

            LeavingFloor?.Invoke();

        }
    }

    public void Interaction(bool isPanelInside)
    {
        
        if (m_canBeInteractedWith)
        {
            m_canBeInteractedWith = false;
            m_timeElapsedSinceLastInteraction = 0f;


            if (TESTING && isPanelInside)
            {
                Debug.Log(TESTING_Counter);
                Testing();
                return;
            }


            if (isPanelInside)
            {
                Debug.Log("Interacting with panel Inside");
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
                Debug.Log("Interacting with panel Outside");
                Control();
            }
        }
    }

    public void AddButton(int floorIndex)
    {
        m_buttons[floorIndex].SetActive(true);
        m_currentTargetFloorSceneName = m_buttons[floorIndex].GetComponent<ElevatorButton>().targetFloorSceneName;
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
