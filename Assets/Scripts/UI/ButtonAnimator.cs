using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonAnimator : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;
    private Color originalColor;
    [SerializeField] private TMP_Text buttonText;

    private void Awake()
    {
        if(buttonText == null) buttonText = GetComponentInChildren<TMP_Text>();
        originalScale = transform.localScale;
        if (buttonText != null)
        {
            originalColor = buttonText.color;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AnimateClick();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AnimateHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        AnimateExit();
    }

    private void AnimateClick()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(originalScale * 0.9f, 0.1f).SetEase(Ease.OutQuad));
        sequence.Append(transform.DOScale(originalScale, 0.1f).SetEase(Ease.OutQuad));
        sequence.Play();
        AudioManager.Instance.PlaySound("UISFX_Select");
    }

    private void AnimateHover()
    {
        if (buttonText != null)
        {
            buttonText.DOColor(Color.red, 0.2f).SetEase(Ease.OutQuad);
        }
        transform.DOScale(originalScale * 1.05f, 0.2f).SetEase(Ease.OutQuad);
        AudioManager.Instance.PlaySound("UISFX_Hover");
    }

    private void AnimateExit()
    {
        if (buttonText != null)
        {
            buttonText.DOColor(originalColor, 0.2f).SetEase(Ease.OutQuad);
        }
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutQuad);
    }
}
