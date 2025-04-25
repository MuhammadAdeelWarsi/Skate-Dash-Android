using UnityEngine;


public class DifficultyManager : MonoBehaviour
{
    public delegate void OnDifficultyIncreased(DifficultyInfo newDifficultyInfo);
    public static event OnDifficultyIncreased onDifficultyIncreasedEvent;

    [SerializeField] int currentDifficultyIndex;
    [Space(7), SerializeField] DifficultyInfo[] difficultiesInfo;


    private void Awake()
    {
        currentDifficultyIndex = 0;

        ScoreManager.onObstacleCounterUpdateEvent += CheckDifficultyChange;
    }
    private void Start()
    {
        GameManager.Instance.onGameRestartEvent += ResetSubscribers;   
    }

    private void ResetSubscribers()
    {
        onDifficultyIncreasedEvent = null;
    }

    public DifficultyInfo GetInitialDifficulty()
    {
        return difficultiesInfo[currentDifficultyIndex];
    }

    private void CheckDifficultyChange(int obstaclesEncountered)    //Checks if difficulty increment is needed
    {
        if(currentDifficultyIndex < difficultiesInfo.Length)
            if (obstaclesEncountered > difficultiesInfo[currentDifficultyIndex + 1].TriggerThreshold)
                IncreaseDifficulty();
    }
    private void IncreaseDifficulty()   //Brings next difficulty level in the run
    {
        currentDifficultyIndex++;
        onDifficultyIncreasedEvent.Invoke(difficultiesInfo[currentDifficultyIndex]);    //Info of new difficulty is provided to all subs
    }


    [System.Serializable]
    public struct DifficultyInfo    //Carries necessary information of a difficulty
    {
        [SerializeField] int triggerThreshold;  //Amount of encountered obstacles as difficulty's activation threshold
        [SerializeField] int allowedObstacles;  //Allowed obstacles for a difficulty
        [SerializeField] float minSpawnDelay, maxSpawnDelay;    //Range of obstacle spawn delay for a difficulty
        [SerializeField] float groundSpeed;     //Movement speed of ground for a difficulty

        public int TriggerThreshold => triggerThreshold;
        public int AllowedObstacles => allowedObstacles;
        public float MinSpawnDelay => minSpawnDelay;
        public float MaxSpawnDelay => maxSpawnDelay;
        public float GroundSpeed => groundSpeed;
    }
}
