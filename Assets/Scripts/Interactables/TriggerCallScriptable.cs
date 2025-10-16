using UnityEngine;

public class TriggerCallScriptable : MonoBehaviour
{
    public int ScriptableEventID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Trigger entered by Player, calling ScriptableObject event.");
            // Assuming you have a reference to your ScriptableObject
            // Example: myScriptableObject.TriggerEvent();
            FloorManager.Instance.ScriptedEvents(ScriptableEventID);
            Destroy(this.gameObject, 0.1f);
        }
    }
}
