using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using CustomControllers;


public class GameLoader : MonoBehaviour
{
    [Header("---Logo---")]
    [SerializeField] Animation logoAnimation;
    [Space(5), SerializeField] Transform skateWord;
    [SerializeField] Transform dashWord;
    [SerializeField] float wordScaleDuration = 0.5f, wordTweenLoopDelay = 0.75f;
    [Space(5), SerializeField] Transform skateboardTransform;
    [SerializeField] Transform starTransform;

    [Header("---Loading---")]
    [SerializeField] Slider loadingSlider;
    [SerializeField] float sliderTargetScale = 3.5f, sliderScaleDuration = 1f;
    [SerializeField] float minLoadingDuration = 5f, maxLoadingDuration = 6.5f;

    [Header("---Audio---")]
    [SerializeField] AudioManager splashAudioManager;


    private void Start()
    {
        PlayLogoAnimation();
    }

    private void PlayLogoAnimation()
    {
        logoAnimation.Play();
    }
    private void PlayExtraAnimations()  //Animating extra objects inside game logo
    {
        Sequence wordsSequence = DOTween.Sequence();
        wordsSequence
            .Append(skateWord.DOScale(skateWord.localScale + (Vector3.one * 0.075f), wordScaleDuration)
                .SetEase(Ease.OutBounce)
                .SetLoops(2, LoopType.Yoyo)) // Skate scales up and shrinks back
            .Append(dashWord.DOScale(dashWord.localScale + (Vector3.one * 0.075f), wordScaleDuration)
                .SetEase(Ease.OutBounce)
                .SetLoops(2, LoopType.Yoyo)) // Dash starts only after Skate finishes
            .AppendInterval(wordTweenLoopDelay) // Delay before restarting
            .SetLoops(-1); // Infinite loop

        starTransform.DOScale(starTransform.localScale + (Vector3.one * 0.25f), 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        skateboardTransform.GetComponent<Animation>().Play();
    }

    public void PlayRevealSound(AudioManager.AudioNames audioName)   //Subscribed to an event, being invoked inside LoadingAnimation
    {
        splashAudioManager.PlayAudio(audioName);
    }

    public void StartLoading()      //Subscribed to an event, being invoked inside LoadingAnimation
    {
        loadingSlider.transform.DOScale(sliderTargetScale, sliderScaleDuration).SetEase(Ease.OutBack).OnComplete(() =>
        {
            PlayExtraAnimations();
            UpdateLoadingStatus();
        });
    }
    private void UpdateLoadingStatus()  //Updates loading slider value
    {
        loadingSlider.DOValue(1f, Random.Range(minLoadingDuration, maxLoadingDuration)).SetEase(Ease.Linear).OnComplete(LoadGame);
    }
    private void LoadGame()
    {
        SceneManager.LoadScene(1);
    }
}
