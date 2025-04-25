using DG.Tweening;
using UnityEngine;


public class PrototypeStar : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("--Brightness--")]
    [SerializeField] float maxBrightnessAlpha;
    [SerializeField] float minBrightnessAlpha;
    [SerializeField] float dimmingDuration;
    private Tween brighteningTween;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnEnable()
    {
        spriteRenderer.DOFade(maxBrightnessAlpha, dimmingDuration).OnComplete(() => 
        {
            brighteningTween = spriteRenderer.DOFade(minBrightnessAlpha, dimmingDuration).SetLoops(-1);
        });
    }

    public void EndShining()    //End displaying brightness when night is ending
    {
        if(brighteningTween != null)
            brighteningTween.Kill();

        spriteRenderer.DOFade(0f, dimmingDuration).OnComplete(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
