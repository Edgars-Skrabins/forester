using UnityEngine;
using DG.Tweening;

public class KeypadButton : Interactable
{
    [SerializeField] private string buttonValue;
    [SerializeField] private KeypadController keypadController;
    [SerializeField] private Vector3 pressedPosition;
    private Vector3 originalPosition;
    
    private void Start()
    {
        originalPosition = transform.localPosition;
    }
    
    public override void Interact()
    {
        Animate();
        keypadController.ButtonPressed(buttonValue);
    }

    private void Animate()
    {
        Sequence s = DOTween.Sequence();
        
        s.Append(transform.DOLocalMoveX(pressedPosition.x, 0.1f).SetEase(Ease.InOutSine));
        s.Append(transform.DOLocalMove(originalPosition, 0.1f).SetEase(Ease.InOutSine));
        s.Play();
    }
}