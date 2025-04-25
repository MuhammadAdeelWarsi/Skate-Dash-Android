using UnityEngine;


public class PrototypeCloud : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private float defaultVerticalPosition;

    [Space(7), SerializeField] float resetPositionThreshold = -0.02f;


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultVerticalPosition = transform.position.y;
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
        Vector3 newPosition = Camera.main.ViewportToWorldPoint(Vector2.right);
        newPosition.x += (spriteRenderer.size.x * 0.6f);
        newPosition.y = defaultVerticalPosition;
        newPosition.z = 0f;

        transform.position = newPosition;
    }
}
