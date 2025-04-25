using DG.Tweening;
using UnityEngine;


public class PrototypeGround : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] Transform adjacentGroundPatch;
    [Space(7), SerializeField] float resetPositionThreshold = -0.02f;
    [SerializeField] float resetXPositionOffset = -0.1f;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        //Debug.Log(viewportPosition);

        if (viewportPosition.x < resetPositionThreshold)
            Reset();
    }

    private void Reset()
    {
        transform.localPosition = adjacentGroundPatch.localPosition + (Vector3.right * (spriteRenderer.size.x + resetXPositionOffset));
    }

    private void UpdateSpeed(DifficultyManager.DifficultyInfo difficultyInfo)
    {
        
    }


    #region DEBUG
    [ContextMenu("GetWorldPosition")]
    public void GetWorldPosition()
    {
        Debug.Log(adjacentGroundPatch.position);
    }
    #endregion
}
