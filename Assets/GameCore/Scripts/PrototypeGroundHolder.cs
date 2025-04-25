using System.Collections.Generic;
using UnityEngine;


public class PrototypeGroundHolder : MonoBehaviour
{
    [SerializeField] List<SpriteRenderer> groundSprites = new List<SpriteRenderer>();


    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(groundSprites[0].transform.position);

        Vector2 initialPosition = Camera.main.ViewportToWorldPoint(Vector2.up * viewportPosition.y);
        initialPosition.x += groundSprites[0].size.x;
        groundSprites[0].transform.position = initialPosition;

        for (int i = 1; i < groundSprites.Count; i++)
        {
            groundSprites[i].transform.position = groundSprites[i - 1].transform.position + (Vector3.right * groundSprites[i].size.x);
        }
    }
}