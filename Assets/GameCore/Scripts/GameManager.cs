using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using MoreMountains.NiceVibrations;
using CustomControllers;


public class GameManager : MonoBehaviour
{
    public delegate void OnGameRestart();
    public event OnGameRestart onGameRestartEvent;

    private static GameManager instance;
    private GameTimer timer;
    public static GameManager Instance => instance;


    [Header("---Initialization and Info---")]
    [SerializeField] float initialGroundSpeed = 4f;
    private bool hasGameFinished, hasGamePaused;

    [Header("---References---")]
    [SerializeField] PrototypeDino player;
    [SerializeField] UiManager uiManager;
    [SerializeField] ScoreManager scoreManager;
    [SerializeField] DifficultyManager difficultyManager;
    [SerializeField] PrototypeObstaclesController prototypeObstaclesController;
    [SerializeField] ScreenFitController screenFitController;
    [SerializeField] AudioManager gameplayAudioManager;
    [SerializeField] VibrationManager vibrationManager;

    public PrototypeDino Player => player;
    public UiManager UiManager => uiManager;
    public ScoreManager ScoreManager => scoreManager;
    public DifficultyManager DifficultyManager => difficultyManager;
    public PrototypeObstaclesController PrototypeObstaclesController => prototypeObstaclesController;
    public ScreenFitController ScreenFitController => screenFitController;
    public AudioManager AudioManager => gameplayAudioManager;
    public VibrationManager VibrationManager => vibrationManager;


    public float InitialGroundSpeed => initialGroundSpeed;
    public int TotalSeconds => (int)timer.ElapsedSeconds;
    public bool HasGameFinished => hasGameFinished;
    public bool HasGamePaused { get { return hasGamePaused; } set { hasGamePaused = value; } }


    private void Awake()
    {
        if(instance == null)
            instance = this;

        //hasGameFinished = false;
        timer = new GameTimer();

        DifficultyManager.DifficultyInfo initialDifficultyInfo = difficultyManager.GetInitialDifficulty();
        initialGroundSpeed = initialDifficultyInfo.GroundSpeed;
        prototypeObstaclesController.InitializeDifficulty(initialDifficultyInfo.AllowedObstacles, initialDifficultyInfo.MinSpawnDelay, initialDifficultyInfo.MaxSpawnDelay);

        Time.timeScale = 1;

    }
    private void OnEnable()
    {
        PrototypeDino.onObstacleHitEvent += FinishGame;
        PrototypeDino.onObstacleHitEvent += () =>
        {
            if(scoreManager.GameHighScore.ObstaclesEncountered < scoreManager.ObstacleEncountered)
                PrefsDataManager.Instance.SaveData(PrefsDataManager.PrefsKeys.EncounteredObstacles, scoreManager.ObstacleEncountered);
        };
        PrototypeDino.onObstacleHitEvent += () =>
        {
            if(scoreManager.GameHighScore.TotalGameplaySeconds < (int)timer.ElapsedSeconds)
                PrefsDataManager.Instance.SaveData(PrefsDataManager.PrefsKeys.GameplayTime, (int)timer.ElapsedSeconds);
        };
    }
    private void Start()
    {
        scoreManager.InitializeHighScoreData();
        uiManager.InitializeScoreUI();
        gameplayAudioManager.InitializeAudioSettings();
        vibrationManager.InitializeVibrationSettings();
        uiManager.InitializeToggleUi();
    }
    private void Update()
    {
        if (uiManager.IsGameplayUiInitialized && !hasGameFinished)  //Wait for jump button to completely initialize
        {
            timer.UpdateTimer();
            uiManager.UpdateTimerUI((int)timer.ElapsedSeconds);
        }
    }

    public void StartGame()     //On event trigger
    {
        uiManager.HidePanel(UiManager.UiPanels.MainMenu);
        player.ToggleStart();
    }
    private void FinishGame()   //On event trigger, level is failed
    {
        if (gameplayAudioManager.IsSoundEnabled)
            gameplayAudioManager.PlayAudio(AudioManager.AudioNames.ObstacleCollision);

        if(vibrationManager.IsVibrationEnabled)
            MMVibrationManager.Haptic(HapticTypes.Warning);

        DOTween.To(() => Time.timeScale, value => Time.timeScale = value, 0f, 0.2f).SetEase(Ease.Linear).SetUpdate(true);
        uiManager.ShowPanel(UiManager.UiPanels.LevelFail);
        hasGameFinished = true;
    }
    public void RestartGame()
    {
        onGameRestartEvent.Invoke();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    #region Nested Classes
    private class GameTimer     //Handles the timer running while level is not failed
    {
        private float elapsedSeconds;
        public float ElapsedSeconds => elapsedSeconds;

        public GameTimer()  //Constructor for initialization
        {
            elapsedSeconds = 0f;
        }

        public void UpdateTimer()
        {
            elapsedSeconds += Time.deltaTime;
        }
    }
    #endregion
}
