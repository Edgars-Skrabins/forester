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


    }
    public override void ScriptedEvents(int EventID)
    {
        switch (EventID)
        {
            case -1:
                break;
            case 0:
                Debug.Log("Event: Open elevator doors 2 seconds after loading the scene.");
                elevatorHandler.CurrentState = "Open";
                elevatorHandler.Control(2f);           
                elevatorHandler.PlayerLeftElevator.AddListenerOnce(() => { 
                    EventID = 2; 
                    ScriptedEvents(EventID); });
                break;
            case 1:
                Debug.Log("Wait for button pickup.");
                ScriptedObjectReferences[0].GetComponent<PickupubleButtonInteractable>().interacted.AddListenerOnce(() => 
                { EventID = 4;
                    playerHasButton = true;
                    elevatorHandler.CurrentSelectedFloor = ScriptedObjectReferences[0].GetComponent<PickupubleButtonInteractable>().buttonIndex;
                    elevatorHandler.CurrentState = "OpenClose";
                    ScriptedEvents(EventID);
                });
                break;
            case 2:
                Debug.Log("Event 2 triggered.");
                elevatorHandler.CurrentState = "Close";
                elevatorHandler.Control();
                elevatorHandler.door.m_ElevatorDoorClosed.AddListenerOnce(() => { 
                    EventID = 1; 
                    ScriptedEvents(EventID); 
                });
                break;
            case 3:
                Debug.Log("Event 3 triggered.");
                elevatorHandler.ElevatorButtonAdded.AddListenerOnce(() => { EventID = 5;
                    canLeaveFloor = true;
                    elevatorHandler.CurrentState = "LeaveFloor";
                });
                break;
            case 4:
                Debug.Log("Event 4 triggered.");
                elevatorHandler.PlayerEnteredElevator.AddListenerOnce(() => {
                    EventID = 3;
                    elevatorHandler.CurrentState = "AddButtonAndLeave";
                    //ScriptedEvents(EventID);
                });
                break;
            default:
                Debug.LogWarning($"No scripted event found for EventID {EventID}.");
                break;
        }
    }
}
