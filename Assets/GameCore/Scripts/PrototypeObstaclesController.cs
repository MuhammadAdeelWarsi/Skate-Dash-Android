using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PrototypeObstaclesController : MonoBehaviour
{
    private bool areObstaclesInitialized, isReadyToSpawn;

    [Header("---Difficulty---")]
    [SerializeField] int allowedObstacles = 1;
    [Space(3), SerializeField] float delayBetweenSpawns;
    [SerializeField] float minSpawnDelay = 2.5f, maxSpawnDelay = 4f;
    [Space(3), SerializeField] float timeElapsedAfterSpawn;

    [Header("---Obstacles---")]
    [SerializeField] PrototypeObstacleNew[] allObstacles;
    private List<PrototypeObstacleNew> activeObstacles;


    private void Awake()
    {
        PrototypeDino.onInitializedEvent += UpdateReadyStatus;
        DifficultyManager.onDifficultyIncreasedEvent += IncreaseDifficulty;

        activeObstacles = new List<PrototypeObstacleNew>();
        InitializeObstacles();
    }
    private void Update()
    {
        if (areObstaclesInitialized && isReadyToSpawn && GameManager.Instance.UiManager.IsGameplayUiInitialized)  //Obstacles are initialized and ready to check spawning
        {
            if(activeObstacles.Count < allowedObstacles)    //More obstacles can be added as per the difficulty
            {
                if(GetSpawnElapsedTime() > delayBetweenSpawns)  //Go to spawn new obstacle after certain delay to avoid overlapping
                {
                    PrototypeObstacleNew newObstacle = GetObstacle();

                    if (newObstacle != null) //Obstacle is available to spawn
                    {
                        SpawnObstacle(newObstacle);
                        ResetTimeAndDelay(true, false);
                        delayBetweenSpawns = GetSpawnDelay();  //Delay time before next spawn
                    }
                }
            }
        }
    }


    public void InitializeDifficulty(int initialObstacles, float initialMinDelay, float initialMaxDelay)
    {
        allowedObstacles = initialObstacles;
        minSpawnDelay = initialMinDelay;
        maxSpawnDelay = initialMaxDelay;
    }
    private void InitializeObstacles()
    {
        foreach (PrototypeObstacleNew obstacle in allObstacles)
        {
            StartCoroutine(obstacle.Initialize());
            obstacle.onScreenLeftEvent += () => { RemoveObstacle(obstacle); };
        }

        areObstaclesInitialized = true;
    }
    private void UpdateReadyStatus()
    {
        isReadyToSpawn = true;
    }

    private PrototypeObstacleNew GetObstacle()  //Returns a random obstacle that is not currently active on the scene
    {
        PrototypeObstacleNew retrievedObstacle = null;
        retrievedObstacle = allObstacles[Random.Range(0, allObstacles.Length)];

        if (!retrievedObstacle.gameObject.activeSelf)
            return retrievedObstacle;
        else
            return null;
    }
    private float GetSpawnDelay()       //Returns delay between obstacle spawning to avoid consecutive obstacles overlapping
    {
        return Random.Range(minSpawnDelay, maxSpawnDelay);
    }
    private float GetSpawnElapsedTime() //Returns elapsed time after spawning is done
    {
        timeElapsedAfterSpawn += Time.deltaTime;
        return timeElapsedAfterSpawn;
    }
    private void ResetTimeAndDelay(bool doResetTime, bool doResetDelay) //Resets the spawn elapsed and delay time
    {
        if(doResetTime)
            timeElapsedAfterSpawn = 0f;

        if(doResetDelay)
            delayBetweenSpawns = 0f;
    }
    private void SpawnObstacle(PrototypeObstacleNew newObstacle)    //Spawns an obstacle by setting its reference position
    {
        newObstacle.SetReferencePosition(null);
        activeObstacles.Add(newObstacle);
        newObstacle.gameObject.SetActive(true);
    }
    private void RemoveObstacle(PrototypeObstacleNew obstacle)      //Removes and hides the encountered obstacle
    {
        obstacle.gameObject.SetActive(false);
        activeObstacles.Remove(obstacle);
    }

    private void IncreaseDifficulty(DifficultyManager.DifficultyInfo difficultyInfo)    //Increases difficulty through obstacles
    {
        minSpawnDelay = difficultyInfo.MinSpawnDelay;
        maxSpawnDelay = difficultyInfo.MaxSpawnDelay;
        allowedObstacles = difficultyInfo.AllowedObstacles;
    }
}
