using System;
using UnityEngine;
using DG.Tweening;

public class Openable : Interactable
{
    private bool isOpen = false;
    private Vector3 closedPosition;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetVector;
    [SerializeField] private string sfxOpenClipName,sfxCloseClipName;
    [SerializeField] private bool isRotational = false;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private Ease animationEase = Ease.InOutSine;

    private void Start()
    {
        if(target == null) target = transform;
        closedPosition = !isRotational ? target.localPosition : target.localRotation.eulerAngles;
        Debug.Log("closedPosition set to: " + closedPosition);
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
        Debug.Log("Opened!" + targetVector);

        if (!isRotational)
        {
            target.DOLocalMove(targetVector, animationDuration).SetEase(animationEase);
            AudioManager.Instance.PlaySound("SFX_Drawer_Open",transform.position);
        }
        else
        {
            target.DOLocalRotate(targetVector, animationDuration, RotateMode.Fast).SetEase(animationEase);
            AudioManager.Instance.PlaySound("SFX_Door_Open",transform.position);
        }

        AudioManager.Instance.PlaySound(sfxOpenClipName, transform.position);
    }

    private void Close()
    {
        if (!isOpen) return;
        isOpen = false;
        Debug.Log("Closed! "+closedPosition);

        if (!isRotational)
        {
            AudioManager.Instance.PlaySound("SFX_Drawer_Close",transform.position);
            target.DOLocalMove(closedPosition, animationDuration).SetEase(animationEase);
        }
        else
        {
            AudioManager.Instance.PlaySound("SFX_Drawer_Close",transform.position);
            target.DOLocalRotate(closedPosition, animationDuration, RotateMode.Fast).SetEase(animationEase);
        }

        AudioManager.Instance.PlaySound(sfxCloseClipName, transform.position);
    }

}
