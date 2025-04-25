using System;
using UnityEngine;


public class ScoreManager : MonoBehaviour
{
    public delegate void OnObstacleCounterUpdate(int obstacleAmount);
    public static event OnObstacleCounterUpdate onObstacleCounterUpdateEvent;


    private HighScore highScore;

    [Header("---Obstacles---")]
    [SerializeField] int obstaclesEncountered;

    public int ObstacleEncountered => obstaclesEncountered;
    public HighScore GameHighScore => highScore;


    private void Awake()
    {
        obstaclesEncountered = 0;
        PrototypeDino.onObstacleEncounteredEvent += UpdateEncounteredObstacle;
    }
    private void Start()
    {
        GameManager.Instance.onGameRestartEvent += ResetSubscribers;
    }

    public void InitializeHighScoreData()
    {
        highScore = new HighScore(PrefsDataManager.Instance.LoadData(PrefsDataManager.PrefsKeys.EncounteredObstacles, 0), PrefsDataManager.Instance.LoadData(PrefsDataManager.PrefsKeys.GameplayTime, 0));
    }
    private void ResetSubscribers()
    {
        onObstacleCounterUpdateEvent = null;
    }
    public void UpdateEncounteredObstacle()
    {
        obstaclesEncountered++;
        onObstacleCounterUpdateEvent.Invoke(obstaclesEncountered);
    }

    #region Nested Class
    [Serializable]
    public class HighScore   //Carries the highest saved score of game
    {
        private int obstaclesEncountered;
        private int totalGameplaySeconds;

        public int ObstaclesEncountered => obstaclesEncountered;
        public int TotalGameplaySeconds => totalGameplaySeconds;

        public HighScore(int obstaclesHighScore, int gameplayTimeHighScore)  //For initializing the high score using saved data
        {
            obstaclesEncountered = obstaclesHighScore;
            totalGameplaySeconds = gameplayTimeHighScore;
        }
    }
    #endregion
}