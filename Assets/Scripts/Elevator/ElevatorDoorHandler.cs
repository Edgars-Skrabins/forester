using System.Collections;
using UnityEngine;

public class ElevatorDoorHandler : MonoBehaviour
{
    [SerializeField] private Transform m_door1;
    [SerializeField] private Transform m_door2;

    private Vector3 m_door1StartPos;
    private Vector3 m_door2StartPos;
    
    [SerializeField] private Vector3 m_door1EndPos;
    [SerializeField] private Vector3 m_door2EndPos;
    
    private void Awake() 
    {
        m_door1StartPos = m_door1.localPosition;
        m_door2StartPos = m_door2.localPosition;;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OpenDoors();
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            CloseDoors();
        }
    }
    
    public void OpenDoors()
    {
        StartCoroutine(CO_OpenDoors());
    }
    
    public void CloseDoors()
    {
        StartCoroutine(CO_CloseDoors());
    }

    IEnumerator CO_OpenDoors()
    {
        float progress = 0;

        while (progress < 1)
        {
            progress += Time.deltaTime;

            m_door1.localPosition = Vector3.Lerp(m_door1StartPos, m_door1EndPos, progress);
            m_door2.localPosition = Vector3.Lerp(m_door2StartPos, m_door2EndPos, progress);
            
            yield return null;
        }
    }

    IEnumerator CO_CloseDoors()
    {
        float progress = 0;

        while (progress < 1)
        {
            progress += Time.deltaTime;

            m_door1.localPosition = Vector3.Lerp(m_door1EndPos, m_door1StartPos, progress);
            m_door2.localPosition = Vector3.Lerp(m_door2EndPos, m_door2StartPos, progress);
            
            yield return null;
        }
    }
}