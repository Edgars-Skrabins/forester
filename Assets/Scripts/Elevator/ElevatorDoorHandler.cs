using DG.Tweening;
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
            OpenDoors(0f);
        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            CloseDoors(0f);
        }
    }
    
    public void OpenDoors(float delay)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Insert(delay + .2f, m_door1.DOLocalMove(m_door1EndPos, 1.2f).SetEase(Ease.InCubic));
        sequence.Insert(delay + 0f, m_door2.DOLocalMove(m_door2EndPos, 1.3f).SetEase(Ease.InCubic));
        sequence.Insert(delay + 1.35f, transform.DOShakePosition(0.1f, strength: 0.01f, vibrato: 2, fadeOut: true));

        sequence.Play();
    }
    
    public void CloseDoors(float delay)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Insert(delay + 0f, m_door1.DOLocalMove(m_door1StartPos, 1.2f).SetEase(Ease.InCubic));
        sequence.Insert(delay + 0.2f, m_door2.DOLocalMove(m_door2StartPos, 1.3f).SetEase(Ease.InCubic));
        sequence.Insert(delay + 1.45f, transform.DOShakePosition(0.1f, strength: 0.01f, vibrato: 2, fadeOut: true));
        
        sequence.Play();
    }
}