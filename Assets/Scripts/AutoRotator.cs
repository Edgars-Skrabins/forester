using DG.Tweening;
using UnityEngine;

public class AutoRotator : MonoBehaviour
{
    private float spinSpeed = 50f;
    
    private void Start()
    {
        // transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime);
        Spin();
    }

    void Spin()
    {
        Sequence sequence = DOTween.Sequence();
        
        sequence.Append(transform.DORotate(new Vector3(0, 360, 0), 5f).SetEase(Ease.Linear));
        sequence.SetLoops(-1, LoopType.Incremental);
        sequence.Play();
    }
}
