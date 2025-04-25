using System;
using UnityEngine;


public class PrototypeTree : MonoBehaviour
{
    [Header("--Animation--")]
    [SerializeField] AnimatorParameters currentActiveState;    //FOR DEBUGGING
    [SerializeField] int currentActiveStateId;  //FOR DEBUGGING
    [Space(7), SerializeField] int shakeStatesCount = 8, fallStatesCount = 2;
    [SerializeField, Range(0, 100)] int probabilityOfShake = 70;
    private Animator animator;

    [Header("--Repositioning--")]
    [SerializeField] float resetPositionThreshold = -0.02f;
    [Space(3), SerializeField] float minXFactor;
    [SerializeField] float maxXFactor;
    [SerializeField] float minYPosition;
    [SerializeField] float maxYPosition;
    [Space(3), Tooltip("Factor of y-position change from 0f, to be deducted from default local scaling"), SerializeField] float scalingDeductionFactor = 0.5f;
    [SerializeField] Vector3 defaultLocalScale;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        PlayAnimation(AnimatorParameters.ShakeState.ToString(), UnityEngine.Random.Range(1, shakeStatesCount + 1));
    }
    private void Update()
    {
        Vector2 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);

        if (viewportPosition.x < resetPositionThreshold)
            Reposition();
    }

    public void PlayAnimation(string stateType, int stateId)
    {
        animator.SetTrigger(stateType + stateId);
        currentActiveState = (AnimatorParameters)Enum.Parse(typeof(AnimatorParameters), stateType);
        currentActiveStateId = stateId;  //FOR DEBUGGING
    }

    public void CheckNextAnimation()
    {
        AnimatorParameters newAnimationState = UnityEngine.Random.Range(1, 11) <= (probabilityOfShake / 10) ? AnimatorParameters.ShakeState : AnimatorParameters.FallState;
        int newStateCount = newAnimationState == AnimatorParameters.ShakeState ? shakeStatesCount : fallStatesCount;
        PlayAnimation(newAnimationState.ToString(), UnityEngine.Random.Range(1, newStateCount + 1));
    }

    [ContextMenu("Reposition")]
    public void Reposition()
    {
        Vector3 newPosition = new Vector3();
        //newPosition.x = UnityEngine.Random.Range(minLocalX, maxLocalX);
        newPosition.x = Camera.main.ViewportToWorldPoint(Vector2.right * (1 - resetPositionThreshold)).x; /*+ UnityEngine.Random.Range(minXFactor, maxXFactor);*/
        newPosition.y = UnityEngine.Random.Range(minYPosition, maxYPosition);
        newPosition.z = 0f;

        float localScaleDeductionAmount = (newPosition.y - minYPosition) * scalingDeductionFactor;
        Vector3 newScale = defaultLocalScale - (Vector3.one * localScaleDeductionAmount);
        transform.localScale = newScale;
        transform.position = newPosition;

        CheckNextAnimation();
    }


    #region ENUMS
    private enum AnimatorParameters
    {
        ShakeState,
        FallState
    }
    #endregion
}
