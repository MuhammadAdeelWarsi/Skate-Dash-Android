using DG.Tweening;
using UnityEngine;


public class PrototypeDino : MonoBehaviour
{
    public delegate void OnInitialized();
    public delegate void OnObstacleEncountered();
    public delegate void OnObstacleHit();
    public static event OnInitialized onInitializedEvent;
    public static event OnObstacleEncountered onObstacleEncounteredEvent;
    public static event OnObstacleHit onObstacleHitEvent;


    private Rigidbody2D rigidBody;
    private Animator targetAnimator;

    [SerializeField] float jumpSpeed = 1f;
    [SerializeField] float defaultGravityScale = 1f;
    [SerializeField] float fallMultiplier = 1f; // Multiplier to speed up falling
    [SerializeField] float maxRiseSpeed = 5f; // Maximum vertical speed during ascent
    [SerializeField] bool hasJumped, isGrounded;

    [Space(7), SerializeField] bool isInitialized;
    public bool IsInitialized { get { return isInitialized; } set { isInitialized = value; } }

    [Space(10), SerializeField] PrototypeEnvironment ground;


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        targetAnimator = GetComponent<Animator>();

        isGrounded = true;
        defaultGravityScale = rigidBody.gravityScale;
    }
    private void Start()
    {
        GameManager.Instance.onGameRestartEvent += ResetSubscribers;
        GameManager.Instance.ScreenFitController.AdjustFieldValue(ref maxRiseSpeed);
        GameManager.Instance.UiManager.onAudioToggleEvent += ToggleSkatingAudio;
    }
    private void FixedUpdate()
    {
        if (isInitialized)
        {
            if (hasJumped && !isGrounded)   //Jumped and not on ground or struck the ground
                RegulateVerticalMovement();
        }
    }


    private void ResetSubscribers()
    {
        onInitializedEvent = null;
        onObstacleEncounteredEvent = null;
        onObstacleHitEvent = null;
    }

    public void Jump() //Jumps on button click
    {
        if (isInitialized)
        {
            if(!hasJumped && isGrounded)    //Not jumping and on ground
            {
                hasJumped = true;
                GameManager.Instance.AudioManager.PauseAudio(CustomControllers.AudioManager.AudioNames.SkateboardRiding);
                rigidBody.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
                ToggleJumpOne();
            }
        }
    }
    private void ToggleJumpOne()
    {
        targetAnimator.SetTrigger(AnimationParameters.JumpOne.ToString());
    }
    public void ToggleStart()
    {
        targetAnimator.SetTrigger(AnimationParameters.Start.ToString());
        float tweenDuration = targetAnimator.GetCurrentAnimatorStateInfo(0).length;
        float targetSpeed = GameManager.Instance.InitialGroundSpeed;
        DOTween.To(() => ground.movementSpeed, x => ground.movementSpeed = x, targetSpeed, tweenDuration).SetEase(Ease.Linear).OnStart(() =>
        {
            if (GameManager.Instance.AudioManager.IsSoundEnabled)
                GameManager.Instance.AudioManager.PlayAudio(CustomControllers.AudioManager.AudioNames.SkateboardRiding);
        }).OnComplete(() =>
        {
            isInitialized = true;
            onInitializedEvent.Invoke();
        });
    }

    private void ToggleSkatingAudio(PrefsDataManager.PrefsKeys audioKey, bool value)
    {
        if(audioKey == PrefsDataManager.PrefsKeys.Sound)
        {
            if (value)
                GameManager.Instance.AudioManager.UnMuteAudio(CustomControllers.AudioManager.AudioNames.SkateboardRiding, false);
            else
                GameManager.Instance.AudioManager.MuteAudio(CustomControllers.AudioManager.AudioNames.SkateboardRiding);
        }
    }

    private void RegulateVerticalMovement() //Updates up and down movements w.r.t threshold
    {
        if (rigidBody.velocity.y < 0) // Apply faster falling
            rigidBody.gravityScale *= fallMultiplier;

        else if (rigidBody.velocity.y > maxRiseSpeed) // Clamp rising speed
            rigidBody.velocity = new Vector2(rigidBody.velocity.x, maxRiseSpeed);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Ground"))     //Struck the ground
        {
            if(!isGrounded && hasJumped)
            {
                rigidBody.gravityScale = defaultGravityScale;
                hasJumped = false;
                isGrounded = true;
                GameManager.Instance.AudioManager.UnPauseAudio(CustomControllers.AudioManager.AudioNames.SkateboardRiding);
            }
        }
        else if (collision.collider.CompareTag("Obstacle")) //Struck the obstacle (Game Over)
        {
            onObstacleHitEvent.Invoke();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))    //Left the ground
        {
            if (isGrounded && hasJumped)
                isGrounded = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isInitialized && !GameManager.Instance.HasGameFinished)  //Don't check if player is not initialized to run or game finished
        {
            if (collision.CompareTag("Obstacle"))   //Player has crossed the obstacle
            {
                onObstacleEncounteredEvent.Invoke();
            }
        }
    }

    #region Enums
    private enum AnimationParameters
    {
        Start,
        JumpOne
    }
    #endregion
}
