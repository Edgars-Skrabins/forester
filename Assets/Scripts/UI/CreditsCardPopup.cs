using UnityEngine;
using DG.Tweening;

public class CreditsCardPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform creditsWindow;
    [SerializeField] private CanvasGroup[] allCards;

    [Header("Tween Settings")]
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float cardInterval = 0.1f;
    [SerializeField] private Ease fadeEase = Ease.OutQuad;

    private bool isAnimating = false;

    private void Awake()
    {
        // Ensure all cards start invisible
        foreach (var card in allCards)
        {
            card.alpha = 0f;
            card.gameObject.SetActive(false);
        }

        creditsWindow.gameObject.SetActive(false);
    }

    public void OpenPanel()
    {
        if (isAnimating) return;
        isAnimating = true;

        creditsWindow.gameObject.SetActive(true);

        Sequence seq = DOTween.Sequence().SetUpdate(true);

        for (int i = 0; i < allCards.Length; i++)
        {
            CanvasGroup card = allCards[i];
            card.gameObject.SetActive(true);
            card.alpha = 0f;
            card.DOKill();

            seq.Append(card.DOFade(1f, fadeDuration).SetEase(fadeEase));
            seq.AppendInterval(cardInterval);
        }

        seq.OnComplete(() => isAnimating = false);
    }

    public void ClosePanel()
    {
        if (isAnimating) return;
        isAnimating = true;

        Sequence seq = DOTween.Sequence().SetUpdate(true);

        for (int i = 0; i < allCards.Length; i++)
        {
            CanvasGroup card = allCards[i];
            card.DOKill();

            seq.Append(card.DOFade(0f, fadeDuration).SetEase(fadeEase));
            seq.AppendInterval(cardInterval);
        }

        seq.OnComplete(() =>
        {
            // Hide everything after fade-out completes
            foreach (var card in allCards)
                card.gameObject.SetActive(false);

            creditsWindow.gameObject.SetActive(false);
            isAnimating = false;
        });
    }
}
