using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FloorManager_N : FloorManager
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
    
    public override void ScriptedEvents(int EventID)
    {
        switch (EventID)
        {
            case -1:
                break;
            case 0:
                Debug.Log("Event: Open elevator doors 2 seconds after loading the scene.");
                elevatorHandler.SetState(ElevatorState.Open);
                elevatorHandler.PlayerLeftElevator.AddListenerOnce(() => { // Condition to move to the next event
                    EventID = 1; 
                    ScriptedEvents(EventID); });
                elevatorHandler.Control(2f);           
                break;
            case 1:
                Debug.Log("Event Close Elevator Doors and Turn On Lights.");
                Sequence sequence = DOTween.Sequence();
                elevatorHandler.SetState(ElevatorState.Close);
                elevatorHandler.Control();
                break;
            case 2:
                Debug.Log("Event 2 triggered.");
                elevatorHandler.PlayerEnteredElevator.AddListenerOnce(() => {
                    //EventID = 3;
                    elevatorHandler.SetState(ElevatorState.AddButtonAndLeave);
                    //ScriptedEvents(EventID);
                });
                break;
            case 3:
                Debug.Log("Event 3 triggered.");
                elevatorHandler.ElevatorButtonAdded.AddListenerOnce(() => { EventID = 3;
                    canLeaveFloor = true;
                    elevatorHandler.SetState(ElevatorState.LeaveFloor);
                });
                break;
            default:
                Debug.LogWarning($"No scripted event found for EventID {EventID}.");
                break;
        }
    }

    private void WaitForButtonPickup(int buttonReferenceIndex, int nextEventID)
    {
        Debug.Log("Wait for button pickup.");
        ScriptedObjectReferences[buttonReferenceIndex].GetComponent<PickupubleButtonInteractable>().interacted.AddListenerOnce(() =>
        {
            playerHasButton = true;
            elevatorHandler.CurrentSelectedFloor = ScriptedObjectReferences[buttonReferenceIndex].GetComponent<PickupubleButtonInteractable>().buttonIndex;
            elevatorHandler.SetState(ElevatorState.OpenClose);
            ScriptedEvents(nextEventID);
        });
    }
}
