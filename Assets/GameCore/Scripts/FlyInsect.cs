using System;
using UnityEngine;


public class FlyInsect : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;

    [SerializeField] Name _name;
    public Name _Name => _name;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    private void OnEnable()
    {
        if(_name == Name.Dragonfly)
        {
            if (GameManager.Instance.AudioManager.IsSoundEnabled)
                audioSource.mute = false;
            else
                audioSource.mute = true;
        }

        animator.SetLayerWeight(1, 0);

        Array animatorParameters = Enum.GetValues(typeof(AnimatorParameters));
        AnimatorParameters targetParameter = (AnimatorParameters)animatorParameters.GetValue(UnityEngine.Random.Range(0, animatorParameters.Length));

        PlayAnimation(targetParameter.ToString());
    }


    public void PlayAnimation(string targetParameter)
    {
        animator.SetLayerWeight(1, 1);
        animator.SetTrigger(targetParameter);
    }
    public AnimatorStateInfo GetActiveState()   //Returns active state's info
    {
        return animator.GetCurrentAnimatorStateInfo(0);
    }


    #region ENUMS
    private enum AnimatorParameters
    {
        FlyOne,
        FlyTwo
    }
    public enum Name
    {
        Dragonfly,
        Butterfly
    }
    #endregion
}
