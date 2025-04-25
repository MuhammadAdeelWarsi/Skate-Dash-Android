using UnityEngine;
using DG.Tweening;


public class PrototypeDay : TimeCycle
{
    [SerializeField] SpriteRenderer[] cloudSpriteRenderers;
    [SerializeField] float colorDuration = 60f;

    [Header("---Animals and Insects---")]
    [SerializeField] FlyInsect[] flyInsects;


    [ContextMenu("StartDay")]
    public override void StartCycle()
    {
        if (cloudSpriteRenderers.Length > 0)
        {
            foreach (SpriteRenderer cloudSpriteRenderer in cloudSpriteRenderers)
            {
                cloudSpriteRenderer.DOFade(1f, colorDuration);
                cloudSpriteRenderer.GetComponent<PrototypeEnvironment>().StopMovement(false);
            }
        }
    }

    [ContextMenu("EndDay")]
    public override void EndCycle()
    {
        if (cloudSpriteRenderers.Length > 0)
        {
            foreach (SpriteRenderer cloudSpriteRenderer in cloudSpriteRenderers)
            {
                cloudSpriteRenderer.DOFade(0f, colorDuration).OnComplete(() =>
                {
                    cloudSpriteRenderer.GetComponent<PrototypeEnvironment>().StopMovement(true);
                });
            }
        }
    }
}
