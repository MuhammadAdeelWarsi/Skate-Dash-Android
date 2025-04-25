using UnityEngine;
using DG.Tweening;


public class PrototypeEnvironment : MonoBehaviour
{
    [SerializeField] EnviroType type;
    [Space(7), SerializeField] bool stopMovement;
    public float movementSpeed;
    [SerializeField] float speedUpdateDuration = 1f;


    private void Awake()
    {
        if(type == EnviroType.Ground)
            DifficultyManager.onDifficultyIncreasedEvent += UpdateSpeed;
    }
    private void Update()
    {
        if (!stopMovement)
            Move();
    }

    private void Move()
    {
        transform.Translate(Vector2.left * movementSpeed * Time.deltaTime, Space.World);
    }

    public void StopMovement(bool stopMovement)
    {
        this.stopMovement = stopMovement;
    }

    private void UpdateSpeed(DifficultyManager.DifficultyInfo difficultyInfo)   //Updates movement speed
    {
        DOTween.To(() => movementSpeed, (newValue) => movementSpeed = newValue, difficultyInfo.GroundSpeed, speedUpdateDuration);
    }


    public enum EnviroType
    {
        Ground,
        Cloud
    }
}
