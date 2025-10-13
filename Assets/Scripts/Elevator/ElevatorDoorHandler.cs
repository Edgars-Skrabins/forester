using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ElevatorDoorHandler : MonoBehaviour
{
    [SerializeField] private Transform m_door1;
    [SerializeField] private Transform m_door2;
    public UnityEvent m_ElevatorDoorOpened;
    public UnityEvent m_ElevatorDoorClosed;
    public bool isOpened = false;
    public bool isSequencePlaying = false;
    private Vector3 m_door1StartPos;
    private Vector3 m_door2StartPos;
    
    [SerializeField] private Vector3 m_door1EndPos;
    [SerializeField] private Vector3 m_door2EndPos;
    
    private void Awake() 
    {
        m_ElevatorDoorOpened = new UnityEvent();
        m_ElevatorDoorClosed = new UnityEvent();

        m_door1StartPos = m_door1.localPosition;
        m_door2StartPos = m_door2.localPosition;;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            OpenDoors(0f);
        }
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            CloseDoors(0f);
        }
    }
    
    public void OpenDoors(float delay)
    {
        if (!isOpened)
        {
            isSequencePlaying = true;
            Sequence sequence = DOTween.Sequence();

            sequence.Insert(delay + .2f, m_door1.DOLocalMove(m_door1EndPos, 1.2f * 2).SetEase(Ease.InCubic));
            sequence.Insert(delay + 0f, m_door2.DOLocalMove(m_door2EndPos, 1.3f * 2).SetEase(Ease.InCubic));
            sequence.Insert(delay + 1.35f * 2, transform.DOShakePosition(0.1f, strength: 0.01f, vibrato: 2, fadeOut: true));
            AudioManager.Instance.PlaySound("SFX_Elevator_Door_Open", transform.position);
            sequence.Play();
            sequence.OnComplete(() =>
            {
                m_ElevatorDoorOpened?.Invoke();
                isSequencePlaying = false;
                isOpened = true;
            });
        }
    }

    public void CloseDoors(float delay)
    {
        if (isOpened)
        {
            isSequencePlaying = true;
            Sequence sequence = DOTween.Sequence();

            sequence.Insert(delay + 0f, m_door1.DOLocalMove(m_door1StartPos, 1.2f * 2).SetEase(Ease.InCubic));
            sequence.Insert(delay + 0.2f, m_door2.DOLocalMove(m_door2StartPos, 1.3f * 2).SetEase(Ease.InCubic));
            sequence.Insert(delay + 1.45f * 2, transform.DOShakePosition(0.1f, strength: 0.01f, vibrato: 2, fadeOut: true));

            AudioManager.Instance.PlaySound("SFX_Elevator_Door_Close", transform.position);
            sequence.Play();
            sequence.OnComplete(() =>
            {
                m_ElevatorDoorClosed?.Invoke();
                isSequencePlaying = false;
                isOpened = false;
            });
        }
    }
    public void OpenDoors(Sequence sequence, string soundName)
    {
        if (!isOpened)
        {
            isSequencePlaying = true;
            AudioManager.Instance.PlaySound(soundName, transform.position);
            sequence.Play();
            sequence.OnComplete(() =>
            {
                m_ElevatorDoorOpened?.Invoke();
                isSequencePlaying = false;
                isOpened = true;
            });
        }
    }
    public void CloseDoors(Sequence sequence, string soundName)
    {
        if (!isOpened)
        {
            isSequencePlaying = true;
            AudioManager.Instance.PlaySound(soundName, transform.position);
            sequence.Play();
            sequence.OnComplete(() =>
            {
                m_ElevatorDoorClosed?.Invoke();
                isSequencePlaying = false;
                isOpened = false;
            });
        }
    }
}