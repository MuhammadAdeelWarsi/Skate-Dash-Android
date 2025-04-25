using UnityEngine;
using System.Collections;
using System;
using DG.Tweening;
using System.Linq;

public class PrototypeNight : TimeCycle
{
    private SpriteRenderer spriteRenderer;

    [Header("--Sky--")]
    [SerializeField] PrototypeDay prototypeDay;
    [SerializeField] bool isFadingStarted;
    [SerializeField] float startingLerpFactor = 0.003f, endingLerpFactor = 0.002f;
    private bool hasDayTriggered = false; // Flag to ensure it triggers only once

    [Header("--Moon--")]
    [SerializeField] int currentMoonTypeIndex;
    [SerializeField] SpriteRenderer moonSprite;
    [SerializeField] MoonPosition[] moonPositions;
    private bool isMoonHidden;

    [Header("--Cloud--")]
    [SerializeField] int currentCloudTypeIndex;
    [SerializeField] SpriteRenderer cloudSprite;
    [SerializeField] CloudPosition[] cloudPositions;
    private bool isCloudHidden;

    [Header("--Star--")]
    [SerializeField] int currentStarsCount;
    [SerializeField] PrototypeStar[] stars;
    [SerializeField] PrototypeStar[] currentTargetedStars;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        isMoonHidden = isCloudHidden = true;
    }


    [ContextMenu("StartNight")]
    public override void StartCycle()
    {
        StartCoroutine(StartNightCoroutine());
    }

    [ContextMenu("EndNight")]
    public override void EndCycle()
    {
        HideMoon();
        HideCloud();
        StartCoroutine(EndNightCoroutine());
    }

    private IEnumerator StartNightCoroutine()
    {
        isFadingStarted = true;
        hasDayTriggered = false;

        while (spriteRenderer.color.a <= 0.99f)
        {
            yield return null;
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, Color.white, startingLerpFactor);

            // Trigger day-to-night change only once when alpha crosses 0.65
            if (!hasDayTriggered && spriteRenderer.color.a > 0.65f)
            {
                if(GameManager.Instance.AudioManager.IsMusicEnabled)
                    GameManager.Instance.AudioManager.PlayAudio(CustomControllers.AudioManager.AudioNames.NightMusic);
                prototypeDay.EndCycle();
                hasDayTriggered = true; // Prevent further triggers
            }
        }

        isFadingStarted = false;

        ShowMoon();
        ShowCloud();
        ShowStars();
    }
    private IEnumerator EndNightCoroutine()
    {
        isFadingStarted = true;
        hasDayTriggered = false;

        while(!isMoonHidden && !isCloudHidden)  //Waiting for moon and cloud to reach their default positions
        {
            yield return new WaitForSeconds(2f);
            continue;
        }

        while (spriteRenderer.color.a >= 0.01f)
        {
            yield return null;
            spriteRenderer.color = Color.Lerp(spriteRenderer.color, new Color(1f, 1f, 1f, 0f), endingLerpFactor);

            // Trigger day-to-night change only once when alpha crosses 0.65
            if (!hasDayTriggered && spriteRenderer.color.a < 0.65f)
            {
                if(GameManager.Instance.AudioManager.IsMusicEnabled)
                    GameManager.Instance.AudioManager.PlayAudio(CustomControllers.AudioManager.AudioNames.DayMusic);
                HideStars();
                prototypeDay.StartCycle();
                hasDayTriggered = true; // Prevent further triggers
            }
        }

        isFadingStarted = false;
    }

    private void ShowMoon()
    {
        int moonTypeIndex = UnityEngine.Random.Range(0, moonPositions.Length);
        currentMoonTypeIndex = moonTypeIndex;   //For debugging

        moonSprite.transform.localPosition = moonPositions[moonTypeIndex].defaultLocalPosition;
        moonSprite.transform.localEulerAngles = moonPositions[moonTypeIndex].targetLocalRotation;
        moonSprite.transform.localScale = moonPositions[moonTypeIndex].targetLocalScale;
        moonSprite.flipX = moonPositions[moonTypeIndex].flipX;
        moonSprite.color = new Color(1f, 1f, 1f, moonPositions[moonTypeIndex].targetColorAlpha);
        moonSprite.gameObject.SetActive(true);

        moonSprite.transform.DOLocalMove(moonPositions[moonTypeIndex].targetLocalPosition, moonPositions[moonTypeIndex].visibilityDuration).OnComplete(() =>
        {
            isMoonHidden = false;
        });
    }
    private void HideMoon()
    {
        moonSprite.transform.DOLocalMove(moonPositions[currentMoonTypeIndex].defaultLocalPosition, moonPositions[currentMoonTypeIndex].hideDuration).OnComplete(() =>
        {
            isMoonHidden = true;
        });
    }

    private void ShowCloud()
    {
        int cloudTypeIndex = UnityEngine.Random.Range(0, cloudPositions.Length);
        currentCloudTypeIndex = cloudTypeIndex;   //For debugging

        cloudSprite.transform.localPosition = cloudPositions[cloudTypeIndex].defaultLocalPosition;
        cloudSprite.transform.localEulerAngles = cloudPositions[cloudTypeIndex].targetLocalRotation;
        cloudSprite.transform.localScale = cloudPositions[cloudTypeIndex].targetLocalScale;
        cloudSprite.color = new Color(1f, 1f, 1f, cloudPositions[cloudTypeIndex].targetColorAlpha);
        cloudSprite.gameObject.SetActive(true);

        cloudSprite.transform.DOLocalMove(cloudPositions[cloudTypeIndex].targetLocalPosition, cloudPositions[cloudTypeIndex].visibilityDuration).OnComplete(() =>
        {
            isCloudHidden = false;
        });
    }
    private void HideCloud()
    {
        cloudSprite.transform.DOLocalMove(cloudPositions[currentCloudTypeIndex].defaultLocalPosition, cloudPositions[currentCloudTypeIndex].hideDuration).OnComplete(() =>
        {
            isCloudHidden = true;
        });
    }

    private void ShowStars()
    {
        int starsCount = UnityEngine.Random.Range(7, stars.Length + 1);
        currentStarsCount = starsCount;

        PrototypeStar[] randomlyOrderedStars = stars.OrderBy(stars => UnityEngine.Random.value).ToArray();
        currentTargetedStars = randomlyOrderedStars.Take(starsCount).ToArray();

        foreach (PrototypeStar star in currentTargetedStars)
            star.gameObject.SetActive(true);
    }
    private void HideStars()
    {
        foreach (PrototypeStar star in currentTargetedStars)
            star.EndShining();
    }

    public bool HasCycleStarted()   //Moon and clouds are completely visible on the sky
    {
        return !isMoonHidden && !isCloudHidden && !isFadingStarted;
    }
    public bool HasCycleEnded()     //Moon and clouds are completely hidden
    {
        return isMoonHidden && isCloudHidden && !isFadingStarted;
    }


    [Serializable]
    public class MoonPosition
    {
        public Vector3 defaultLocalPosition;
        public Vector3 targetLocalPosition;
        public Vector3 targetLocalRotation;
        public Vector3 targetLocalScale;
        public bool flipX;
        public float targetColorAlpha;
        public float visibilityDuration;
        public float hideDuration;
    }

    [Serializable]
    public class CloudPosition
    {
        public Vector3 defaultLocalPosition;
        public Vector3 targetLocalPosition;
        public Vector3 targetLocalRotation;
        public Vector3 targetLocalScale;
        public float targetColorAlpha;
        public float visibilityDuration;
        public float hideDuration;
    }
}