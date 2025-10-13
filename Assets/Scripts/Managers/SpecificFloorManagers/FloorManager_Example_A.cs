using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FloorManager_Example_A : FloorManager
{
    Scene scene;
    private IEnumerator WaitOnLoad()
    {
        while (!scene.isLoaded)
        {
            yield return null;
        }
        ScriptedEvents(0);
    }
    private void Start()
    {
        scene = SceneManager.GetActiveScene();
        StartCoroutine(WaitOnLoad());


    }
    public override void ScriptedEvents(int EventID)
    {
        switch (EventID)
        {
            case -1:
                break;
            case 0:
                Debug.Log("Event: Open elevator doors after 2 seconds.");
                elevatorHandler.CurrentState = "Open";
                elevatorHandler.Control(2f);           
                elevatorHandler.PlayerLeftElevator.AddListenerOnce(() => { 
                    EventID = 2; 
                    ScriptedEvents(EventID); });
                break;
            case 1:
                Debug.Log("Event 1 triggered.");
                ScriptedObjectReferences[0].GetComponent<PickupubleButtonInteractable>().interacted.AddListenerOnce(() => 
                { EventID = 3;
                    playerHasButton = true;
                    elevatorHandler.currentSelectedFloor = ScriptedObjectReferences[0].GetComponent<PickupubleButtonInteractable>().buttonIndex;
                    elevatorHandler.CurrentState = "AddButton";
                    ScriptedEvents(EventID);
                });
                break;
            case 2:
                Debug.Log("Event 2 triggered.");
                elevatorHandler.CurrentState = "Close";
                elevatorHandler.Control();
                elevatorHandler.door.m_ElevatorDoorClosed.AddListenerOnce(() => { EventID = 1; ScriptedEvents(EventID); });
                break;
            case 3:
                Debug.Log("Event 3 triggered.");
                elevatorHandler.ElevatorButtonAdded.AddListenerOnce(() => { EventID = 4;
                    canLeaveFloor = true;
                    elevatorHandler.CurrentState = "LeaveFloor";
                });
                break;
            default:
                Debug.LogWarning($"No scripted event found for EventID {EventID}.");
                break;
        }
    }
}
