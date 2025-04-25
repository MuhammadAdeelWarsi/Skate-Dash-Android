using DG.Tweening;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;


public class PrototypeObstacle : MonoBehaviour
{
    private Vector2 spawnPosition, currentViewportSpawnPosition;
    private int testCount = 0;
    private Tweener movementTween;

    [SerializeField] ObstacleType obstacleType;
    [SerializeField] int obstacleId;
    [SerializeField] PrototypeObstacle previousObstacle, nextObstacle;

    [Space(7), SerializeField] float initialXSpawnPositionOnViewportOffset = 0.4f, minXSpawnPositionOnViewportOffset = 0.2f, maxXSpawnPositionOnViewportOffset = 0.4f, minYSpawnPositionOnViewport = 0.2f, maxYSpawnPositionOnViewport = 0.7f;
    [Tooltip("Minimum viewport 'x' value before reseting"), SerializeField] float minimumThresholdForReset = -0.01f;
    [SerializeField] float movementTime = 1f;

    [Space(7), SerializeField] bool hasPlayerCollided;


    public Vector2 CurrentViewportSpawnPosition => currentViewportSpawnPosition;


    private void Start()
    {
        Reset(true);
    }
    private void Update()
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);

        if (viewportPosition.x < minimumThresholdForReset)
            Reset(false);
    }

    private void Reset(bool atInitial)
    {
        if(movementTween != null)
            movementTween.Kill();

        spawnPosition = Vector2.zero;

        float referenceXPosition = previousObstacle != null ? previousObstacle.CurrentViewportSpawnPosition.x : 1.1f;
        float offsetX = atInitial ? initialXSpawnPositionOnViewportOffset : Random.Range(minXSpawnPositionOnViewportOffset, maxXSpawnPositionOnViewportOffset);

        Vector2 viewportSpawnPosition = new Vector2(referenceXPosition + offsetX, Random.Range(minYSpawnPositionOnViewport, maxYSpawnPositionOnViewport));

        currentViewportSpawnPosition = viewportSpawnPosition;   //updating for the reference of adjacent obstacle

        spawnPosition = Camera.main.ViewportToWorldPoint(viewportSpawnPosition);
        transform.position = spawnPosition;

        testCount++;
        //if(testCount == 2 && nextObstacle != null)
        //    nextObstacle.gameObject.SetActive(true);

        if (testCount >= 4) //Move when crossed the screen four or more times
            Move();
    }

    private void Move()
    {
        float viewportYPosition = currentViewportSpawnPosition.y;

        if (viewportYPosition <= 0.5f)
        {
            float targetYPosition = Camera.main.ViewportToWorldPoint(Vector2.up * maxYSpawnPositionOnViewport).y;
            movementTween = transform.DOMoveY(targetYPosition, movementTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
        else if (viewportYPosition > 0.5f)
        {
            float targetYPosition = Camera.main.ViewportToWorldPoint(Vector2.up * minYSpawnPositionOnViewport).y;
            movementTween = transform.DOMoveY(targetYPosition, movementTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Skateboard"))
        {
            hasPlayerCollided = true;
            Time.timeScale = 0f;
        }
    }


    public enum ObstacleType
    {
        Fixed
    }
}