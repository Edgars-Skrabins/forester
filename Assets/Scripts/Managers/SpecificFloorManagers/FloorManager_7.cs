using DG.Tweening;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FloorManager_7 : FloorManager
{
    GameObject endDoor;
    GameObject endTrigger;
    GameObject door6;
    GameObject door1;
    GameObject door3;
    GameObject endLight;
    GameObject endText;
    Scene scene;
    private IEnumerator WaitOnLoad()
    {
        while (!scene.isLoaded)
        {
            yield return null;
        }
        endDoor = ScriptedObjectReferences[0];
        endTrigger = ScriptedObjectReferences[1];
        door6 = ScriptedObjectReferences[2];
        door1 = ScriptedObjectReferences[3];
        door3 = ScriptedObjectReferences[4];
        endLight = ScriptedObjectReferences[5];
        endText = ScriptedObjectReferences[6];
        endLight.SetActive(false);
        endTrigger.SetActive(false);
        endText.SetActive(false);
        ScriptedEvents(0);
        ScriptedEvents(24);
    }

    public override void ScriptedEvents(int EventID)
    {
        switch (EventID)
        {
            case 0:
                Debug.Log("Event: Open elevator doors 2 seconds after loading the scene.");
                elevatorHandler.SetState(ElevatorState.Open);
                elevatorHandler.Control(2f);
                elevatorHandler.PlayerEnteredElevator.AddListenerOnce(() => { // Condition to move to Event 666
                    ScriptedEvents(666);
                });
                ScriptedEvents(1);
                break;
            case 1:
                Debug.Log("Event Close Elevator Doors and Allow Leaving Floor.");
                elevatorHandler.PlayerLeftElevator.AddListenerOnce(() => { // Condition to move to Event 666
                    elevatorHandler.SetState(ElevatorState.Close);
                    elevatorHandler.Control();
                });
                break;
            case 24:
                door3.GetComponent<Unlockable>().AttemptUnlock("1234");
                break;
            case 664:
                endDoor.SetActive(true);
                endLight.SetActive(false);
                endTrigger.SetActive(false);
                endText.SetActive(true);
                break;
            case 665:
                endDoor.SetActive(false); // Disable the end door
                endLight.SetActive(true);
                endTrigger.SetActive(true); // Enable the trigger collider
                break;
            case 666:
                elevatorHandler.SetState(ElevatorState.AddButtonAndLeave);
                canLeaveFloor = true;
                break;
            default:
                Debug.LogWarning($"No scripted event found for EventID {EventID}.");
                break;
        }
    }
}
