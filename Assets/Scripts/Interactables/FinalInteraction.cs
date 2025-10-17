using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class FinalInteraction : Interactable
{
    [SerializeField] private GameObject finalCam;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private CanvasGroup blackScreen;
    
    
    public override void Interact()
    {
        base.Interact();
        AudioManager.Instance.PlaySound("SFX_Final_Trigger", transform.position);
        AudioManager.Instance.PlaySound("BGM_Finale");
        
        canvasGroup.alpha = 0;
        finalCam.SetActive(true);
        Player.Instance.gameObject.SetActive(false);
        
        canvasGroup.DOFade(1, 2f).SetDelay(2f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            blackScreen.DOFade(1, 2f).SetDelay(2f).SetEase(Ease.InOutSine);
        });
    }
}
