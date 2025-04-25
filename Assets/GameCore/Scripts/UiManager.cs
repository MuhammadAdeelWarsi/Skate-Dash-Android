using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using CustomControllers;


public class UiManager : MonoBehaviour
{
    public delegate void OnAudioToggle(PrefsDataManager.PrefsKeys toggleKey, bool value);
    public event OnAudioToggle onAudioToggleEvent;

    private static UiManager instance;
    public static UiManager Instance => instance;


    [Header("---Initial Setup---")]
    [SerializeField] Image mainMenuPanelImage;

    [Header("---Game Settings---")]
    [SerializeField] GameObject settingsPanel;
    [SerializeField] Button musicButton, soundButton, vibrationButton;
    [SerializeField] Sprite[] toggleButtonSprites;

    [Header("---Game Pause---")]
    [SerializeField] GameObject pausePanel;

    [Header("---Gameplay---")]
    [SerializeField] GameObject gameplayUIPanel;
    [SerializeField] GameObject timerBar;
    [SerializeField] GameObject counterBar;
    [SerializeField] Button jumpButton;
    private bool isGameplayUiInitialized;
    private Text gameplayTime, obstacleAmount;
    private int previousSeconds = 0;

    [Header("---Level Fail---")]
    [SerializeField] GameObject levelFailPanel;
    [SerializeField] float minScoreAnimationTime = 1.5f, maxScoreAnimationTime = 3f;
    private bool isTimerAnimated, isCounterAnimated;

    [Header("---Combine References---")]
    [SerializeField] ScoreText.TimerText[] timerTexts;
    [SerializeField] ScoreText.ObstacleEncounteredText[] obstaclesAmountTexts;

    #region Tween Variables
    [Header("\n---Tweens Info---\n\n -> Main Menu")]
    [SerializeField] UiTweener miniLogoTweener;
    [SerializeField] UiTweener highScoreBoxTweener, timeBarTweener, obstacleBarTweener;
    [SerializeField] UiTweener settingButtonTweener, playButtonTweener;

    [Header("-> Settings Menu")]
    [SerializeField] UiTweener settingsPopupTweener;
    [SerializeField] UiTweener musicBarTweener, soundBarTweener, vibrationBarTweener;

    [Header("-> Pause Menu")]
    [SerializeField] UiTweener pausePopupTweener;
    [SerializeField] UiTweener resumeButtonTweener, restartButtonTweener, inGameSettingButtonTweener;

    [Header("-> Gameplay Menu")]
    [SerializeField] UiTweener jumpButtonTweener;
    [SerializeField] UiTweener timerBarTweener, counterBarTweener, currentScoreBoxTweener;
    [SerializeField] UiTweener pauseButtonTweener;

    [Header("-> Level Fail Menu")]
    [SerializeField] UiTweener levelFailPopupTweener;
    [SerializeField] UiTweener finalTimerBarTweener, finalCounterBarTweener, timerNewTextTweener, counterNewTextTweener;
    [SerializeField] UiTweener exitButtonTweener, nextButtonTweener;
    #endregion


    public bool IsGameplayUiInitialized => isGameplayUiInitialized;


    private void Awake()
    {
        if(instance == null)
            instance = this;

        PrototypeDino.onInitializedEvent += EnableGameplayUI;
        ScoreManager.onObstacleCounterUpdateEvent += UpdateCounterUI;

        gameplayTime = timerTexts.Single(timerText => timerText.uiPanel == UiPanels.GameplayMenu).text;
        obstacleAmount = obstaclesAmountTexts.Single(obstaclesText => obstaclesText.uiPanel == UiPanels.GameplayMenu).text;
    }
    private void Start()
    {
        if (GameManager.Instance.AudioManager.IsSoundEnabled)
            GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.WindBlowing);

        miniLogoTweener.PlayTween(UiTweener.TweenType.LocalMove, null, () => miniLogoTweener.PlayTween(UiTweener.TweenType.LocalRotate));
        highScoreBoxTweener.PlayTween(UiTweener.TweenType.Scale);
        timeBarTweener.PlayTween(UiTweener.TweenType.Scale);
        obstacleBarTweener.PlayTween(UiTweener.TweenType.Scale);
        settingButtonTweener.PlayTween(UiTweener.TweenType.Scale);
        playButtonTweener.PlayTween(UiTweener.TweenType.Scale, UiTweener.TweenPurpose.Initialize, null, () =>
        {
            playButtonTweener.PlayTween(UiTweener.TweenType.Scale, UiTweener.TweenPurpose.Emphasize);
        });
    }


    public void InitializeScoreUI()    //For initializing all scoreboards
    {
        foreach(ScoreText.ObstacleEncounteredText obstacleEncounteredText in obstaclesAmountTexts)
        {
            if (obstacleEncounteredText.uiPanel != UiPanels.GameplayMenu && obstacleEncounteredText.uiPanel != UiPanels.LevelFail)
                obstacleEncounteredText.text.text = GameManager.Instance.ScoreManager.GameHighScore.ObstaclesEncountered.ToString();
        }

        int totalSeconds = GameManager.Instance.ScoreManager.GameHighScore.TotalGameplaySeconds;
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        foreach (ScoreText.TimerText timerText in timerTexts)
        {
            if (timerText.uiPanel != UiPanels.GameplayMenu && timerText.uiPanel != UiPanels.LevelFail)
                timerText.text.text = String.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
    private void EnableGameplayUI() //Enabling in-game UI for interactions
    {
        Action tweenStartCallback = null;
        if (GameManager.Instance.AudioManager.IsSoundEnabled)
            tweenStartCallback = () => { GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.ButtonPop); };

        gameplayUIPanel.SetActive(true);
        timerBarTweener.PlayTween(UiTweener.TweenType.Scale, tweenStartCallback);
        counterBarTweener.PlayTween(UiTweener.TweenType.Scale, tweenStartCallback);
        jumpButtonTweener.PlayTween(UiTweener.TweenType.Scale, tweenStartCallback, () => isGameplayUiInitialized = true);
        pauseButtonTweener.PlayTween(UiTweener.TweenType.Scale, tweenStartCallback);
        currentScoreBoxTweener.PlayTween(UiTweener.TweenType.Scale);
    }
    public void UpdateTimerUI(int time)  //Updates game timer
    {
        int minutes = time / 60;  // Get the number of minutes
        int seconds = time % 60;  // Get the remaining seconds

        gameplayTime.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Format as MM:SS

        if(previousSeconds != seconds)
        {
            previousSeconds = seconds;
            if (GameManager.Instance.AudioManager.IsSoundEnabled)
                GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.ClockTick);
        }
    }
    public void UpdateCounterUI(int amount) //Updates obstacle counter
    {
        obstacleAmount.text = amount.ToString();
        if (GameManager.Instance.AudioManager.IsSoundEnabled)
            GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.PointGranted);

        if(GameManager.Instance.VibrationManager.IsVibrationEnabled)
            MMVibrationManager.Haptic(HapticTypes.MediumImpact);
    }

    private void UpdateFinalScoreUI()    //Updates final score UI on level fail panel
    {
        //FOR TIMER
        isTimerAnimated = true;
        bool isTimerAnimationRequired = GameManager.Instance.ScoreManager.GameHighScore.TotalGameplaySeconds < GameManager.Instance.TotalSeconds;

        Text gameplayMinutesText = timerTexts.Single(timerText => timerText.uiPanel == UiPanels.LevelFail && timerText.text.CompareTag("TimerMinutes")).text;
        Text gameplaySecondsText = timerTexts.Single(timerText => timerText.uiPanel == UiPanels.LevelFail && timerText.text.CompareTag("TimerSeconds")).text;

        int minutes = GameManager.Instance.TotalSeconds / 60;
        int seconds = GameManager.Instance.TotalSeconds % 60;

        if (isTimerAnimationRequired)
            StartCoroutine(AnimateScoreUI(gameplayMinutesText, gameplaySecondsText, minutes, seconds));
        else
            (gameplayMinutesText.text, gameplaySecondsText.text) = (String.Format("{0:00}", minutes), " : " + (String.Format("{0:00}", seconds)));

        //FOR OBSTACLE
        isCounterAnimated = true;
        bool isCounterAnimationRequired = GameManager.Instance.ScoreManager.GameHighScore.ObstaclesEncountered < GameManager.Instance.ScoreManager.ObstacleEncountered;

        Text obstacleAmountText = obstaclesAmountTexts.Single(amountText => amountText.uiPanel == UiPanels.LevelFail).text;

        if (isCounterAnimationRequired)
            StartCoroutine(AnimateScoreUI(obstacleAmountText, GameManager.Instance.ScoreManager.ObstacleEncountered));
        else
            obstacleAmountText.text = GameManager.Instance.ScoreManager.ObstacleEncountered.ToString();

        //FOR AUDIO
        if ((isCounterAnimationRequired || isTimerAnimationRequired) && GameManager.Instance.AudioManager.IsSoundEnabled)
            GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.ScoreCounterFill);

        //FINAL BUTTONS
        if(!isCounterAnimationRequired)
        {
            exitButtonTweener.PlayTween(UiTweener.TweenType.Scale);
            nextButtonTweener.PlayTween(UiTweener.TweenType.Scale, () =>
            {
                if (GameManager.Instance.AudioManager.IsSoundEnabled)
                    GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.ButtonPop);
            });
        }
    }

    private IEnumerator AnimateScoreUI(Text minutesText, Text secondsText, int minutes, int seconds) //For timer
    {
        isTimerAnimated = false;    //Reset bool and set after animation is completed
        float scoreAnimationTime = seconds < 12 ? minScoreAnimationTime : maxScoreAnimationTime;
        float delayPerAnimation = scoreAnimationTime / seconds;
        int secondsAnimated = 0, minutesAnimated = 0;

        bool areMinutesAnimated = false, areSecondsAnimated = false; 
        while (true)
        {
            if(secondsAnimated < seconds)
            {
                secondsAnimated++;
                secondsText.text = " : " + String.Format("{0:00}", secondsAnimated);
            }
            else if (!areSecondsAnimated)
            {
                secondsText.text = " : " + String.Format("{0:00}", seconds);
                areSecondsAnimated = true;
            }

            if (minutesAnimated < minutes)
            {
                minutesAnimated++;
                minutesText.text = String.Format("{0:00}", minutesAnimated);
            }
            else if (!areMinutesAnimated)
            {
                minutesText.text = String.Format("{0:00}", minutes);
                areMinutesAnimated = true;
            }

            yield return new WaitForSecondsRealtime(delayPerAnimation);
            if (areMinutesAnimated && areSecondsAnimated)   //Both minutes and seconds animated amount have reached their target points
                break;
        }
        isTimerAnimated = true; //Timer animation is done

        if (isCounterAnimated)  //Obstacle counter is animated so disable filler sound
            GameManager.Instance.AudioManager.StopAudio(AudioManager.AudioNames.ScoreCounterFill);

        //Final value set for safe side
        timerNewTextTweener.PlayTween(UiTweener.TweenType.Scale, () =>
        {
            if (GameManager.Instance.AudioManager.IsSoundEnabled)
                GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.GameEndWithHighscore);

            if(GameManager.Instance.VibrationManager.IsVibrationEnabled)
                MMVibrationManager.Haptic(HapticTypes.SoftImpact);
        });
    }
    private IEnumerator AnimateScoreUI(Text amountText, int obstacleAmount) //For obstacle
    {
        isCounterAnimated = false;  //Reset bool and set after animation is completed
        float scoreAnimationTime = obstacleAmount < 12 ? minScoreAnimationTime : maxScoreAnimationTime;
        float delayPerAnimation = scoreAnimationTime / obstacleAmount;
        int amountAnimated = 0;
        while(amountAnimated < obstacleAmount)
        {
            amountAnimated++;
            amountText.text = amountAnimated.ToString();
            yield return new WaitForSecondsRealtime(delayPerAnimation);
        }
        isCounterAnimated = true;   //Obstacle counter animation is done
        amountText.text = obstacleAmount.ToString();    //Final value set for safe side

        if (isTimerAnimated)    //Timer is animated so disable filler sound
            GameManager.Instance.AudioManager.StopAudio(AudioManager.AudioNames.ScoreCounterFill);

        counterNewTextTweener.PlayTween(UiTweener.TweenType.Scale, () =>
        {
            if (GameManager.Instance.AudioManager.IsSoundEnabled)
                GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.GameEndWithHighscore);

            if(GameManager.Instance.VibrationManager.IsVibrationEnabled)
                MMVibrationManager.Haptic(HapticTypes.SoftImpact);
        }, () =>
        {
            exitButtonTweener.PlayTween(UiTweener.TweenType.Scale);
            nextButtonTweener.PlayTween(UiTweener.TweenType.Scale, () =>
            {
                if (GameManager.Instance.AudioManager.IsSoundEnabled)
                    GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.ButtonPop);
            });
        });
    }

    public void OpenPanel(GameObject targetPanel)   //For subscribing UI Buttons to show the target panel on clicking
    {
        UiPanels uiPanel = UiPanels.MainMenu;

        if (targetPanel == settingsPanel)       //Settings button is clicked
            uiPanel = UiPanels.SettingsMenu;
        else if (targetPanel == pausePanel)     //Pause button is clicked
            uiPanel = UiPanels.PauseMenu;

        ShowPanel(uiPanel);
    }
    public void ClosePanel(GameObject targetPanel)  //For subscribing UI Buttons to hide the target panel on clicking
    {
        UiPanels uiPanel = UiPanels.MainMenu;

        if (targetPanel == settingsPanel)
            uiPanel = UiPanels.SettingsMenu;
        else if (targetPanel == pausePanel)
            uiPanel = UiPanels.PauseMenu;

        HidePanel(uiPanel);
    }

    public void ShowPanel(UiPanels targetPanel) //Shows target panel
    {
        Action buttonTweenStartCallback = null;

        switch (targetPanel)
        {
            case UiPanels.SettingsMenu:
                if (GameManager.Instance.AudioManager.IsSoundEnabled)
                {
                    GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.ButtonClick);
                    buttonTweenStartCallback = () => { GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.ButtonPop); };
                }

                settingsPanel.SetActive(true);
                settingsPopupTweener.PlayTween(UiTweener.TweenType.Scale, UiTweener.TweenPurpose.Initialize);
                musicBarTweener.PlayTween(UiTweener.TweenType.Scale, buttonTweenStartCallback);
                soundBarTweener.PlayTween(UiTweener.TweenType.Scale, buttonTweenStartCallback);
                vibrationBarTweener.PlayTween(UiTweener.TweenType.Scale, buttonTweenStartCallback);
                break;

            case UiPanels.PauseMenu:
                Time.timeScale = 0;
                GameManager.Instance.HasGamePaused = true;
                if (GameManager.Instance.AudioManager.IsSoundEnabled)
                {
                    GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.ButtonClick);
                    buttonTweenStartCallback = () => { GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.ButtonPop); };
                }

                pausePanel.SetActive(true);
                pausePopupTweener.PlayTween(UiTweener.TweenType.Scale, UiTweener.TweenPurpose.Initialize, () =>
                {
                    GameManager.Instance.AudioManager.PauseAudio(AudioManager.AudioNames.WindBlowing);
                    GameManager.Instance.AudioManager.PauseAudio(AudioManager.AudioNames.SkateboardRiding);
                    GameManager.Instance.AudioManager.PauseAudio(AudioManager.AudioNames.ClockTick);
                });
                resumeButtonTweener.PlayTween(UiTweener.TweenType.Scale, buttonTweenStartCallback);
                restartButtonTweener.PlayTween(UiTweener.TweenType.Scale, buttonTweenStartCallback);
                inGameSettingButtonTweener.PlayTween(UiTweener.TweenType.Scale, buttonTweenStartCallback);
                break;

            case UiPanels.LevelFail:
                GameManager.Instance.AudioManager.StopAudio(AudioManager.AudioNames.WindBlowing);
                GameManager.Instance.AudioManager.StopAudio(AudioManager.AudioNames.ClockTick);
                levelFailPanel.SetActive(true);
                levelFailPopupTweener.PlayTween(UiTweener.TweenType.Scale);
                finalTimerBarTweener.PlayTween(UiTweener.TweenType.Scale);
                finalCounterBarTweener.PlayTween(UiTweener.TweenType.Scale, null, UpdateFinalScoreUI);
                break;
        }
    }
    public void HidePanel(UiPanels targetPanel) //Hides target panel
    {
        switch (targetPanel)
        {
            case UiPanels.MainMenu:
                if (GameManager.Instance.AudioManager.IsSoundEnabled)
                {
                    GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.ButtonClick);
                    GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.UiSwoosh, 0.35f);
                }
                mainMenuPanelImage.DOFillAmount(0, 1f).SetEase(Ease.Linear).OnComplete(() =>
                {
                    mainMenuPanelImage.gameObject.SetActive(false);
                    if(GameManager.Instance.AudioManager.IsMusicEnabled)
                        GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.DayMusic);
                });
                break;

            case UiPanels.SettingsMenu:
                if (GameManager.Instance.AudioManager.IsSoundEnabled)
                    GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.PanelPop);
                settingsPopupTweener.PlayTween(UiTweener.TweenType.Scale, UiTweener.TweenPurpose.Hide, null, () =>
                {
                    settingsPanel.SetActive(false);
                });
                break;

            case UiPanels.PauseMenu:
                if (GameManager.Instance.AudioManager.IsSoundEnabled)
                    GameManager.Instance.AudioManager.PlayAudio(AudioManager.AudioNames.PanelPop);
                pausePopupTweener.PlayTween(UiTweener.TweenType.Scale, UiTweener.TweenPurpose.Hide, null, () =>
                {
                    pausePanel.SetActive(false);
                    Time.timeScale = 1;
                    GameManager.Instance.AudioManager.UnPauseAudio(AudioManager.AudioNames.WindBlowing);
                    GameManager.Instance.AudioManager.UnPauseAudio(AudioManager.AudioNames.SkateboardRiding);
                    GameManager.Instance.AudioManager.UnPauseAudio(AudioManager.AudioNames.ClockTick);
                });
                break;
        }
    }

    public void ToggleState(Button targetButton)    //For toggling button's state in settings menu
    {
        AudioManager.AudioNames toggleAudio = AudioManager.AudioNames.None;

        if(targetButton == musicButton)
        {
            Image image = musicButton.GetComponent<Image>();
            UpdateToggleUi(image, PrefsDataManager.PrefsKeys.Music);

            toggleAudio = image.sprite == toggleButtonSprites[0] ? AudioManager.AudioNames.ButtonOffToggle : AudioManager.AudioNames.ButtonOnToggle;

            onAudioToggleEvent.Invoke(PrefsDataManager.PrefsKeys.Music, image.sprite == toggleButtonSprites[0] ? false : true);
        }
        else if (targetButton == soundButton)
        {
            Image image = soundButton.GetComponent<Image>();
            UpdateToggleUi(image, PrefsDataManager.PrefsKeys.Sound);

            toggleAudio = image.sprite == toggleButtonSprites[0] ? AudioManager.AudioNames.ButtonOffToggle : AudioManager.AudioNames.ButtonOnToggle;
       
            onAudioToggleEvent.Invoke(PrefsDataManager.PrefsKeys.Sound, image.sprite == toggleButtonSprites[0] ? false : true);
        }
        else if (targetButton == vibrationButton)
        {
            Image image = vibrationButton.GetComponent<Image>();
            UpdateToggleUi(image, PrefsDataManager.PrefsKeys.Vibration);

            toggleAudio = image.sprite == toggleButtonSprites[0] ? AudioManager.AudioNames.ButtonOffToggle : AudioManager.AudioNames.ButtonOnToggle;

            GameManager.Instance.VibrationManager.UpdateVibrationSettings(image.sprite == toggleButtonSprites[0] ? false : true);
        }

        if(toggleAudio != AudioManager.AudioNames.None && GameManager.Instance.AudioManager.IsSoundEnabled)
            GameManager.Instance.AudioManager.PlayAudio(toggleAudio);
    }
    public void InitializeToggleUi()   //For initializing toggle states based on settings saved data
    {
        //For music and sound
        musicButton.GetComponent<Image>().sprite = GameManager.Instance.AudioManager.IsMusicEnabled ? toggleButtonSprites[1] : toggleButtonSprites[0];
        soundButton.GetComponent<Image>().sprite = GameManager.Instance.AudioManager.IsSoundEnabled ? toggleButtonSprites[1] : toggleButtonSprites[0];

        //For Vibration
        vibrationButton.GetComponent<Image>().sprite = GameManager.Instance.VibrationManager.IsVibrationEnabled ? toggleButtonSprites[1] : toggleButtonSprites[0];
    }
    private void UpdateToggleUi(Image switchImage, PrefsDataManager.PrefsKeys buttonPrefsKey)   //Updates toggle state during runtime
    {
        switchImage.sprite = switchImage.sprite == toggleButtonSprites[0] ? toggleButtonSprites[1] : toggleButtonSprites[0];

        int buttonSettingIndex = switchImage.sprite == toggleButtonSprites[0] ? 0 : 1;
        PrefsDataManager.Instance.SaveData(buttonPrefsKey, buttonSettingIndex); //Saves setting data of current toggle button
    }


    #region Nested Classes
    public class ScoreText
    {
        [Serializable]
        public class TimerText
        {
            public UiPanels uiPanel;
            public Text text;
        }

        [Serializable]
        public class ObstacleEncounteredText
        {
            public UiPanels uiPanel;
            public Text text;
        }
    }
    #endregion

    #region Enums
    public enum UiPanels
    {
        MainMenu,
        SettingsMenu,
        PauseMenu,
        GameplayMenu,
        GameplayScoreMenu,
        LevelFail
    }
    #endregion
}
