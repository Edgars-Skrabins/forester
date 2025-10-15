using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FloorManager_Lobby : FloorManager
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
            case 0:
                Debug.Log("Event: Open elevator doors 2 seconds after loading the scene.");
                elevatorHandler.SetState(ElevatorState.Open);
                elevatorHandler.PlayerEnteredElevator.AddListenerOnce(() => { // Condition to move to the next event
                    EventID = 1; 
                    ScriptedEvents(EventID); });
                elevatorHandler.Control(2f);           
                break;
            case 1:
                Debug.Log("Event Close Elevator Doors and Allow Leaving Floor.");
                elevatorHandler.SetState(ElevatorState.Close);
                elevatorHandler.Control();
                elevatorHandler.SetState(ElevatorState.LeaveFloor);
                elevatorHandler.CurrentSelectedFloor = 3; // Set to next floor (Officetel)
                canLeaveFloor = true;
                break;
            default:
                Debug.LogWarning($"No scripted event found for EventID {EventID}.");
                break;
        }
    }
}
