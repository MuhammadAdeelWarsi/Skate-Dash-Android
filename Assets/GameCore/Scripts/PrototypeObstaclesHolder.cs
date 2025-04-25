using UnityEngine;


public class PrototypeObstaclesHolder : MonoBehaviour
{
    [SerializeField] PrototypeObstacleNew[] allObstacles;
    [SerializeField] PrototypeObstacleNew currentlyActiveObstacle;

    [Header("****TESTING PURPOSE****")]
    public int testIndex;


    //private void Awake()
    //{
    //    InitializeObstacles();
    //}


    //private void InitializeObstacles()
    //{
    //    foreach(PrototypeObstacleNew obstacle in allObstacles)
    //    {
    //        obstacle.Initialize();
    //        obstacle.onScreenPassEvent += SpawnObstacle;
    //    }
    //}

    //[ContextMenu("SpawnObstacle")]  //First spawning is done after player completes its initial animations
    //private void SpawnObstacle()
    //{
    //    if (currentlyActiveObstacle != null)
    //        ResetObstacle(currentlyActiveObstacle);

    //    PrototypeObstacleNew newObstacle = allObstacles[Random.Range(0, allObstacles.Length)];
    //    newObstacle.gameObject.SetActive(true);

    //    currentlyActiveObstacle = newObstacle;
    //}
    //private void ResetObstacle(PrototypeObstacleNew currentObstacle)
    //{
    //    currentObstacle.gameObject.SetActive(false);
    //    currentObstacle.ResetLocation();
    //}
}