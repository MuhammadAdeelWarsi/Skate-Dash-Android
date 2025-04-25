using System.Collections;
using UnityEngine;


public class PrototypeObstacleNew : MonoBehaviour
{
    public delegate void OnScreenLeft();
    public event OnScreenLeft onScreenLeftEvent;

    private SpriteRenderer spriteRenderer;

    [SerializeField] float minimumThresholdForReset = -0.01f;

    [Header("---Positioning---")]
    [SerializeField] float minXPositionOffset;              //Minimum X-Offset on world position
    [SerializeField] float maxXPositionOffset;              //Maximum X-Offset on world position
    [Space(7), SerializeField] float minYPositionOffset;    //Minimum Y-Offset on world position
    [SerializeField] float maxYPositionOffset;              //Maximum Y-Offset on world position
    public Vector2 referencePosition;  //Position just after viewport x = 1


    private void OnEnable()
    {
        UpdateSpawnLocation();
    }
    private void Start()
    {
        GameManager.Instance.onGameRestartEvent += ResetSubscribers;
    }
    private void Update()
    {
        //SetSpawnLocation();

        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);

        if (viewportPosition.x < minimumThresholdForReset)
            onScreenLeftEvent.Invoke();
    }


    private void ResetSubscribers()
    {
        onScreenLeftEvent = null;
    }
    public IEnumerator Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        yield return new WaitForSeconds(1f);    //Providing delay to let the below instance set its value

        GameManager.Instance.ScreenFitController.AdjustFieldValue(ref minXPositionOffset);
        GameManager.Instance.ScreenFitController.AdjustFieldValue(ref maxXPositionOffset);
        GameManager.Instance.ScreenFitController.AdjustFieldValue(ref minYPositionOffset);
        GameManager.Instance.ScreenFitController.AdjustFieldValue(ref maxYPositionOffset);
    }

    public void SetReferencePosition(PrototypeObstacleNew referenceObstacle)
    {
        if(referenceObstacle != null)   //Reference position is being provided by an existing obstacle
        {
            return;
        }
        else    //Reference position is calculated from camera's viewport
        {
            referencePosition = Camera.main.ViewportToWorldPoint(Vector2.right) + (Vector3.right * spriteRenderer.size.x);
        }
    }

    [ContextMenu("UpdateSpawnLocation")]
    private void UpdateSpawnLocation()
    {
        transform.position = referencePosition + new Vector2(Random.Range(minXPositionOffset, maxXPositionOffset), Random.Range(minYPositionOffset, maxYPositionOffset));
    }


    #region DEBUG
    [ContextMenu("CheckSize")]
    public void GetSize()
    {
        Debug.Log(GetComponent<SpriteRenderer>().size);
    }
    private void SetSpawnLocation()
    {
        transform.position = referencePosition + new Vector2(maxXPositionOffset, maxYPositionOffset);
    }
    #endregion
}
