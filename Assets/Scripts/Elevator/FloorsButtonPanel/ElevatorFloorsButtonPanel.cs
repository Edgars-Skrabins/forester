using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
using UnityEngine.InputSystem;

public class ElevatorFloorsButtonPanel : MonoBehaviour, IInteractable
{
    private int m_floorEventGroupsToInitialize = 10; //Number of Event Groups to initialize    Set on Awake to Number of Floors
    [SerializeField] private List<GameObject> m_buttons;
    public List<GameObject> Buttons { get { return m_buttons; } set { m_buttons = value; } }
    //List of Events called upon interaction with panel
    private List<UnityEvent> m_EventsOnInteraction;

    public void Interact()
    {
        Debug.Log("Interacted with Button Panel");
        EventConditionCheck(GameObject.FindWithTag("Player").GetComponent<Player>());
    }

    private void EventConditionCheck(Player player)
    {
        /*
        if (player.HasButton()>0)
        {
            UnityEvent currentEvent = m_EventsOnInteraction[0];
            m_EventsOnInteraction.RemoveAt(0);
            currentEvent.Invoke();
        }
        */
        AddButton(1); //Temporary Floor Number
    }

    private void AddButton(int floorNumber)
    {
        m_buttons[floorNumber].SetActive(true);
    }

    private void Awake()
    {
        //m_floorEventGroupsToInitialize = Floors.Count-1; //Set to Number of Floors -1

        //Initialize Events List if not already done
        if (m_EventsOnInteraction == null)
        {
            m_EventsOnInteraction = new List<UnityEvent>();
        }
        

    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
}
