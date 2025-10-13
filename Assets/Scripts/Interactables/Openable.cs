using System;
using UnityEngine;
using DG.Tweening;

public class Openable : Interactable
{
    private bool isOpen = false;
    private Vector3 closedPosition;
    [SerializeField] private Vector3 targetVector;
    [SerializeField] private string sfxOpenClipName,sfxCloseClipName;
    [SerializeField] private bool isRotational = false;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private Ease animationEase = Ease.InOutSine;

    private void Awake()
    {
        closedPosition = !isRotational ? transform.localPosition : transform.eulerAngles;
    }

    public override void Interact()
    {
        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }
    
    private void Open()
    {
        if (isOpen) return;
        isOpen = true;
        Debug.Log("Opened!");

        if (!isRotational)
        {
            transform.DOLocalMove(targetVector, animationDuration).SetEase(animationEase);
        }
        else
        {
            transform.DOLocalRotate(targetVector, animationDuration).SetEase(animationEase);
        }

        AudioManager.Instance.PlaySound(sfxOpenClipName, transform.position);
    }

    private void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        Debug.Log("Closed!");

        if (!isRotational)
        {
            transform.DOLocalMove(closedPosition, animationDuration).SetEase(animationEase);
        }
        else
        {
            transform.DOLocalRotate(closedPosition, animationDuration).SetEase(animationEase);
        }

        AudioManager.Instance.PlaySound(sfxCloseClipName, transform.position);
    }

}
